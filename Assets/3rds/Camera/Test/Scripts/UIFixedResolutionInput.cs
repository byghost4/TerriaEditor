using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hanatric.Unity.Common.UI
{
    /// <summary>
    /// 本UI中用于覆盖默认EventSystem输入的Input
    /// </summary>
    public class UIFixedResolutionInput : BaseInput
    {
        protected override void Awake()
        {
            base.Awake();
            GetComponent<BaseInputModule>().inputOverride = this;
        }
        [SerializeField]
        private RectTransform canvasRectTransform;
        public override Vector2 mousePosition
        {
            get
            {
                //转化屏幕坐标到目标分辨率坐标     
                var screenSize = new Vector2(Screen.width, Screen.height);
                var screenPercent = base.mousePosition / screenSize;
                var scaleMp = screenPercent * canvasRectTransform.sizeDelta;

                return scaleMp;
            }
        }

        private readonly Vector2[] dragButtonsStartPos = new Vector2[6];

        /// <summary>
        /// 判断当前帧是否时拖拽, 需要配合 Get[MouseButton]Up 使用
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsDrag(int button)
        {
            return (base.GetMouseButton(button) || base.GetMouseButtonUp(button)) &&
                Vector2.Distance(mousePosition, dragButtonsStartPos[button]) > EventSystem.current.pixelDragThreshold;
        }

        private void Update()
        {
            for (int i = 0; i < 6; i++)
            {
                if (base.GetMouseButtonDown(i))
                {
                    dragButtonsStartPos[i] = mousePosition;
                }
            }
        }

    }
}
