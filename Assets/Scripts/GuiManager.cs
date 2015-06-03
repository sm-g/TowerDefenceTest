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
        private GameObject buildPanel;
        private GameObject lostPanel;
        private GameObject winPanel;
        private GameObject shadowPanel;
        private Text livesText;
        private Text timerText;
        private GameObject statPanel;

        public void OnRestartClick()
        {
            GameManager.Instance.Restart();
        }

        public void OnBuildClick(GameObject turret)
        {
            Builder.Instance.Build(turret);
        }

        private void Awake()
        {
            if (BuildTurretBtnPrefab == null)
                Debug.LogError("Add BuildTurretBtnPrefab to " + this.GetType());

            FindGuiElements();

            AddListeners();

            CreateBuildButtons();

            Builder.Instance.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "SelectedPlace":
                        // показываем панель строительства для выбранного места
                        buildPanel.SetActive(Builder.Instance.SelectedPlace != null);
                        break;
                }
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
            var restartBtn = GameObject.Find(Gui.RestartButton);
            restartBtn.GetComponent<Button>().onClick.AddListener(OnRestartClick);
            lostPanel.GetComponent<Button>().onClick.AddListener(OnRestartClick);
            winPanel.GetComponent<Button>().onClick.AddListener(OnRestartClick);
        }

        private void FindGuiElements()
        {
            buildPanel = GameObject.Find(Gui.BuildPanel);
            statPanel = GameObject.Find(Gui.StatPanel);
            shadowPanel = GameObject.Find(Gui.ShadowPanel);
            winPanel = GameObject.Find(Gui.WinText);
            lostPanel = GameObject.Find(Gui.LostText);
            timerText = GameObject.Find(Gui.TimerText).GetComponent<Text>();
            livesText = GameObject.Find(Gui.LivesText).GetComponent<Text>();
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
            foreach (GameObject turret in Builder.Instance.turretPrefabs)
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