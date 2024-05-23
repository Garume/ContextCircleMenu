namespace ContextCircleMenu.Editor
{
    /// <inheritdoc />
    public sealed class LeafCircleMenu : CircleMenu
    {
        public LeafCircleMenu(CircleMenuAction action, IButtonFactory factory) : base(action, factory)
        {
        }

        /// <inheritdoc />
        protected override CircleButton[] CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption)
        {
            return null;
        }
    }
}