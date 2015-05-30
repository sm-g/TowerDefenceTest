using System;
using System.Linq;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public TurretAI Turret;
    public bool isSelected;

    private void OnMouseDown()
    {
        isSelected = true;
    }

    private void Awake()
    {
        Globals.instance.PlacementList.Add(gameObject);
    }

    private void OnDestroy()
    {
        Globals.instance.PlacementList.Remove(gameObject);
    }
}