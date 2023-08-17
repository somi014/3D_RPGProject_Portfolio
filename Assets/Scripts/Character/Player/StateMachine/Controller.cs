using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public partial class PlayerStateManager : MonoBehaviour
{
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    private void Start()
    {
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
#if ENABLE_INPUT_SYSTEM
        playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Player input 확인");
#endif
        stats = GetComponent<StatAttribute>();
        damageDealer = GetComponentInChildren<DamageDealer>();

        AssignAnimationIDs();

        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    private void OnEnable()
    {
        transform.position = respawn_tr.position;
        transform.rotation = respawn_tr.rotation;

        currentState = idlingState;
        SwitchState(currentState);
    }

    private void Update()
    {
        if (stats.isDead == true)
        {
            if (deadProcess == false)
            {
                PlayerDie();
            }
            return;
        }

        if (uiOpen == true)
        {
            return;
        }

        hasAnimator = TryGetComponent(out animator);

        JumpAndGravity();
        GroundedCheck();
        Move();

        currentState.UpdateState(this);
        currentState.HandleInput(this);
    }

    private void LateUpdate()
    {
        if (stats.isDead == true)
        {
            return;
        }

        if (uiOpen == true)
        {
            return;
        }

        CameraRotation();
    }

    void PlayerDie()
    {
        deadProcess = true;

        GameEventsManager.instance.playerEvents.PlayerDead();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDCombo = Animator.StringToHash("Combo");
        animIDHeal = Animator.StringToHash("Heal");
        animIDDie = Animator.StringToHash("Die");
    }

    /// <summary>
    /// 바닥 체크
    /// </summary>
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (hasAnimator == true)
        {
            animator.SetBool(animIDGrounded, Grounded);
        }
    }

    /// <summary>
    /// 상태 변환하기
    /// </summary>
    /// <param name="state"></param>
    public void SwitchState(PlayerBaseState state)
    {
        stateText.text = state.ToString();

        currentState.ExitState(this);

        currentState = state;
        state.EnterState(this);
    }


    #region Movement
    private void CameraRotation()
    {
        //마우스 움직임이 있고 카메라 고정 아닐때
        if (look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            //마우스 입력일 때는 1.0f
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += look.x * deltaTimeMultiplier;
            cinemachineTargetPitch += look.y * deltaTimeMultiplier;
        }

        //회전 제한
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        //타겟 따라서 회전
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        float targetSpeed = sprint ? SprintSpeed : MoveSpeed;

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // => Vector2 == 연산이 부동 소수점 오류 발생 ↓ magnitude보다 비용 ↓
        if (move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        //플레이어의 현재 horizontal velocity에 따라서
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = analogMovement ? move.magnitude : 1f;

        // 목표 스피드까지 가속 or 감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);     //스피드 값 수정
            speed = Mathf.Round(speed * 1000f) / 1000f;                                                                     //소수점 아래 3 반올림
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f)
        {
            animationBlend = 0f;
        }

        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

        // 이동 중 회전
        if (move != Vector2.zero && currentState != rollState)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        if (currentState == rollState)
        {
            controller.Move(transform.forward * (5f * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);                //앞으로 구르기
        }
        else
        {
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);    //이동

            if (hasAnimator == true)
            {
                animator.SetFloat(animIDSpeed, animationBlend);
                animator.SetFloat(animIDMotionSpeed, inputMagnitude);
            }
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded == true)
        {
            fallTimeoutDelta = FallTimeout;                         //초기화

            if (hasAnimator == true)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
            }

            if (verticalVelocity < 0.0f)                            //바닥에 닿았을 때 
            {
                verticalVelocity = -2f;
            }

            //점프
            if (jump && jumpTimeoutDelta <= 0.0f)
            {
                //목표 높이까지 도달하기 위해 필요한 velocity
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                animator.SetBool(animIDJump, true);
            }

            //점프 끝
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = JumpTimeout;                         //점프 타임 아웃 초기화

            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else                                                    //하강 중
            {
                animator.SetBool(animIDFreeFall, true);
            }

            jump = false;
        }

        //목표 값 아래면 시간에 따라 중력 적용
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded == true)
        {
            Gizmos.color = transparentGreen;
        }
        else
        {
            Gizmos.color = transparentRed;
        }

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    /// <summary>
    /// 사이드 카메라 활성화
    /// </summary>
    /// <param name="sideOn"></param>
    public void CamaraChange(bool sideOn)
    {
        LockCameraPosition = sideOn;

        if (sideOn == false)
        {
            playerFollowCamera.SetActive(true);
            sideCamera.SetActive(false);
        }
        else
        {
            playerFollowCamera.SetActive(false);
            sideCamera.SetActive(true);
        }
    }

    #region Animation Event
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                SoundManager.instance.Play(FootstepAudioClips[index], Sound.Effect, FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            SoundManager.instance.Play(LandingAudioClip, Sound.Effect);
        }
    }

    private void StartDealDamage(AnimationEvent animationEvent)
    {
        damageDealer.StartDealDamage();
    }

    private void EndDealDamage(AnimationEvent animationEvent)
    {
        damageDealer.EndDealDamage();
    }

    private void OnAttackEffect(AnimationEvent animationEvent)
    {
        particle[animationEvent.intParameter].Play();
    }

    /// <summary>
    /// Attack 2 event
    /// </summary>
    /// <param name="index"></param>
    private void AttackEffect(int index)
    {
        particle[index].Play();
    }

    private void AttackSound()
    {
        SoundManager.instance.Play(AttackAudioClips, Sound.Effect, AttackAudioVolume);
    }

    private void HealEffect()
    {
        heal_particle.Play();
        stats.LifeRegeneration();
    }

    private void AnimationEnd()
    {
        SwitchState(idlingState);
    }
    #endregion

    public void LevelEffect()
    {
        level_particle.Play();
    }
}