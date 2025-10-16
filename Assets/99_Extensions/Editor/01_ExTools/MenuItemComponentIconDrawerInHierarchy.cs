#nullable enable
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// Hierarchyウィンドウにコンポーネントアイコンを表示する拡張機能
    /// </summary>
    public static class MenuItemComponentIconDrawerInHierarchy
    {
        private const int IconSize = 16; // アイコンサイズ（px）
        private const string MENU_PATH = "ExTools/ComponentsDrawer"; // メニュー名 & EditorPrefsキー
        private const string ScriptIconName = "cs Script Icon"; // スクリプト用アイコン名

        private static readonly Color colorWhenDisabled = new(1.0f, 1.0f, 1.0f, 0.5f); // 無効コンポーネント用半透明色
        private static Texture? scriptIcon; // スクリプトアイコンのキャッシュ

        /// <summary>
        /// Editor起動時に呼ばれる初期化
        /// Hierarchy描画の有効/無効を更新し、スクリプトアイコンをキャッシュ
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            UpdateEnabled();

#pragma warning disable UNT0023 // Coalescing assignment on Unity objects
            scriptIcon ??= EditorGUIUtility.IconContent(ScriptIconName).image;
#pragma warning restore UNT0023
        }

        /// <summary>
        /// メニューからON/OFF切替してEditorPrefsに保存
        /// </summary>
        [MenuItem(MENU_PATH, priority = 40)]
        private static void MenuToggle()
        {
            EditorPrefs.SetBool(MENU_PATH, !EditorPrefs.GetBool(MENU_PATH, false));
        }

        /// <summary>
        /// メニュー表示時のチェック状態更新
        /// </summary>
        /// <returns>常にtrueを返してメニューを有効化</returns>
        [MenuItem(MENU_PATH, true, priority = 40)]
        private static bool MenuToggleValidate()
        {
            Menu.SetChecked(MENU_PATH, EditorPrefs.GetBool(MENU_PATH, false));
            UpdateEnabled();
            return true;
        }

        /// <summary>
        /// Hierarchy描画が有効か判定
        /// </summary>
        public static bool IsValid()
        {
            return EditorPrefs.GetBool(MENU_PATH, false);
        }

        /// <summary>
        /// Hierarchy描画コールバックの登録/解除
        /// </summary>
        private static void UpdateEnabled()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DisplayIcons;
            if (IsValid())
                EditorApplication.hierarchyWindowItemOnGUI += DisplayIcons;
        }

        /// <summary>
        /// Hierarchyにコンポーネントアイコンを描画
        /// </summary>
        /// <param name="instanceID">Hierarchy上のオブジェクトID</param>
        /// <param name="selectionRect">描画用の矩形</param>
        private static void DisplayIcons(int instanceID, Rect selectionRect)
        {
            // instanceIDからGameObject取得
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GameObject gameObject)) return;

            // 描画位置の初期化（右端から順にIconSizeずつ左へ）
            var pos = selectionRect;
            pos.x = pos.xMax - IconSize * 2;
            pos.width = IconSize;
            pos.height = IconSize;

            // TransformとParticleSystemRenderer以外のコンポーネントを取得
            var components = gameObject
                .GetComponents<Component>()
                .Where(x => !(x is Transform || x is ParticleSystemRenderer))
                .Reverse() // 描画順調整（右端から順に）
                .ToList();

            bool existsScriptIcon = false;

            foreach (var component in components)
            {
                Texture image = AssetPreview.GetMiniThumbnail(component);
                if (image == null) continue;

                // スクリプトアイコンは1つのみ描画
                if (image == scriptIcon)
                {
                    if (existsScriptIcon) continue;
                    existsScriptIcon = true;
                }

                // アイコン描画（有効なら白、無効なら半透明）
                DrawIcon(ref pos, image, component.IsEnabled() ? Color.white : colorWhenDisabled);
            }
        }

        /// <summary>
        /// アイコンを描画し、描画位置を左にずらす
        /// </summary>
        /// <param name="pos">描画位置</param>
        /// <param name="image">描画するアイコン画像</param>
        /// <param name="color">描画カラー（省略時は白）</param>
        private static void DrawIcon(ref Rect pos, Texture image, Color? color = null)
        {
            Color? defaultColor = null;
            if (color.HasValue)
            {
                defaultColor = GUI.color;
                GUI.color = color.Value;
            }

            GUI.DrawTexture(pos, image, ScaleMode.ScaleToFit);
            pos.x -= pos.width; // 次のアイコンは左にずらす

            if (defaultColor.HasValue)
                GUI.color = defaultColor.Value; // 元の色に戻す
        }

        /// <summary>
        /// コンポーネントが有効かどうかを確認する拡張メソッド
        /// </summary>
        /// <param name="this">拡張対象のコンポーネント</param>
        /// <returns>有効であればtrue、無効ならfalse</returns>
        private static bool IsEnabled(this Component @this)
        {
            // enabledプロパティをリフレクションで取得
            var property = @this.GetType().GetProperty("enabled", typeof(bool));
            return (bool)(property?.GetValue(@this, null) ?? true);
        }
    }
}
