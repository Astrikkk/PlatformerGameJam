using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class lamps : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private int direction = 1;
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite spriteOn;
    SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = transform.parent.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        gameObject.GetComponent<Light2D>().intensity += direction * speed * Time.deltaTime;
        
        if (gameObject.GetComponent<Light2D>().intensity >= 3f)
        {
            m_SpriteRenderer.sprite = spriteOn;
            gameObject.GetComponent<Light2D>().intensity = 3f;
            direction = -1;
        }
        else if (gameObject.GetComponent<Light2D>().intensity <= 0f)
        {
            m_SpriteRenderer.sprite = spriteOff;
            gameObject.GetComponent<Light2D>().intensity = 0f;
            direction = 1;
        }

    }

    public float GetCurrentValue()
    {
        return gameObject.GetComponent<Light2D>().intensity;
    }
}
