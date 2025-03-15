using System;
using UnityEngine;

namespace GridPlacement
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField, Tooltip("鼠标指示器")] private GameObject mouseIndicator;
        [SerializeField, Tooltip("单元格指示器")] private GameObject cellIndicator;
        [SerializeField, Tooltip("网格")] private Grid grid;

        [SerializeField, Tooltip("物体数据")] private ObjectsDatabaseSo database;
        private int selectedObjectIndex = -1;// 当前选择的物体序号

        [SerializeField, Tooltip("可视网格")] private GameObject gridVisualization; 
        
        private InputManager inputManager;

        private void Start()
        {
            inputManager = InputManager.Instance;

            StopPlacement();
        }

        public void StartPlacement(int id)
        {
            StopPlacement();
            selectedObjectIndex = database.objectsData.FindIndex(data => data.id == id);
            if (selectedObjectIndex < 0)
            {
                Debug.LogError($"No ID found:{id}");
                return;
            }

            gridVisualization.SetActive(true);// 显示网格
            cellIndicator.SetActive(true);// 显示单元格指示器
            inputManager.OnClicked += PlaceStructure;// 订阅事件
            inputManager.OnExit += StopPlacement;
        }

        private void PlaceStructure()
        {
            if (inputManager.IsPointerOverUI()) return;
            // 获取位置
            var mousePosition = inputManager.GetSelectedMapPosition();
            var gridPosition = grid.WorldToCell(mousePosition);
            // 放置
            var structure = Instantiate(database.objectsData[selectedObjectIndex].prefab);
            structure.transform.position = gridPosition;
        }

        private void StopPlacement()
        {
            selectedObjectIndex = -1;
            gridVisualization.SetActive(false);// 隐藏网格
            cellIndicator.SetActive(false);// 隐藏单元格指示器
            inputManager.OnClicked -= PlaceStructure;// 取消订阅事件
            inputManager.OnExit -= StopPlacement;
        }

        private void Update()
        {
            if (selectedObjectIndex < 0) return;//如果没有物体就直接返回
            
            var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
            var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置
            mouseIndicator.transform.position = mousePosition;
            cellIndicator.transform.position = gridPosition;
        }
    }
}