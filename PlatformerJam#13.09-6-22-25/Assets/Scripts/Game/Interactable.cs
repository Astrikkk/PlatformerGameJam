using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected bool isActive = false;
    [SerializeField] protected GameObject ObjectsUI; // put canvas here


    private void ChangeActive(bool active)
    {
        isActive = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerScript player = collision.GetComponent<PlayerScript>();
        if (player != null)
        {
            ChangeActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerScript player = collision.GetComponent<PlayerScript>();
        if (player != null)
        {
            ChangeActive(false);
        }
    }
}
