using System;
using System.Collections.Generic;
using UnityEngine;

namespace CI
{
    /// <summary>
    /// オーバーレイの種類を表す列挙体
    /// </summary>
    public enum Type
    {
        Normal,
        Header,
        Separator,
    }

    /// <summary>
    /// ヒエラルキーオーバーレイのデータ構造
    /// 各種オーバーレイの種類や色設定を保持する
    /// </summary>
    [Serializable]
    public class OverlayData
    {
        /// <summary>
        /// デフォルトコンストラクタ
        /// 初期状態の色やパラメータを設定する
        /// </summary>
        public OverlayData()
        {
            type = Type.Normal; // 初期値は通常オーバーレイ

            // ヘッダー用の初期色設定
            headerBarColor = Color.green;
            headerTextColor = Color.black;

            // セパレーター用の初期色設定
            separatorLineColor = new Color(0.66f, 0.66f, 0.66f); // グレー
            separatorLineHeight = 1f;
        }

        public Type type;

        public Color headerBarColor;
        public Color headerTextColor;

        public Color separatorLineColor;
        public float separatorLineHeight;
    }

    /// <summary>
    /// ヒエラルキーウィンドウに関連付けられたオーバーレイ情報を保持する ScriptableObject
    /// ID（GameObject の GlobalObjectId 等）と OverlayData を対応付ける
    /// </summary>
    public class HierarchyOverlayData : ScriptableObject
    {
        public List<string> globalIDs = new();
        public List<OverlayData> colorData = new();

        /// <summary>
        /// globalIDs と colorData を Dictionary に変換する便利関数
        /// （ID をキーとして OverlayData にアクセス可能にする）
        /// </summary>
        public Dictionary<string, OverlayData> ToDictionary()
        {
            var dict = new Dictionary<string, OverlayData>();

            // globalIDs と colorData の数が異なる可能性に備え、短い方に合わせて処理する
            for (int i = 0; i < Mathf.Min(globalIDs.Count, colorData.Count); i++)
            {
                dict[globalIDs[i]] = colorData[i];
            }
            return dict;
        }

        /// <summary>
        /// Dictionary データから globalIDs と colorData を再構築する
        /// 既存データはクリアされ、新しい内容で上書きされる
        /// </summary>
        /// <param name="dict">ID をキーとした OverlayData の辞書</param>
        public void FromDictionary(Dictionary<string, OverlayData> dict)
        {
            // 初期化
            globalIDs.Clear();
            colorData.Clear();

            // データの登録
            foreach (var kv in dict)
            {
                globalIDs.Add(kv.Key);
                colorData.Add(kv.Value);
            }
        }
    }
}
