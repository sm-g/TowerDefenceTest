using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Место для строительства башни.
/// </summary>
public class Placement : MonoBehaviour
{
    public Color color = Color.gray;

    /// <summary>
    /// Цвет места, которое выбрано для строительства.
    /// </summary>
    public Color selectedColor = Color.yellow;

    private bool _isSelected;
    private GameObject turretsFolder;

    public static event EventHandler<EventArgs<Placement>> SelectedChanged;

    /// <summary>
    /// Башня на этом месте.
    /// </summary>
    public GameObject Turret { get; private set; }

    /// <summary>
    /// Место выбрано. Одновременно выбрано только одно место на сцене.
    /// </summary>
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            gameObject.GetComponent<Renderer>().material.color = _isSelected ? selectedColor : color;

            OnSelectedChanged();
        }
    }
    /// <summary>
    /// Ставит новую башню.
    /// </summary>
    /// <param name="ai"></param>
    public void SetTurret(TurretAI ai)
    {
        if (Turret != null)
            GameObject.Destroy(Turret);

        Turret = Instantiate(Globals.instance.Turrets[ai], transform.position + transform.up, Quaternion.identity) as GameObject;

        turretsFolder.AddChild(Turret);
    }

    protected virtual void OnSelectedChanged()
    {
        var h = SelectedChanged;
        if (h != null)
        {
            h(this, new EventArgs<Placement>(this));
        }
    }

    private void Awake()
    {
        GameManager.Instance.Register(gameObject);

        IsSelected = false;
        turretsFolder = GameObject.Find("Turrets");
        if (turretsFolder == null)
            turretsFolder = new GameObject("Turrets");

        SelectedChanged += (s, e) =>
        {
            // only one selected
            if (e.arg != this && e.arg.IsSelected)
                IsSelected = false;
        };
    }

    private void OnMouseDown()
    {
        IsSelected = !IsSelected;
    }

    private void OnDestroy()
    {
        GameManager.Instance.Unregister(gameObject);
    }
}