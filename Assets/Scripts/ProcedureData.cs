using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Procedure", menuName = "Procedure/Procedure")]
public class ProcedureData : ScriptableObject
{
    public string title;
    public List<StepData> steps;
}