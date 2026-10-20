using System.Collections.Generic;

[System.Serializable]
public class StepResult
{
    public string instruction;
    public float seconds;
    public int errors;
}

[System.Serializable]
public class ProcedureResult
{
    public string title;    
    public int errors;
    public int safetyErrors;
    public int hints;
    public float seconds;
    public int score;
    public List<StepResult> steps = new List<StepResult>();
}