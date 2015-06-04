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

        private int _finishedMobs;
        private int secToWin = -1;
        private float goalTime;
        private LevelSettings levelScript;
        private GameState _state = GameState.Start;

        public event EventHandler<EventArgs<GameState>> StateChanged = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public GameState State
        {
            get { return _state; }
            private set
            {
                if (_state == value) return;

                _state = value;
                Debug.Log("Game state = " + value);

                OnPropertyChanged("State");
                StateChanged(this, new EventArgs<GameState>(_state));
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
                    secToWin = (int)(goalTime - Time.timeSinceLevelLoad);
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
                var res = levelScript.livesAtStart - FinishedMobs;
                return res > 0 ? res : 0;
            }
        }

        /// <summary>
        /// Сколько мобов пересекли финиш.
        /// </summary>
        public int FinishedMobs
        {
            get { return _finishedMobs; }
            private set
            {
                _finishedMobs = value;

                Debug.LogFormat("finished mobs = {0}, lives = {1}", _finishedMobs, Lives);

                OnPropertyChanged("Lives");
            }
        }

        public IEnumerable<GameObject> Mobs { get { return _mobs; } }

        public IEnumerable<GameObject> Turrets { get { return _turrets; } }

        public IEnumerable<GameObject> Placements { get { return _placements; } }

        private void Awake()
        {
            DontDestroyOnLoad(Instance.gameObject);
            levelScript = GetComponent<LevelSettings>();

            StartCoroutine(DoCleanUps());
        }

        /// <summary>
        /// Начинает новый раунд.
        /// </summary>
        public void Restart()
        {
            // перезапуск
            State = Scripts.GameState.Start;
            ClearLevel();
            State = Scripts.GameState.Playing;

            StartCoroutine(DoPlayingChecks());
        }

        private void ClearLevel()
        {
            Debug.Log("clear level");

            _mobs.ForEach(x => GameObject.Destroy(x));
            _turrets.ForEach(x => GameObject.Destroy(x));

            // чтобы не было мобов за финишем
            _mobs.Clear();

            FinishedMobs = 0;
            goalTime = levelScript.totalTime + Time.timeSinceLevelLoad;
        }

        public void Register(GameObject go)
        {
            var placement = go.GetComponent<Placement>();
            if (placement != null)
                _placements.Add(go);

            var mob = go.GetComponent<MobHP>();
            if (mob != null)
                _mobs.Add(go);

            var turret = go.GetComponent<TurretAI>();
            if (turret != null)
                _turrets.Add(go);
        }

        public void Unregister(GameObject go)
        {
            //Debug.Log("unregister " + go);
            _placements.Remove(go);
            _mobs.Remove(go);
            _turrets.Remove(go);
        }

        private IEnumerator DoPlayingChecks()
        {
            while (State == GameState.Playing)
            {
                GameManager.Instance.CheckFinishedMobs();
                GameManager.Instance.CheckRound();
                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Разрушает объекты, которые больше не нужны.
        /// </summary>
        private IEnumerator DoCleanUps()
        {
            while (true)
            {
                if (State == GameState.Lost)
                {
                    // продолжается спаун мобов
                    _mobs.Where(mob => mob.transform.position.x < levelScript.finishX)
                        .ForEach(mob => GameObject.Destroy(mob));

                }
                else if (State == GameState.Won)
                {
                    // все мобы побеждены
                    _mobs.ForEach(x => GameObject.Destroy(x));
                }
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Проверяет мобов за финишем. Живой моб отнимает одну жизнь.
        /// Если жизней не осталось, игра заканчивается поражением.
        /// </summary>
        private void CheckFinishedMobs()
        {
            Mobs.Where(mob => mob.transform.position.x < levelScript.finishX)
                .ForEach(mob =>
                {
                    var hp = mob.GetComponent<MobHP>();
                    if (hp.curHP > 0)
                        FinishedMobs++;

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
                State = Scripts.GameState.Won;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}