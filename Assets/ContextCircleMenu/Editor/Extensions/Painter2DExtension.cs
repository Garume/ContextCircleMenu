using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public static class Painter2DExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawCircle(this Painter2D painter, Vector2 center, float radius, float startAngle,
            float endAngle, float lineWidth, Color color)
        {
            painter.lineWidth = lineWidth;
            painter.strokeColor = color;
            painter.BeginPath();
            painter.Arc(center, radius, startAngle, endAngle);
            painter.Stroke();
        }
    }
}