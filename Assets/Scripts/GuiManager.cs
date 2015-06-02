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
        private static float margin = 10;
        private Placement _selectedPlace;
        private GameObject buildPanel;
        private GameObject lostText;
        private GameObject winText;
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

        private void Awake()
        {
            Placement.SelectedChanged += (s, e) =>
            {
                if (e.arg.IsSelected)
                    SelectedPlace = e.arg;
                else if (SelectedPlace != null && !SelectedPlace.IsSelected) // сняли выделение
                    SelectedPlace = null;
            };

            GameManager.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Lives")
                    livesText.text = GetLivesString();
            };

            buildPanel = GameObject.Find("BuildPanel");
            statPanel = GameObject.Find("StatPanel");
            winText = GameObject.Find("WinText");
            lostText = GameObject.Find("LostText");
            timerText = GameObject.Find("TimerText").GetComponent<Text>();
            livesText = GameObject.Find("LivesText").GetComponent<Text>();
            shadowPanel = GameObject.Find("ShadowPanel");
            if (BuildTurretBtnPrefab == null)
                Debug.LogError("Add BuildTurretBtnPrefab to " + typeof(GuiManager));

            livesText.text = GetLivesString();
            timerText.text = GetTimeToWinString();

            CreateBuildButtons();
            SetInitialVisibility();

            StartCoroutine(OneSecondTimer(() => timerText.text = GetTimeToWinString()));
        }

        public void OnResetClick()
        {
            GameManager.Instance.Restart();
            SetInitialVisibility();
        }

        public void OnBuildClick(GameObject turret)
        {
            // button active only when SelectedPlace != null
            if (SelectedPlace != null)
                SelectedPlace.SetTurret(turret);
        }

        private static string GetTimeToWinString()
        {
            var guiTime = GameManager.Instance.SecondsToWin;
            var text = string.Format("{0:00}:{1:00}", guiTime / 60, guiTime % 60);
            return text;
        }

        private static string GetLivesString()
        {
            return new String(liveChar, GameManager.Instance.Lives);
        }

        private IEnumerator OneSecondTimer(Action act)
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                act();
            }
        }

        private void CreateBuildButtons()
        {
            foreach (GameObject turret in Globals.instance.turretPrefabs)
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

        private void SetInitialVisibility()
        {
            if (buildPanel != null)
                buildPanel.SetActive(false);
            if (shadowPanel != null)
                shadowPanel.SetActive(false);
            if (winText != null)
                winText.SetActive(false);
            if (lostText != null)
                lostText.SetActive(false);
        }
    }
}