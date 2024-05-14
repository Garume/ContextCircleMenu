using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public class LeafCircleMenu : CircleMenu
    {
        public LeafCircleMenu(string path, GUIContent icon, Action onSelected, IButtonFactory factory,
            CircleMenu parent = null,
            int radius = 100) : base(path, icon, onSelected, parent, factory, radius)
        {
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateButtons(IButtonFactory factory)
        {
            return null;
        }
    }
}