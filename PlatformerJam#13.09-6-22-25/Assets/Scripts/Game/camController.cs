using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class camController : MonoBehaviour
{
    public static Action<float, float, float> cameraShake;
    public static Action<float> changeCameraSizeEvent;
    public static Action<Transform> changeFollowTargetEvent;

    [HideInInspector] public CinemachineFramingTransposer transposer;
    private CinemachineBasicMultiChannelPerlin channelPerlin;

    private CinemachineVirtualCamera cam;

    private float camSize;
    public float leftOffset, rightOffset;

    [SerializeField] private float flipDuration = 1.0f;

    private bool isFlipped = false;
    private float targetDutch = 0f;

    void OnEnable()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        channelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cameraShake += shake;
        changeCameraSizeEvent += changeCameraSize;
        changeFollowTargetEvent += changeFollowTarget;
    }

    void OnDisable()
    {
        cameraShake -= shake;
        changeCameraSizeEvent -= changeCameraSize;
        changeFollowTargetEvent -= changeFollowTarget;
    }

    /* private void FixedUpdate()
    {
        if(!isFlipped)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transposer.m_ScreenX = leftOffset;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                transposer.m_ScreenX = rightOffset;
            }
        }
        else if(isFlipped)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                transposer.m_ScreenX = leftOffset;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                transposer.m_ScreenX = rightOffset;
            }
        }
    } */

    public void camToCenter()
    {
        transposer.m_ScreenX = 0.5f;
    }

    void shake(float strength, float time, float fadeTime)
    {
        StartCoroutine(shakeCam(strength, time, fadeTime));
    }

    void changeCameraSize(float newSize)
    {
        StopCoroutine(changeSize(newSize));
        camSize = cam.m_Lens.OrthographicSize;
        StartCoroutine(changeSize(newSize));
    }

    void changeFollowTarget(Transform followObject)
    {
        if (followObject != null) cam.m_Follow = followObject;
    }

    private IEnumerator changeSize(float newSize)
    {
        if (cam.m_Lens.OrthographicSize == newSize) yield break;

        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(camSize, newSize, EaseInOut(i));
            yield return null;
        }
    }

    private IEnumerator shakeCam(float strength, float time, float fadeTime)
    {
        float originStrength = strength;
        channelPerlin.m_AmplitudeGain = strength;

        yield return new WaitForSeconds(time);

        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            strength -= Time.deltaTime * originStrength / fadeTime;
            channelPerlin.m_AmplitudeGain = strength;
            yield return null;
        }
        channelPerlin.m_AmplitudeGain = 0;
    }

    public void changeGravity()
    {
        targetDutch = Mathf.Approximately(cam.m_Lens.Dutch, 0f) ? 180f : 0f;
        StartCoroutine(FlipDutchCoroutine(targetDutch, flipDuration));
    }

    private IEnumerator FlipDutchCoroutine(float target, float duration)
    {
        float startDutch = cam.m_Lens.Dutch;
        float timeElapsed = 0f;
        if(transposer.m_ScreenY == 0.2f)
        {
            transposer.m_ScreenY = 0.7f;
            isFlipped = false;
        }   
        else if(transposer.m_ScreenY == 0.7f)
        {
            transposer.m_ScreenY = 0.2f;
            isFlipped = true;
        }

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float currentDutch = Mathf.Lerp(startDutch, target, t);
            cam.m_Lens.Dutch = currentDutch;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cam.m_Lens.Dutch = target;
    }


    //Функция сглаживания
    float EaseInOut(float x)
    {
        return x < 0.5 ? x * x * 2 : (1 - (1 - x) * (1 - x) * 2);
    }
}
