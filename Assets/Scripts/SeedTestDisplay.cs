using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedTestDisplay : MonoBehaviour
{
    private static SeedTestDisplay instance;
    public static SeedTestDisplay Instance => instance;
    public TextMeshProUGUI display;
    private void Awake()
    {
        instance = this;
    }

    public void SetDisplayText(string text)
    {
        display.text = text;
    }

    public void ClearText()
    {
        display.text = "";
    }
}
