using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public class FolderCircleMenu : CircleMenu
    {
        private readonly Action _onBack;

        public FolderCircleMenu(string path, Action<CircleMenu> onOpen, Action onBack, CircleMenu parent,
            IButtonFactory factory,
            int radius = 100) :
            base(path, EditorGUIUtility.IconContent(EditorIcons.FolderIcon),
                null, parent, factory, radius, false)
        {
            OnSelected = () => onOpen(this);
            _onBack = onBack;
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateButtons(IButtonFactory factory)
        {
            var buttons = new VisualElement[Children.Count + 1];
            buttons[0] = factory.Create("Back", EditorGUIUtility.IconContent(EditorIcons.Back2x), _onBack, -1, false);
            for (var index = 1; index < buttons.Length; index++)
            {
                var item = Children[index - 1];
                buttons[index] =
                    factory.Create(
                        item.Children.Count > 0 ? item.Path + "" : item.Path,
                        item.Icon,
                        item.OnSelected,
                        Children.Count - index + 1,
                        item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateUtilityElements()
        {
            var label = new Label(Path)
            {
                style =
                {
                    marginBottom = 100f * 0.5f + 5.0f,
                    fontSize = 10,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = Color.white,
                    textShadow = new TextShadow
                    {
                        offset = new Vector2(0.2f, 0.2f),
                        blurRadius = 0,
                        color = Color.black
                    }
                }
            };
            return new VisualElement[] { label };
        }
    }
}