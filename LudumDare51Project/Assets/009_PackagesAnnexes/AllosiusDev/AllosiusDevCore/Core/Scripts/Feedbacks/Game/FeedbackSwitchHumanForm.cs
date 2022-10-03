using AllosiusDevCore;
using System;
using System.Collections;
using UnityEngine;

namespace AllosiusDevCore.Game
{
    [Serializable]
    public class FeedbackSwitchHumanForm : BaseFeedback
    {
        [Required]
        [SerializeField] private Sprite humanBodySprite;
        [Required]
        [SerializeField] private Sprite humanLegsSprite;
        [Required]
        [SerializeField] private Sprite humanArmsSprite;
        [Required]
        [SerializeField] private Sprite humanHatSprite;
        [Required]
        [SerializeField] private Sprite humanHeadSprite;
        [Required]
        [SerializeField] private Sprite humanHairSprite;

        public override IEnumerator Execute(FeedbacksReader _owner)
        {
            if (IsActive && _owner.activeEffects)
            {
                GameCore.Instance.CharacterController.SetNewHumanForm(humanBodySprite, humanLegsSprite, humanArmsSprite, humanHatSprite, humanHeadSprite, humanHairSprite);
            }
            return base.Execute(_owner);
        }
    }
}
