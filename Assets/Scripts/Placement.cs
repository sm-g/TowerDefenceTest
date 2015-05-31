using System;
using System.Linq;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public Color color = Color.gray;
    public Color selectedColor = Color.yellow;
    private bool _isSelected;

    private GameObject turretsFolder;

    public event EventHandler SelectedChanged;

    public GameObject Turret { get; private set; }

    public bool isSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            gameObject.GetComponent<Renderer>().material.color = _isSelected ? selectedColor : color;

            OnSelectedChanged(EventArgs.Empty);
        }
    }

    public void SetTurret(TurretAI ai)
    {
        if (Turret != null)
            GameObject.Destroy(Turret);

        Turret = Instantiate(TurretPlacer.instance.Turrets[ai], transform.position + transform.up, Quaternion.identity) as GameObject;

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
        Globals.instance.Register(gameObject);

        isSelected = false;
        turretsFolder = GameObject.Find("Turrets");
        if (turretsFolder == null)
            turretsFolder = new GameObject("Turrets");
    }

    private void OnMouseDown()
    {
        isSelected = !isSelected;
    }

    private void OnDestroy()
    {
        Globals.instance.Unregister(gameObject);
    }
}