using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public struct CircleMenuEventInformation
    {
        public CircleMenuEventInformation(Vector2 mousePosition, Vector2 position)
        {
            MousePosition = mousePosition;
            Position = position;
        }

        public Vector2 MousePosition { get; }
        public Vector2 Position { get; }
    }
}