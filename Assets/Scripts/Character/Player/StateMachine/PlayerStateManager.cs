using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public partial class PlayerStateManager
{
    #region 상태 
    public IdleState idlingState = new IdleState();
    public JumpState jumpState = new JumpState();
    public RollState rollState = new RollState();
    public AttackState attackState = new AttackState();
    public DamageState damageState = new DamageState();
    public HealState healState = new HealState();
    #endregion

    public PlayerBaseState currentState;                        //현재 상태
    public TextMeshProUGUI stateText;                           //상태 표시용

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Header("Sound Source")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public AudioClip AttackAudioClips;
    [Range(0, 1)] public float AttackAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    [HideInInspector] 
    public int _animIDSpeed;
    [HideInInspector] 
    public int _animIDGrounded;
    [HideInInspector] 
    public int _animIDJump;
    [HideInInspector] 
    public int _animIDFreeFall;
    [HideInInspector]
    public int _animIDMotionSpeed;
    [HideInInspector]
    public int _animIDCombo;
    [HideInInspector] 
    public int _animIDHeal;
    [HideInInspector]
    public int _animIDDie;

#if ENABLE_INPUT_SYSTEM
    [HideInInspector] 
    public PlayerInput _playerInput;
#endif

    [HideInInspector]
    public Animator _animator;
    [HideInInspector]
    public StatAttribute stats;

    private CharacterController _controller;
    private GameObject _mainCamera;
    private DamageDealer damageDealer;

    private const float _threshold = 0.01f;         //오차 값

    private bool _hasAnimator;
    [HideInInspector] 
    public bool deadProcess = false;                //죽었을 때 프로세스 진행했는지 여부

    [Header("Camera")]
    public GameObject playerFollowCamera;
    public GameObject sideCamera;

    [Header("Spawn")]
    public Transform respawn_tr;

    [Header("Particle")]
    [SerializeField]
    private ParticleSystem[] particle;              //공격 파티클
    [SerializeField]
    private ParticleSystem heal_particle;           //회복 파티클
    [SerializeField]
    private ParticleSystem level_particle;          //레벨 업 파티클

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
}