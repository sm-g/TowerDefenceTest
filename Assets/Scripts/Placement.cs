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

    public event EventHandler SelectedChanged;

    /// <summary>
    /// Башня на этом месте.
    /// </summary>
    public GameObject Turret { get; private set; }

    /// <summary>
    /// Место выбрано.
    /// </summary>
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            gameObject.GetComponent<Renderer>().material.color = _isSelected ? selectedColor : color;

            OnSelectedChanged(EventArgs.Empty);
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

    protected virtual void OnSelectedChanged(EventArgs e)
    {
        var h = SelectedChanged;
        if (h != null)
        {
            h(this, e);
        }
    }

    private void Awake()
    {
        GameManager.Instance.Register(gameObject);

        IsSelected = false;
        turretsFolder = GameObject.Find("Turrets");
        if (turretsFolder == null)
            turretsFolder = new GameObject("Turrets");
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