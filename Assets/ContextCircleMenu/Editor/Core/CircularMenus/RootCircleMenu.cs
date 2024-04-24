using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public class RootCircleMenu : CircleMenu
    {
        public RootCircleMenu() : base("root", default, null, null)
        {
        }

        protected override VisualElement[] CreateButtons()
        {
            var buttons = new VisualElement[Children.Count];
            for (var index = 0; index < Children.Count; index++)
            {
                var item = Children[index];
                buttons[index] =
                    new CircularButton(
                        item.Children.Count > 0 ? item.Path + "" : item.Path,
                        item.Icon,
                        Children.Count - index,
                        item.OnSelected,
                        item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

        protected override VisualElement[] CreateUtilityElements()
        {
            return new VisualElement[] { new Label("") };
        }
    }
}