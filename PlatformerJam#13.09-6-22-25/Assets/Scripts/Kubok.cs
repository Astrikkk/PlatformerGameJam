using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kubok : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager.WinMenu.SetActive(true);
    }
}
