using System;
using JetBrains.Annotations;
using UnityEngine;

namespace AllosiusDevCore.Controller2D
{
    /// <summary>
    /// This seems hacky, but I just wanted to chuck it in quickly. Works perfectly fine. 
    /// </summary>
    public class OneWayPlatform : MonoBehaviour {
        private BoxCollider2D _col;
        private IPlayerController _controller;

        [SerializeField] private float _fallThroughUnlockTime = 0.25f;

        private float _timeToUnlock = float.MinValue;
        private void Awake() => _col = GetComponent<BoxCollider2D>();

        private void Update() {
            if (_controller == null) return;
            if (_controller.Input.Y < 0) _timeToUnlock = Time.time + _fallThroughUnlockTime;

            _col.enabled = _controller.RawMovement.y <= 0 && Time.time >= _timeToUnlock;
        }


        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out IPlayerController controller)) {
                _controller = controller;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.TryGetComponent(out IPlayerController controller)) _controller = null;
        }
    }
}