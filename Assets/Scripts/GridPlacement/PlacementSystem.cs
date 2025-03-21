using UnityEngine;

namespace GridPlacement
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField, Tooltip("网格")] private Grid grid;
        [SerializeField, Tooltip("可视网格")] private GameObject gridVisualization;
        private GridData floorData, furnitureData;// 地板类物体放置信息、家具类物体放置信息

        [SerializeField, Tooltip("物体数据")] private ObjectsDatabaseSo database;

        private Vector3Int lastDetectedPosition = Vector3Int.zero;
        
        private InputManager inputManager;
        private PreviewSystem preview;
        private ObjectPlacer objectPlacer;// 逻辑与实现分离

        private IBuildingState buildingState;// 当前状态

        private void Start()
        {
            inputManager = InputManager.Instance;
            preview = PreviewSystem.Instance;
            objectPlacer = ObjectPlacer.Instance;
            
            gridVisualization.SetActive(false);
            floorData = new GridData();
            furnitureData = new GridData();
        }

        public void StartPlacement(int id)
        {
            StopPlacement();
            gridVisualization.SetActive(true);// 显示网格
            // 创新建造状态
            buildingState = new PlacementState(id, grid, preview, database, floorData, furnitureData, objectPlacer);
            // 订阅事件
            inputManager.OnClicked += PlaceStructure;
            inputManager.OnExit += StopPlacement;
            inputManager.OnRotateLeft += RotateStructureLeft;
            inputManager.OnRotateRight += RotateStructureRight;
        }

        public void StartRemoving()
        {
            StopPlacement();
            gridVisualization.SetActive(true) ;
            buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
            // 订阅事件
            inputManager.OnClicked += PlaceStructure;
            inputManager.OnExit += StopPlacement;
        }

        #region 旋转
        private void RotateStructureLeft()
        {
            Debug.Log("左转");
            if (buildingState is PlacementState placementState)
            {
                var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
                var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置
                var rotateAngle = -90;
                placementState.UpdateRotation(gridPosition, rotateAngle);
            }
        }
        private void RotateStructureRight()
        {
            Debug.Log("右转");
            if (buildingState is PlacementState placementState)
            {
                var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
                var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置
                var rotateAngle = 90;
                placementState.UpdateRotation(gridPosition, rotateAngle);
            }
        }
        #endregion
        
        
        private void PlaceStructure()
        {
            if (inputManager.IsPointerOverUI()) return;
            // 获取位置
            var mousePosition = inputManager.GetSelectedMapPosition();
            var gridPosition = grid.WorldToCell(mousePosition);
            // 执行状态
            buildingState.OnAction(gridPosition);
        }

        private void StopPlacement()
        {
            // TODO:播放鼠标点击音效
            if (buildingState == null) return;// 如果没有状态就直接返回
            
            gridVisualization.SetActive(false);// 隐藏网格
            buildingState.EndState();// 结束状态
            inputManager.OnClicked -= PlaceStructure;// 取消订阅事件
            inputManager.OnExit -= StopPlacement;
            inputManager.OnRotateLeft -= RotateStructureLeft;
            inputManager.OnRotateRight -= RotateStructureRight;
            lastDetectedPosition = Vector3Int.zero;
            buildingState = null;
        }

        private void Update()
        {
            if (buildingState == null) return;// 如果没有状态就直接返回
            
            var mousePosition = inputManager.GetSelectedMapPosition();// 获取鼠标位置
            var gridPosition = grid.WorldToCell(mousePosition);// 计算单元格指示器的位置

            if (lastDetectedPosition != gridPosition)// 更新状态
            {
                buildingState.UpdateState(gridPosition);
                lastDetectedPosition = gridPosition;
            }
        }
    }
}