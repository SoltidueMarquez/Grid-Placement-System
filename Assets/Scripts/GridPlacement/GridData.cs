using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridPlacement
{
    public class GridData
    {
        Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

        /// <summary>
        /// 在对应位置上添加物体
        /// </summary>
        /// <param name="gridPosition">网格位置</param>
        /// <param name="objectSize">物体占用大小</param>
        /// <param name="placeOperationID"></param>
        /// <param name="placedObjectIndex">物体id</param>
        public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int placedObjectIndex, int placeOperationID)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize);  //计算占用位置
            var data = new PlacementData(positionToOccupy, placeOperationID, placedObjectIndex); //构造放置数据
            foreach (var pos in positionToOccupy.Where(pos => placedObjects.ContainsKey(pos)))//冲突判断
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            foreach (var pos in positionToOccupy) {placedObjects[pos] = data;}      //将放置数据放入每个占用位置的字典项
        }
        
        // 返回占用网格坐标的列表
        private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var positionToOccupy = new List<Vector3Int>();
            for (var x = 0; x < objectSize.x; x++)
            {
                for (var y = 0; y < objectSize.y; y++)
                {
                    positionToOccupy.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
            return positionToOccupy;
        }
        
        // 是否可以放置物体
        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize);
            return positionToOccupy.All(pos => !placedObjects.ContainsKey(pos));
        }
        
        // 获取当前操作在操作列表中的序列索引
        internal int GetRepresentationIndex(Vector3Int gridPosition)
        {
            if (!placedObjects.ContainsKey(gridPosition))
                return -1;
            return placedObjects[gridPosition].operationID;
        }

        // 从字典中移除当前物体
        internal void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var pos in placedObjects[gridPosition].occupiedPositions)
            {
                placedObjects.Remove(pos);
            }
        }
    }

    public class PlacementData
    {
        public List<Vector3Int> occupiedPositions;  // 此物体占用的位置
        public int operationID { get; private set; }         // 用于移除操作
        public int placedObjectIndex { get; private set; }
        
        public PlacementData(List<Vector3Int> occupiedPositions, int operationID, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            this.operationID = operationID;
            this.placedObjectIndex = placedObjectIndex;
        }
    }
}