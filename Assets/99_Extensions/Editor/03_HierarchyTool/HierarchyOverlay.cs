using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// ヒエラルキーにカスタムオーバーレイ（ヘッダー・セパレーターなど）を描画・保存するエディタ拡張
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyOverlay
    {
        private const string SAVE = "ExTools/Save HierarchyData";

        /// <summary>
        /// メニューからデータを保存する処理
        /// </summary>
        [MenuItem(SAVE, priority = 0)]
        public static void SaveData() => Save();

        private const string path = "Assets/99_Extensions/Editor/03_HierarchyTool/HierarchyData.asset";
        public static HierarchyOverlayData asset;
        public static Dictionary<string, OverlayData> labelData = new();

        private static bool isLoaded = false;

        // ヘッダー描画用 GUIStyle（キャッシュして使い回す）
        private static GUIStyle headerStyle;

        /// <summary>
        /// 静的コンストラクタ（エディタ起動時やリコンパイル時に呼ばれる）
        /// </summary>
        static HierarchyOverlay()
        {
            // 初期化処理
            Initialize();

            // シーン保存時に Save を呼ぶようイベント登録
            EditorSceneManager.sceneSaving += (scene, path) => Save();
        }

        /// <summary>
        /// InitializeOnLoadMethod による安全な初期化呼び出し
        /// （静的コンストラクタが呼ばれない場合の保険）
        /// </summary>
        [InitializeOnLoadMethod]
        private static void EnsureInitialize() => Initialize();

        /// <summary>
        /// 初期化処理
        /// データをロードし、必要なイベントを登録する
        /// </summary>
        private static void Initialize()
        {
            // まだロードしていない場合のみロード
            if (!isLoaded)
            {
                Load();
                isLoaded = true;
            }

            // ヒエラルキー描画イベントを再登録
            // （重複登録を防ぐために一度解除してから再登録する）
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

            // Unity 終了時に Save を呼ぶようイベント登録
            // （最後に最新状態を確実に保存するため）
            EditorApplication.quitting -= Save;
            EditorApplication.quitting += Save;
        }

        /// <summary>
        /// データをロード（ScriptableObject を読み込み、辞書に展開）
        /// 保存用の ScriptableObject が存在しない場合は新規作成する
        /// </summary>
        private static void Load()
        {
            if (asset == null)
            {
                // アセットパスから ScriptableObject をロード
                asset = AssetDatabase.LoadAssetAtPath<HierarchyOverlayData>(path);

                // 存在しなければ新規作成して保存
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<HierarchyOverlayData>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                }
            }

            // ScriptableObject 内のデータを Dictionary に展開
            labelData = asset.ToDictionary();
        }

        /// <summary>
        /// データを保存（存在しないオブジェクトを除外し、ScriptableObject に反映）
        /// 無効な参照を整理してから保存する
        /// </summary>
        private static void Save()
        {
            if (asset == null) return;

            // 一時的に有効データを格納する辞書
            var validData = new Dictionary<string, OverlayData>();

            // すべての登録データをチェック
            foreach (var kvp in labelData)
            {
                // ノーマルは除外
                if (kvp.Value.type == Type.Normal) continue;

                // 文字列から GlobalObjectId に変換できるか確認
                if (GlobalObjectId.TryParse(kvp.Key, out var gid))
                {
                    // 実際の Unity オブジェクトに変換
                    // （Destroy されていないかを確認するため）
                    Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                    if (obj != null)
                    {
                        // 存在するオブジェクトのみ残す
                        validData.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            // 不要なデータを除外した状態で置き換え
            labelData = validData;

            // ScriptableObject に反映
            asset.FromDictionary(labelData);

            // アセットを Dirty 状態にして保存
            // （Unity に「変更された」と認識させるため）
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// ヒエラルキーウィンドウの GUI 描画処理
        /// オブジェクトごとに Header・Separator を描画する
        /// </summary>
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            // InstanceID から GameObject を取得
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            // オブジェクトの GlobalObjectId を取得（文字列化してキーに使用）
            string id = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();

            // データが存在しなければ何もしない
            if (!labelData.TryGetValue(id, out var data)) return;

            // オーバーレイ種別ごとに処理
            switch (data.type)
            {
                case Type.Normal:
                    // 通常は何も描画しない
                    break;

                case Type.Header:
                    // 背景矩形を描画（少し幅を広げて強調）
                    Rect colorRect = new(selectionRect.x, selectionRect.y, selectionRect.width + 10, selectionRect.height);
                    EditorGUI.DrawRect(colorRect, data.headerBarColor);

                    // テキストを中央・太字で描画
                    EditorGUI.LabelField(selectionRect, obj.name, GetHeaderStyle(data.headerTextColor));
                    break;

                case Type.Separator:
                    // 背景色を描画（暗めのグレー エディターに合わせる）
                    Rect backRect = new(selectionRect.x, selectionRect.y, selectionRect.width + 10, selectionRect.height);
                    EditorGUI.DrawRect(backRect, new Color(0.21f, 0.21f, 0.21f));

                    // 中央に区切り線を描画
                    Rect lineRect = new(selectionRect.x, selectionRect.y + selectionRect.height / 2,
                        selectionRect.width, data.separatorLineHeight);
                    EditorGUI.DrawRect(lineRect, data.separatorLineColor);
                    break;
            }
        }

        /// <summary>
        /// ヘッダー用の GUIStyle を取得（キャッシュして再利用）
        /// 毎回 new すると GC Alloc が発生するため static で保持
        /// </summary>
        private static GUIStyle GetHeaderStyle(Color textColor)
        {
            if (headerStyle == null)
            {
                // 初回のみ生成
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }

            // 色だけは毎回更新
            headerStyle.normal.textColor = textColor;
            return headerStyle;
        }
    }
}
