using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class ProcedureManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] ProcedureData procedure;

    [Header("Hints")]
    [SerializeField] float hintDelay = 10f;
    [SerializeField] GameObject hintArrowPrefab;

    [Header("Score")]
    [SerializeField] int errorPenalty = 10;
    [SerializeField] int hintPenalty = 5;

    // Events other members listen to
    public event Action<int, StepData> OnStepChanged;
    public event Action<string> OnWrongAction;
    public event Action<string> OnHint;
    public event Action<int, float> OnFinished;

    public ProcedureResult LastResult { get; private set; }
    public int StepCount => procedure.steps.Count;
    public int CurrentIndex => current;
    public bool IsFinished { get; private set; }

    readonly Dictionary<string, StepTarget> targets = new Dictionary<string, StepTarget>();
    ProcedureResult result;
    Coroutine hintRoutine;
    GameObject arrow;
    int current, stepErrors, safetyErrors, hints;
    float procedureStart, stepStart;

    void Start()
    {
        foreach (StepTarget t in FindObjectsByType<StepTarget>(FindObjectsSortMode.None))
            targets[t.actionId] = t;

        result = new ProcedureResult { title = procedure.title };
        procedureStart = Time.time;
        EnterStep();
    }

    // State machine
    void EnterStep()
    {
        ClearHighlight();
        if (hintRoutine != null) StopCoroutine(hintRoutine);
        if (current >= procedure.steps.Count) { Finish(); return; }

        StepData step = procedure.steps[current];
        stepStart = Time.time;
        stepErrors = 0;
        Highlight(step.actionId);
        OnStepChanged?.Invoke(current, step);
        hintRoutine = StartCoroutine(HintRoutine(step));
    }

    public void ReportAction(string actionId)
    {
        if (IsFinished || current >= procedure.steps.Count) return;

        StepData step = procedure.steps[current];
        if (actionId == step.actionId)
        {
            result.steps.Add(new StepResult
            {
                instruction = step.instruction,
                seconds = Time.time - stepStart,
                errors = stepErrors
            });
            current++;
            EnterStep();
        }
        else
        {
            stepErrors++;
            result.errors++;
            OnWrongAction?.Invoke("Wrong action. " + step.hint);
        }
    }

    public void ReportSafetyError()
    {
        if (IsFinished) return;
        safetyErrors++;
        OnWrongAction?.Invoke("Safety error: the machine was not locked out.");
    }

    // Hints: first the text, then a stronger one with the arrow
    IEnumerator HintRoutine(StepData step)
    {
        yield return new WaitForSeconds(hintDelay);
        hints++;
        OnHint?.Invoke(step.hint);

        yield return new WaitForSeconds(hintDelay);
        hints++;
        OnHint?.Invoke("Look at the highlighted object.");
        SpawnArrow(step.actionId);
    }

    void SpawnArrow(string actionId)
    {
        if (hintArrowPrefab == null) return;
        if (!targets.TryGetValue(actionId, out StepTarget target)) return;

        arrow = Instantiate(hintArrowPrefab, target.transform.position + Vector3.up * 0.35f, Quaternion.identity);
    }

    void Highlight(string actionId)
    {
        if (targets.TryGetValue(actionId, out StepTarget target)) target.SetHighlight(true);
    }

    void ClearHighlight()
    {
        if (arrow != null) Destroy(arrow);
        foreach (StepTarget t in targets.Values) t.SetHighlight(false);
    }

    // Evaluation
    void Finish()
    {
        IsFinished = true;
        result.seconds = Time.time - procedureStart;
        result.safetyErrors = safetyErrors;
        result.hints = hints;
        int totalErrors = result.errors + safetyErrors;
        result.score = Mathf.Max(0, 100 - (totalErrors * errorPenalty) - (hints * hintPenalty));
        LastResult = result;
        SaveCsv(result);
        OnFinished?.Invoke(totalErrors, result.seconds);
    }

    void SaveCsv(ProcedureResult r)
    {
        string path = Path.Combine(Application.persistentDataPath, "results.csv");
        CultureInfo ci = CultureInfo.InvariantCulture;

        if (!File.Exists(path))
            File.AppendAllText(path, "date,procedure,errors,safety_errors,hints,seconds,score\n");

        string line = string.Join(",",
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", ci),
            r.title.Replace(",", " "),
            r.errors, r.safetyErrors, r.hints,
            r.seconds.ToString("F1", ci), r.score);

        File.AppendAllText(path, line + "\n");
        Debug.Log("Results saved to " + path);
    }
}