using Sirenix.OdinInspector.AllosiusDevCore.Controller2D;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.Controller2D {
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IPlayerController 
    {
        #region Fields

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private PlayerInput _input;
        private Vector2 _lastPosition;
        private Vector2 _velocity;
        private Vector2 _speed;
        private int _fixedFrame;

        private Vector2 _lastPos;
        private bool _cornerStuck;

        private bool allowDoubleJump;
        private bool _allowDoubleJump
        {
            get 
            {
                if (playerData != null)
                {
                    allowDoubleJump = playerData._allowDoubleJump;
                    return playerData._allowDoubleJump;
                }
                else
                {
                    allowDoubleJump = false;
                    return false;
                }
            }
        }

        #region CollisionsVariables

        private RaycastHit2D[] _hitsDown = new RaycastHit2D[3];
        private RaycastHit2D[] _hitsUp = new RaycastHit2D[1];
        private RaycastHit2D[] _hitsLeft = new RaycastHit2D[1];
        private RaycastHit2D[] _hitsRight = new RaycastHit2D[1];

        private bool _hittingCeiling, _grounded, _colRight, _colLeft;

        private bool onClimbWall;
        private bool onClimbWallOffset;

        private float _timeLeftGrounded;

        #endregion

        #region CrouchVariables

        private Vector2 _defaultColliderSize, _defaultColliderOffset;
        private float _velocityOnCrouch;
        private bool _crouching;
        private int _frameStartedCrouching;

        #endregion

        #region HorizontalMovementsVariables

        private float _frameClamp;

        #endregion

        #region GravityVariables

        private float _fallSpeed;

        #endregion

        #region JumpVariables

        private bool _jumpToConsume;
        private bool _coyoteUsable;
        private bool _executedBufferedJump;
        private bool _endedJumpEarly = true;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed = float.MinValue;
        private bool _doubleJumpUsable;

        private int coyoteTimeThreshold;
        private int _coyoteTimeThreshold
        {
            get 
            {
                if (playerData != null)
                {
                    coyoteTimeThreshold = playerData._coyoteTimeThreshold;
                    return playerData._coyoteTimeThreshold;
                }
                else
                {
                    coyoteTimeThreshold = 0;
                    return 0;
                }
            }
        }

        private int jumpBuffer;
        private int _jumpBuffer
        {
            get
            {
                if (playerData != null)
                {
                    jumpBuffer = playerData._jumpBuffer;
                    return playerData._jumpBuffer;
                }
                else
                {
                    jumpBuffer = 0;
                    return 0;
                }
            }
        }

        private float jumpEndEarlyGravityModifier;
        private float _jumpEndEarlyGravityModifier
        {
            get
            {
                if (playerData != null)
                {
                    jumpEndEarlyGravityModifier = playerData._jumpEndEarlyGravityModifier;
                    return playerData._jumpEndEarlyGravityModifier;
                }
                else
                {
                    jumpEndEarlyGravityModifier = 0.0f;
                    return 0.0f;
                }
            }
        }

        #endregion

        #region ClimbVariables

        private bool wallGrab;

        #endregion

        #region DashVariables

        private float _startedDashing;
        private bool _canDash;
        private Vector2 _dashVel;

        private bool _dashing;
        private bool _dashToConsume;

        #endregion

        #region Effectors & Forces Variables

        private readonly List<IPlayerEffector> _usedEffectors = new List<IPlayerEffector>();

        private Vector2 _forceBuildup;

        #endregion

        #endregion

        #region Properties

        public FrameInput Input { get; protected set; }
        public Vector2 RawMovement { get; protected set; }
        public Vector2 MoveApplyValue { get; protected set; }

        public bool IsTurned { get; protected set; }

        public bool Grounded => _grounded;
        public bool WallGrab => wallGrab;

        #region JumpVariables

        private bool CanUseCoyote => _coyoteUsable && !_grounded && _timeLeftGrounded + _coyoteTimeThreshold > _fixedFrame;
        private bool HasBufferedJump => ((_grounded && !_executedBufferedJump) || _cornerStuck) && _lastJumpPressed + _jumpBuffer > _fixedFrame;
        private bool CanDoubleJump => _allowDoubleJump && _doubleJumpUsable && !_coyoteUsable;

        #endregion

        #endregion

        #region Events

        public event Action<bool> OnGroundedChanged;
        public event Action OnJumping, OnDoubleJumping;
        public event Action<bool> OnDashingChanged;
        public event Action<bool> OnCrouchingChanged;
        public event Action OnWalking;
        public event Action OnGrab;
        public event Action OnWallGrab;
        public event Action OnClimbing;

        #endregion

        #region UnityInspector

        [SerializeField] private PlayerData playerData;

        #endregion

        #region Behaviour

        #region Init

        void Awake() 
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _input = GetComponent<PlayerInput>();

            LoadPlayerData();

           

            _defaultColliderSize = _collider.size;
            _defaultColliderOffset = _collider.offset;
        }

        public void LoadPlayerData()
        {
            if(playerData != null)
            {
                
            }
            else
            {
                Debug.LogWarning("Player Data is null");
            }
        }

        #endregion

        #region Update

        private void Update()
        {
            if(playerData == null)
            {
                return;
            }

            UpdateDebugVariables();

            GatherInput();
        }

        void FixedUpdate() 
        {
            if(playerData == null)
            {
                return;
            }

            _fixedFrame++;
            _frameClamp = playerData._moveClamp;

            // Calculate velocity
            _velocity = (_rb.position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = _rb.position;

            
            RunCollisionChecks();

            WallGrabCollisionChecks();

            CalculateCrouch();

            CalculateHorizontal();
            
            CalculateJumpApex();
            CalculateGravity();
            CalculateJump();
            CalculateDash();
            CalculateClimb();

            MoveCharacter();
        }

        void UpdateDebugVariables()
        {
            if (coyoteTimeThreshold != _coyoteTimeThreshold)
            {
                coyoteTimeThreshold = _coyoteTimeThreshold;
            }

            if (jumpBuffer != _jumpBuffer)
            {
                jumpBuffer = _jumpBuffer;
            }

            if (jumpEndEarlyGravityModifier != _jumpEndEarlyGravityModifier)
            {
                jumpEndEarlyGravityModifier = _jumpEndEarlyGravityModifier;
            }
        }

        #endregion

        #region Gather Input

        private void GatherInput() 
        {
            Debug.Log("GatherInput");

            Input = _input.GatherInput();

            if (Input.DashDown) _dashToConsume = true;
            if (Input.JumpDown) {
                _lastJumpPressed = _fixedFrame;
                _jumpToConsume = true;
                
                wallGrab = false;
                OnWallGrab.Invoke();
            }

            if(Input.WallGrabDown && onClimbWall)
            {
                wallGrab = !wallGrab;
                if (wallGrab)
                {
                    OnGrab.Invoke();
                }
                _speed.y = 0;
                OnWallGrab.Invoke();
            }
        }

        #endregion

        #region Collisions

        private void SetFlip()
        {
            if (Input.X > 0)
            {
                IsTurned = false;
            }
            else if(Input.X < 0)
            {
                IsTurned = true;
            }
        }

        private void WallGrabCollisionChecks()
        {
            Vector2 currentSideOffset = SetSideOffset();

            var hit = Physics2D.OverlapCircle((Vector2)transform.position + currentSideOffset, playerData.collisionRadius, playerData.ClimbLayerMask);
            if (hit != null)
            {
                //Debug.Log(hit.name);
                onClimbWall = true;
            }
            else
            {
                onClimbWall = false;
            }

            var hitOffset = Physics2D.OverlapCircle((Vector2)transform.position + currentSideOffset + playerData.aboveSideOffset, playerData.collisionRadius, playerData.ClimbLayerMask);
            if (hitOffset != null)
            {
                //Debug.Log(hitOffset.name);
                onClimbWallOffset = true;
            }
            else
            {
                onClimbWallOffset = false;
            }

            /*if (_colRight == false && _colLeft == false)
            {
                wallGrab = false;
                OnWallGrab.Invoke();
            }*/

            if (onClimbWall == false)
            {
                wallGrab = false;
                OnWallGrab.Invoke();
            }
        }

        // We use these raycast checks for pre-collision information
        private void RunCollisionChecks() {
            // Generate ray ranges. 
            var b = _collider.bounds;

            // Ground
            var groundedCheck = RunDetection(Vector2.down, out _hitsDown);
            _colLeft = RunDetection(Vector2.left, out _hitsLeft);
            _colRight = RunDetection(Vector2.right, out _hitsRight);
            _hittingCeiling = RunDetection(Vector2.up, out _hitsUp);


            if (_grounded && !groundedCheck) {
                _timeLeftGrounded = _fixedFrame; // Only trigger when first leaving
                OnGroundedChanged?.Invoke(false);
            }
            else if (!_grounded && groundedCheck) {
                _coyoteUsable = true; // Only trigger when first touching
                _executedBufferedJump = false;
                _doubleJumpUsable = true;
                _canDash = true;
                OnGroundedChanged?.Invoke(true);
                _speed.y = 0;
            }

            _grounded = groundedCheck;

            bool RunDetection(Vector2 dir, out RaycastHit2D[] hits) 
            {
                // Array.Clear(hits, 0, hits.Length);
                // Physics2D.BoxCastNonAlloc(b.center, b.size, 0, dir, hits, _detectionRayLength, _groundLayer);

                // This produces garbage, but is significantly more performant. Also less buggy.
                hits = Physics2D.BoxCastAll(b.center, b.size, 0, dir, playerData._detectionRayLength, playerData._groundLayer);

                foreach (var hit in hits)
                    if (hit.collider && !hit.collider.isTrigger)
                        return true;
                return false;
            }
        }

        private Vector2 SetSideOffset()
        {
            Vector2 currentSideOffset = Vector2.zero;

            if (IsTurned)
            {
                currentSideOffset = new Vector2(-playerData.sideOffset.x, playerData.sideOffset.y);
            }
            else
            {
                currentSideOffset = new Vector2(playerData.sideOffset.x, playerData.sideOffset.y);
            }

            return currentSideOffset;
        }

        #endregion

        #region Crouch

        private bool CanStand 
        {
            get 
            {
                var col = Physics2D.OverlapBox((Vector2)transform.position + _defaultColliderOffset, _defaultColliderSize * 0.95f, 0, playerData._groundLayer);
                return (col == null || col.isTrigger);
            }
        }

        void CalculateCrouch() 
        {
            if (!playerData._allowCrouch) return;

            if (_crouching) 
            {
                var immediate = _velocityOnCrouch <= playerData._immediateCrouchSlowdownThreshold ? playerData._crouchSlowdownFrames : 0;
                var crouchPoint = Mathf.InverseLerp(0, playerData._crouchSlowdownFrames, _fixedFrame - _frameStartedCrouching + immediate);
                _frameClamp *= Mathf.Lerp(1, playerData._crouchSpeedModifier, crouchPoint);
            }

            if (_grounded && Input.Y < 0 && !_crouching && !wallGrab) 
            {
                _crouching = true;
                OnCrouchingChanged?.Invoke(true);
                _velocityOnCrouch = Mathf.Abs(_velocity.x);
                _frameStartedCrouching = _fixedFrame;

                _collider.size = _defaultColliderSize * new Vector2(1, playerData._crouchSizeModifier);

                // Lower the collider by the difference extent
                var difference = _defaultColliderSize.y - (_defaultColliderSize.y * playerData._crouchSizeModifier);
                _collider.offset = -new Vector2(0, difference * 0.5f);
            }
            else if (!_grounded || (Input.Y >= 0 && _crouching) || wallGrab) 
            {
                // Detect obstruction in standing area. Add a .1 y buffer to avoid the ground.
                if (!CanStand) return;

                _crouching = false;
                OnCrouchingChanged?.Invoke(false);

                _collider.size = _defaultColliderSize;
                _collider.offset = _defaultColliderOffset;
            }
        }

        #endregion

        #region HorizontalMovements

        private void CalculateHorizontal() {
            if (Input.X != 0) {
                // Set horizontal move speed
                if (playerData._allowCreeping) _speed.x = Mathf.MoveTowards(_speed.x, _frameClamp * Input.X, playerData._horizontalAcceleration * Time.fixedDeltaTime);
                else _speed.x += Input.X * playerData._horizontalAcceleration * Time.fixedDeltaTime;

                // Clamped by max frame movement
                _speed.x = Mathf.Clamp(_speed.x, -_frameClamp, _frameClamp);

                // Apply bonus at the apex of a jump
                var apexBonus = Mathf.Sign(Input.X) * playerData._apexBonus * _apexPoint;
                _speed.x += apexBonus * Time.fixedDeltaTime;
            }
            else {
                // No input. Let's slow the character down
                _speed.x = Mathf.MoveTowards(_speed.x, 0, playerData._horizontalDeceleration * Time.fixedDeltaTime);
            }

            if (!_grounded && (_speed.x > 0 && _colRight || _speed.x < 0 && _colLeft)) {
                // Don't pile up useless horizontal (prevents sticking to walls mid-air)
                _speed.x = 0;
            }
        }

        #endregion

        #region VerticalMovements

        private void CalculateVertical()
        {
            if (Input.Y != 0)
            {
                // Set vertical move speed
                _speed.y += Input.Y * playerData._verticalAcceleration * Time.fixedDeltaTime;

                // Clamped by max frame movement
                _speed.y = Mathf.Clamp(_speed.y, -_frameClamp, _frameClamp);
            }
            else
            {
                // No input. Let's slow the character down
                _speed.y = Mathf.MoveTowards(_speed.y, 0, playerData._horizontalDeceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void CalculateGravity() {
            if (_grounded) 
            {
                if (Input.X == 0)  return;

                // Slopes
                _speed.y = playerData._groundingForce;
                foreach (var hit in _hitsDown) {
                    if (hit.collider.isTrigger) continue;
                    var slopePerp = Vector2.Perpendicular(hit.normal).normalized;

                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    // This needs improvement. Prioritize front hit for smoother slope apex
                    if (slopeAngle != 0) {
                        _speed.y = _speed.x * -slopePerp.y;
                        _speed.y += playerData._groundingForce; // Honestly, this feels like cheating. I'll work on it
                        break;
                    }
                }
            }
            else if (!wallGrab)
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _speed.y > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

                // Fall
                _speed.y -= fallSpeed * Time.fixedDeltaTime;

                // Clamp
                if (_speed.y < playerData._fallClamp) _speed.y = playerData._fallClamp;
            }
        }

        #endregion

        #region Jump

        private void CalculateJumpApex() {
            if (!_grounded) {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(playerData._jumpApexThreshold, 0, Mathf.Abs(_velocity.y));
                _fallSpeed = Mathf.Lerp(playerData._minFallSpeed, playerData._maxFallSpeed, _apexPoint);
            }
            else {
                _apexPoint = 0;
            }
        }

        private void CalculateJump() {
            if (_crouching && !CanStand) return;

            if (_jumpToConsume && CanDoubleJump) {
                _speed.y = playerData._jumpHeight;
                _doubleJumpUsable = false;
                _endedJumpEarly = false;
                _jumpToConsume = false;
                OnDoubleJumping?.Invoke();
            }

            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if ((_jumpToConsume && CanUseCoyote) || HasBufferedJump) {
                _speed.y = playerData._jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _jumpToConsume = false;
                _timeLeftGrounded = _fixedFrame;
                _executedBufferedJump = true;
                OnJumping?.Invoke();
            }

            // End the jump early if button released
            if (!_grounded && !Input.JumpHeld && !_endedJumpEarly && _velocity.y > 0) _endedJumpEarly = true;

            if (_hittingCeiling && _speed.y > 0) _speed.y = 0;
        }

        #endregion

        #region Dash

        void CalculateDash() {
            if (!playerData._allowDash) return;
            if (_dashToConsume && _canDash && !_crouching) {
                var vel = new Vector2(Input.X, _grounded && Input.Y < 0 ? 0 : Input.Y).normalized;
                if (vel == Vector2.zero) return;
                _dashVel = vel * playerData._dashPower;
                _dashing = true;
                OnDashingChanged?.Invoke(true);
                _canDash = false;
                _startedDashing = _fixedFrame;

                // Strip external buildup
                _forceBuildup = Vector2.zero;
            }

            if (_dashing) {
                _speed.x = _dashVel.x;
                _speed.y = _dashVel.y;
                // Cancel when the time is out or we've reached our max safety distance
                if (_startedDashing + playerData._dashLength < _fixedFrame) {
                    _dashing = false;
                    OnDashingChanged?.Invoke(false);
                    if (_speed.y > 0) _speed.y = 0;
                    _speed.x *= playerData._dashEndHorizontalMultiplier;
                    if (_grounded) _canDash = true;
                }
            }

            _dashToConsume = false;
        }

        #endregion

        #region Climb

        private void CalculateClimb()
        {
            if(wallGrab)
            {
                CalculateVertical();

                OnClimbing.Invoke();
            }
        }

        #endregion

        #region Move

        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter() 
        {
            RawMovement = _speed; // Used externally
            var move = RawMovement * Time.fixedDeltaTime;

            // Apply effectors
            move += EvaluateEffectors();

            move += EvaluateForces();

            if(wallGrab)
            {
                move.x = 0;
            }
            else
            {
                SetFlip();
            }

            MoveApplyValue = move;

            _rb.MovePosition(_rb.position + move);
            OnWalking?.Invoke();


            RunCornerPrevention();
        }

        #region Corner Stuck Prevention

        // This is a little hacky, but it's very difficult to fix.
        // This will allow walking and jumping while right on the corner of a ledge.
        void RunCornerPrevention() {
            // There's a fiddly thing where the rays will not detect ground (right inline with the collider),
            // but the collider won't fit. So we detect if we're meant to be moving but not.
            // The downside to this is if you stand still on a corner and jump straight up, it won't trigger the land
            // when you touch down. Sucks... but not sure how to go about it at this stage
            _cornerStuck = !_grounded && _lastPos == _rb.position && _lastJumpPressed + 1 < _fixedFrame;
            _speed.y = _cornerStuck ? 0 : _speed.y;
            _lastPos = _rb.position;
        }

        #endregion

        #endregion

        #region Effectors & Forces

        /// For more passive force effects like moving platforms, underwater etc...
        private Vector2 EvaluateEffectors() {
            var effectorDirection = Vector2.zero;
            // Repeat this for other directions and possibly even area effectors. Wind zones, underwater etc
            effectorDirection += Process(_hitsDown);

            _usedEffectors.Clear();
            return effectorDirection;

            Vector2 Process(IEnumerable<RaycastHit2D> hits) {
                foreach (var hit2D in hits) {
                    if (!hit2D.transform) return Vector2.zero;
                    if (hit2D.transform.TryGetComponent(out IPlayerEffector effector)) {
                        if (_usedEffectors.Contains(effector)) continue;
                        _usedEffectors.Add(effector);
                        return effector.EvaluateEffector();
                    }
                }

                return Vector2.zero;
            }
        }

        public void AddForce(Vector2 force, PlayerForce mode = PlayerForce.Burst, bool cancelMovement = true) {
            if (cancelMovement) _speed = Vector2.zero;

            switch (mode) {
                case PlayerForce.Burst:
                    _speed += force;
                    break;
                case PlayerForce.Decay:
                    _forceBuildup += force * Time.fixedDeltaTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private Vector2 EvaluateForces() {
            // Prevent bouncing. This *could* cause problems, but I'm yet to find any
            if (_colLeft || _colRight) _forceBuildup.x = 0;
            if (_grounded || _hittingCeiling) _forceBuildup.y = 0;

            var force = _forceBuildup;

            _forceBuildup = Vector2.MoveTowards(_forceBuildup, Vector2.zero, playerData._forceDecay * Time.fixedDeltaTime);

            return force;
        }

        #endregion

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!_collider) _collider = GetComponent<BoxCollider2D>();

            Gizmos.color = Color.blue;

            if(playerData != null)
            {
                var b = _collider.bounds;
                b.Expand(playerData._detectionRayLength);

                Gizmos.DrawWireCube(b.center, b.size);

                Gizmos.color = Color.red;
                Vector2 currentSideOffset = SetSideOffset();
                Gizmos.DrawWireSphere((Vector2)transform.position + currentSideOffset, playerData.collisionRadius);
                Gizmos.DrawWireSphere((Vector2)transform.position + currentSideOffset + playerData.aboveSideOffset, playerData.collisionRadius);
            }

        }

        #endregion
    }
}