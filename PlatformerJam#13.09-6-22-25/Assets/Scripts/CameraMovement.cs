using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject target;
    public float zoomSpeed = 1f;
    public float minZoom = 2f;
    public float maxZoom = 18f;
    private Vector3 offset;
    private Camera cam;
    void Start()
    {
        target = GameObject.FindWithTag("Player");
        offset = transform.position - target.transform.position;
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            target = GameObject.FindWithTag("Player");
            Vector3 targetPosition = target.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3);
        }
    }
    private void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0)
        {
            ZoomIn();
        }
        else if (scrollDelta < 0)
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        float newZoom = cam.orthographicSize - zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
    }

    private void ZoomOut()
    {
        float newZoom = cam.orthographicSize + zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
    }


}