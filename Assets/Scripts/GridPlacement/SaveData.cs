using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridPlacement
{
    [CreateAssetMenu]
    public class SaveData : ScriptableObject
    {
        public HashSet<int> availableIDs = new HashSet<int>(); // 存储回收的ID
        public Dictionary<int, PlaceInfo> placeGameObjectsDic = new Dictionary<int, PlaceInfo>();// 用字典存放场景的物体
        
        public GridData floorData, furnitureData;// 地板类物体放置信息、家具类物体放置信息

        public void Clear()
        {
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
        public Vector3Int gridPosition; // 位置
        public Quaternion rotation; // 旋转

        public PlaceInfo(GameObject structure, Vector3Int gridPosition, Quaternion rotation)
        {
            this.structure = structure;
            this.gridPosition = gridPosition;
            this.rotation = rotation;
        }
    }
}