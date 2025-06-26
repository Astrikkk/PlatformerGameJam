using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isEdible;
    public bool isDead;

    public void startDissApearing()
    {
        StartCoroutine(dissAppear());
    }

    public IEnumerator dissAppear()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
