using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanatric.Unity.MonoComponent
{
    public class RenderCameraWorld : MonoBehaviour
    {
        private Camera cam;
        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            cam.targetTexture = RenderResultManager.Instance.RenderResultWorld;
        }
    }
}