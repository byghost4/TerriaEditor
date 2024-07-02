using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hanatric.Unity.MonoComponent
{

    public interface ICommonCameraInput
    {
        float MouseDeltaX { get; }
        float MouseDeltaY { get; }
        float MouseScrollWheel { get; }
        bool IsMouse0Down { get; }
        bool IsMouse0 { get; }
        bool IsMouse0Up { get; }

        bool IsMouse1Down { get; }
        bool IsMouse1 { get; }
        bool IsMouse1Up { get; }

        /// <summary>
        /// 当前鼠标是否可以开始交互
        /// eg. 当鼠标悬浮在UI上时一般是不允许相机被交互的, 通过这个属性来指示是否接受交互开始
        /// </summary>
        bool CanMouseInteractStart { get; }
    }

    public class DefaultCommonCameraInput : ICommonCameraInput
    {
        public virtual float MouseDeltaX => Input.GetAxis("Mouse X");
        public virtual float MouseDeltaY => Input.GetAxis("Mouse Y");
        public virtual float MouseScrollWheel => Input.GetAxis("Mouse ScrollWheel");

        public virtual bool IsMouse0Down => Input.GetMouseButtonDown(0);
        public virtual bool IsMouse0 =>  Input.GetMouseButton(0);
        public virtual bool IsMouse0Up => Input.GetMouseButtonUp(0);
        public virtual bool IsMouse1Down => Input.GetMouseButtonDown(1);
        public virtual bool IsMouse1 => Input.GetMouseButton(1);
        public virtual bool IsMouse1Up => Input.GetMouseButtonUp(1);
        public virtual bool CanMouseInteractStart
        {
            get
            {
                //使用全局的默认EventSystem来判断鼠标是否浮在UI上
                var eventSystem = EventSystem.current;
                PointerEventData pointerEventData = new PointerEventData(eventSystem);
                var mp = eventSystem.currentInputModule.input.mousePosition;
                pointerEventData.position = mp;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                eventSystem.RaycastAll(pointerEventData, raycastResults);
                return raycastResults.Count <= 0;
            }
        }
    }
    public class KeyboardMouseCommonCameraInput : ICommonCameraInput
    {
        public virtual float MouseDeltaX => GetMouseDeltaX;
        public virtual float MouseDeltaY => GetMouseDeltaY;
        public virtual float MouseScrollWheel => Input.GetAxis("Mouse ScrollWheel");

        public virtual bool IsMouse0Down => (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
        public virtual bool IsMouse0 => (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
        public virtual bool IsMouse0Up => (Input.GetKeyUp(KeyCode.W) && Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.D));
        public virtual bool IsMouse1Down => Input.GetMouseButtonDown(2);
        public virtual bool IsMouse1 => Input.GetMouseButton(2);
        public virtual bool IsMouse1Up => Input.GetMouseButtonUp(2);
        public virtual bool CanMouseInteractStart
        {
            get
            {
                //使用全局的默认EventSystem来判断鼠标是否浮在UI上
                var eventSystem = EventSystem.current;
                PointerEventData pointerEventData = new PointerEventData(eventSystem);
                var mp = eventSystem.currentInputModule.input.mousePosition;
                pointerEventData.position = mp;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                eventSystem.RaycastAll(pointerEventData, raycastResults);
                return raycastResults.Count <= 0;
            }
        }

        private float GetMouseDeltaX
        {
            get
            {
                //Debug.Log("鼠标右键是否按下:" + IsMouse1Down);
                return IsMouse1 ? Input.GetAxis("Mouse X") : -Input.GetAxis("Horizontal");
            }        
        }

        private float GetMouseDeltaY
        {
            get
            {
                //Debug.Log("鼠标右键是否按下:" + IsMouse1Down);
                return IsMouse1 ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Vertical");
            }
        }
    }
    public class TouchCommonCameraInput : ICommonCameraInput
    {
        public virtual float MouseDeltaX => GetTouchX;
        public virtual float MouseDeltaY => GetTouchY;
        public virtual float MouseScrollWheel => DoubleTouchWheel;

        public virtual bool IsMouse0Down => GetTouchZeroStatus == TouchStatus.Began;
        public virtual bool IsMouse0 => GetTouchZeroStatus == TouchStatus.Moved;
        public virtual bool IsMouse0Up => GetTouchZeroStatus == TouchStatus.Ended;
        public virtual bool IsMouse1Down => GetTouchOneStatus == TouchStatus.Began;
        public virtual bool IsMouse1 => GetTouchOneStatus == TouchStatus.Moved;
        public virtual bool IsMouse1Up => GetTouchOneStatus == TouchStatus.Ended;

        public virtual bool CanMouseInteractStart
        {
            get
            {
                //使用全局的默认EventSystem来判断鼠标是否浮在UI上
                var eventSystem = EventSystem.current;
                PointerEventData pointerEventData = new PointerEventData(eventSystem);
                var mp = eventSystem.currentInputModule.input.mousePosition;
                pointerEventData.position = mp;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                eventSystem.RaycastAll(pointerEventData, raycastResults);
                return raycastResults.Count <= 0;
            }
        }

        /// <summary>
        /// 获取第一个触点的点击状态
        /// </summary>
        private TouchStatus GetTouchZeroStatus
        {
            get
            {
                TouchStatus touchStatus = TouchStatus.None;
                if (Input.touches.Length != 1)
                    return touchStatus;
                // 遍历所有触摸事件
                for (int i = 0; i < 1; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    // 检查触摸状态
                    if (touch.phase == TouchPhase.Began)
                    {
                        // 触摸开始
                        touchStatus = TouchStatus.Began;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 触摸移动
                        touchStatus = TouchStatus.Moved;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // 触摸结束
                        touchStatus = TouchStatus.Ended;
                    }
                }
                return touchStatus;
            }
        }

        /// <summary>
        /// 获取第二个触点的点击状态
        /// </summary>
        private TouchStatus GetTouchOneStatus
        {
            get
            {
                TouchStatus touchStatus = TouchStatus.None;
                if (Input.touches.Length != 3)
                    return touchStatus;
                // 遍历所有触摸事件
                for (int i = 0; i < 1; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    // 检查触摸状态
                    if (touch.phase == TouchPhase.Began)
                    {
                        // 触摸开始
                        touchStatus = TouchStatus.Began;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 触摸移动
                        touchStatus = TouchStatus.Moved;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // 触摸结束
                        touchStatus = TouchStatus.Ended;
                    }
                }
                return touchStatus;
            }
        }

        /// <summary>
        /// 获取单机X轴偏量
        /// </summary>
        private float GetTouchX
        {
            get
            {
                float deltaX = 0;
                if (Input.touches.Length <= 0)
                    return deltaX;
                deltaX = Input.touches[0].deltaPosition.x;
                return deltaX;
            }
        }

        /// <summary>
        /// 获取单机Y轴偏量
        /// </summary>
        private float GetTouchY
        {
            get
            {
                float deltaY = 0;
                if (Input.touches.Length <= 0)
                    return deltaY;
                deltaY = Input.touches[0].deltaPosition.y;
                return deltaY;
            }
        }

        /// <summary>
        /// 获取双指缩放量
        /// </summary>
        private float DoubleTouchWheel
        {
            get
            {
                float simulatedScroll = 0;
                if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // 模拟鼠标滚轮效果
                    simulatedScroll = deltaMagnitudeDiff * -0.01f; // 缩放因子调整
                }
                return simulatedScroll;
            }
        }
    }

    public enum TouchStatus
    {
        None,
        Began,
        Pressed,
        Moved,
        Ended
    }

    public class CommonCamera : MonoBehaviour
    {
        /// <summary>
        /// 设置目标
        /// </summary>
        public class Target
        {
            public Vector3 TargetPosition { get; set; }
            public Quaternion SelfRotation { get; set; }
            public float Distance { get; set; }
        }

        /// <summary>
        /// 灵敏度
        /// </summary>
        public class Sensitivity
        {
            public float MoveSpeed { get; set; }
            public float RotateSpeedHorizontal { get; set; }
            public float RotateSpeedVertical { get; set; }
            public float ScaleSpeed { get; set; }
            public float MoveSlowdown { get; set; }
            public float RotateSlowdown { get; set; }
            public float ScaleSlowdown { get; set; }
            public float LookTargetMoveSlowdown { get; set; }
            public float LookTargetRotSlowdown { get; set; }
        }

        /// <summary>
        /// 整合Target和Limit的Preset
        /// </summary>
        public class Preset
        {
            public Target Target { get; set; }
            public Limit Limit { get; set; }
        }

        /// <summary>
        /// 操作限制
        /// </summary>
        public class Limit
        {
            public Range VerticalRotateRange { get; set; }
            public Range ScaleRange { get; set; }
            public bool CanRotate { get; set; }
            public bool CanMove { get; set; }
            public bool CanScale { get; set; }
        }

        public enum InputMoudule
        {
            Mouse,
            KeyboardAndMouse,
            Touch
        }

        /// <summary>
        /// 范围值
        /// </summary>
        [Serializable]
        public class Range
        {
            [SerializeField]
            private float min;

            [SerializeField]
            private float max;

            public float Min { get => min; set => min = value; }
            public float Max { get => max; set => max = value; }

            public float ActualMin => Mathf.Min(min, max);
            public float ActualMax => Mathf.Max(min, max);
        }

        /// <summary>
        /// 相机使用的平面
        /// </summary>
        [SerializeField]
        private Transform plane;

        /// <summary>
        /// 是否为触屏模式
        /// </summary>
        [SerializeField]
        private InputMoudule inputMoudule;

        /// <summary>
        /// 是否在Awake时初始化toPosition和toRotation值
        /// </summary>
        [SerializeField]
        private bool isInitToOnAwake = true;

#if UNITY_EDITOR
        [SerializeField]
        private bool showGizoms = true;
#endif

        #region 鼠标灵敏度
        [SerializeField]
        private float moveSpeed = 5;
        [SerializeField]
        private float rotateSpeedHorizontal = 5;
        [SerializeField]
        private float rotateSpeedVertical = 5;
        [SerializeField]
        private float scaleSpeed = 5;
        #endregion

        #region 鼠标操作后缓动
        [SerializeField]
        private float moveSlowdown = 3;
        [SerializeField]
        private float rotateSlowdown = 3;
        [SerializeField]
        private float scaleSlowdown = 3;
        #endregion

        #region 直接设置观察目标时的缓动
        [SerializeField]
        private float lookTargetMoveSlowdown = 3;
        [SerializeField]
        private float lookTargetRotSlowdown = 3;
        #endregion

        #region 鼠标操作限制
        [SerializeField]
        private Range verticalRotateRange;
        [SerializeField]
        private Range scaleRange;
        [SerializeField]
        private bool canMove = true;
        [SerializeField]
        private bool canRotate = true;
        [SerializeField]
        private bool canScale = true;
        [SerializeField]
        private List<Vector3> boundarys = new List<Vector3>()
        {
            new Vector3(-200, 0, -200),
            new Vector3(200, 0, -200),
            new Vector3(200, 0, 200),
            new Vector3(-200, 0, 200),
        };

        public bool CanMove { get => canMove; set => canMove = value; }
        public bool CanRotate { get => canRotate; set => canRotate = value; }
        public bool CanScale { get => canScale; set => canScale = value; }
        public Range ScaleRange { get => scaleRange; set => scaleRange = value; }
        public Range VerticalRotateRange { get => verticalRotateRange; set => verticalRotateRange = value; }
        #endregion


        /// <summary>
        /// 鼠标输入源
        /// </summary>
        public ICommonCameraInput CameraInput 
        { 
            get 
            {
                switch (inputMoudule)
                {
                    case InputMoudule.Mouse:
                        return new DefaultCommonCameraInput();
                    case InputMoudule.KeyboardAndMouse:
                        return new KeyboardMouseCommonCameraInput();
                    case InputMoudule.Touch:
                        return new TouchCommonCameraInput();
                }
                return new DefaultCommonCameraInput();
            }
        }

        private new Camera camera;
        public Camera Camera => camera;

        private Vector3 deltaMove;
        private float deltaRotHorizontal;
        private float deltaRotVertical;
        private float deltaScale;

        private Vector3 toPosition;
        private Quaternion toRotation;

        private Vector3 centerPos;

        private Vector3 ToForward => toRotation * Vector3.forward;
        private Vector3 ToRight => toRotation * Vector3.right;
        private Vector3 ToUp => toRotation * Vector3.up;

        /// <summary>
        /// 是否处于高度重置状态
        /// 位于此状态下, 高度线会一直被拉回到0
        /// </summary>
        private bool isHeightReseting = true;
        /// <summary>
        /// 是否处于可交互状态
        /// 此状态通常由鼠标按下时位置状态决定
        /// </summary>
        private bool isInteracting = false;

        private void Awake()
        {
            camera = GetComponent<Camera>();

            if (isInitToOnAwake)
            {
                toPosition = transform.position;
                toRotation = transform.rotation;
            }

            if (plane != null)
            {
                var pos = GetIntersectWithRayToPlane(toPosition, ToForward, plane.up, plane.position);
                if (Vector3.Angle(Vector3.ProjectOnPlane(ToForward, plane.up), ToForward) > 0.01f) centerPos = pos;
            }
        }

        private void Update()
        {

            if (CameraInput.IsMouse0Up || CameraInput.IsMouse1Up) isInteracting = false;
            if (CameraInput.CanMouseInteractStart && (CameraInput.IsMouse0Down || CameraInput.IsMouse1Down)) isInteracting = true;

            float mouseX = CameraInput.MouseDeltaX;
            float mouseY = CameraInput.MouseDeltaY;

            float deltaTime = Time.deltaTime;

            float centerDistance = Vector3.Distance(centerPos, toPosition);

            float actualMoveSpeed = moveSpeed * centerDistance / 100;
            float actualScaleSpeed = scaleSpeed * centerDistance / 100;

            float mouseScroll = -CameraInput.MouseScrollWheel;

            if (CameraInput.CanMouseInteractStart && canScale)
            {
                deltaScale += mouseScroll * actualScaleSpeed;
            }

            #region ========== 限制缩放 ==========
            var scaleToPosition = Vector3.MoveTowards(toPosition, centerPos, -deltaScale);
            var scaleToDistance = Vector3.Distance(centerPos, scaleToPosition);
            if (scaleToDistance < scaleRange.ActualMin)
            {
                if (centerDistance > scaleRange.ActualMin)
                {
                    deltaScale += scaleRange.ActualMin - scaleToDistance;
                }
                else if (deltaScale < 0)
                {
                    deltaScale = 0;
                }
            }
            if (scaleToDistance > scaleRange.ActualMax)
            {
                if (centerDistance < scaleRange.ActualMax)
                {
                    deltaScale += scaleRange.ActualMax - scaleToDistance;
                }
                else if (deltaScale > 0)
                {
                    deltaScale = 0;
                }
            }
            #endregion

            if (CameraInput.IsMouse1 && isInteracting && canRotate)
            {
                deltaRotHorizontal += rotateSpeedHorizontal * deltaTime * mouseX;
                deltaRotVertical += rotateSpeedVertical * -deltaTime * mouseY;
            }

            #region ========== 限制垂直旋转角度 ==========
            float currentVecticalAngle = toRotation.eulerAngles.x;
            // 垂直角度范围为[-90,90]度, 表现在四元数转换的角度上为[270,360]&[0,90]度
            if (currentVecticalAngle > 180) currentVecticalAngle -= 360;

            float changeToAngle = currentVecticalAngle + deltaRotVertical;
            float max = verticalRotateRange.ActualMax;
            float min = verticalRotateRange.ActualMin;

            // 垂直角度越过90度会导致画面逆向或不可计算, 这里强制限制在89度以下
            max = Mathf.Clamp(max, -89f, 89f);
            min = Mathf.Clamp(min, -89f, 89f);

            if (changeToAngle > max)
            {
                if (currentVecticalAngle < max)
                {
                    deltaRotVertical += max - changeToAngle;
                }
                else if (deltaRotVertical > 0)
                {
                    deltaRotVertical = 0;
                }
            }
            if (changeToAngle < min)
            {
                if (currentVecticalAngle > min)
                {
                    deltaRotVertical += min - changeToAngle;
                }
                else if (deltaRotVertical < 0)
                {
                    deltaRotVertical = 0;
                }
            }
            #endregion

            var deltaRotHorizontalOffset = Mathf.Lerp(0, deltaRotHorizontal, Time.deltaTime * rotateSlowdown);
            deltaRotHorizontal -= deltaRotHorizontalOffset;

            var deltaRotVerticalOffset = Mathf.Lerp(0, deltaRotVertical, Time.deltaTime * rotateSlowdown);
            deltaRotVertical -= deltaRotVerticalOffset;

            RotateAroundCenter(deltaRotHorizontalOffset, deltaRotVerticalOffset);

            if (CameraInput.IsMouse0 && isInteracting && canMove)
            {
                Vector3 upDir = ToUp;
                Vector3 rightDir = ToRight;

                if (plane != null)
                {
                    upDir = -Vector3.ProjectOnPlane(ToUp, plane.up).normalized;
                    rightDir = -Vector3.ProjectOnPlane(ToRight, plane.up).normalized;
                }

                // 先判断移动方向
                float angleUp = Vector3.Angle(upDir, deltaMove);
                float angleRight = Vector3.Angle(rightDir, deltaMove);

                // 判断移动方向与鼠标方向相反, 则快速缓动将原移动量向降为0
                if (mouseX != 0 && (angleRight > 90 ^ mouseX < 0) || mouseY != 0 && (angleUp > 90 ^ mouseY < 0))
                {
                    deltaMove = Vector3.Lerp(deltaMove, Vector3.zero, Time.deltaTime * moveSlowdown * 3);
                }

                deltaMove += actualMoveSpeed * deltaTime * (mouseX * rightDir + mouseY * upDir);
                if (mouseX != 0 || mouseY != 0)
                {
                    isHeightReseting = true;
                }
            }

            #region ========== 限制移动范围 ==========
            if (plane != null)
            {
                var toCenterPos = centerPos + deltaMove;
                var toCenterPosXZ = GetPointXZFromProjectedPlane(toCenterPos, plane.forward, plane.right, plane.position);

                if (!IsPointInPolygon(toCenterPosXZ, boundarys))
                {
                    var closestPoint = FindClosestPointOnPolygonWithOutsidePointApproximately(toCenterPosXZ, boundarys);
                    Vector3 deltaXZ = toCenterPosXZ - closestPoint;
                    Vector3 delta = plane.right * deltaXZ.x + plane.forward * deltaXZ.z;
                    deltaMove -= delta;
                }
            }
            #endregion

            var deltaMoveOffset = Vector3.Lerp(Vector3.zero, deltaMove, Time.deltaTime * moveSlowdown);
            deltaMove -= deltaMoveOffset;


            var deltaScaleOffset = Mathf.Lerp(0, deltaScale, Time.deltaTime * scaleSlowdown);
            deltaScale -= deltaScaleOffset;

            toPosition += deltaMoveOffset;
            centerPos += deltaMoveOffset;

            if (Vector3.Distance(Vector3.MoveTowards(toPosition, centerPos, -deltaScaleOffset), centerPos) > 1e-4)
            {
                toPosition = Vector3.MoveTowards(toPosition, centerPos, -deltaScaleOffset);
                if (float.IsNaN(toPosition.x) || float.IsNaN(toPosition.y) || float.IsNaN(toPosition.z))
                {
                    toPosition = Vector3.zero;
                }
            }

            // 重置高度
            if (isHeightReseting && plane != null)
            {
                Vector3 centerProjectedPoint = Vector3.ProjectOnPlane(centerPos, plane.up) + Vector3.Project(plane.position, plane.up);
                var centerProjectedPointOffset = Vector3.Lerp(Vector3.zero, centerPos - centerProjectedPoint, Time.deltaTime * moveSlowdown);
                centerPos -= centerProjectedPointOffset;
                toPosition -= centerProjectedPointOffset;
            }
            // 设置坐标和角度
            transform.SetPositionAndRotation(toPosition, toRotation);
        }

        /// <summary>
        /// 相机绕中心旋转
        /// </summary>
        /// <param name="angleHorizontal"></param>
        /// <param name="angleVertical"></param>
        private void RotateAroundCenter(float angleHorizontal, float angleVertical)
        {
            var center = centerPos;

            // 先计算水平旋转, 在计算竖直旋转, 必须按顺序计算
            // (竖直旋转依赖于目标右方向, 目标右方向由旋转后的Rotation得来)

            #region ========== 处理水平旋转 ==========
            Quaternion rotHorizontal = Quaternion.AngleAxis(angleHorizontal, Vector3.up);
            var horizontal = toPosition - center;
            horizontal = rotHorizontal * horizontal;
            toPosition = center + horizontal;
            toRotation = rotHorizontal * toRotation;
            #endregion

            #region ========== 处理垂直旋转 ==========
            Quaternion rotVertical = Quaternion.AngleAxis(angleVertical, ToRight);
            var vertical = toPosition - center;
            vertical = rotVertical * vertical;
            toPosition = center + vertical;
            toRotation = rotVertical * toRotation;
            #endregion

            // 相机在跳转聚焦物体过程中, Slerp函数会产生不等于0的z轴角度, 导致水平度偏移, 这里获取z轴角度消除偏差
            #region ========== 水平度校准 ==========
            float checkZ = toRotation.eulerAngles.z;
            Quaternion rotCheckZ = Quaternion.AngleAxis(-checkZ, ToForward);
            toRotation = rotCheckZ * toRotation;
            #endregion

        }

        public void SetCenterPosition(Vector3 centerPosition)
        {
            var dis = centerPosition - centerPos;
            deltaMove = dis;

            isHeightReseting = false;
        }

        public void SetCenterPosition(Vector3 centerPosition,float targetDistance)
        {
            SetCenterPosition(centerPosition,transform.rotation, targetDistance);
        }

        public void SetCenterPosition(Vector3 centerPosition, Quaternion targetRotation, float targetDistance)
        {
            isHeightReseting = false;

            Vector3 toForward = ToForward;
            Vector3 targetForward = targetRotation * Vector3.forward;

            float angleToForwardYAxis = Vector3.Angle(Vector3.up, toForward);
            float angleTargetForwardYAxis = Vector3.Angle(Vector3.up, targetForward);

            deltaRotVertical = angleTargetForwardYAxis - angleToForwardYAxis;

            Vector3 toForwardXZ = new Vector3(toForward.x, 0, toForward.z);
            Vector3 targetForwardXZ = new Vector3(targetForward.x, 0, targetForward.z);

            deltaRotHorizontal = Vector3.SignedAngle(toForwardXZ, targetForwardXZ, Vector3.up);

            var targetCenterDistance = centerPosition - centerPos;
            deltaMove = targetCenterDistance;

            var toCenterDistance = (toPosition - centerPos).magnitude;
            deltaScale = targetDistance - toCenterDistance;
        }

        /// <summary>
        /// 加载鼠标灵敏度, 一般来说程序生命周期中只加载一份灵敏度参数
        /// </summary>
        /// <param name="sensitivity"></param>
        public void LoadSensitivity(Sensitivity sensitivity)
        {
            moveSpeed = sensitivity.MoveSpeed;
            rotateSpeedHorizontal = sensitivity.RotateSpeedHorizontal;
            rotateSpeedVertical = sensitivity.RotateSpeedVertical;
            scaleSpeed = sensitivity.ScaleSpeed;

            moveSlowdown = sensitivity.MoveSlowdown;
            rotateSlowdown = sensitivity.RotateSlowdown;
            scaleSlowdown = sensitivity.ScaleSlowdown;

            lookTargetMoveSlowdown = sensitivity.LookTargetMoveSlowdown;
            lookTargetRotSlowdown = sensitivity.LookTargetRotSlowdown;
        }

        /// <summary>
        /// 加载目标, 加载后相机会立刻开始向目标变化
        /// 一般会和 LoadLimit 一起使用
        /// </summary>
        /// <param name="target"></param>
        public void LoadTarget(Target target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            SetCenterPosition(target.TargetPosition, target.SelfRotation, target.Distance);
        }

        /// <summary>
        /// 加载相机限制, 加载后立刻生效
        /// 一般会和 LoadTarget 一起使用
        /// </summary>
        /// <param name="limit"></param>
        public void LoadLimit(Limit limit)
        {
            if (limit is null) throw new ArgumentNullException(nameof(limit));

            verticalRotateRange = new Range
            {
                Min = limit.VerticalRotateRange.Min,
                Max = limit.VerticalRotateRange.Max,
            };
            scaleRange = new Range
            {
                Min = limit.ScaleRange.Min,
                Max = limit.ScaleRange.Max,
            };

            canMove = limit.CanMove;
            canRotate = limit.CanRotate;
            canScale = limit.CanScale;
        }

        /// <summary>
        /// 加载预设
        /// </summary>
        /// <param name="preset"></param>
        public void LoadPreset(Preset preset)
        {
            if (preset is null) throw new ArgumentNullException(nameof(preset));
            LoadTarget(preset.Target);
            LoadLimit(preset.Limit);
        }

        public Sensitivity SaveSensitivity()
        {
            return new Sensitivity
            {
                MoveSpeed = moveSpeed,
                RotateSpeedHorizontal = rotateSpeedHorizontal,
                RotateSpeedVertical = rotateSpeedVertical,
                ScaleSpeed = scaleSpeed,
                MoveSlowdown = moveSlowdown,
                RotateSlowdown = rotateSlowdown,
                ScaleSlowdown = scaleSlowdown,
                LookTargetMoveSlowdown = lookTargetMoveSlowdown,
                LookTargetRotSlowdown = lookTargetRotSlowdown,
            };
        }

        public Target SaveTarget()
        {
            return new Target
            {
                TargetPosition = centerPos,
                SelfRotation = toRotation,
                Distance = (centerPos - toPosition).magnitude,
            };
        }

        public Limit SaveLimit()
        {
            return new Limit
            {
                CanMove = canMove,
                CanRotate = canRotate,
                CanScale = canScale,
                ScaleRange = new Range
                {
                    Max = scaleRange.Max,
                    Min = scaleRange.Min,
                },
                VerticalRotateRange = new Range
                {
                    Max = verticalRotateRange.Max,
                    Min = verticalRotateRange.Min,
                },
            };
        }

        public Preset SavePreset()
        {
            return new Preset
            {
                Limit = SaveLimit(),
                Target = SaveTarget(),
            };
        }

        /// <summary>
        /// 获取射线和平面的交点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direct"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        private static Vector3 GetIntersectWithRayToPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }

        /// <summary>
        /// 判断点在多边形内
        /// X-Z平面
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
        {
            int polygonLength = polygon.Count, i = 0;
            bool inside = false;
            float pointX = point.x, pointZ = point.z;
            float startX, startZ, endX, endZ;
            Vector3 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endZ = endPoint.z;
            while (i < polygonLength)
            {
                startX = endX; startZ = endZ;
                endPoint = polygon[i++];
                endX = endPoint.x; endZ = endPoint.z;
                inside ^= (endZ > pointZ ^ startZ > pointZ) && ((pointX - endX) < (pointZ - endZ) * (startX - endX) / (startZ - endZ));
            }
            return inside;
        }

        /// <summary>
        /// 取某点相对对平面的XZ轴坐标分量 (即平面上的坐标)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeForward"></param>
        /// <param name="planeRight"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static Vector3 GetPointXZFromProjectedPlane(Vector3 point, Vector3 planeForward, Vector3 planeRight, Vector3 planePoint)
        {
            Vector3 planeNormal = Vector3.Cross(planeForward, planeRight).normalized;
            Vector3 projectOnPlane = Vector3.ProjectOnPlane(point - planePoint, planeNormal);
            Vector3 projectX = Vector3.Project(projectOnPlane, planeRight);
            float angleX = Vector3.Angle(projectX, planeRight) - 90;
            Vector3 projectZ = Vector3.Project(projectOnPlane, planeForward);
            float angleZ = Vector3.Angle(projectZ, planeForward) - 90;
            return new Vector3(projectX.magnitude * -Mathf.Sign(angleX), 0, projectZ.magnitude * -Mathf.Sign(angleZ));
        }

        /// <summary>
        /// 找到 多边形上 的, 距离 多边形外部某点 的 最近距离点
        /// 需要保证外部点和多边形在一个平面上
        /// </summary>
        /// <param name="outsidePoint"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static Vector3 FindClosestPointOnPolygonWithOutsidePointApproximately(Vector3 outsidePoint, List<Vector3> polygon)
        {
            // 先拿到点在边上的投射点
            // 如果点在线段上, 则直接使用两点的距离作为点边最小距离
            // 如果点超出线段范围, 则使用点到两个端点的最小值作为点边最小距离
            // 遍历所有点边距离, 找到点边距离最小的, 则使用该线段上的点作为 最近距离点
            float[] distances = new float[polygon.Count];
            Vector3[] closestPoints = new Vector3[polygon.Count];
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector3 a = polygon[i];
                Vector3 b = polygon[(i + 1) % polygon.Count];
                Vector3 projectPoint = a + Vector3.Project(outsidePoint - a, b - a);
                distances[i] = Vector3.Distance(projectPoint, outsidePoint);
                closestPoints[i] = projectPoint;
                float angle = Vector3.Angle(projectPoint - a, b - a);
                if (angle > 90)
                {
                    distances[i] = Vector3.Distance(outsidePoint, a);
                    closestPoints[i] = a;
                }
                angle = Vector3.Angle(projectPoint - b, a - b);
                if (angle > 90)
                {
                    distances[i] = Vector3.Distance(outsidePoint, b);
                    closestPoints[i] = b;
                }
            }


            float minDistance = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (distances[i] < minDistance)
                {
                    minDistance = distances[i];
                    closestPoint = closestPoints[i];
                }
            }

            return closestPoint;
        }

        public Vector3 ForwardInPlane()
        {
            if (plane) return Vector3.ProjectOnPlane(ToForward, plane.up).normalized;
            throw new ArgumentException("使用plane关联属性时需要保证plane不为空");
        }
        
        public Vector3 RightInPlane()
        {
            if (plane) return Vector3.ProjectOnPlane(ToRight, plane.up).normalized;
            throw new ArgumentException("使用plane关联属性时需要保证plane不为空");
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showGizoms)
            {
                Gizmos.color = Color.yellow;

                var center = centerPos;
                Gizmos.DrawLine(toPosition, center);
                Gizmos.DrawWireSphere(center, 2f);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(toPosition, 2f);

                Gizmos.color = Color.green;

                if (plane != null)
                {
                    for (int i = 0; i < boundarys.Count; i++)
                    {
                        Vector3 v3 = boundarys[i];
                        v3 = plane.position + plane.right * v3.x + plane.forward * v3.z;

                        Vector3 v3Next = boundarys[(i + 1) % boundarys.Count];
                        v3Next = plane.position + plane.right * v3Next.x + plane.forward * v3Next.z;

                        Gizmos.DrawLine(v3, v3Next);
                    }
                }
            }

        }
