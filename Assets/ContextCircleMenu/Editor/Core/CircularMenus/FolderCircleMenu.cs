using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public class FolderCircleMenu : CircleMenu
    {
        private readonly Action _onBack;

        public FolderCircleMenu(string path, Action onSelected, Action onBack, CircleMenu parent, int radius = 100) :
            base(path,
                EditorGUIUtility.IconContent(EditorIcons.FolderIcon), onSelected, parent, radius, false)
        {
            _onBack = onBack;
        }

        protected override VisualElement[] CreateButtons()
        {
            var buttons = new VisualElement[Children.Count + 1];
            buttons[0] = CircularButton.CreateBackButton(_onBack);
            for (var index = 1; index < buttons.Length; index++)
            {
                var item = Children[index - 1];
                buttons[index] =
                    new CircularButton(
                        item.Children.Count > 0 ? item.Path + "" : item.Path,
                        item.Icon,
                        Children.Count - index + 1,
                        item.OnSelected,
                        item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

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