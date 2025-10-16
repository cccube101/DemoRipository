using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CI
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class CustomInspectorEditor : Editor
    {
        // ---------------------------- Field
        private SerializedProperty[] serializedProps;



        // ---------------------------- UnityMessage
        private void OnEnable()
        {
            // SerializedProperty を配列にキャッシュ
            if (target == null || serializedObject == null) return;
            var so = serializedObject;
            var prop = so.GetIterator();
            if (prop == null) return;


            System.Collections.Generic.List<SerializedProperty> list = new();
            bool expanded = true;
            while (prop.NextVisible(expanded))
            {
                expanded = false;
                list.Add(prop.Copy());
            }
            serializedProps = list.ToArray();
        }



        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            serializedObject.Update();

            var type = target.GetType();
            var member = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var m in member)
            {
                // ラインの描画
                DrawLine(m);

                // 変数の描画
                if (m is FieldInfo field)
                {
                    DrawField(field);
                }
                // ボタンの描画
                else if (m is MethodInfo method)
                {
                    DrawBtn(m, method);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }



        // ---------------------------- PrivateMethod
        /// <summary>
        /// 線を描画
        /// </summary>
        /// <param name="member">取得したメンバー</param>
        private void DrawLine(MemberInfo member)
        {
            if (GetAttribute(member, out HorizontalLineAttribute line))
            {
                Rect rect = EditorGUILayout.GetControlRect(false, 20f);
                float centerY = rect.y + rect.height / 2;

                var style = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
                var labelSize = style.CalcSize(new GUIContent(line._label));

                // ラベル
                Rect labelRect = new(
                    rect.x + (rect.width - labelSize.x) / 2,
                    rect.y,
                    labelSize.x,
                    rect.height
                );
                EditorGUI.LabelField(labelRect, line._label, style);

                // 色取得
                var color = GetColor();
                Color GetColor()
                {
                    if (ColorUtility.TryParseHtmlString(line._colorCord, out Color c))
                        return c;

                    return Color.cyan;
                }

                // 左線
                Rect leftLine = new(
                    rect.x,
                    centerY - line._lineHeight / 2,
                    labelRect.xMin - rect.x - line._spacing,
                    line._lineHeight
                );
                EditorGUI.DrawRect(leftLine, color);

                // 右線
                Rect rightLine = new(
                    labelRect.xMax + line._spacing,
                    centerY - line._lineHeight / 2,
                    rect.xMax - labelRect.xMax - line._spacing,
                    line._lineHeight
                );
                EditorGUI.DrawRect(rightLine, color);

            }
        }


        /// <summary>
        /// 変数描画
        /// </summary>
        /// <param name="field">取得した変数</param>
        private void DrawField(FieldInfo field)
        {
            // 変数のプロパティを取得
            var sp = serializedObject.FindProperty(field.Name);
            if (sp != null)
            {
                // 描画状態を保存
                bool wasEnabled = GUI.enabled;
                // 属性持ちか判定
                var fieldAttributes = field.GetCustomAttributes(true);
                // 属性にDisableがあった場合描画状態を変更
                if (Array.Exists(fieldAttributes, x => x is DisableAttribute))
                    GUI.enabled = false;

                // 描画
                EditorGUILayout.PropertyField(sp, true);
                // 描画状態を更新
                GUI.enabled = wasEnabled;
            }
        }

        /// <summary>
        /// ボタン描画
        /// </summary>
        /// <param name="m">取得したメンバー</param>
        /// <param name="method">取得した関数</param>
        private void DrawBtn(MemberInfo m, MethodInfo method)
        {
            if (GetAttribute(m, out ButtonAttribute btn))
            {
                btn.LayOutAndInvoke(method, target);
            }
        }

        /// <summary>
        /// 種ごとのアトリビュートを取得
        /// </summary>
        /// <typeparam name="T">任意のアトリビュート</typeparam>
        /// <param name="member">メンバーインフォ</param>
        /// <param name="att">取得アトリビュート</param>
        /// <returns>アトリビュート取得可否</returns>
        private bool GetAttribute<T>(MemberInfo member, out T att) where T : Attribute
        {
            // アトリビュートの取得
            if (member.GetCustomAttribute<T>() != null)
            {
                att = member.GetCustomAttribute<T>();
                return true;
            }
            else
            {
                att = null;
                return false;
            }
        }
    }
}