using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    /// <summary>
    /// Слот для строительства башни.
    /// </summary>
    public class Placement : MonoBehaviour
    {
        public Color color = Color.gray;

        /// <summary>
        /// Цвет выбранного слота.
        /// </summary>
        public Color selectedColor = Color.yellow;

        private static GameObject turretsFolder;
        private Material material;
        private bool _isSelected;

        public static event EventHandler<EventArgs<Placement>> SelectedChanged = delegate { };

        /// <summary>
        /// Башня на этом месте.
        /// </summary>
        public GameObject Turret { get; private set; }

        /// <summary>
        /// Слот выбран. Одновременно выбран только один слот на сцене.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                material.color = _isSelected ? selectedColor : color;

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

            SelectedChanged += (s, e) =>
            {
                // only one selected
                if (e.arg != this && e.arg.IsSelected)
                    IsSelected = false;
            };

            turretsFolder = turretsFolder ?? new GameObject(Generated.Turrets);
            material = gameObject.GetComponent<Renderer>().material;
            material.color = color;
        }

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // do not select if click on ui element
                IsSelected = !IsSelected;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Unregister(gameObject);
        }
    }
}