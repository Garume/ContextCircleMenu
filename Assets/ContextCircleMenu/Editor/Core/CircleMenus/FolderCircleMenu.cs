using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public class FolderCircleMenu : CircleMenu
    {
        private Vector3[] _buttonPositions;

        public FolderCircleMenu(string path, IMenuControllable menu,
            GUIContent icon,
            CircleMenu parent,
            IButtonFactory factory) :
            base(path, icon, null, parent, factory, false)
        {
            OnSelected = () => menu.Open(this);
        }

        internal FolderCircleMenu(string path, IMenuControllable menu,
            CircleMenu parent,
            IButtonFactory factory) :
            this(path, menu, EditorGUIUtility.IconContent(EditorIcons.FolderIcon), parent, factory)
        {
        }

        /// <inheritdoc />
        protected override CircleButton[] CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption)
        {
            var buttons = new CircleButton[Children.Count];
            for (var index = 0; index < buttons.Length; index++)
            {
                var item = Children[index];
                var button = factory.Create(
                    item.Path,
                    item.Icon,
                    item.OnSelected,
                    Children.Count - index);
                button.ShouldCloseMenuAfterSelection = item.ShouldCloseMenuAfterSelection;
                buttons[index] = button;
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

        /// <inheritdoc />
        protected override void OnInitialized(ref ContextCircleMenuOption menuOption)
        {
            var buttonCount = ButtonElements.Length;
            _buttonPositions = new Vector3[buttonCount];
            for (var i = 0; i < buttonCount; i++)
                _buttonPositions[i] = GetPositionForIndex(i, buttonCount, menuOption.Radius);
        }

        /// <inheritdoc />
        protected override void OnBuild()
        {
            for (var i = 0; i < ButtonElements.Length; i++)
            {
                var button = ButtonElements[i];
                button.transform.position = Vector3.zero;
                var to = _buttonPositions[i];
                button.experimental.animation.Position(to, 100);
            }
        }

        private Vector3 GetPositionForIndex(float index, float totalCount, float radius)
        {
            var angle = index / totalCount * 360f;
            return new Vector2(
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius
            );
        }
    }
}