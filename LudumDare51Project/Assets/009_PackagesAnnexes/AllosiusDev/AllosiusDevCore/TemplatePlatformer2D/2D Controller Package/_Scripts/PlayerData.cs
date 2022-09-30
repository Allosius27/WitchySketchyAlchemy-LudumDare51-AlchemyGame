using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.AllosiusDevCore.Controller2D
{
    [CreateAssetMenu(fileName = "New PlayerData", menuName = "TemplateController2D/PlayerData")]
    public class PlayerData : SerializedScriptableObject
    {
        #region Fields

        protected const string LEFT_VERTICAL_GROUP = "Split/Left";
        protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings";
        protected const string CARACS_BOX_GROUP = "Split/Left/Basic Caracs";
        protected const string HORIZONTAL_MOVEMENTS_GROUP = "Split/Left/Walking";
        protected const string VERTICAL_MOVEMENTS_GROUP = "Split/Left/Climb";
        protected const string JUMPING_GROUP = "Split/Left/Jumping";

        protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
        protected const string NOTES_GROUP = "Split/Right/Notes";
        protected const string COLLISIONS_GROUP = "Split/Right/Collisions";
        protected const string GRAVITY_GROUP = "Split/Right/Gravity";
        protected const string CROUCH_GROUP = "Split/Right/Crouch";
        protected const string DASH_GROUP = "Split/Right/Dash";
        protected const string EFFECTORS_GROUP = "Split/Right/Effectors";

        #endregion

        #region UnityInspector

        #region LeftVerticalGroup

        [HideLabel, PreviewField(55)]
        [VerticalGroup(LEFT_VERTICAL_GROUP)]

        #region GeneralSettings

        [HorizontalGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Split", 55, LabelWidth = 67)]
        public GameObject Visual;

        [BoxGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Split/Right")]
        public string Name;

        #endregion

        #region Stats

        [BoxGroup(CARACS_BOX_GROUP)]
        public bool _allowDoubleJump;

        [BoxGroup(CARACS_BOX_GROUP)]
        public bool _allowDash;

        [BoxGroup(CARACS_BOX_GROUP)]
        public bool _allowCrouch;

        #endregion

        #region HorizontalMovementsVariables

        [BoxGroup(HORIZONTAL_MOVEMENTS_GROUP)]
        public float _horizontalAcceleration = 120;

        [BoxGroup(HORIZONTAL_MOVEMENTS_GROUP)]
        public float _moveClamp = 13;

        [BoxGroup(HORIZONTAL_MOVEMENTS_GROUP)]
        public float _horizontalDeceleration = 60f;

        [BoxGroup(HORIZONTAL_MOVEMENTS_GROUP)]
        public float _apexBonus = 100;

        [BoxGroup(HORIZONTAL_MOVEMENTS_GROUP)]
        public bool _allowCreeping;

        #endregion

        #region VerticalMovementsVariables

        [BoxGroup(VERTICAL_MOVEMENTS_GROUP)]
        public float _verticalAcceleration = 60;

        [BoxGroup(VERTICAL_MOVEMENTS_GROUP)]
        public float _verticalDeceleration = 30f;

        #region ClimbVariables

        [BoxGroup(VERTICAL_MOVEMENTS_GROUP)]
        public LayerMask ClimbLayerMask;

        #endregion

        #endregion

        #region JumpVariables

        [BoxGroup(JUMPING_GROUP)]
        public float _jumpHeight = 35;

        [BoxGroup(JUMPING_GROUP)]
        public float _jumpApexThreshold = 40f;

        [BoxGroup(JUMPING_GROUP)]
        public int _coyoteTimeThreshold = 7;

        [BoxGroup(JUMPING_GROUP)]
        public int _jumpBuffer = 7;

        [BoxGroup(JUMPING_GROUP)]
        public float _jumpEndEarlyGravityModifier = 3;

        #endregion

        #endregion

        #region RightVerticalGroup

        [VerticalGroup(RIGHT_VERTICAL_GROUP)]

        #region Notes

        [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        [BoxGroup(NOTES_GROUP)]
        [HideLabel, TextArea(4, 9)]
        public string Notes;

        #endregion

        #region CollisionsVariables

        [BoxGroup(COLLISIONS_GROUP)]
        public LayerMask _groundLayer;

        [BoxGroup(COLLISIONS_GROUP)]
        public float _detectionRayLength = 0.1f;

        [BoxGroup(COLLISIONS_GROUP)]
        public float collisionRadius;

        [BoxGroup(COLLISIONS_GROUP)]
        public Vector2 sideOffset;
        [BoxGroup(COLLISIONS_GROUP)]
        public Vector2 aboveSideOffset;

        #endregion

        #region GravityVariables

        [BoxGroup(GRAVITY_GROUP)]
        public float _fallClamp = -60f;

        [BoxGroup(GRAVITY_GROUP)]
        public float _minFallSpeed = 80f;

        [BoxGroup(GRAVITY_GROUP)]
        public float _maxFallSpeed = 160f;

        [BoxGroup(GRAVITY_GROUP)]
        [ProgressBar(0, -10), ShowInInspector]
        public float _groundingForce = -1.5f;

        #endregion

        #region CrouchVariables

        [BoxGroup(CROUCH_GROUP)]
        public float _crouchSizeModifier = 0.5f;

        [BoxGroup(CROUCH_GROUP)]
        public float _crouchSpeedModifier = 0.1f;

        [BoxGroup(CROUCH_GROUP)]
        public int _crouchSlowdownFrames = 50;

        [BoxGroup(CROUCH_GROUP)]
        public float _immediateCrouchSlowdownThreshold = 0.1f;

        #endregion

        #region DashVariables

        [BoxGroup(DASH_GROUP)]
        public float _dashPower = 30;

        [BoxGroup(DASH_GROUP)]
        public int _dashLength = 6;

        [BoxGroup(DASH_GROUP)]
        public float _dashEndHorizontalMultiplier = 0.25f;

        #endregion

        #region Effectors & Forces Variables

        [BoxGroup(EFFECTORS_GROUP)]
        public float _forceDecay = 1;

        #endregion

        #endregion

        #endregion
    }
}

