using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace GridPlacement
{
    public class GridData
    {
        [OdinSerialize] private Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

        /// <summary>
        /// 在对应位置上添加物体
        /// </summary>
        /// <param name="gridPosition">网格位置</param>
        /// <param name="objectSize">物体占用大小</param>
        /// <param name="placeOperationID"></param>
        /// <param name="placedObjectIndex">物体id</param>
        public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int rotateAngle, int placedObjectIndex, int placeOperationID, List<BoolListWrapper> irregularSize = null)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize, rotateAngle, irregularSize);  //计算占用位置
            var data = new PlacementData(positionToOccupy, placeOperationID, placedObjectIndex); //构造放置数据
            foreach (var pos in positionToOccupy.Where(pos => placedObjects.ContainsKey(pos)))//冲突判断
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            foreach (var pos in positionToOccupy) {placedObjects[pos] = data;}      //将放置数据放入每个占用位置的字典项
        }
        
        // 返回占用网格坐标的列表,出现了旋转的情况，返回的size可能会出现x，y小于0的情况，这个时候需要--遍历所有的点
        private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, int rotateAngle, List<BoolListWrapper> irregularSize = null)
        {
            var positionToOccupy = new List<Vector3Int>();
            if (irregularSize == null || irregularSize.Count == 0)// 规则网格占用计算
            {
                for (var x = 0; x < objectSize.x; x++)
                {
                    for (var y = 0; y < objectSize.y; y++)
                    {
                        positionToOccupy.Add(gridPosition + new Vector3Int(x, 0, y));
                    }
                }
            }
            else // 异形网格占用计算
            {
                for (var x = 0; x < irregularSize.Count; x++)
                {
                    for (var y = 0; y < irregularSize[x]?.count; y++)
                    {
                        if (irregularSize[x].values[y])
                        {
                            positionToOccupy.Add(gridPosition + new Vector3Int(x, 0, y));
                        }
                    }
                }
            }
            // 计算实际占用的网格范围
            return RotatePositions(positionToOccupy, gridPosition, rotateAngle);
        }

        public static List<Vector3Int> RotatePositions(List<Vector3Int> positions, Vector3Int rotateCenter, int rotateAngle)
        {
            var rotatedPositions = new List<Vector3Int>();

            // 计算旋转原点（第一个方块的左下角）
            Vector3 rotateOrigin = new Vector3(rotateCenter.x - 0.5f, 0, rotateCenter.z - 0.5f);

            // 将角度转换为弧度
            float radian = Mathf.Deg2Rad * rotateAngle;
            float cosTheta = Mathf.Cos(radian);
            float sinTheta = Mathf.Sin(radian);

            foreach (var pos in positions)
            {
                // 计算相对左下角的坐标
                float relativeX = pos.x - rotateOrigin.x;
                float relativeZ = pos.z - rotateOrigin.z;

                // 旋转计算（绕左下角旋转）
                float rotatedX = relativeX * cosTheta + relativeZ * sinTheta; // 注意这里的正负调整
                float rotatedZ = -relativeX * sinTheta + relativeZ * cosTheta;

                // 计算新的中心点，回到网格坐标
                int newX = Mathf.RoundToInt(rotatedX + rotateOrigin.x);
                int newZ = Mathf.RoundToInt(rotatedZ + rotateOrigin.z);

                // 生成新的位置
                rotatedPositions.Add(new Vector3Int(newX, pos.y, newZ));
            }

            return rotatedPositions;
        }
        
        // 是否可以放置物体
        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int rotateAngle = 0)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize, rotateAngle);
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
        [field: SerializeField] public int operationID { get; private set; }         // 用于移除操作
        [field: SerializeField] public int placedObjectIndex { get; private set; }
        
        public PlacementData(List<Vector3Int> occupiedPositions, int operationID, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            this.operationID = operationID;
            this.placedObjectIndex = placedObjectIndex;
        }
    }
}