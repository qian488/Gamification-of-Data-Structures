using UnityEngine;

namespace Sokoban
{
    public class UIRoot : MonoBehaviour
    {
        public static GameObject CreateUIRoot()
        {
            // 创建Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // 创建层级
            CreateLayer(canvasObj.transform, "Bot");
            CreateLayer(canvasObj.transform, "Mid");
            CreateLayer(canvasObj.transform, "Top");
            CreateLayer(canvasObj.transform, "System");

            return canvasObj;
        }

        private static void CreateLayer(Transform parent, string name)
        {
            GameObject layer = new GameObject(name, typeof(RectTransform));
            layer.transform.SetParent(parent, false);
            RectTransform rect = layer.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
} 