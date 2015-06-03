using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
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

        private static GameObject turretsFolder;
        private bool _isSelected;

        public static event EventHandler<EventArgs<Placement>> SelectedChanged = delegate { };

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

                SelectedChanged(this, new EventArgs<Placement>(this));
            }
        }

        /// <summary>
        /// Ставит новую башню. Существующая башня разрушается.
        /// </summary>
        /// <param name="turretPrefab">Префаб башни или null.</param>
        public void SetTurret(GameObject turretPrefab)
        {
            if (Turret != null)
                GameObject.Destroy(Turret);

            if (turretPrefab != null)
            {
                Turret = Instantiate(turretPrefab, transform.position + transform.up, Quaternion.identity) as GameObject;
                turretsFolder.AddChild(Turret);
            }
        }

        private void Awake()
        {
            GameManager.Instance.Register(gameObject);

            turretsFolder = turretsFolder ?? new GameObject(Generated.Turrets);

            SelectedChanged += (s, e) =>
            {
                // only one selected
                if (e.arg != this && e.arg.IsSelected)
                    IsSelected = false;
            };

            IsSelected = false; // set color
        }

        private void OnMouseDown()
        {
            if (GameManager.Instance.State == GameState.Playing)
                IsSelected = !IsSelected;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Unregister(gameObject);
        }
    }
}