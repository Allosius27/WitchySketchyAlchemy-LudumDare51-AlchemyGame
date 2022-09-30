using UnityEngine;

namespace AllosiusDevCore.Controller2D
{
    public class Explosion : MonoBehaviour {
        [SerializeField] private float _growSpeed = 1;
        [SerializeField] private float _growSize = 1;
        [SerializeField] private float _explosionForce = 50;
  
        void Update() {
            var scale = (Mathf.Sin(Time.time * _growSpeed) + 2) * _growSize;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out IPlayerController controller)) {
                var dir = other.transform.position - transform.position;
                controller.AddForce(dir.normalized * _explosionForce, PlayerForce.Decay);
            }
        }
    }
}