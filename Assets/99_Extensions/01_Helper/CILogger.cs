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


        // OnGUI�ł̃��O�\���p�p�����[�^
        private static (Rect[], GUIStyle) _logParam = GetLogParam();
        public static (Rect[] pos, GUIStyle style) LogParam => _logParam;

        /// <summary>
        /// ���O�p�����[�^�擾
        /// </summary>
        /// <returns>���O�p�p�����[�^</returns>
        [HideInCallstack]
        private static (Rect[], GUIStyle) GetLogParam()
        {
            //  �p�����[�^����
            var pos = new Rect[30];

            //  �ʒu�ۑ�
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = new Rect(10, 1075 - i * 30, 300, 30);
            }

            //  �o�̓X�^�C���ۑ�
            var style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 25;


            return (pos, style);

        }
    }
}