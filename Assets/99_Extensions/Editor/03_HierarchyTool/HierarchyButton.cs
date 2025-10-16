using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// ヒエラルキーに「+」ボタンをアイコンで表示し、クリックで HierarchyWindow を開くクラス
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyButton
    {
        private const int ICON_SIZE = 15; // アイコンサイズ

        static HierarchyButton()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GameObject obj)) return;

            // ボタン描画位置
            Rect buttonRect = new Rect(
                selectionRect.xMax - ICON_SIZE + 3,   // 右端に寄せる
                selectionRect.y + (selectionRect.height - ICON_SIZE) / 2f, // 縦中央
                ICON_SIZE,
                ICON_SIZE
            );

            // 表示するアイコンを取得
            Texture icon = EditorGUIUtility.IconContent("CustomTool@2x").image;

            // 透明ボタンでクリックを検知
            if (GUI.Button(buttonRect, GUIContent.none))
            {
                HierarchyWindow.ShowWindow(obj, selectionRect);
            }

            // アイコンをボタンサイズに合わせて描画
            GUI.DrawTexture(buttonRect, icon, ScaleMode.ScaleToFit);
        }
    }
}
