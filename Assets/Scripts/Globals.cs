using System.Collections.Generic;
using UnityEngine;

public class Globals : Singleton<Globals>
{
    public List<GameObject> MobList = new List<GameObject>();

    public List<GameObject> TurretList = new List<GameObject>();

    public List<GameObject> PlacementList = new List<GameObject>();

    public float totalTime = 3 * 60;
    [HideInInspector]
    public float finishX;

    private float goalTime;

    public string timeToWin
    {
        get
        {
            var guiTime = (int)(goalTime - Time.time);

            int minutes = guiTime / 60;
            int seconds = guiTime % 60;

            var text = string.Format("{0:00}:{1:00}", minutes, seconds);
            return text;
        }
    }

    public int passedMobs { get; set; }

    public void Awake()
    {
        totalTime = Mathf.Max(totalTime, 60);
        goalTime = totalTime;

        var f = GameObject.Find("Finish");
        if (f == null)
            Debug.LogError("Place Finish object to game scene.");
        else
            finishX = f.transform.position.x;
    }

}