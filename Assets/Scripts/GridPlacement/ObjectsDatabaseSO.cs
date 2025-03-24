using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridPlacement
{
    [CreateAssetMenu]
    public class ObjectsDatabaseSo : ScriptableObject
    {
        public List<ObjectData> objectsData;
    }
    
    [Serializable]
    public class ObjectData
    {
        [field: SerializeField, Tooltip("名称")] public string objectName { get; private set; }
        [field: SerializeField, Tooltip("ID")] public int id { get; private set; }
        [field: SerializeField, Tooltip("预制体")] public GameObject prefab { get; private set; }
        
        [field: SerializeField, Tooltip("是否异形")] public bool ifIrregularShape { get; private set; } 
        [field: SerializeField, Tooltip("大小")] public Vector2Int size { get; private set; } = Vector2Int.one;
        [field: SerializeField, Tooltip("异形大小")] public List<BoolListWrapper> irregularSize { get; private set; }  = new List<BoolListWrapper>();
    }
    
    [System.Serializable]
    public class BoolListWrapper // 为了方便序列化对List<bool>做一个包装
    {
        public List<bool> values = new List<bool>();
        public int count => values.Count;
    }
}