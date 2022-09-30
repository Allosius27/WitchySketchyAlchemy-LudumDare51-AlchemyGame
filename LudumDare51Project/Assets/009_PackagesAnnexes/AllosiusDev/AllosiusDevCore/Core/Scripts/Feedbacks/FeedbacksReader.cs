using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore
{
    public class FeedbacksReader : MonoBehaviour
    {
        #region Properties

        public bool activeEffects { get; protected set; }

        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public Animator Animator => animator;

        #endregion

        #region UnityInspector

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private Animator animator;

        #endregion

        #region Behaviour

        public void SetCurrentSpriteRenderer(SpriteRenderer renderer)
        {
            spriteRenderer = renderer;
        }

        public void SetCurrentAnimator(Animator anim)
        {
            animator = anim;
        }

        public void ReadFeedback(FeedbacksData feedbackToRead, bool _activeEffects = true)
        {
            activeEffects = _activeEffects;
            StartCoroutine(feedbackToRead.Execute(this));
        }

        #endregion
    }
}