#endif

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CommonCamera))]
    public class CommonCameraEditor : Editor
    {

        private CommonCamera commonCamera;

        private void OnEnable()
        {
            commonCamera = target as CommonCamera;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            GUIStyle groupBoxStyle = new GUIStyle("GroupBox");
            groupBoxStyle.padding = new RectOffset(16, 10, 10, 10);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(commonCamera), typeof(CommonCamera), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            Property("showGizoms", "显示Gizmos");
            Property("plane", "目标平面");
            Property("inputMoudule", "操作模式");
            Property("isInitToOnAwake", "Awake时初始化目标坐标/角度");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            Property("moveSpeed", "移动 速度");
            Property("rotateSpeedHorizontal", "水平旋转 速度");
            Property("rotateSpeedVertical", "垂直旋转 速度");
            Property("scaleSpeed", "缩放 速度");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            EditorGUILayout.HelpBox("缩放缓动 与 移动缓动 共享该值", MessageType.Info);
            Property("moveSlowdown", "移动/缩放 缓动");
            Property("rotateSlowdown", "旋转 缓动");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            EditorGUILayout.LabelField("设置目标时的缓动");
            Property("lookTargetMoveSlowdown", "移动/缩放 缓动");
            Property("lookTargetRotSlowdown", "旋转 缓动");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            Property("canMove", "允许移动");
            Property("canRotate", "允许旋转");
            Property("canScale", "允许缩放");
            Property("verticalRotateRange", "垂直旋转角度限制");
            Property("scaleRange", "缩放限制 (相机目标位置到相机朝向中心的距离)");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(groupBoxStyle);
            Property("boundarys", "中心移动限制");
            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }

        private void Property(string name, string label) => EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(label));

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            var plane = serializedObject.FindProperty("plane").objectReferenceValue as Transform;
            if (plane == null) return;

            var boundarys = serializedObject.FindProperty("boundarys");

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            for (int i = 0; i < boundarys.arraySize; i++)
            {
                var v3 = boundarys.GetArrayElementAtIndex(i);
                Vector3 src = v3.vector3Value;
                Vector3 point = plane.position + plane.right * src.x + plane.forward * src.z;
                Handles.Label(point, i + "", style);
                Vector3 pointNext = Handles.PositionHandle(point, plane.rotation);
                v3.vector3Value = CommonCamera.GetPointXZFromProjectedPlane(pointNext, plane.forward, plane.right, plane.position);
            }
            // 这里应用值会实时覆盖Undo, 所以要排除
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

    }
#endif

}


