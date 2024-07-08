using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageTextChanger : MonoBehaviour
{
    public void ChangeTextMenu(string text)
    {
        GameObject ErrorPanel = GameObject.Find("ErrorMessagePanel/ErrorMessage");
        Text ErrorMessage = ErrorPanel.GetComponentInChildren<Text>();
        if (ErrorMessage != null)
        {
            ErrorMessage.text = text;
        }
    }
}
