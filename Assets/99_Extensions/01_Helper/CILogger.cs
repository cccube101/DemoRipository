using UnityEngine;

namespace CI
{
    public static class CILogger
    {
        [HideInCallstack]
        public static void LogN(object message)
        {
            Debug.Log(message);
        }

        [HideInCallstack]
        public static void LogW(object message)
        {
            Debug.LogWarning(message);
        }

        [HideInCallstack]
        public static void LogE(object message)
        {
            Debug.LogError(message);
        }


        // OnGUIでのログ表示用パラメータ
        private static (Rect[], GUIStyle) _logParam = GetLogParam();
        public static (Rect[] pos, GUIStyle style) LogParam => _logParam;

        /// <summary>
        /// ログパラメータ取得
        /// </summary>
        /// <returns>ログ用パラメータ</returns>
        [HideInCallstack]
        private static (Rect[], GUIStyle) GetLogParam()
        {
            //  パラメータ生成
            var pos = new Rect[30];

            //  位置保存
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = new Rect(10, 1075 - i * 30, 300, 30);
            }

            //  出力スタイル保存
            var style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 25;


            return (pos, style);

        }
    }
}