using System;
using System.Linq;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public TurretAI Turret;
    public Color color = Color.gray;
    public Color selectedColor = Color.yellow;
    private bool _isSelected;
    public event EventHandler SelectedChanged;

    protected virtual void OnSelectedChanged(EventArgs e)
    {
        var h = SelectedChanged;
        if (h != null)
        {
            h(this, e);
        }
    }

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

    private void Awake()
    {
        Globals.instance.Register(gameObject);
        isSelected = false;
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