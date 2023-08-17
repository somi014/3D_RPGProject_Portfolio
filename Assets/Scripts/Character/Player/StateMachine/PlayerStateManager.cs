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
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;

    [Tooltip("이동 방향으로 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Header("Sound Source")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    public AudioClip AttackAudioClips;
    [Range(0, 1)] 
    public float FootstepAudioVolume = 0.5f;
    [Range(0, 1)]
    public float AttackAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("점프 높이")]
    public float JumpHeight = 1.2f;

    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("다시 점프 가능할 때까지 필요한 시간(0f 점프 가능")]
    public float JumpTimeout = 0.50f;

    [Tooltip("하강 상태로 진입하기 전에 경과해야하는 시간")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    public bool Grounded = true;                        //CharacterController에서 수행x
    public float GroundedOffset = -0.14f;

    [Tooltip("CharacterController radius와 일치해야함")]
    public float GroundedRadius = 0.28f;

    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    public GameObject playerFollowCamera;
    public GameObject sideCamera;
    [Tooltip("카메라가 따라갈 플레이어 위치")]
    public GameObject CinemachineCameraTarget;

    public float TopClamp = 70.0f;                      //카메라 각도 위로
    public float BottomClamp = -30.0f;                  //카메라 각도 아래로

    [Tooltip("카메라 위치 추가 조정용")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("카메라 고정")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    [HideInInspector] 
    public int animIDSpeed;
    [HideInInspector] 
    public int animIDGrounded;
    [HideInInspector] 
    public int animIDJump;
    [HideInInspector] 
    public int animIDFreeFall;
    [HideInInspector]
    public int animIDMotionSpeed;
    [HideInInspector]
    public int animIDCombo;
    [HideInInspector] 
    public int animIDHeal;
    [HideInInspector]
    public int animIDDie;

#if ENABLE_INPUT_SYSTEM
    [HideInInspector] 
    public PlayerInput playerInput;
#endif

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public StatAttribute stats;

    private CharacterController controller;
    private GameObject mainCamera;
    private DamageDealer damageDealer;

    private const float threshold = 0.01f;         //오차 값

    private bool hasAnimator;
    [HideInInspector] 
    public bool deadProcess = false;                //죽었을 때 프로세스 진행했는지 여부

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
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
}