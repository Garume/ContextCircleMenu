namespace ContextCircleMenu.Editor
{
    public struct ContextCircleMenuOption
    {
        public ContextCircleMenuOption(float radius, float height, float width)
        {
            Radius = radius;
            Height = height;
            Width = width;
        }

        public readonly float Radius;
        public readonly float Height;
        public readonly float Width;
    }
}