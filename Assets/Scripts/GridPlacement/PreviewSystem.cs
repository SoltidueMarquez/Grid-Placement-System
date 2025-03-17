using UnityEngine;

namespace GridPlacement
{
    public class PreviewSystem : Singleton<PreviewSystem>
    {
        [SerializeField, Tooltip("预览物体高度偏差值")] private float previewYOffset = 0.06f;

        [SerializeField, Tooltip("网格指示器")] private GameObject cellIndicator;
        private GameObject previewObject;   // 预览物体

        [SerializeField] private Material previewMaterialPrefab; // 透明预览材质
        private Material previewMaterialInstance;                // 透明预览材质实例

        private Renderer cellIndicatorRenderer;         // 网格指示器渲染器

        private void Start()
        {
            previewMaterialInstance = new Material(previewMaterialPrefab);// 在预览材质上创建一个副本用于修改
            cellIndicator.SetActive(false);// 隐藏网格指示器
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();// 获取网格指示器材质
        }

        public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
        {
            previewObject = Instantiate(prefab);// 生成预览物体
            PreparePreview(previewObject);
            PrepareCursor(size);
            cellIndicator.SetActive(true);
        }

        private void PrepareCursor(Vector2Int size) // 更改网格指示器的范围
        {
            if (size is { x: <= 0, y: <= 0 }) return;
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y); // 扩大网格显示器
            cellIndicatorRenderer.material.mainTextureScale = size;
        }

        private void PreparePreview(GameObject previewObject) // 更改预览物体的材质
        {
            var renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach(var renderer in renderers)
            {
                var materials = renderer.materials;
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterialInstance;
                }
                renderer.materials = materials;
            }
        }

        public void StopShowingPreview()
        {
            cellIndicator.SetActive(false); // 隐藏网格指示器
            if(previewObject!= null) Destroy(previewObject); // 摧毁预览物体
        }

        public void UpdatePosition(Vector3 position, bool validity) // 位置更新与颜色更改
        {
            if(previewObject != null)
            {
                MovePreview(position);
                ApplyFeedbackToPreview(validity);
            }
            MoveCursor(position);
            ApplyFeedbackToCursor(validity);
        }

        private void ApplyFeedbackToPreview(bool validity) // 更改预览物体颜色
        {
            var c = validity ? Color.white : Color.red;
            c.a = 0.5f;
            previewMaterialInstance.color = c;
        }

        private void ApplyFeedbackToCursor(bool validity) // 更改网格指示器颜色
        {
            var c = validity ? Color.white : Color.red;
            c.a = 0.5f;
            cellIndicatorRenderer.material.color = c;
        }

        private void MoveCursor(Vector3 position) // 移动网格指示器
        {
            cellIndicator.transform.position = position;
        }

        private void MovePreview(Vector3 position) // 移动预览物体
        {
            previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
        }

        internal void StartShowingRemovePreview()
        {
            cellIndicator.SetActive(true);
            PrepareCursor(Vector2Int.one);
            ApplyFeedbackToCursor(false);
        }
    }
}
