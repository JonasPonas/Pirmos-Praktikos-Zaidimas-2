using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int fuelPrice = 10;
    public float fuelEfficency = -0.001f; // -0.002 - hard //-0.001 - easy
}