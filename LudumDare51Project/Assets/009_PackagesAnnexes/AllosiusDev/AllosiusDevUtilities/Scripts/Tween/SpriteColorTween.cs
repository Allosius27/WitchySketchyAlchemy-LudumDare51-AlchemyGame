//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using UnityEngine;

namespace AllosiusDevUtilities.Tween
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColorTween : ColorTween
    {
        private SpriteRenderer m_Sprite;

#region Override Functions
        protected override void Init() {
            m_Sprite = GetComponent<SpriteRenderer>();
            base.Init();
        }

        protected override void OnSetValue(Color _val) {
            m_Sprite.color = _val;
        }

        protected override void OnMoveValue(Color _curr, Color _target, float _nTime) {
            m_Sprite.color = Color.Lerp(_curr, _target, _nTime);
        }
#endregion

    }

}
