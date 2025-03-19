using UnityEngine;

namespace GridPlacement
{
    public class RemovingState : IBuildingState
    {
        private int gameObjectIndex = -1;
        private Grid grid;
        private PreviewSystem previewSystem;
        private GridData floorData;
        private GridData furnitureData;
        private ObjectPlacer objectPlacer; 

        public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
        {
            this.grid = grid;
            this.previewSystem = previewSystem;
            this.floorData = floorData;
            this.furnitureData = furnitureData;
            this.objectPlacer = objectPlacer;
            previewSystem.StartShowingRemovePreview();
        }

        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }

        public void OnAction(Vector3Int gridPosition)
        {
            GridData selectedData = null;// 首先判断要删除的物体是地板物体还是家具物体
            if(!furnitureData.CanPlaceObjectAt(gridPosition,Vector2Int.one)) { selectedData = furnitureData; }
            else if(!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) { selectedData = floorData; }

            if(selectedData == null)
            {
                //TODO:播放错误音效
            }
            else
            {
                //TODO:播放移除音效
                gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);//获取物体的索引
                if (gameObjectIndex == -1)
                    return;
                selectedData.RemoveObjectAt(gridPosition);
                objectPlacer.RemoveObjectAt(gameObjectIndex);
            }
            
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), CheckIfSelectionIsValid(gridPosition));
        }

        private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
        {
            return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            var validity = CheckIfSelectionIsValid(gridPosition);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
        }
    }
}
