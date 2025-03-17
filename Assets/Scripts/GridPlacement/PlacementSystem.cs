using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridPlacement
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField, Tooltip("鼠标指示器")] private GameObject mouseIndicator;

        [SerializeField, Tooltip("网格")] private Grid grid;

        [SerializeField, Tooltip("物体数据")] private ObjectsDatabaseSo database;
        private int selectedObjectIndex = -1;// 当前选择的物体序号

        [SerializeField, Tooltip("可视网格")] private GameObject gridVisualization;

        private GridData floorData, furnitureData;// 地板类物体放置信息、家具类物体放置信息

        private List<GameObject> palcedGameObjects = new List<GameObject>();
        
        private Vector3Int lastDetectedPosition = Vector3Int.zero;
        
        private InputManager inputManager;
        private PreviewSystem preview;
        

        private void Start()
        {
            inputManager = InputManager.Instance;
            preview = PreviewSystem.Instance;
            StopPlacement();
            floorData = new GridData();
            furnitureData = new GridData();
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
            preview.StartShowingPlacementPreview(// 显示预览
                database.objectsData[selectedObjectIndex].prefab,
                database.objectsData[selectedObjectIndex].size);
            inputManager.OnClicked += PlaceStructure;// 订阅事件
            inputManager.OnExit += StopPlacement;
        }

        private void PlaceStructure()
        {
            if (inputManager.IsPointerOverUI()) return;
            // 获取位置
            var mousePosition = inputManager.GetSelectedMapPosition();
            var gridPosition = grid.WorldToCell(mousePosition);
            // 合法性判断
            var placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                //TODO:播放警告音频
                return;
            }
            
            //TODO:播放放置音频
            
            // 放置
            var structure = Instantiate(database.objectsData[selectedObjectIndex].prefab);
            structure.transform.position = gridPosition;
            palcedGameObjects.Add(structure);
            //更新对应的gridData
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            selectedData.AddObjectAt(gridPosition,
                database.objectsData[selectedObjectIndex].size,
                database.objectsData[selectedObjectIndex].id,
                palcedGameObjects.Count - 1);
            
            // 更新预览表现
            preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            // 判断是地板物体还是家具物体
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            //判断合法性
            return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].size);
        }

        private void StopPlacement()
        {
            selectedObjectIndex = -1;
            gridVisualization.SetActive(false);// 隐藏网格
            preview.StopShowingPreview();// 隐藏预览
            inputManager.OnClicked -= PlaceStructure;// 取消订阅事件
            inputManager.OnExit -= StopPlacement;
            lastDetectedPosition = Vector3Int.zero;
        }

        private void Update()
        {
            if (selectedObjectIndex < 0) return;// 如果没有物体就直接返回
            
            var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
            var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置

            if (lastDetectedPosition != gridPosition)// 更新预览表现
            {
                var placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
                mouseIndicator.transform.position = mousePosition;
                preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            }
        }
    }
}