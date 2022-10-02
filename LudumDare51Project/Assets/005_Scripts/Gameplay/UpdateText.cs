using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int timeScore = 10 - (int)Math.Round(GameCore.Instance.CurrentTimer);
        TextMeshProUGUI textmeshProUGUI = GetComponent<TextMeshProUGUI>();
        textmeshProUGUI.SetText("+" + (50 * (1 + timeScore)));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
