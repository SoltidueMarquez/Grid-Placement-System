using System;
using System.Linq;
using UnityEngine;

namespace GridPlacement
{
    public class PlacementState : IBuildingState
    {
        private int selectedObjectIndex = -1;
        private Grid grid;
        private PreviewSystem previewSystem;
        private ObjectsDatabaseSo database;
        private GridData floorData;
        private GridData furnitureData;
        private ObjectPlacer objectPlacer;
        
        private int currentRotation = 0; // 新增旋转角度跟踪
        private Vector2Int originalSize = new Vector2Int(); // 存储原始尺寸

        /// <summary>
        /// 放置状态
        /// </summary>
        /// <param name="placeGameObjectIndex">物品的序列索引</param>
        /// <param name="grid"></param>
        /// <param name="previewSystem"></param>
        /// <param name="database"></param>
        /// <param name="floorData"></param>
        /// <param name="furnitureData"></param>
        /// <param name="objectPlacer"></param>
        /// <exception cref="Exception"></exception>
        public PlacementState(int placeGameObjectIndex, Grid grid,  PreviewSystem previewSystem,  ObjectsDatabaseSo database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
        {
            this.grid = grid;
            this.previewSystem = previewSystem;
            this.database = database;
            this.floorData = floorData;
            this.furnitureData = furnitureData;
            this.objectPlacer = objectPlacer;
            // 开始预览放置
            selectedObjectIndex = database.objectsData.FindIndex(data => data.id == placeGameObjectIndex);
            if (selectedObjectIndex > -1)
            {
                Vector2Int newSize;// 首先计算网格指示器的尺寸
                var selectedObject = database.objectsData[selectedObjectIndex];
                if (selectedObject.ifIrregularShape && selectedObject.irregularSize.Count > 0)
                {
                    // size.x 取 irregularSize 的行数
                    var width = selectedObject.irregularSize.Count;
                    // irregularSize的各个元素大小可能不相同，size.y 取所有行中 values.Count 的最大值
                    var height = selectedObject.irregularSize.Max(row => row.values.Count);
                    newSize = new Vector2Int(width, height);
                }
                else { newSize = selectedObject.size; }
                
                previewSystem.StartShowingPlacementPreview(selectedObject.prefab, newSize);
                originalSize = database.objectsData[selectedObjectIndex].size;
            }
            else throw new System.Exception($"No object with ID {placeGameObjectIndex}");
        }

        // 结束状态
        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }
        
        //执行放置
        public void OnAction(Vector3Int gridPosition)
        {
            // 合法性判断
            var placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                //TODO:播放警告音频
                return;
            }
            
            //TODO:播放放置音频
            
            // 放置
            var rotation = Quaternion.Euler(0, currentRotation, 0);
            var index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].prefab, gridPosition, rotation);
            //更新对应的gridData
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            if (database.objectsData[selectedObjectIndex].ifIrregularShape)// 如果是异形家具就要传入异形尺寸描述
            {
                selectedData.AddObjectAt(gridPosition,
                    database.objectsData[selectedObjectIndex].size, currentRotation,
                    database.objectsData[selectedObjectIndex].id,
                    index, 
                    database.objectsData[selectedObjectIndex].irregularSize);
            }
            else
            {
                selectedData.AddObjectAt(gridPosition,
                    database.objectsData[selectedObjectIndex].size, currentRotation,
                    database.objectsData[selectedObjectIndex].id,
                    index);
            }
            // 更新预览表现
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            // 判断是地板物体还是家具物体
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            //判断合法性
            return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].size, currentRotation);
        }

        #region 旋转
        // 在PlacementState中添加旋转处理
        public void UpdateRotation(Vector3Int gridPosition, int rotateAngle)
        {
            currentRotation += rotateAngle;
            currentRotation %= 360;

            previewSystem.UpdateRotate(rotateAngle, CheckPlacementValidity(gridPosition, selectedObjectIndex));
        }
        #endregion

        // 更新状态
        public void UpdateState(Vector3Int gridPosition)
        {
            var placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}