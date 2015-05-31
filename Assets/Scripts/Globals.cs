using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Globals : Singleton<Globals>
{
    [Range(60, 10 * 60)]
    public float totalTime = 3 * 60;
    [Range(1, 50)]
    public int mobsPassedToLose = 3;

    private float finishX;
    private float goalTime;
    private List<GameObject> _mobs = new List<GameObject>();
    private List<GameObject> _turrets = new List<GameObject>();
    private List<Placement> _placements = new List<Placement>();


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
        goalTime = totalTime;

        var f = GameObject.Find("Finish");
        if (f == null)
            Debug.LogError("Place Finish object to game scene.");
        else
            finishX = f.transform.position.x;
    }

    public void Start()
    {
        StartCoroutine(CheckPassedMobs());
    }

    private IEnumerator CheckPassedMobs()
    {
        while (true)
        {
            Mobs.Where(mob => mob.transform.position.x < Globals.instance.finishX)
                .ForEach(mob =>
                {
                    var hp = mob.GetComponent<MobHP>();
                    if (hp.curHP > 0)
                        passedMobs++;
                    hp.curHP = 0;
                });

            if (passedMobs >= mobsPassedToLose)
            {
                OnLose();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnLose()
    {
        StopAllCoroutines();
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