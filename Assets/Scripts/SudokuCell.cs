using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    private TMP_InputField inputField;
    public int row, col; 
    public int fontSize = 44; 

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>(); 
        inputField.textComponent.fontSize = fontSize;
    }

   
    public void SetNumber(int number, bool isLocked)
    {
        inputField.text = number.ToString(); 
        if (isLocked)
        {
            inputField.interactable = false; 
            inputField.textComponent.color = Color.black; 
        }
        else
        {
            inputField.interactable = true;
            inputField.textComponent.color = Color.blue; 
        }
    }
}
