using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameState
    {
        Start,
        Playing,
        Won,
        Lost
    }

    public class GameManager : INotifyPropertyChanged
    {
        private static GameManager _instance;

        private List<GameObject> _mobs = new List<GameObject>();
        private List<GameObject> _turrets = new List<GameObject>();
        private List<GameObject> _placements = new List<GameObject>();

        private int _passedMobs;

        private GameState _state;

        public event EventHandler Won = delegate { };

        public event EventHandler Lost = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager() { State = GameState.Playing };
                }

                return _instance;
            }
        }
        public GameState State
        {
            get { return _state; }
            private set
            {
                if (_state == value) return;

                _state = value;
                OnPropertyChanged("State");
                if (value == Scripts.GameState.Won)
                    Won(this, EventArgs.Empty);
                else if (value == Scripts.GameState.Lost)
                    Lost(this, EventArgs.Empty);
            }
        }

        public int SecondsToWin
        {
            get
            {
                return (int)(Globals.instance.goalTime - Time.time);
            }
        }

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
                State = Scripts.GameState.Lost;
        }

        public void CheckRound()
        {
            if (SecondsToWin == 0)
                State = Scripts.GameState.Won;
        }

        public void Restart()
        {
            State = Scripts.GameState.Playing;
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private static bool IsFinished(GameObject mob)
        {
            return mob.transform.position.x < Globals.instance.finishX;
        }
    }
}