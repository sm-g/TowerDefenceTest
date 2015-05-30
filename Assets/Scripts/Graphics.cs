using UnityEngine;

public class Graphics : MonoBehaviour
{
    public Rect buildMenu;
    public Rect firstTurret;
    public Rect secondTurret;
    public Rect thirdTurret;

    public Rect gameStats;
    public Rect gameStatsTime;
    public Rect gameStatsPassed;

    private const float margin = 10;

    private void Awake()
    {
        buildMenu = new Rect(margin, Screen.height - 60, Screen.width - margin, 70);

        var bW = 100;
        var bH = buildMenu.height - 30;
        firstTurret = new Rect(buildMenu.x + margin, buildMenu.y + 20, bW, bH);
        secondTurret = new Rect(firstTurret.x + bW + margin, buildMenu.y + 20, bW, bH);
        thirdTurret = new Rect(secondTurret.x + bW + margin, buildMenu.y + 20, bW, bH);

        gameStats = new Rect(margin, margin, 200, 100);
        gameStatsTime = new Rect(gameStats.x + margin, gameStats.y + 30, 125, 25);
        gameStatsPassed = new Rect(gameStats.x + margin, gameStats.y + 50, 125, 25);
    }

    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.Box(buildMenu, "Select Turret to build");
        if (GUI.Button(firstTurret, "1 Turret"))
        {
        }
        if (GUI.Button(secondTurret, "2 Turret"))
        {
        }
        if (GUI.Button(thirdTurret, "3 Turret"))
        {
        }

        GUI.Box(gameStats, "Stats");
        GUI.Label(gameStatsTime, "Time to win: " + Globals.instance.timeToWin);
        GUI.Label(gameStatsPassed, "Passed mobs: " + Globals.instance.passedMobs);
    }
}