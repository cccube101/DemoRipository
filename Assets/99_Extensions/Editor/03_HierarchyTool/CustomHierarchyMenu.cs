using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// ヒエラルキーの右クリックメニュー拡張を行うクラス
    /// </summary>
    public class CustomHierarchyMenu
    {
        /// <summary>
        /// ヒエラルキーから「Header」を作成するメニュー
        /// </summary>
        [MenuItem("GameObject/CI/Create Header", false, 10)]
        private static void CreateHeader(MenuCommand menuCommand)
        {
            // 共通処理で Header を作成
            Create(menuCommand, Type.Header);
        }

        /// <summary>
        /// ヒエラルキーから「Separator」を作成するメニュー
        /// </summary>
        [MenuItem("GameObject/CI/Create Separator", false, 10)]
        private static void CreateSeparator(MenuCommand menuCommand)
        {
            // 共通処理で Separator を作成
            Create(menuCommand, Type.Separator);
        }



        /// <summary>
        /// 指定された種類のオブジェクトをヒエラルキーに作成し、Overlay に登録する
        /// </summary>
        /// <param name="menuCommand">メニューコマンド</param>
        /// <param name="type">描画タイプ</param>
        private static void Create(MenuCommand menuCommand, Type type)
        {
            // オブジェクト名を種類に応じて取得
            var name = GetTypeName(type);

            // 新規オブジェクトを作成
            GameObject obj = new(name);

            // 「EditorOnly」タグが存在する場合のみ付与
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains("EditorOnly"))
            {
                obj.tag = "EditorOnly";
            }

            // 親オブジェクトが指定されていればアラインして配置
            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

            // Undo に登録（作成の取り消しが可能）
            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");

            // Undo に親子関係も登録（親が指定されている場合）
            if (menuCommand.context is GameObject parent)
            {
                Undo.SetTransformParent(obj.transform, parent.transform, "Set Parent");
            }

            if (type == Type.Header)
            {
                Selection.activeObject = obj;
            }

            // オーバーレイデータを登録
            string id = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();

            // すでに同じオブジェクトが登録されていなければ追加
            if (!HierarchyOverlay.labelData.ContainsKey(id))
            {
                HierarchyOverlay.labelData[id] = new OverlayData { type = type };
            }

            // データを保存（頻繁な作成時はパフォーマンスに注意）
            HierarchyOverlay.SaveData();

            /// <summary>
            /// 種類に応じたオブジェクト名を返す
            /// </summary>
            static string GetTypeName(Type type)
            {
                return type switch
                {
                    Type.Header => "Header",
                    Type.Separator => "Separator",
                    _ => "Unknown",
                };
            }
        }
    }
}
