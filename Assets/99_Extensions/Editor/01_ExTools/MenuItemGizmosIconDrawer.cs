using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// Gizmos�A�C�R�����G�f�B�^��ɕ\������g��
    /// ���j���[����ON/OFF�ؑ։\
    /// </summary>
    [InitializeOnLoad]
    public class MenuItemGizmosIconDrawer
    {
        // �������i����͓��ɏ����Ȃ��j
        static MenuItemGizmosIconDrawer() { }

        /// <summary>
        /// ���j���[����EditorPrefs�L�[
        /// </summary>
        private const string MENU_PATH = "ExTools/GizmosIconDrawer";

        /// <summary>
        /// ���j���[����ON/OFF��؂�ւ���EditorPrefs�ɕۑ�
        /// </summary>
        [MenuItem(MENU_PATH, priority = 41)]
        private static void MenuToggle()
        {
            bool current = EditorPrefs.GetBool(MENU_PATH, false);
            EditorPrefs.SetBool(MENU_PATH, !current);
        }

        /// <summary>
        /// ���j���[�̃`�F�b�N��Ԃ��X�V����iON/OFF�\���j
        /// </summary>
        /// <returns>��� true��Ԃ��ă��j���[��L����</returns>
        [MenuItem(MENU_PATH, true, priority = 41)]
        private static bool MenuToggleValidate()
        {
            Menu.SetChecked(MENU_PATH, EditorPrefs.GetBool(MENU_PATH, false));
            return true;
        }

        /// <summary>
        /// Gizmos�`����s�����ǂ�������
        /// </summary>
        /// <returns>ON�Ȃ�true</returns>
        private static bool IsValid()
        {
            return EditorPrefs.GetBool(MENU_PATH, false);
        }

        /// <summary>
        /// �I�𒆁^��I�𒆂�Transform�ɃA�C�R����`��
        /// </summary>
        /// <param name="scr">�Ώ�Transform</param>
        /// <param name="gizmoType">Gizmo�`��^�C�v</param>
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmo(Transform scr, GizmoType gizmoType)
        {
            if (IsValid())
            {
                // Assets/Gizmos/ �܂��̓v���W�F�N�g���[�g�ɂ���uTransformIcon.png�v��\��
                Gizmos.DrawIcon(scr.position, "TransformIcon.png", true);
            }
        }
    }
}
