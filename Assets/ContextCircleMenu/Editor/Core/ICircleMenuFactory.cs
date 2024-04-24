namespace ContextCircleMenu.Editor
{
    public interface ICircleMenuFactory
    {
        public string[] PathSegments { get; }
        public CircleMenu Create();
    }
}