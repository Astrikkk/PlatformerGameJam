using System.Collections;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Transform Position1;
    public Transform Position2;
    public Transform DoorPart1;
    public Transform DoorPart2;
    public float openSpeed = 2f;

    private Vector3 doorPart1ClosedPos;
    private Vector3 doorPart2ClosedPos;
    private bool isOpening = false;
    private bool isOpen = false;

    private void Start()
    {
        doorPart1ClosedPos = DoorPart1.position;
        doorPart2ClosedPos = DoorPart2.position;
    }

    public void Open()
    {
        if (!isOpening && !isOpen)
        {
            StartCoroutine(OpenDoors());
        }
    }

    private IEnumerator OpenDoors()
    {
        isOpening = true;

        float time = 0f;
        Vector3 startPos1 = DoorPart1.position;
        Vector3 startPos2 = DoorPart2.position;

        while (time < 1f)
        {
            time += Time.deltaTime * openSpeed;

            DoorPart1.position = Vector3.Lerp(startPos1, Position1.position, time);
            DoorPart2.position = Vector3.Lerp(startPos2, Position2.position, time);

            yield return null;
        }

        DoorPart1.position = Position1.position;
        DoorPart2.position = Position2.position;

        isOpening = false;
        isOpen = true;
    }

    public void Close()
    {
        if (!isOpening && isOpen)
        {
            StartCoroutine(CloseDoors());
        }
    }

    private IEnumerator CloseDoors()
    {
        isOpening = true;

        float time = 0f;
        Vector3 startPos1 = DoorPart1.position;
        Vector3 startPos2 = DoorPart2.position;

        while (time < 1f)
        {
            time += Time.deltaTime * openSpeed;

            DoorPart1.position = Vector3.Lerp(startPos1, doorPart1ClosedPos, time);
            DoorPart2.position = Vector3.Lerp(startPos2, doorPart2ClosedPos, time);

            yield return null;
        }

        DoorPart1.position = doorPart1ClosedPos;
        DoorPart2.position = doorPart2ClosedPos;

        isOpening = false;
        isOpen = false;
    }
}