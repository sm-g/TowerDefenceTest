using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GuiManager : MonoBehaviour
    {
        public Button BuildTurretBtnPrefab;

        private static char liveChar = '+';
        private Placement _selectedPlace;
        private GameObject buildPanel;
        private GameObject lostPanel;
        private GameObject winPanel;
        private GameObject shadowPanel;
        private Text livesText;
        private Text timerText;
        private GameObject statPanel;

        public Placement SelectedPlace
        {
            get { return _selectedPlace; }
            set
            {
                if (buildPanel != null)
                {
                    // показываем панель строительства для выбранного места
                    buildPanel.SetActive(value != null);
                }

                _selectedPlace = value;
            }
        }

        public void OnRestartClick()
        {
            GameManager.Instance.Restart();
        }

        public void OnBuildClick(GameObject turret)
        {
            if (SelectedPlace != null)
            {
                SelectedPlace.SetTurret(turret);
                SelectedPlace.IsSelected = false;
            }
        }

        private void Awake()
        {
            if (BuildTurretBtnPrefab == null)
                Debug.LogError("Add BuildTurretBtnPrefab to " + typeof(GuiManager));

            FindGuiElements();

            AddListeners();

            CreateBuildButtons();

            Placement.SelectedChanged += (s, e) =>
            {
                if (e.arg.IsSelected)
                    SelectedPlace = e.arg;
                else if (SelectedPlace != null && !SelectedPlace.IsSelected) // сняли выделение
                    SelectedPlace = null;
            };

            GameManager.Instance.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Lives":
                        livesText.text = GetLivesString();
                        break;

                    case "State":
                        SetVisibility(GameManager.Instance.State);
                        break;
                }
            };

            SetVisibility(GameManager.Instance.State);
            StartCoroutine(RefreshTimer(() => timerText.text = GetTimeToWinString()));
        }

        private void AddListeners()
        {
            var restartBtn = GameObject.Find("RestartBtn");
            restartBtn.GetComponent<Button>().onClick.AddListener(OnRestartClick);
            lostPanel.GetComponent<Button>().onClick.AddListener(OnRestartClick);
            winPanel.GetComponent<Button>().onClick.AddListener(OnRestartClick);
        }

        private void FindGuiElements()
        {
            buildPanel = GameObject.Find("BuildPanel");
            statPanel = GameObject.Find("StatPanel");
            shadowPanel = GameObject.Find("ShadowPanel");
            winPanel = GameObject.Find("WinText");
            lostPanel = GameObject.Find("LostText");
            timerText = GameObject.Find("TimerText").GetComponent<Text>();
            livesText = GameObject.Find("LivesText").GetComponent<Text>();
        }

        private string GetTimeToWinString()
        {
            var guiTime = GameManager.Instance.SecondsToWin;
            var text = string.Format("{0:00}:{1:00}", guiTime / 60, guiTime % 60);
            return text;
        }

        private string GetLivesString()
        {
            return new String(liveChar, GameManager.Instance.Lives);
        }

        private void CreateBuildButtons()
        {
            foreach (GameObject turret in Globals.Instance.turretPrefabs)
            {
                var tCopy = turret;

                // create btn on build panel
                var btn = Instantiate(BuildTurretBtnPrefab) as Button;
                btn.transform.SetParent(buildPanel.transform, false);

                var text = btn.GetComponentsInChildren<Text>();
                text[0].text = tCopy.GetComponent<TurretAI>().ToString(); // assume TurretAI immutable
                btn.GetComponent<Button>().onClick.AddListener(() => OnBuildClick(tCopy));
            }
        }

        private void SetVisibility(GameState state)
        {
            switch (state)
            {
                case GameState.Start:
                    statPanel.SetActive(false);
                    buildPanel.SetActive(false);
                    shadowPanel.SetActive(true);
                    winPanel.SetActive(false);
                    lostPanel.SetActive(false);
                    break;

                case GameState.Playing:
                    statPanel.SetActive(true);
                    buildPanel.SetActive(false);
                    shadowPanel.SetActive(false);
                    winPanel.SetActive(false);
                    lostPanel.SetActive(false);
                    break;

                case GameState.Won:
                    buildPanel.SetActive(false);
                    shadowPanel.SetActive(true);
                    winPanel.SetActive(true);
                    break;

                case GameState.Lost:
                    buildPanel.SetActive(false);
                    shadowPanel.SetActive(true);
                    lostPanel.SetActive(true);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerator RefreshTimer(Action act)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                act();
            }
        }
    }
}