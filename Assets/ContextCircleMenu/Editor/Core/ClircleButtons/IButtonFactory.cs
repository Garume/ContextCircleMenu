using System;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public interface IButtonFactory
    {
        public CircleButton Create(string path, GUIContent icon, Action onSelected, int section,
            bool shouldCloseMenuAfterSelection);
    }
}