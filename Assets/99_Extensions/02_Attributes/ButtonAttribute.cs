using System;
using System.Reflection;
using UnityEngine;

namespace CI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        private readonly string Label;

        public ButtonAttribute(string label = null)
        {
            Label = label;
        }

        // �`��ƃ��\�b�h�̎w��
        public void LayOutAndInvoke(MethodInfo method, UnityEngine.Object obj)
        {
            if (GUILayout.Button(Label))
            {
                method.Invoke(obj, null);
            }
        }
    }
}