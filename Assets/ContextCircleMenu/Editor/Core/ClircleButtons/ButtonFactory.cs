using System;
using UnityEditor;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class ButtonFactory : IButtonFactory
    {
        public CircleButton Create(string path, GUIContent icon, Action onSelected, int section)
        {
            return new SimpleCircleButton(path, icon, section, onSelected);
        }

        public CircleButton CreateBackButton(Action onBack)
        {
            return new SimpleCircleButton("Back", EditorGUIUtility.IconContent(EditorIcons.Back2x), -1, onBack);
        }
    }
}