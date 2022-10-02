using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestroyOnCreate : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 3f);

        int timeScore = 10 - (int)Math.Round(GameCore.Instance.CurrentTimer);
        TextMeshProUGUI textmeshProUGUI = GetComponent<TextMeshProUGUI>();
        textmeshProUGUI.SetText("+" + (50 * (1 + timeScore)));
     
    }


}
