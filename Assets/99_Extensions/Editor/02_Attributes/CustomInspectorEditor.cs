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
            // SerializedProperty ��z��ɃL���b�V��
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
                // ���C���̕`��
                DrawLine(m);

                // �ϐ��̕`��
                if (m is FieldInfo field)
                {
                    DrawField(field);
                }
                // �{�^���̕`��
                else if (m is MethodInfo method)
                {
                    DrawBtn(m, method);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }



        // ---------------------------- PrivateMethod
        /// <summary>
        /// ����`��
        /// </summary>
        /// <param name="member">�擾���������o�[</param>
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

                // ���x��
                Rect labelRect = new(
                    rect.x + (rect.width - labelSize.x) / 2,
                    rect.y,
                    labelSize.x,
                    rect.height
                );
                EditorGUI.LabelField(labelRect, line._label, style);

                // �F�擾
                var color = GetColor();
                Color GetColor()
                {
                    if (ColorUtility.TryParseHtmlString(line._colorCord, out Color c))
                        return c;

                    return Color.cyan;
                }

                // ����
                Rect leftLine = new(
                    rect.x,
                    centerY - line._lineHeight / 2,
                    labelRect.xMin - rect.x - line._spacing,
                    line._lineHeight
                );
                EditorGUI.DrawRect(leftLine, color);

                // �E��
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
        /// �ϐ��`��
        /// </summary>
        /// <param name="field">�擾�����ϐ�</param>
        private void DrawField(FieldInfo field)
        {
            // �ϐ��̃v���p�e�B���擾
            var sp = serializedObject.FindProperty(field.Name);
            if (sp != null)
            {
                // �`���Ԃ�ۑ�
                bool wasEnabled = GUI.enabled;
                // ��������������
                var fieldAttributes = field.GetCustomAttributes(true);
                // ������Disable���������ꍇ�`���Ԃ�ύX
                if (Array.Exists(fieldAttributes, x => x is DisableAttribute))
                    GUI.enabled = false;

                // �`��
                EditorGUILayout.PropertyField(sp, true);
                // �`���Ԃ��X�V
                GUI.enabled = wasEnabled;
            }
        }

        /// <summary>
        /// �{�^���`��
        /// </summary>
        /// <param name="m">�擾���������o�[</param>
        /// <param name="method">�擾�����֐�</param>
        private void DrawBtn(MemberInfo m, MethodInfo method)
        {
            if (GetAttribute(m, out ButtonAttribute btn))
            {
                btn.LayOutAndInvoke(method, target);
            }
        }

        /// <summary>
        /// �킲�Ƃ̃A�g���r���[�g���擾
        /// </summary>
        /// <typeparam name="T">�C�ӂ̃A�g���r���[�g</typeparam>
        /// <param name="member">�����o�[�C���t�H</param>
        /// <param name="att">�擾�A�g���r���[�g</param>
        /// <returns>�A�g���r���[�g�擾��</returns>
        private bool GetAttribute<T>(MemberInfo member, out T att) where T : Attribute
        {
            // �A�g���r���[�g�̎擾
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