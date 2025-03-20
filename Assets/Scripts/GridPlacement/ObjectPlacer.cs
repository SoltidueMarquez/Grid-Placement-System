using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridPlacement
{
    public class ObjectPlacer : Singleton<ObjectPlacer>
    {
        private HashSet<int> availableIDs = new HashSet<int>(); // 存储回收的ID
        private int nextID = 0; // 递增的唯一ID
        private Dictionary<int, GameObject> placeGameObjectsDic = new Dictionary<int, GameObject>();// 用字典存放场景的物体

        /// <summary>
        /// 获取一个唯一的ID
        /// </summary>
        private int GetUniqueID()
        {
            if (availableIDs.Count > 0)
            {
                var id = availableIDs.OrderBy(x => x).First(); // 获取最小ID
                availableIDs.Remove(id); // 从回收池删除
                return id;
            }
            else
            {
                return nextID++; // 生成新的ID
            }
        }

        // 放置物体
        public int PlaceObject(GameObject prefab, Vector3Int gridPosition)
        {
            var structure = Instantiate(prefab);
            structure.transform.position = gridPosition;
            // 添加字典项
            var id = GetUniqueID();
            placeGameObjectsDic.Add(id, structure);
            
            return id;
        }
        
        // 移除物体
        internal void RemoveObjectAt(int gameObjectIndex)
        {
            if (!placeGameObjectsDic.ContainsKey(gameObjectIndex)) return;
            Destroy(placeGameObjectsDic[gameObjectIndex]);// 摧毁物体
            placeGameObjectsDic.Remove(gameObjectIndex);
            availableIDs.Add(gameObjectIndex); // 回收ID
        }
    }
}