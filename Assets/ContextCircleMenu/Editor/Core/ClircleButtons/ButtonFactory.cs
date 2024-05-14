using System;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class ButtonFactory : IButtonFactory
    {
        public CircleButton Create(string path, GUIContent icon, Action onSelected, int section,
            bool shouldCloseMenuAfterSelection)
        {
            return new SimpleCircleButton(path, icon, section, onSelected, shouldCloseMenuAfterSelection);
        }
    }
}