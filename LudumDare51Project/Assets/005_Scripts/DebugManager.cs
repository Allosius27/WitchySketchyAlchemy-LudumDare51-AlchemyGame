using AllosiusDevCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DebugManager : MonoBehaviour
{
    private FeedbacksReader feedbacksReader;

    public FeedbacksData testFeedbacks;

    private void Awake()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    [Button(ButtonSizes.Large)]
    public void PlayFeedbacks()
    {
        feedbacksReader.ReadFeedback(testFeedbacks);
    }
}
