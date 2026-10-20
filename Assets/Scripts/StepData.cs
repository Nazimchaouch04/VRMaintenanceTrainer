using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Procedure/Step")]
public class StepData : ScriptableObject
{
    public string actionId; // ex: PRESS_STOP
    [TextArea] public string instruction;
    [TextArea] public string hint;
}

