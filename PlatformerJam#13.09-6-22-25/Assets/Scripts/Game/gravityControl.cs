using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityControl : MonoBehaviour
{
    [SerializeField] private camController _cm;


    public void InvertGravityForAllRigidbodies()
    {
        Rigidbody2D[] allRigidbodies = FindObjectsOfType<Rigidbody2D>();
        
        foreach (Rigidbody2D rb in allRigidbodies)
        {
            rb.gravityScale = -rb.gravityScale;
        }

        _cm.changeGravity();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InvertGravityForAllRigidbodies();
        }
    }
}
