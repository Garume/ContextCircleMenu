using System;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public sealed class LeafCircleMenu : CircleMenu
    {
        public LeafCircleMenu(string path, GUIContent icon, Action onSelected, IButtonFactory factory,
            CircleMenu parent = null) : base(path, icon, onSelected, parent, factory)
        {
        }

        /// <inheritdoc />
        protected override CircleButton[] CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption)
        {
            return null;
        }
    }
}