using UnityEngine;
using Random = UnityEngine.Random;
using AllosiusDevUtilities;
using AllosiusDevUtilities.Audio;

namespace AllosiusDevCore.Controller2D
{
    public class PlayerAnimator : MonoBehaviour 
    {
        #region Fields

        private IPlayerController _player;
        private ParticleSystem.MinMaxGradient _currentGradient;
        private Vector2 _movement;
        private Vector2 _defaultSpriteSize;

        #endregion

        #region Properties

        #region Animation Keys

        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int WallGrabKey = Animator.StringToHash("WallGrab");
        private static readonly int GrabKey = Animator.StringToHash("Grab");
        private static readonly int WallClimbKey = Animator.StringToHash("WallClimb");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");

        #endregion

        #endregion

        #region UnityInspector

        [Header("Components")]
        [SerializeField] private Animator _anim;
        [SerializeField] private AudioSource _source;
        [SerializeField] private LayerMask _groundMask;

        [Header("Jump")]
        [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;

        [Header("Movements")]
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;
        [SerializeField] private AudioData[] _footsteps;
        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 30;
        [SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
        [SerializeField] private float _maxParticleFallSpeed = -40;

        [Header("Crouch")]
        [SerializeField] private Vector2 _crouchScaleModifier = new Vector2(1, 0.5f);

        [Header("Climb")]
        [SerializeField] private bool _flipSpriteAtClimb = true;

        #region Extended

        [Header("Extended")]
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private ParticleSystem _doubleJumpParticles;
        [SerializeField] private AudioData _doubleJumpClip, _dashClip;
        [SerializeField] private ParticleSystem _dashParticles, _dashRingParticles;
        [SerializeField] private Transform _dashRingTransform;
        [SerializeField] private AudioClip[] _slideClips;

        #endregion

        #endregion

        #region Behaviour

        #region Init

        void Awake() {
            _player = GetComponentInParent<IPlayerController>();

            _defaultSpriteSize = _sprite.size;

            _player.OnGroundedChanged += OnLanded;
            _player.OnJumping += OnJumping;
            _player.OnDoubleJumping += OnDoubleJumping;
            _player.OnDashingChanged += OnDashing;
            _player.OnCrouchingChanged += OnCrouching;
            _player.OnWalking += OnWalking;
            _player.OnGrab += OnGrab;
            _player.OnWallGrab += OnWallGrab;
            _player.OnClimbing += OnWallClimb;
        }

        #endregion

        private void OnDoubleJumping() {
            //_source.PlayOneShot(_doubleJumpClip);
            AudioController.Instance.PlayAudio(_doubleJumpClip);
            _doubleJumpParticles.Play();
        }

        private void OnDashing(bool dashing) {
            if (dashing) {
                _dashParticles.Play();
                _dashRingTransform.up = new Vector3(_player.Input.X, _player.Input.Y);
                _dashRingParticles.Play();
                //_source.PlayOneShot(_dashClip);
                AudioController.Instance.PlayAudio(_dashClip);
            }
            else {
                _dashParticles.Stop();
            }
        }

        private void OnWalking()
        {
            if(_player.MoveApplyValue.x != 0 && _player.Input.X != 0)
            {
                _anim.SetBool("Walking", true);
            }
            else
            {
                _anim.SetBool("Walking", false);
            }
        }

        private void OnJumping() {
            _anim.SetTrigger(JumpKey);
            //_anim.ResetTrigger(GroundedKey);

            // Only play particles when grounded (avoid coyote)
            if (_player.Grounded) {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        private void OnGrab()
        {
            _anim.SetTrigger(GrabKey);
        }

        private void OnWallGrab()
        {
            _anim.SetBool(WallGrabKey, _player.WallGrab);

            if (_flipSpriteAtClimb)
            {
                SpriteRenderer spriteRenderer = _anim.GetComponent<SpriteRenderer>();
                if (_player.WallGrab)
                {
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipX = true;
                    }
                }
                else
                {
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipX = false;
                    }
                }
            }
        }

        private void OnWallClimb()
        {
            if (_player.WallGrab && _player.Input.Y != 0)
            {
                _anim.SetBool(WallClimbKey, true);
            }
            else
            {
                _anim.SetBool(WallClimbKey, false);
            }
        }

        private void OnLanded(bool grounded) 
        {
            _anim.SetBool(GroundedKey, grounded);

            if (grounded) {
                //_anim.SetTrigger(GroundedKey);
                //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                AudioController.Instance.PlayAudio(_footsteps[Random.Range(0, _footsteps.Length)]);
                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _movement.y);
                SetColor(_landParticles);
                _landParticles.Play();
            }
            else {
                _moveParticles.Stop();
            }
        }

        private void OnCrouching(bool crouching) {
            if (crouching) {
                _sprite.size = _defaultSpriteSize * _crouchScaleModifier;
                _source.PlayOneShot(_slideClips[Random.Range(0, _slideClips.Length)], Mathf.InverseLerp(0, 5, Mathf.Abs(_movement.x)));
            }
            else {
                _sprite.size = _defaultSpriteSize;
            }
        }

        void DetectGroundColor() 
        {
            // Detect ground color. Little bit of garbage allocation, but faster computationally.
            var groundHits = Physics2D.RaycastAll(transform.position, Vector3.down, 2, _groundMask);
            foreach (var hit in groundHits) {
                if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) continue;
                _currentGradient = new ParticleSystem.MinMaxGradient(r.color * 0.9f, r.color * 1.2f);
                SetColor(_moveParticles);
                return;
            }
        }


        void SetColor(ParticleSystem ps) {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        void Update()
        {
            if (_player == null) return;

            var inputPoint = Mathf.Abs(_player.Input.X);

            // Flip the sprite
            if (_player.Input.X != 0 && _player.WallGrab == false)
            {
                transform.localScale = new Vector3(_player.Input.X > 0 ? 1 : -1, 1, 1);

                // Lean while running
                var targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, _player.Input.X)));
                _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);
            }

            // Speed up idle while running
            _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputPoint));

            DetectGroundColor();

            _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputPoint, 2 * Time.deltaTime);

            _movement = _player.RawMovement; // Previous frame movement is more valuable
        }
        private void OnDisable() {
            _moveParticles.Stop();
        }

        private void OnEnable() {
            _moveParticles.Play();
        }

        void OnDestroy()
        {
            _player.OnGroundedChanged -= OnLanded;
            _player.OnJumping -= OnJumping;
            _player.OnDoubleJumping -= OnDoubleJumping;
            _player.OnDashingChanged -= OnDashing;
            _player.OnCrouchingChanged -= OnCrouching;
            _player.OnWalking -= OnWalking;
            _player.OnGrab -= OnGrab;
            _player.OnWallGrab -= OnWallGrab;
            _player.OnClimbing -= OnWallClimb;
        }

        #endregion
    }
}