namespace ContextCircleMenu.Editor
{
    public interface IButtonFactory
    {
        public CircleButton Create(CircleMenuAction menuAction, int section);

        public CircleButton CreateBackButton(CircleMenuAction menuAction, int section);
    }
}