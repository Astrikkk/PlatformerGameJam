using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lamps : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private int direction = 1;

    void Update()
    {
        gameObject.GetComponent<Light>().intensity += direction * speed * Time.deltaTime;
        
        if (gameObject.GetComponent<Light>().intensity >= 0.7f)
        {
            gameObject.GetComponent<Light>().intensity = 0.7f;
            direction = -1;
        }
        else if (gameObject.GetComponent<Light>().intensity <= 0f)
        {
            gameObject.GetComponent<Light>().intensity = 0f;
            direction = 1;
        }

    }

    public float GetCurrentValue()
    {
        return gameObject.GetComponent<Light>().intensity;
    }
}
