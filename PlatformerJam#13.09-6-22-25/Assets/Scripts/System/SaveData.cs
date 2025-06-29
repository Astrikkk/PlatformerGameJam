using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class SaveData
{
    public int CurrentLevel;
    public float[] PlayerPosition; // Stored as [x, y, z] array

    public SaveData()
    {
        CurrentLevel = 1;
        PlayerPosition = new float[3] { 0, 0, 0 };
    }

    // Helper method to convert to Vector3
    public Vector3 GetPositionVector()
    {
        return new Vector3(PlayerPosition[0], PlayerPosition[1], PlayerPosition[2]);
    }

    // Helper method to set from Vector3
    public void SetPositionVector(Vector3 position)
    {
        PlayerPosition = new float[3] { position.x, position.y, position.z };
    }
}