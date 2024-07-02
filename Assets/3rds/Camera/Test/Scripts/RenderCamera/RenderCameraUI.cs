using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanatric.Unity.MonoComponent
{
    public class RenderCameraUI : MonoBehaviour
    {
        private Camera cam;
        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            if (RenderResultManager.Instance.FirstHasUI)
            {
                // 把World相机的图绘制到UI底图上, 原理是使用UV对应绘制, 与尺寸大小无关
                Graphics.Blit(RenderResultManager.Instance.RenderResultWorld, RenderResultManager.Instance.RenderResultUI);
                // 这个只在第一个UI相机时操作
                RenderResultManager.Instance.FirstHasUI = false;
            }

            cam.targetTexture = RenderResultManager.Instance.RenderResultUI;
        }
    }
}