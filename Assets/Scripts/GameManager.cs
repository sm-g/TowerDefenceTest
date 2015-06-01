using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager
{
    private static GameManager _instance;

    private List<GameObject> _mobs = new List<GameObject>();
    private List<GameObject> _turrets = new List<GameObject>();
    private List<Placement> _placements = new List<Placement>();

    public event EventHandler Won;
    public event EventHandler Lost;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
        }
    }
    public int SecondsToWin { get { return (int)(Globals.instance.goalTime - Time.time); } }

    public int PassedMobs { get; private set; }
    public int Lives { get { return Globals.instance.livesAtStart - PassedMobs; } }


    public IEnumerable<GameObject> Mobs { get { return _mobs; } }

    public IEnumerable<GameObject> Turrets { get { return _turrets; } }

    public IEnumerable<Placement> Placements { get { return _placements; } }

    public void CheckPassedMobs()
    {
        Mobs.Where(mob => IsFinished(mob))
            .ForEach(mob =>
            {
                var hp = mob.GetComponent<MobHP>();
                if (hp.curHP > 0)
                    PassedMobs++;
                hp.curHP = 0; // kill?
            });

        if (Lives == 0)
        {
            OnLost(EventArgs.Empty);
        }
    }

    private static bool IsFinished(GameObject mob)
    {
        return mob.transform.position.x < Globals.instance.finishX;
    }

    public void CheckRound()
    {
        if (SecondsToWin == 0)
            OnWon(EventArgs.Empty);
    }

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

    protected virtual void OnLost(EventArgs e)
    {
        var h = Lost;
        if (h != null)
        {
            h(this, e);
        }
    }

    protected virtual void OnWon(EventArgs e)
    {
        var h = Won;
        if (h != null)
        {
            h(this, e);
        }
    }
    private void placement_SelectedChanged(object sender, System.EventArgs e)
    {
        var p = sender as Placement;
        if (p.IsSelected)
        {
            Placements.Except(p).ForAll(x => x.IsSelected = false);

            Graphics.instance.showTurretMenu = true;
        }

        if (Placements.All(x => !x.IsSelected))
        {
            Graphics.instance.showTurretMenu = false;
        }
    }
}