#nullable enable
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// Hierarchy�E�B���h�E�ɃR���|�[�l���g�A�C�R����\������g���@�\
    /// </summary>
    public static class MenuItemComponentIconDrawerInHierarchy
    {
        private const int IconSize = 16; // �A�C�R���T�C�Y�ipx�j
        private const string MENU_PATH = "ExTools/ComponentsDrawer"; // ���j���[�� & EditorPrefs�L�[
        private const string ScriptIconName = "cs Script Icon"; // �X�N���v�g�p�A�C�R����

        private static readonly Color colorWhenDisabled = new(1.0f, 1.0f, 1.0f, 0.5f); // �����R���|�[�l���g�p�������F
        private static Texture? scriptIcon; // �X�N���v�g�A�C�R���̃L���b�V��

        /// <summary>
        /// Editor�N�����ɌĂ΂�鏉����
        /// Hierarchy�`��̗L��/�������X�V���A�X�N���v�g�A�C�R�����L���b�V��
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
        /// ���j���[����ON/OFF�ؑւ���EditorPrefs�ɕۑ�
        /// </summary>
        [MenuItem(MENU_PATH, priority = 40)]
        private static void MenuToggle()
        {
            EditorPrefs.SetBool(MENU_PATH, !EditorPrefs.GetBool(MENU_PATH, false));
        }

        /// <summary>
        /// ���j���[�\�����̃`�F�b�N��ԍX�V
        /// </summary>
        /// <returns>���true��Ԃ��ă��j���[��L����</returns>
        [MenuItem(MENU_PATH, true, priority = 40)]
        private static bool MenuToggleValidate()
        {
            Menu.SetChecked(MENU_PATH, EditorPrefs.GetBool(MENU_PATH, false));
            UpdateEnabled();
            return true;
        }

        /// <summary>
        /// Hierarchy�`�悪�L��������
        /// </summary>
        public static bool IsValid()
        {
            return EditorPrefs.GetBool(MENU_PATH, false);
        }

        /// <summary>
        /// Hierarchy�`��R�[���o�b�N�̓o�^/����
        /// </summary>
        private static void UpdateEnabled()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DisplayIcons;
            if (IsValid())
                EditorApplication.hierarchyWindowItemOnGUI += DisplayIcons;
        }

        /// <summary>
        /// Hierarchy�ɃR���|�[�l���g�A�C�R����`��
        /// </summary>
        /// <param name="instanceID">Hierarchy��̃I�u�W�F�N�gID</param>
        /// <param name="selectionRect">�`��p�̋�`</param>
        private static void DisplayIcons(int instanceID, Rect selectionRect)
        {
            // instanceID����GameObject�擾
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GameObject gameObject)) return;

            // �`��ʒu�̏������i�E�[���珇��IconSize�����ցj
            var pos = selectionRect;
            pos.x = pos.xMax - IconSize * 2;
            pos.width = IconSize;
            pos.height = IconSize;

            // Transform��ParticleSystemRenderer�ȊO�̃R���|�[�l���g���擾
            var components = gameObject
                .GetComponents<Component>()
                .Where(x => !(x is Transform || x is ParticleSystemRenderer))
                .Reverse() // �`�揇�����i�E�[���珇�Ɂj
                .ToList();

            bool existsScriptIcon = false;

            foreach (var component in components)
            {
                Texture image = AssetPreview.GetMiniThumbnail(component);
                if (image == null) continue;

                // �X�N���v�g�A�C�R����1�̂ݕ`��
                if (image == scriptIcon)
                {
                    if (existsScriptIcon) continue;
                    existsScriptIcon = true;
                }

                // �A�C�R���`��i�L���Ȃ甒�A�����Ȃ甼�����j
                DrawIcon(ref pos, image, component.IsEnabled() ? Color.white : colorWhenDisabled);
            }
        }

        /// <summary>
        /// �A�C�R����`�悵�A�`��ʒu�����ɂ��炷
        /// </summary>
        /// <param name="pos">�`��ʒu</param>
        /// <param name="image">�`�悷��A�C�R���摜</param>
        /// <param name="color">�`��J���[�i�ȗ����͔��j</param>
        private static void DrawIcon(ref Rect pos, Texture image, Color? color = null)
        {
            Color? defaultColor = null;
            if (color.HasValue)
            {
                defaultColor = GUI.color;
                GUI.color = color.Value;
            }

            GUI.DrawTexture(pos, image, ScaleMode.ScaleToFit);
            pos.x -= pos.width; // ���̃A�C�R���͍��ɂ��炷

            if (defaultColor.HasValue)
                GUI.color = defaultColor.Value; // ���̐F�ɖ߂�
        }

        /// <summary>
        /// �R���|�[�l���g���L�����ǂ������m�F����g�����\�b�h
        /// </summary>
        /// <param name="this">�g���Ώۂ̃R���|�[�l���g</param>
        /// <returns>�L���ł����true�A�����Ȃ�false</returns>
        private static bool IsEnabled(this Component @this)
        {
            // enabled�v���p�e�B�����t���N�V�����Ŏ擾
            var property = @this.GetType().GetProperty("enabled", typeof(bool));
            return (bool)(property?.GetValue(@this, null) ?? true);
        }
    }
}
