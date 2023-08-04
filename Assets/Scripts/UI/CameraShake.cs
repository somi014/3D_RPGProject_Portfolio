using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public float shakeTime = 1f;
    
    public float IdleAmplitude = 0.1f;
    public float IdleFrequency = 1f;

    public float DefaultShakeAmplitude = 0.5f;
    public float DefaultShakeFrequency = 10f;

    protected Vector3 _initialPosition;
    protected Quaternion _initialRotation;

    protected CinemachineBasicMultiChannelPerlin perlin;
    protected CinemachineVirtualCamera _virtualCamera;

    protected virtual void Awake()
    {
        _virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        perlin = _virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }


    protected virtual void Start()
    {
        CameraReset();
    }

    /// <summary>
    ///카메라 흔들기
    /// </summary>
    /// <param name="duration">Duration.</param>
    public virtual void ShakeCamera(float duration)
    {
        StartCoroutine(ShakeCameraCo(shakeTime, DefaultShakeAmplitude, DefaultShakeFrequency));
    }

    /// <summary>
    /// 카메라 흔들기
    /// </summary>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    public virtual void ShakeCamera(float duration, float amplitude, float frequency)
    {
        StartCoroutine(ShakeCameraCo(duration, amplitude, frequency));
    }

    /// <summary>
    /// 카메라 흔들기 코루틴
    /// </summary>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    protected virtual IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency)
    {
        float time = duration;

        perlin.m_AmplitudeGain = amplitude;
        do
        {
            float value = Mathf.Lerp(amplitude, IdleAmplitude, 1 - (time / duration));
            yield return null;
            time -= Time.deltaTime;
            perlin.m_AmplitudeGain = value;
            //perlin.m_FrequencyGain = frequency;

        } while (time > 0f);

        //yield return new WaitForSeconds(duration);
        CameraReset();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public virtual void CameraReset()
    {
        perlin.m_AmplitudeGain = IdleAmplitude;
        //perlin.m_FrequencyGain = IdleFrequency;
    }
}