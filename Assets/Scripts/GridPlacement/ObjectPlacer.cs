using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridPlacement
{
    public class ObjectPlacer : Singleton<ObjectPlacer>
    {
        private HashSet<int> availableIDs; // 存储回收的ID
        private Dictionary<int, PlaceInfo> placeGameObjectsDic;// 用字典存放场景的物体

        private void Start()
        {
            availableIDs = SaveManager.Instance.data.availableIDs;
            placeGameObjectsDic = SaveManager.Instance.data.placeGameObjectsDic;
        }

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
                return SaveManager.Instance.data.nextID++; // 生成新的ID
            }
        }

        // 放置物体
        public int PlaceObject(GameObject prefab, Vector3Int gridPosition, Quaternion rotation)
        {
            var structure = Instantiate(prefab, gridPosition, rotation);
            // 添加字典项
            var id = GetUniqueID();
            placeGameObjectsDic.Add(id, new PlaceInfo(structure, prefab, gridPosition, rotation));
            
            return id;
        }

        // 移除物体
        internal void RemoveObjectAt(int gameObjectIndex)
        {
            if (!placeGameObjectsDic.ContainsKey(gameObjectIndex)) return;
            Destroy(placeGameObjectsDic[gameObjectIndex].structure);// 摧毁物体
            placeGameObjectsDic.Remove(gameObjectIndex);
            availableIDs.Add(gameObjectIndex); // 回收ID
        }
    }
}