using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// プロジェクトウィンドウ上でボタンを押すと、ファイルをエクスプローラーで開く
    /// </summary>
    public class FileOpener
    {
        const int ICON_SIZE = 12; // アイコンサイズ

        [InitializeOnLoadMethod]
        static void Init()
        {
            // 重複登録防止
            EditorApplication.projectWindowItemOnGUI -= OnGUI;
            EditorApplication.projectWindowItemOnGUI += OnGUI;
        }

        static void OnGUI(string guid, Rect selectionRect)
        {
            // ボタン位置を右端・縦中央に配置
            var pos = selectionRect;
            pos.width = ICON_SIZE + 4;
            pos.height = ICON_SIZE;
            pos.x = selectionRect.xMax - ICON_SIZE - 2;
            pos.y += (selectionRect.height - ICON_SIZE) / 2f;

            // 白フォルダアイコン取得
            Texture icon = EditorGUIUtility.IconContent("d_FolderOpened Icon").image;

            // 透明ボタンでクリックを検知
            if (GUI.Button(pos, GUIContent.none))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid).Replace("/", "\\");
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            }

            // アイコンをボタンサイズに合わせて描画
            GUI.DrawTexture(pos, icon, ScaleMode.ScaleToFit);
        }
    }
}
