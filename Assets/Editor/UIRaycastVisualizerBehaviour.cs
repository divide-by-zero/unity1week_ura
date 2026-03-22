using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TohokuEpco.Editor
{
    /// <summary>
    /// UI当たり判定をSceneViewにオーバーレイ表示するデバッグツール（Editor実行時専用）。
    /// ON/OFF切り替え: SceneView の Overlays メニューから "UI Raycast Visualizer" を表示
    /// </summary>
    [InitializeOnLoad]
    public static class UIRaycastVisualizerBehaviour
    {
        private const string PrefKey = "UIRaycastVisualizer.Enabled";
        private const string OpacityKey = "UIRaycastVisualizer.Opacity";
        private const string DepthRangeKey = "UIRaycastVisualizer.DepthRange";

        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PrefKey, false);
            set => EditorPrefs.SetBool(PrefKey, value);
        }

        public static float Opacity
        {
            get => EditorPrefs.GetFloat(OpacityKey, 0.25f);
            set => EditorPrefs.SetFloat(OpacityKey, value);
        }

        public static float DepthRange
        {
            get => EditorPrefs.GetFloat(DepthRangeKey, 100f);
            set => EditorPrefs.SetFloat(DepthRangeKey, value);
        }

        static UIRaycastVisualizerBehaviour()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        // ---- 色定義 --------------------------------------------------------------
        private static readonly Color ColorBlockerFill = new Color(1.0f, 0.2f, 0.2f, 0.25f);
        private static readonly Color ColorBlockerBorder = new Color(1.0f, 0.2f, 0.2f, 0.90f);

        private static GUIStyle _labelStyle;
        private static GUIStyle _labelOutlineStyle;

        // ---- SceneView描画 -------------------------------------------------------

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!IsEnabled) return;
            if (sceneView != SceneView.lastActiveSceneView) return;

            var isRepaint = Event.current.type == EventType.Repaint;

            _labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                wordWrap = true,
            };
            _labelOutlineStyle ??= new GUIStyle(_labelStyle)
            {
                normal = { textColor = Color.black },
                hover = { textColor = Color.black },
            };

            var activeScene = SceneManager.GetActiveScene();

            // Graphic（RaycastTarget=true）→ 緑（Selectable有り）/ 赤（Selectable無し）
            var graphics = Object.FindObjectsByType<Graphic>(FindObjectsSortMode.None);
            System.Array.Sort(graphics, (a, b) =>
                CompareRenderOrder(a.canvas, a.transform, b.canvas, b.transform));

            var targetGraphics =
                graphics.Where(g => g.gameObject.scene == activeScene)
                    .Where(g => g.raycastTarget)
                    .Where(g =>
                    {
                        var cg = g.GetComponentInParent<CanvasGroup>();
                        return cg == null || IsBlockRaycastRecursive(cg);
                    }).ToList();

            if (targetGraphics.Any() == false) return;

            var beginz = targetGraphics.DefaultIfEmpty().First().transform.position.z;
            var graphicsCount = targetGraphics.Count;
            for (var index = 0; index < graphicsCount; index++)
            {
                var g = targetGraphics[index];
                var fillBase = ColorBlockerFill;
                fillBase.a = Opacity;
                var border = ColorBlockerBorder;
                string sortInfo = g.canvas != null ? $" [{g.canvas.sortingOrder}]" : "";
                DrawRect(g.rectTransform,
                    index / (float)graphicsCount,
                    beginz,
                    fillBase,
                    border,
                    $"{g.name}{sortInfo}\n{GetHierarchyPath(g.transform)}",
                    isRepaint);
            }
        }

        private static readonly Vector3[] _corners = new Vector3[4];

        private static void DrawRect(RectTransform rt, float depth, float z, Color fill, Color border, string label, bool isRepaint)
        {
            rt.GetWorldCorners(_corners);

            // 3D 的に当たり判定を表示したいので、描画順番にZを少しずつずらす
            for (var i = 0; i < _corners.Length; i++)
            {
                _corners[i].z = z - depth * DepthRange;
            }

            border.g = border.b = fill.b = fill.g = (1 - depth) * 0.8f;

            if (isRepaint)
            {
                Handles.DrawSolidRectangleWithOutline(_corners, fill, border);
            }

            Handles.BeginGUI();
// ワールド座標をGUI座標に変換
            var guiPos = HandleUtility.WorldToGUIPoint(_corners[1]);
            var content = new GUIContent(label);
            var size = _labelStyle.CalcSize(content);

            var rect = new Rect(guiPos.x, guiPos.y, size.x, size.y);

            // アウトライン（上下左右に1pxずらして黒で描画）
            const int o = 1;
            GUI.Label(new Rect(rect.x + o, rect.y + o, rect.width, rect.height), content, _labelOutlineStyle);

            // 見た目はラベル風、でもクリック判定あり
            if (GUI.Button(rect, content, _labelStyle))
            {
                Selection.activeGameObject = rt.gameObject;
                EditorGUIUtility.PingObject(rt.gameObject);
            }

            Handles.EndGUI();
        }

        // ---- ソート --------------------------------------------------------------

        private static int CompareRenderOrder(Canvas ca, Transform ta, Canvas cb, Transform tb)
        {
            int orderA = ca != null ? ca.sortingOrder : 0;
            int orderB = cb != null ? cb.sortingOrder : 0;
            if (orderA != orderB) return orderA.CompareTo(orderB);
            return CompareSiblingPath(ta, tb);
        }

        private static int CompareSiblingPath(Transform a, Transform b)
        {
            _siblingBufA.Clear();
            _siblingBufB.Clear();
            for (var t = a; t != null; t = t.parent) _siblingBufA.Insert(0, t.GetSiblingIndex());
            for (var t = b; t != null; t = t.parent) _siblingBufB.Insert(0, t.GetSiblingIndex());
            int len = Mathf.Min(_siblingBufA.Count, _siblingBufB.Count);
            for (int i = 0; i < len; i++)
                if (_siblingBufA[i] != _siblingBufB[i])
                    return _siblingBufA[i].CompareTo(_siblingBufB[i]);
            return _siblingBufA.Count.CompareTo(_siblingBufB.Count);
        }

        private static readonly System.Collections.Generic.List<int> _siblingBufA = new();
        private static readonly System.Collections.Generic.List<int> _siblingBufB = new();

        // ---- ユーティリティ ------------------------------------------------------

        private static string GetHierarchyPath(Transform t)
        {
            var path = t.name;
            var current = t.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private static bool IsBlockRaycastRecursive(CanvasGroup canvasGroup)
        {
            // IgnoreParentGroupsまたは親にCanvasGroupがなければ、自身のblocksRaycastsが確定値
            if (canvasGroup.ignoreParentGroups) return canvasGroup.blocksRaycasts;
            var parentCanvasGroup = canvasGroup.transform.parent != null
                ? canvasGroup.transform.parent.GetComponentInParent<CanvasGroup>()
                : null;
            if (parentCanvasGroup == null) return canvasGroup.blocksRaycasts;

            // 親が優先されるので親の結果を返す
            return IsBlockRaycastRecursive(parentCanvasGroup);
        }
    }
}
