namespace ContextCircleMenu.Editor
{
    public interface IMenuControllable
    {
        public void Show();
        public void Hide();
        public void Open(CircleMenu menu);
        public void Back();
    }
}