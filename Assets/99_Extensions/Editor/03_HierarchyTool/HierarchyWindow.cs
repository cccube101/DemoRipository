using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// ヒエラルキー対象オブジェクトのカスタムプロパティ編集ウィンドウ
    /// </summary>
    public class HierarchyWindow : EditorWindow
    {
        private GameObject target;      // 編集対象の GameObject
        private OverlayData _data;      // 対応するオーバーレイデータ
        private string _cachedId;       // GlobalObjectId をキャッシュ（高コスト計算を避ける）

        /// <summary>
        /// ウィンドウを開く（対象 GameObject をセット）
        /// </summary>
        /// <param name="go">対象 GameObject</param>
        /// <param name="hierarchyRect">ヒエラルキー内のオブジェクト矩形</param>
        public static void ShowWindow(GameObject go, Rect hierarchyRect)
        {
            if (go == null) return;

            // 新しいウィンドウインスタンス作成
            var window = CreateInstance<HierarchyWindow>();
            window.target = go;

            // オブジェクトの GlobalObjectId をキャッシュ
            // これにより OnGUI や OnDisable で何度も計算する必要がなくなる
            window._cachedId = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

            // 既存ラベルデータがあれば取得、なければデフォルトを作成
            if (HierarchyOverlay.labelData.TryGetValue(window._cachedId, out var data))
            {
                window._data = data;
            }
            else
            {
                // デフォルト値は Normal（表示なし）に設定
                window._data = new OverlayData { type = Type.Normal };
            }

            // ヒエラルキー座標をスクリーン座標に変換して表示位置を決定
            var screenPos = GUIUtility.GUIToScreenPoint(new Vector2(hierarchyRect.x, hierarchyRect.y - 134));
            var size = new Vector2(300, 150); // ドロップダウンサイズ
            var pos = new Rect(screenPos, size);

            // ドロップダウンウィンドウとして表示
            window.ShowAsDropDown(pos, size);
        }

        /// <summary>
        /// ウィンドウ GUI 描画
        /// </summary>
        private void OnGUI()
        {
            GUILayout.BeginVertical("box");

            // 対象オブジェクトが null の場合は GUI を描画せず警告表示
            if (target == null)
            {
                EditorGUILayout.HelpBox("No Target", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            // オブジェクト名表示（太字）
            EditorGUIEx.DrawSubTitle($"Target [ {target.name} ]");
            EditorGUILayout.Space();
            EditorGUIEx.DrawSeparator();

            // =========================
            // タグ切替
            // =========================

            // タグフィールドを表示し、新しいタグを取得
            string newTag = EditorGUILayout.TagField("Tag", target.tag);

            // 現在のタグと異なる場合は変更を適用
            if (!target.CompareTag(newTag))
            {
                // Undo に登録（タグ変更を取り消せるようにする）
                Undo.RecordObject(target, "Change Tag");

                // タグを更新
                target.tag = newTag;

                // Unity にオブジェクト変更を通知
                EditorUtility.SetDirty(target);
            }

            // ボタンで強制的に EditorOnly タグを設定
            if (GUILayout.Button("Set EditorOnly"))
            {
                Undo.RecordObject(target, "Set EditorOnly Tag");
                target.tag = "EditorOnly";
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUIEx.DrawSeparator();

            // =========================
            // ラベルタイプ変更（Header / Separator / Normal）
            // =========================
            EditorGUI.BeginChangeCheck();

            // ドロップダウンで Overlay の種類を変更
            _data.type = (Type)EditorGUILayout.EnumPopup("Draw Type", _data.type);

            // 種類ごとにカラーやライン高さなどのプロパティを表示
            switch (_data.type)
            {
                case Type.Header:
                    _data.headerBarColor = EditorGUILayout.ColorField("Bar Color", _data.headerBarColor);
                    _data.headerTextColor = EditorGUILayout.ColorField("Text Color", _data.headerTextColor);
                    break;

                case Type.Separator:
                    _data.separatorLineColor = EditorGUILayout.ColorField("Line Color", _data.separatorLineColor);
                    _data.separatorLineHeight = EditorGUILayout.FloatField("Line Height", _data.separatorLineHeight);
                    break;

                case Type.Normal:
                    EditorGUILayout.Space(35f);
                    break;

                default:
                    break;
            }

            // 変更があった場合はデータを反映
            if (EditorGUI.EndChangeCheck())
            {
                // 高コスト計算を避けるためキャッシュ済み ID を使用
                Undo.RecordObject(HierarchyOverlay.asset, "Update LabelData");

                // Overlay データを更新
                HierarchyOverlay.labelData[_cachedId] = _data;

                // 保存
                HierarchyOverlay.SaveData();
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// ウィンドウが閉じられたときに確実にデータを保存
        /// </summary>
        private void OnDisable()
        {
            if (target == null) return;

            // キャッシュ済み ID を利用して Overlay データを保存
            HierarchyOverlay.labelData[_cachedId] = _data;

            // ScriptableObject に反映して保存
            HierarchyOverlay.SaveData();
        }
    }
}
