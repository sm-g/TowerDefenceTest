using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraManager : MonoBehaviour
    {
        private Resolution oldRes;

        private void Awake()
        {
            StartCoroutine(OnWindowSizeChanged(() => FixFOV(Camera.main)));
        }

        private IEnumerator OnWindowSizeChanged(Action act)
        {
            while (true)
            {
                var res = GetWindowResolution();
                if (res.width != oldRes.width || res.height != oldRes.height)
                {
                    oldRes = res;
                    act();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Меняем FOV камеры, чтобы видеть только игровое поле.
        /// </summary>
        private void FixFOV(Camera camera)
        {
            // camera centered by x and z above game field
            // so x - half width of field, y - distance to field
            var pos = camera.transform.position;
            var camH = pos.x / pos.y / camera.aspect;
            var vFOV = Mathf.Atan(camH) * Mathf.Rad2Deg * 2;
            camera.fieldOfView = vFOV;
        }

        private Resolution GetWindowResolution()
        {
            return new Resolution() { width = Screen.width, height = Screen.height };
        }
    }
}