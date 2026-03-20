using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace TohokuEpco.Editor
{
    /// <summary>
    /// Hierarchy の PlayableDirector 付き GameObject を右クリックしたとき、
    /// PlayableAsset (.playable) も一緒に複製してバインドし直すメニューを提供する。
    /// </summary>
    public static class DuplicateWithTimelineMenu
    {
        private const string MenuPath = "GameObject/Timeline 付きで複製";

        // ─── 実行 ────────────────────────────────────────────────────

        [MenuItem(MenuPath, false, 11)] // "Duplicate" の直後 (priority=10) に並べる
        private static void Execute()
        {
            // 選択中の GameObject を順番に処理（親→子の順になるよう降順ソートはしない。独立した選択を想定）
            foreach (var original in Selection.gameObjects)
            {
                DuplicateGameObjectWithTimeline(original);
            }
        }

        // ─── バリデーション ──────────────────────────────────────────

        [MenuItem(MenuPath, true)]
        private static bool Validate()
        {
            // PlayableDirector を持つ GameObject が 1 つでも選択されていれば有効
            return Selection.gameObjects.Any(go => go.GetComponent<PlayableDirector>() != null);
        }

        // ─── 複製処理 ────────────────────────────────────────────────

        private static void DuplicateGameObjectWithTimeline(GameObject original)
        {
            var director = original.GetComponent<PlayableDirector>();

            // PlayableDirector がない、または PlayableAsset が未設定の場合は通常複製
            if (director == null || director.playableAsset == null)
            {
                var plain = Object.Instantiate(original, original.transform.parent);
                plain.name = original.name;
                Undo.RegisterCreatedObjectUndo(plain, "Duplicate GameObject");
                GameObjectUtility.EnsureUniqueNameForSibling(plain);
                return;
            }

            // ── 1. PlayableAsset をコピー ────────────────────────────
            var originalAssetPath = AssetDatabase.GetAssetPath(director.playableAsset);
            var directory = Path.GetDirectoryName(originalAssetPath)!.Replace('\\', '/');
            var newAssetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{directory}/{Path.GetFileName(originalAssetPath)}");

            if (!AssetDatabase.CopyAsset(originalAssetPath, newAssetPath))
            {
                Debug.LogError($"[DuplicateWithTimeline] PlayableAsset のコピーに失敗しました: {originalAssetPath}");
                return;
            }

            AssetDatabase.ImportAsset(newAssetPath);
            var newAsset = AssetDatabase.LoadAssetAtPath<PlayableAsset>(newAssetPath);
            if (newAsset == null)
            {
                Debug.LogError($"[DuplicateWithTimeline] コピーした PlayableAsset の読み込みに失敗しました: {newAssetPath}");
                return;
            }

            // ── 2. GameObject を複製 ────────────────────────────────
            var newGo = Object.Instantiate(original, original.transform.parent);
            newGo.name = original.name;
            Undo.RegisterCreatedObjectUndo(newGo, "Duplicate With Timeline");
            GameObjectUtility.EnsureUniqueNameForSibling(newGo);

            // ── 3. 新 PlayableAsset を割り当て ──────────────────────
            var newDirector = newGo.GetComponent<PlayableDirector>();
            Undo.RecordObject(newDirector, "Set PlayableAsset");
            newDirector.playableAsset = newAsset;

            // ── 4. Binding を引き継ぐ ───────────────────────────────
            // 元・複製それぞれのOutputsを順番で対応させる。
            // CopyAsset 後のトラック構造は元と同一順序になるため、インデックスで照合する。
            var originalOutputs = director.playableAsset.outputs.ToList();
            var newOutputs = newAsset.outputs.ToList();

            for (var i = 0; i < originalOutputs.Count && i < newOutputs.Count; i++)
            {
                var bound = director.GetGenericBinding(originalOutputs[i].sourceObject);
                if (bound != null)
                {
                    newDirector.SetGenericBinding(newOutputs[i].sourceObject, bound);
                }
            }

            EditorUtility.SetDirty(newGo);
            AssetDatabase.SaveAssets();

            Debug.Log($"[DuplicateWithTimeline] 複製完了: {newGo.name}  →  {newAssetPath}");
        }
    }
}
