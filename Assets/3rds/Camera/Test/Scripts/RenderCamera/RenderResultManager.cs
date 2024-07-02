using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanatric.Unity.MonoComponent
{
    public class RenderResultManager : MonoBehaviour
    {
        [SerializeField]
        private int widthBase = 7680;

        public RenderTexture RenderResultWorld { get; private set; }
        public RenderTexture RenderResultUI { get; private set; }

        public bool FirstWorld { get; set; } = true;
        public bool FirstHasUI { get; set; } = true;

        public static RenderResultManager Instance { get; private set; }

        [SerializeField]
        private RawImage destRawImage;

        private void Awake()
        {
            Instance = this;
            RenderResultWorld = RenderTexture.GetTemporary(Screen.width, Screen.height,24);

            int height = (int)((float)widthBase / Screen.width * Screen.height);
            RenderResultUI = RenderTexture.GetTemporary(widthBase, height);
            destRawImage.texture = RenderResultUI;
            destRawImage.gameObject.SetActive(true);
        }
        
        private void OnResize()
        {
            
        }

        private void Update()
        {
            FirstWorld = true;
            FirstHasUI = true;
        }
    }
}