using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : MonoBehaviour
{
    /* [SerializeField] GameObject lightHolder;

    void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.name == "Player")
        {
            lightHolder.SetActive(true);
        }
						
	}

    void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.name == "Player")
        {
            lightHolder.SetActive(false);
        }
						
	} */

    public void DeActivateObjects(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
