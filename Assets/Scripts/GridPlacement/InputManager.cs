using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridPlacement
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField, Tooltip("场景相机")] private Camera sceneCamera;

        private Vector3 lastPosition;

        [SerializeField, Tooltip("放置层")] private LayerMask placementLayerMask;

        [SerializeField, Tooltip("最大射线检测距离")] private float maxRayDistance = 100f;

        [SerializeField, Tooltip("放置按键"), Range(0, 1)] private int placeKey = 0;
        [SerializeField, Tooltip("取消按键")] private KeyCode cancelKey = KeyCode.Escape;

        public event Action OnClicked, OnExit;  // 点击和取消事件

        private void Update()
        {
            if(Input.GetMouseButtonDown(placeKey))
                OnClicked?.Invoke();
            if(Input.GetKeyDown(cancelKey))
                OnExit?.Invoke();
        }

        public bool IsPointerOverUI() // 检测鼠标或触摸是否悬停在 UI 界面上。
            => EventSystem.current.IsPointerOverGameObject();
        
        public Vector3 GetSelectedMapPosition()
        {
            var mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            var ray = sceneCamera.ScreenPointToRay(mousePos);// 从鼠标位置打出一条射线
            if (Physics.Raycast(ray, out var hit, maxRayDistance, placementLayerMask))
            {
                lastPosition = hit.point;// 获取射线打到的点
            }

            return lastPosition;
        }
    }
}

