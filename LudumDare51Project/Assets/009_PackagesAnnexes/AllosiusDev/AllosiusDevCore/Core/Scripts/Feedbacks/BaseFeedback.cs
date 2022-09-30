using System;
using System.Collections;
using UnityEngine;

namespace AllosiusDevCore
{
    [Serializable]
    public class BaseFeedback
    {
        [Tooltip("Determines whether feedback is active or not")]
        public bool IsActive = true;

        //public Transform target { get; protected set; }

        //public SpriteRenderer spriteRenderer { get; protected set; }
        //public Color spriteRendererBaseColor { get; protected set; }

        /*public virtual void SetTarget(Transform _target)
        {
            target = _target;
        }*/

        /*public virtual void SetSpriteRenderer(SpriteRenderer _targetRenderer)
        {
            spriteRenderer = _targetRenderer;
        }*/

        /*public virtual void SetSpriteRendererBaseColor(Color _baseColor)
        {
            spriteRendererBaseColor = _baseColor;
        }*/

        public virtual IEnumerator Execute(FeedbacksReader _owner)
        {
            yield break;
        }
    }
}
