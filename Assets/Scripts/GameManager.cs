using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;


public class GameManager : INotifyPropertyChanged
{
    private static GameManager _instance;

    private List<GameObject> _mobs = new List<GameObject>();
    private List<GameObject> _turrets = new List<GameObject>();
    private List<GameObject> _placements = new List<GameObject>();

    private int _passedMobs;

    public event EventHandler Won;

    public event EventHandler Lost;

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
    public int PassedMobs
    {
        get { return _passedMobs; }
        private set
        {
            _passedMobs = value;
            OnPropertyChanged("Lives");
        }
    }

    public int Lives
    {
        get
        {
            var res = Globals.instance.livesAtStart - PassedMobs;
            return res > 0 ? res : 0;
        }
    }

    public IEnumerable<GameObject> Mobs { get { return _mobs; } }

    public IEnumerable<GameObject> Turrets { get { return _turrets; } }

    public IEnumerable<GameObject> Placements { get { return _placements; } }

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

    public void CheckRound()
    {
        if (SecondsToWin == 0)
            OnWon(EventArgs.Empty);
    }

    public void Restart()
    {

    }

    public void Register(GameObject go)
    {
        var placement = go.GetComponent<Placement>();
        if (placement != null)
        {
            _placements.Add(go);
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
        _placements.Remove(go);
        _mobs.Remove(go);
        _turrets.Remove(go);
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

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    private static bool IsFinished(GameObject mob)
    {
        return mob.transform.position.x < Globals.instance.finishX;
    }
}