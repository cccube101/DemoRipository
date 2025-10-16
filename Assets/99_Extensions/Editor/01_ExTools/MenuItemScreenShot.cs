using UnityEditor;
using UnityEngine;
using System.IO;

namespace CI
{
    /// <summary>
    /// メニューからスクリーンショットを撮影するエディタ拡張
    /// ショートカット：Shift + F12
    /// </summary>
    public sealed class MenuItemScreenShot : Editor
    {
        /// <summary>
        /// メニューに表示するパスとショートカット
        /// # → Shift
        /// % → Ctrl (Windows) / Cmd (Mac)
        /// </summary>
        const string MENU_PATH = "ExTools/Screen Shot #%F12";

        /// <summary>
        /// メニューからスクリーンショットを撮影する
        /// </summary>
        [MenuItem(MENU_PATH, priority = 60)]
        static void CaptureScreenShot()
        {
            // 保存先フォルダ名
            string folder = "Assets/98_ScreenShot";

            // フォルダが存在しなければ作成
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // ファイル名を日時で生成（例：ScreenShot/20250917-114530.png）
            string filename = Path.Combine(folder, $"{System.DateTime.Now:yyyyMMdd-HHmmss}.png");

            // スクリーンショットを撮影して保存
            ScreenCapture.CaptureScreenshot(filename);

            // GameView を取得して更新（表示を最新状態に）
            var assembly = typeof(EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(type);
            gameView.Repaint();

            // 保存先のフルパスをコンソールに表示
            Debug.Log($"ScreenShot captured to {Path.GetFullPath(filename)}");
        }
    }
}
