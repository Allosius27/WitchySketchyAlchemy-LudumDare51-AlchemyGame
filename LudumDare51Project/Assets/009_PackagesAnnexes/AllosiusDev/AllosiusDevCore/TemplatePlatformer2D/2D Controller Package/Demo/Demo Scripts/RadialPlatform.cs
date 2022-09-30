using System;
using UnityEngine;

namespace AllosiusDevCore.Controller2D
{
    public class RadialPlatform : MonoBehaviour, IPlayerEffector {
        [SerializeField] private float _speed = 3;
        [SerializeField] private float _size = 2;

        private Transform _t;
        private Vector3 _startPos;
        private Vector3 _change;
        private Vector3 _lastPos;

        private Vector3 Pos => _t.position;

        void Awake() {
            _t = transform;
            _startPos = Pos;
        }

        void Update() {
            _t.position = _startPos + new Vector3(Mathf.Cos(Time.time * _speed), Mathf.Sin(Time.time * _speed)) * _size;
        }

        private void FixedUpdate() {
            _change = _lastPos - Pos;

            _lastPos = Pos;
        }

        public Vector2 EvaluateEffector() {
            return _change.normalized * _speed * Time.deltaTime;
        }

        private void OnDrawGizmosSelected() {
            if (Application.isPlaying) return;
            Gizmos.DrawWireSphere(transform.position, _size);
        }
    }
}