using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public class RootCircleMenu : CircleMenu
    {
        public RootCircleMenu(IButtonFactory factory) : base("root", default, null, null, factory)
        {
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateButtons(IButtonFactory factory)
        {
            var buttons = new VisualElement[Children.Count];
            for (var index = 0; index < Children.Count; index++)
            {
                var item = Children[index];
                buttons[index] =
                    factory.Create(
                        item.Children.Count > 0 ? item.Path + "" : item.Path,
                        item.Icon,
                        item.OnSelected,
                        Children.Count - index,
                        item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

        /// <inheritdoc />
        protected override VisualElement[] CreateUtilityElements()
        {
            return new VisualElement[] { new Label("") };
        }
    }
}