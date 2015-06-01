using UnityEngine;
using System.Linq;
public class Graphics : SingletonMB<Graphics>
{
    public Rect buildMenu;
    public Rect turretBtn;

    public Rect gameStats;
    public Rect gameStatsTime;
    public Rect gameStatsPassed;

    private const float margin = 10;
    const float btnW = 100;

    public bool showTurretMenu { get; set; }

    public string TimeToWin
    {
        get
        {
            var guiTime = GameManager.Instance.SecondsToWin;

            int minutes = guiTime / 60;
            int seconds = guiTime % 60;

            var text = string.Format("{0:00}:{1:00}", minutes, seconds);
            return text;
        }
    }

    private void Awake()
    {
        buildMenu = new Rect(margin, Screen.height - 60, Screen.width - margin, 70);

        var btnH = buildMenu.height - 30;
        turretBtn = new Rect(buildMenu.x + margin, buildMenu.y + 20, btnW, btnH);

        gameStats = new Rect(margin, margin, 200, 100);
        gameStatsTime = new Rect(gameStats.x + margin, gameStats.y + 30, 125, 25);
        gameStatsPassed = new Rect(gameStats.x + margin, gameStats.y + 50, 125, 25);
    }

    private void OnGUI()
    {
        if (showTurretMenu)
        {
            GUI.Box(buildMenu, "Select Turret to build");

            var selectedPlace = GameManager.Instance.Placements.FirstOrDefault(x => x.IsSelected);
            var btnRect = turretBtn;
            foreach (TurretAI ai in Globals.instance.Turrets.Keys)
            {
                if (GUI.Button(btnRect, "{0}*{1}/{2} hp/s, {3} m".FormatStr(
                    ai.attackDamage,
                    ai.shotsAtOnce,
                    ai.reloadTimer,
                    ai.attackMaxDistance)))
                {
                    selectedPlace.SetTurret(ai);
                }
                btnRect = new Rect(btnRect) { x = btnRect.x + btnW + margin };
            }
        }

        GUI.Box(gameStats, "Stats");
        GUI.Label(gameStatsTime, "Time to win: " + TimeToWin);
        GUI.Label(gameStatsPassed, "Passed mobs: " + GameManager.Instance.PassedMobs);
    }
}