using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject Canvas2;
    public GameObject Canvas3;
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        Canvas.gameObject.SetActive(false);
        Canvas3.gameObject.SetActive(false);
        Canvas2.gameObject.SetActive(true);
    }

    public void Info()
    {
        Canvas.gameObject.SetActive(false);
        Canvas2.gameObject.SetActive(false);
        Canvas3.gameObject.SetActive(true);
    }

    public void Menu()
    {
        Canvas2.gameObject.SetActive(false);
        Canvas3.gameObject.SetActive(false);
        Canvas.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
