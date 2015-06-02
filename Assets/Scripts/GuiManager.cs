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
        }

        public void OnBuildClick(TurretAI ai)
        {
            // button active only when SelectedPlace != null
            if (SelectedPlace != null)
                SelectedPlace.SetTurret(ai);
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

        //public void CreateButton(GameObject panel, Vector3 position, Vector2 size, UnityAction method, string text)
        //{
        //    GameObject button = new GameObject();
        //    panel.AddChild(button);
        //    button.AddComponent<RectTransform>().sizeDelta = size;
        //    button.AddComponent<Text>().text = text;
        //    button.AddComponent<Button>().onClick.AddListener(method);
        //    button.transform.position = position;
        //}

        private void CreateBuildButtons()
        {
            foreach (TurretAI ai in Globals.instance.Turrets.Keys)
            {
                var aiCopy = ai;

                // create btn on build panel
                var btn = Instantiate(BuildTurretBtnPrefab) as Button;
                btn.transform.SetParent(buildPanel.transform, false);

                var t = btn.GetComponentsInChildren<Text>();
                t[0].text = aiCopy.ToString(); // assume TurretAI will not change
                btn.GetComponent<Button>().onClick.AddListener(() => OnBuildClick(aiCopy));
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