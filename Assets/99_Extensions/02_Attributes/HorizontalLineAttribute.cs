using System;
using UnityEditor;
using UnityEngine;

namespace CI
{
    public class HorizontalLineAttribute : Attribute
    {
        public readonly string _label;
        public readonly string _colorCord;
        public readonly float _lineHeight;
        public readonly float _spacing;

        public HorizontalLineAttribute
            (string label = null
            , string color = ""
            , float lineHeight = 1f
            , float spacing = 4f)
        {
            _label = label;
            _colorCord = color;
            _lineHeight = lineHeight;
            _spacing = spacing;
        }
    }
}