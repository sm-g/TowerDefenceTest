using System;
using System.Collections;
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

    public class GameManager : SingletonMB<GameManager>, INotifyPropertyChanged
    {
        private List<GameObject> _mobs = new List<GameObject>();
        private List<GameObject> _turrets = new List<GameObject>();
        private List<GameObject> _placements = new List<GameObject>();

        private int passedMobs;
        private int secToWin = -1;
        private GameState _state = GameState.Start;

        public event EventHandler Won = delegate { };
        public event EventHandler Lost = delegate { };
        public event EventHandler RoundStarted = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
                else if (value == Scripts.GameState.Playing)
                    RoundStarted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Сколько секунд осталось продержаться до победы.
        /// </summary>
        public int SecondsToWin
        {
            get
            {
                if (State == GameState.Playing)
                    secToWin = (int)(Globals.Instance.goalTime - Time.time);
                // stop update Time after win/lose

                return secToWin;
            }
        }

        /// <summary>
        /// Сколько осталось жизней.
        /// </summary>
        public int Lives
        {
            get
            {
                var res = Globals.Instance.livesAtStart - passedMobs;
                return res > 0 ? res : 0;
            }
        }

        public IEnumerable<GameObject> Mobs { get { return _mobs; } }

        public IEnumerable<GameObject> Turrets { get { return _turrets; } }

        public IEnumerable<GameObject> Placements { get { return _placements; } }

        private void Start()
        {
        }

        /// <summary>
        /// Начинает новый раунд.
        /// </summary>
        public void Restart()
        {
            State = Scripts.GameState.Playing;
            StartCoroutine(DoChecks());

            // _mobs.ForEach(x => GameObject.Destroy(x));
            //  _turrets.ForEach(x => GameObject.Destroy(x));
            //Application.LoadLevel(Application.loadedLevel);
            //  PassedMobs = 0;
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

        private IEnumerator DoChecks()
        {
            while (State == GameState.Playing)
            {
                GameManager.Instance.CheckPassedMobs();
                GameManager.Instance.CheckRound();
                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Проверяет мобов за финишем. Живой моб отнимает одну жизнь.
        /// Если жизней не осталось, игра заканчивается поражением.
        /// </summary>
        private void CheckPassedMobs()
        {
            Mobs.Where(mob => mob.transform.position.x < Globals.Instance.finishX)
                .ForEach(mob =>
                {
                    var hp = mob.GetComponent<MobHP>();
                    if (hp.curHP > 0)
                    {
                        passedMobs++;
                        OnPropertyChanged("Lives");
                    }

                    GameObject.Destroy(mob);
                });

            if (Lives == 0)
                State = Scripts.GameState.Lost;
        }

        /// <summary>
        /// Проверяет время раунда. Если вышло время, игра заканчивается победой.
        /// </summary>
        private void CheckRound()
        {
            if (SecondsToWin == 0)
            {
                // all mobs beaten
                _mobs.ForEach(x => GameObject.Destroy(x));
                State = Scripts.GameState.Won;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}