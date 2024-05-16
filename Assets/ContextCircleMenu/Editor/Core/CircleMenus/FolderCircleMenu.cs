using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public class FolderCircleMenu : CircleMenu
    {
        public FolderCircleMenu(string path, IMenuControllable menu,
            GUIContent icon,
            CircleMenu parent,
            IButtonFactory factory,
            int radius = 100) :
            base(path, icon,
                null, parent, factory, radius, false)
        {
            OnSelected = () => menu.Open(this);
        }

        internal FolderCircleMenu(string path, IMenuControllable menu,
            CircleMenu parent,
            IButtonFactory factory,
            int radius = 100) :
            this(path, menu, EditorGUIUtility.IconContent(EditorIcons.FolderIcon), parent, factory, radius)
        {
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption)
        {
            var buttons = new VisualElement[Children.Count];
            for (var index = 0; index < buttons.Length; index++)
            {
                var item = Children[index];
                buttons[index] = factory.Create(item.Path, item.Icon, item.OnSelected,
                    Children.Count - index,
                    item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateUtilityElements(ref ContextCircleMenuOption menuOption)
        {
            var label = new Label(Path)
            {
                style =
                {
                    marginBottom = menuOption.Height * 0.5f + 5.0f,
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