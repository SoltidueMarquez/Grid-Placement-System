using UnityEngine;

namespace GridPlacement
{
    public class PlacementState : IBuildingState
    {
        private int selectedObjectIndex = -1;
        private int id;
        private Grid grid;
        private PreviewSystem previewSystem;
        private ObjectsDatabaseSo database;
        private GridData floorData;
        private GridData furnitureData;
        private ObjectPlacer objectPlacer;

        public PlacementState(int id, Grid grid,  PreviewSystem previewSystem,  ObjectsDatabaseSo database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
        {
            this.id = id;
            this.grid = grid;
            this.previewSystem = previewSystem;
            this.database = database;
            this.floorData = floorData;
            this.furnitureData = furnitureData;
            this.objectPlacer = objectPlacer;
            // 开始预览放置
            selectedObjectIndex = database.objectsData.FindIndex(data => data.id == id);
            if (selectedObjectIndex > -1)
            {
                previewSystem.StartShowingPlacementPreview(
                    database.objectsData[selectedObjectIndex].prefab,
                    database.objectsData[selectedObjectIndex].size);
            }
            else throw new System.Exception($"No object with ID {id}");
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
            var index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].prefab, gridPosition);
            //更新对应的gridData
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            selectedData.AddObjectAt(gridPosition,
                database.objectsData[selectedObjectIndex].size,
                database.objectsData[selectedObjectIndex].id,
                index);
            
            // 更新预览表现
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            // 判断是地板物体还是家具物体
            var selectedData = database.objectsData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            //判断合法性
            return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].size);
        }

        
        // 更新状态
        public void UpdateState(Vector3Int gridPosition)
        {
            var placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }

    }
}