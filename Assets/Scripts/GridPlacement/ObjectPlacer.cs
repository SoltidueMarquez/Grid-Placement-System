using System.Collections.Generic;
using UnityEngine;

namespace GridPlacement
{
    public class ObjectPlacer : Singleton<ObjectPlacer>
    {
        private List<GameObject> palcedGameObjects = new List<GameObject>();

        public int PlaceObject(GameObject prefab, Vector3Int gridPosition)
        {
            var structure = Instantiate(prefab);
            structure.transform.position = gridPosition;
            palcedGameObjects.Add(structure);
            return palcedGameObjects.Count - 1;
        }
        
        internal void RemoveObjectAt(int gameObjectIndex)
        {
            if (palcedGameObjects.Count <= gameObjectIndex || palcedGameObjects[gameObjectIndex] == null) return;
            Destroy(palcedGameObjects[gameObjectIndex]);
            palcedGameObjects[gameObjectIndex] = null;
        }
    }
}