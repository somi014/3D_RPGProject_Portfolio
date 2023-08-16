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

    protected Vector3 initialPosition;
    protected Quaternion initialRotation;

    protected CinemachineBasicMultiChannelPerlin perlin;
    protected CinemachineVirtualCamera virtualCamera;

    private  void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        CameraReset();
    }

    /// <summary>
    ///카메라 흔들기
    /// </summary>
    /// <param name="duration">Duration.</param>
    public void ShakeCamera(float duration)
    {
        StartCoroutine(ShakeCameraCo(shakeTime, DefaultShakeAmplitude, DefaultShakeFrequency));
    }

    /// <summary>
    /// 카메라 흔들기
    /// </summary>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    public void ShakeCamera(float duration, float amplitude, float frequency)
    {
        StartCoroutine(ShakeCameraCo(duration, amplitude, frequency));
    }

    /// <summary>
    /// 카메라 흔들기 코루틴
    /// </summary>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    private IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency)
    {
        float time = duration;

        perlin.m_AmplitudeGain = amplitude;
        do
        {
            float value = Mathf.Lerp(amplitude, IdleAmplitude, 1 - (time / duration));
           
            yield return null;
            time -= Time.deltaTime;
          
            perlin.m_AmplitudeGain = value;

        } while (time > 0f);

        CameraReset();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public void CameraReset()
    {
        perlin.m_AmplitudeGain = IdleAmplitude;
    }
}