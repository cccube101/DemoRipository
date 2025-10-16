using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// Gizmosアイコンをエディタ上に表示する拡張
    /// メニューからON/OFF切替可能
    /// </summary>
    [InitializeOnLoad]
    public class MenuItemGizmosIconDrawer
    {
        // 初期化（今回は特に処理なし）
        static MenuItemGizmosIconDrawer() { }

        /// <summary>
        /// メニュー名とEditorPrefsキー
        /// </summary>
        private const string MENU_PATH = "ExTools/GizmosIconDrawer";

        /// <summary>
        /// メニューからON/OFFを切り替えてEditorPrefsに保存
        /// </summary>
        [MenuItem(MENU_PATH, priority = 41)]
        private static void MenuToggle()
        {
            bool current = EditorPrefs.GetBool(MENU_PATH, false);
            EditorPrefs.SetBool(MENU_PATH, !current);
        }

        /// <summary>
        /// メニューのチェック状態を更新する（ON/OFF表示）
        /// </summary>
        /// <returns>常に trueを返してメニューを有効化</returns>
        [MenuItem(MENU_PATH, true, priority = 41)]
        private static bool MenuToggleValidate()
        {
            Menu.SetChecked(MENU_PATH, EditorPrefs.GetBool(MENU_PATH, false));
            return true;
        }

        /// <summary>
        /// Gizmos描画を行うかどうか判定
        /// </summary>
        /// <returns>ONならtrue</returns>
        private static bool IsValid()
        {
            return EditorPrefs.GetBool(MENU_PATH, false);
        }

        /// <summary>
        /// 選択中／非選択中のTransformにアイコンを描画
        /// </summary>
        /// <param name="scr">対象Transform</param>
        /// <param name="gizmoType">Gizmo描画タイプ</param>
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmo(Transform scr, GizmoType gizmoType)
        {
            if (IsValid())
            {
                // Assets/Gizmos/ またはプロジェクトルートにある「TransformIcon.png」を表示
                Gizmos.DrawIcon(scr.position, "TransformIcon.png", true);
            }
        }
    }
}
