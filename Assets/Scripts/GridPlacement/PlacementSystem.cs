using System;
using UnityEngine;

namespace GridPlacement
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField,Tooltip("鼠标指示器")] private GameObject mouseIndicator;
        [SerializeField,Tooltip("单元格指示器")] private GameObject cellIndicator;
        [SerializeField,Tooltip("网格")] private Grid grid;
        private InputManager inputManager;

        private void Start()
        {
            inputManager = InputManager.Instance;
        }

        private void Update()
        {
            var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
            var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置
            mouseIndicator.transform.position = mousePosition;
            cellIndicator.transform.position = gridPosition;
        }
    }
}