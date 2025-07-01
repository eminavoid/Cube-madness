using System;
using TMPro;
using UnityEngine;

public enum MathOperation
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public class MathWall : MonoBehaviour
{
    public MathOperation operationType = MathOperation.Add;
    public int operationValue = 1;
    public string dotTag = "Player"; // Assign this tag to your "Dot" GameObject

    public TMP_Text operatorTextDisplay;
    public TMP_Text valueTextDisplay;

    private void OnEnable()
    {
        UpdateTextDisplay();    
    }

    private void UpdateTextDisplay()
    {
        if (operatorTextDisplay != null) 
        {
            switch (operationType) 
            { 
                case MathOperation.Add:
                    operatorTextDisplay.text = "+";
                    break;
                case MathOperation.Subtract:
                    operatorTextDisplay.text = "-";
                    break;
                case MathOperation.Multiply:
                    operatorTextDisplay.text = "x";
                    break;
                case MathOperation.Divide:
                    operatorTextDisplay.text = "/";
                    break;
                default:
                    operatorTextDisplay.text = "?";
                    break;
            }
        }
        if (valueTextDisplay != null)
        {
            valueTextDisplay.text = operationValue.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(dotTag))
        {
            DotController dotController = other.GetComponent<DotController>();
            if (dotController != null)
            {
                Debug.Log($"Math Wall hit. Operation: {operationType}, Value: {operationValue}. Current count before op: {dotController.numberOfMiniPlayers}");

                int currentMiniPlayers = dotController.numberOfMiniPlayers;
                int newMiniPlayers = currentMiniPlayers;

                switch (operationType)
                {
                    case MathOperation.Add:
                        newMiniPlayers += operationValue;
                        break;
                    case MathOperation.Subtract:
                        newMiniPlayers -= operationValue;
                        break;
                    case MathOperation.Multiply:
                        newMiniPlayers *= operationValue;
                        break;
                    case MathOperation.Divide:
                        if (operationValue != 0)
                        {
                            newMiniPlayers /= operationValue;
                        }
                        else
                        {
                            Debug.LogWarning("Division by zero attempted on Math Wall!");
                        }
                        break;
                }

                // Ensure the number of mini players is not negative
                if (newMiniPlayers < 0)
                {
                    newMiniPlayers = 0;
                }

                // Update the number of mini players in the DotController
                if (newMiniPlayers != currentMiniPlayers)
                {
                    dotController.numberOfMiniPlayers = newMiniPlayers;
                    dotController.AdjustMiniPlayerCount(); // We'll add this method to DotController
                    Debug.Log($"Math Wall hit. New count after op: {dotController.numberOfMiniPlayers}");

                }
                //Get the pearent MathPanel and deactivate both walls
                //MathWallPearent pearentWall = GetComponentInParent<MathWallPearent>();
                //if( pearentWall != null)
                //{
                //    pearentWall.DeactivateWalls();
                //}

                
                foreach (Transform child in this.transform.parent)
                {
                    child.gameObject.SetActive(false);
                }
                

                // Optionally destroy the math wall after it's been touched
                // Destroy(gameObject);
            }
        }
    }
}