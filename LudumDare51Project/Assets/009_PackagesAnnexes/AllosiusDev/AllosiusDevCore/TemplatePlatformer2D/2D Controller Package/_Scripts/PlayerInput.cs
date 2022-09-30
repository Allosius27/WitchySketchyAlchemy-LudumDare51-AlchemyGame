using UnityEngine;
#if (ENABLE_INPUT_SYSTEM)
using UnityEngine.InputSystem;
#endif

namespace AllosiusDevCore.Controller2D {
    public class PlayerInput : MonoBehaviour {
#if (ENABLE_LEGACY_INPUT_MANAGER)
        public FrameInput GatherInput() {
            return new FrameInput {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                DashDown = Input.GetButtonDown("Dash"),

                WallGrabDown = Input.GetButtonDown("Climb"),

                X = Input.GetAxisRaw("Horizontal"),
                Y = Input.GetAxisRaw("Vertical")
            };
        }
#elif (ENABLE_INPUT_SYSTEM)
        private PlayerInputActions _actions;
        private InputAction _move, _jump, _dash;

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _move = _actions.Player.Move;
            _jump = _actions.Player.Jump;
            _dash = _actions.Player.Dash;
        }

        private void OnEnable() => _actions.Enable();

        private void OnDisable() => _actions.Disable();

        public FrameInput GatherInput() {
            return new FrameInput {
                JumpDown = _jump.WasPressedThisFrame(),
                JumpHeld = _jump.IsPressed(),
                DashDown = _dash.WasPressedThisFrame(),

                X = _move.ReadValue<Vector2>().x,
                Y = _move.ReadValue<Vector2>().y
            };
        }
#endif
    }
}