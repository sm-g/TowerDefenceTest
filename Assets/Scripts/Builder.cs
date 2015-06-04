using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Builder : SingletonMB<Builder>, INotifyPropertyChanged
    {
        public GameObject[] turretPrefabs;

        public GameObject projectilePrefab;

        private Placement _selectedPlace;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Placement SelectedPlace
        {
            get { return _selectedPlace; }
            private set
            {
                if (_selectedPlace != null && value == null && _selectedPlace.IsSelected)
                    _selectedPlace.IsSelected = false;

                _selectedPlace = value;
                OnPropertyChanged("SelectedPlace");
            }
        }

        public void Build(GameObject turret)
        {
            if (SelectedPlace != null)
            {
                Debug.Log("Build turret " + turret);

                SelectedPlace.SetTurret(turret);
                SelectedPlace.IsSelected = false;
            }
        }

        private void Awake()
        {
            CheckPrefabs();
            DontDestroyOnLoad(gameObject);

            Placement.SelectedChanged += (s, e) =>
            {
                if (e.arg.IsSelected)
                    SelectedPlace = e.arg;
                else if (SelectedPlace != null && !SelectedPlace.IsSelected) // сняли выделение
                    SelectedPlace = null;
            };
            GameManager.Instance.StateChanged += (s, e) =>
            {
                if (e.arg == GameState.Won || e.arg == GameState.Lost)
                {
                    SelectedPlace = null;
                }
            };
        }

        private void CheckPrefabs()
        {
            if (projectilePrefab == null)
                Debug.LogError("Add projectile prefab to " + this.GetType());
            if (projectilePrefab != null && projectilePrefab.GetComponent<ProjectileAI>() == null)
                Debug.LogErrorFormat("Add '{0}' to projectile prefab", typeof(ProjectileAI));

            if (turretPrefabs.Length == 0)
                Debug.LogError("Add turret prefabs to " + this.GetType());
            foreach (var prefab in turretPrefabs)
            {
                if (prefab.GetComponent<TurretAI>() == null)
                    Debug.LogErrorFormat("No '{0}' in prefab.", typeof(TurretAI));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}