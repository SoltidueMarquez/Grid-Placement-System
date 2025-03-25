using UnityEngine;

namespace GridPlacement
{
    public class SaveManager : Singleton<SaveManager>
    {
        [field: SerializeField, Tooltip("存档")] public SaveData data { get; private set;}
    }
}