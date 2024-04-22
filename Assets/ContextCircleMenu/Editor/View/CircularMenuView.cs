using System;
using System.Collections.Generic;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class CircularMenuView
    {
        public readonly List<CircularMenuView> Children = new();
        public readonly GUIContent Icon;
        public readonly Action OnRadialMenuItemSelected;
        public readonly CircularMenuView Parent;
        public readonly string Path;

        public CircularMenuView(string path, GUIContent icon, Action onRadialMenuItemSelected,
            CircularMenuView parent)
        {
            Path = path;
            Icon = icon;
            OnRadialMenuItemSelected = onRadialMenuItemSelected;
            Parent = parent;
        }
    }
}