using System;
using TMPro;
using UnityEngine;

public class MathWall : BaseHazard
{
    public MathOperation operationType;
    public int operationValue;
    public string dotTag = "Player";

    public TMP_Text operatorTextDisplay;
    public TMP_Text valueTextDisplay;

    private bool hasActivated = false;
    public bool HasActivated
    {
        get => hasActivated;
        set => hasActivated = value;
    }
    private void OnEnable()
    {
        UpdateTextDisplay();    
    }

    private void UpdateTextDisplay()
    {
        if (operatorTextDisplay != null)
        {
            operatorTextDisplay.text = operationType switch
            {
                MathOperation.Add => "+",
                MathOperation.Subtract => "-",
                MathOperation.Multiply => "x",
                MathOperation.Divide => "/",
                _ => "?"
            };
        }
        if (valueTextDisplay != null)
        {
            valueTextDisplay.text = operationValue.ToString();
        }
    }
}