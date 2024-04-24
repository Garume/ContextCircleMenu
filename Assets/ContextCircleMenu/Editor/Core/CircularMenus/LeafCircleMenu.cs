using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public class LeafCircleMenu : CircleMenu
    {
        public LeafCircleMenu(string path, GUIContent icon, Action onSelected, CircleMenu parent = null,
            int radius = 100) : base(path, icon, onSelected, parent, radius)
        {
        }

        protected override VisualElement[] CreateButtons()
        {
            return null;
        }
    }
}