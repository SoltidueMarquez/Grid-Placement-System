using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GridPlacement
{
    [CreateAssetMenu]
    public class SaveData : SerializedScriptableObject
    {
        [OdinSerialize] public HashSet<int> availableIDs = new HashSet<int>(); // 存储回收的ID
        [OdinSerialize] public Dictionary<int, PlaceInfo> placeGameObjectsDic = new Dictionary<int, PlaceInfo>();// 用字典存放场景的物体
        public int nextID = 0;// 递增的唯一ID
        [OdinSerialize] public GridData floorData, furnitureData;// 地板类物体放置信息、家具类物体放置信息

        [Button("重置")] public void Clear()
        {
            nextID = 0;
            availableIDs.Clear();
            placeGameObjectsDic.Clear();
            floorData = new GridData();
            furnitureData = new GridData();
        }
    }
    
    [Serializable]
    public class PlaceInfo
    {
        public GameObject structure; // 物体
        public GameObject prefab; // 预制体
        public Vector3Int gridPosition; // 位置
        public Quaternion rotation; // 旋转

        public PlaceInfo(GameObject structure, GameObject prefab, Vector3Int gridPosition, Quaternion rotation)
        {
            this.structure = structure;
            this.prefab = prefab;
            this.gridPosition = gridPosition;
            this.rotation = rotation;
        }
    }
}