using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Globals : Singleton<Globals>
{
    public float totalTime = 3 * 60;
    [HideInInspector]
    public float finishX;

    private List<GameObject> _mobs = new List<GameObject>();
    private List<GameObject> _turrets = new List<GameObject>();
    private List<Placement> _placements = new List<Placement>();

    private float goalTime;

    public IEnumerable<GameObject> Mobs { get { return _mobs; } }

    public IEnumerable<GameObject> Turrets { get { return _turrets; } }

    public IEnumerable<Placement> Placements { get { return _placements; } }

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

    public void Register(GameObject go)
    {
        var placement = go.GetComponent<Placement>();
        if (placement != null)
        {
            placement.SelectedChanged += placement_SelectedChanged;
            _placements.Add(placement);
        }

        var mob = go.GetComponent<MobHP>();
        if (mob != null)
        {
            _mobs.Add(go);
        }

        var turret = go.GetComponent<TurretAI>();
        if (turret != null)
        {
            _turrets.Add(go);
        }
    }

    public void Unregister(GameObject go)
    {
        var placement = go.GetComponent<Placement>();
        if (placement != null)
        {
            placement.SelectedChanged -= placement_SelectedChanged;
            _placements.Remove(placement);
        }

        var mob = go.GetComponent<MobHP>();
        if (mob != null)
        {
            _mobs.Remove(go);
        }

        var turret = go.GetComponent<TurretAI>();
        if (turret != null)
        {
            _turrets.Remove(go);
        }
    }

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

    private void placement_SelectedChanged(object sender, System.EventArgs e)
    {
        var p = sender as Placement;
        if (p.isSelected)
        {
            Placements.Except(p).ForAll(x => x.isSelected = false);

            Graphics.instance.showTurretMenu = true;
        }

        if (Placements.All(x => !x.isSelected))
        {
            Graphics.instance.showTurretMenu = false;
        }
    }
}