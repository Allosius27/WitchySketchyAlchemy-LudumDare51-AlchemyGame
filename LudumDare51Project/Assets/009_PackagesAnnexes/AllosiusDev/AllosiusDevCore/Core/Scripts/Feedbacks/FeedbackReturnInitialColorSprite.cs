using AllosiusDevCore;
using System;
using System.Collections;
using UnityEngine;

namespace AllosiusDevCore
{
    [Serializable]
    public class FeedbackTimerChangeColorSprite : BaseFeedback
    {
        public Color newSpriteColor;

        public float colorChangeDuration = 0.1f;

        public override IEnumerator Execute(FeedbacksReader _owner)
        {
            if (IsActive && _owner.activeEffects)
            {
                Color baseSpriteColor = Color.white;

                if (_owner.SpriteRenderer != null)
                {
                    Debug.Log("ChangeColor");
                    baseSpriteColor = _owner.SpriteRenderer.color;
                    _owner.SpriteRenderer.color = newSpriteColor;
                }
                else if (_owner.GetComponent<SpriteRenderer>() != null)
                {
                    baseSpriteColor = _owner.GetComponent<SpriteRenderer>().color;
                    _owner.GetComponent<SpriteRenderer>().color = newSpriteColor;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer is null");
                }

                yield return new WaitForSeconds(colorChangeDuration);

                if (_owner.SpriteRenderer != null)
                {
                    Debug.Log("ChangeColor");
                    _owner.SpriteRenderer.color = baseSpriteColor;
                }
                else if (_owner.GetComponent<SpriteRenderer>() != null)
                {
                    _owner.GetComponent<SpriteRenderer>().color = baseSpriteColor;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer is null");
                }
            }
        }
    }
}
