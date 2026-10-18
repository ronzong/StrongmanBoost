using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Text buttonText; // Reference to the Text component of the button
    private bool isOn = false; // Initial state of the button
    public ItemButtonsManager itemButtonsManager;

    private void Start()
    {
        string val = PlayerPrefs.GetString("IsAutoClickOn", "False");

        if (val == "True")
            OnButtonClick();
    }

    // This method will be called when the button is clicked
    public void OnButtonClick()
    {
        isOn = !isOn; // Toggle the state
        UpdateButtonLabel(); // Update the button's label

        if (isOn)
        {
            itemButtonsManager.itemButtons[0].StartAutoClick();
            SaveButtonState("True");
        }
        else
        {
            itemButtonsManager.itemButtons[0].StopAutoClick();
            SaveButtonState("False");
        }
    }

    private void SaveButtonState(string val)
    {
        PlayerPrefs.SetString("IsAutoClickOn", val);
    }

    // Update the button's label based on the current state
    private void UpdateButtonLabel()
    {
        buttonText.text = isOn ? "OFF" : "ON";
        // You can add any other logic or visual changes based on the state here
    }
}
