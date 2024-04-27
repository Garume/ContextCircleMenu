using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public abstract class CircleMenu
    {
        private readonly int _radius;
        protected internal readonly List<CircleMenu> Children = new();
        protected internal readonly GUIContent Icon;
        protected internal readonly CircleMenu Parent;
        protected internal readonly string Path;
        protected internal readonly bool ShouldCloseMenuAfterSelection;
        protected internal Action OnSelected;

        public CircleMenu(string path, GUIContent icon, Action onSelected, CircleMenu parent, int radius = 100,
            bool shouldCloseMenuAfterSelection = true)
        {
            Path = path;
            Icon = icon;
            OnSelected = onSelected;
            Parent = parent;
            _radius = radius;
            ShouldCloseMenuAfterSelection = shouldCloseMenuAfterSelection;
        }

        internal ReadOnlySpan<VisualElement> CreateElements()
        {
            var buttons = CreateButtons();
            var utilityElements = CreateUtilityElements();

            for (var i = 0; i < buttons.Length; i++)
            {
                var button = buttons[i];
                button.transform.position = Vector3.zero;
                var to = Vector2.zero + GetPositionForIndex(i, buttons.Length);
                button.experimental.animation.Position(to, 100);
            }

            var pool = ArrayPool<VisualElement>.Shared;
            var buffer = pool.Rent(buttons.Length + utilityElements.Length);
            buttons.CopyTo(buffer, 0);
            utilityElements.CopyTo(buffer, buttons.Length);
            var combinedSpan = new Span<VisualElement>(buffer, 0, buttons.Length + utilityElements.Length);
            pool.Return(buffer);
            return combinedSpan;
        }

        protected abstract VisualElement[] CreateButtons();

        protected virtual VisualElement[] CreateUtilityElements()
        {
            return null;
        }


        private Vector2 GetPositionForIndex(float index, float totalCount)
        {
            var angle = index / totalCount * 360f;
            return new Vector2(
                Mathf.Sin(angle * Mathf.Deg2Rad) * _radius,
                Mathf.Cos(angle * Mathf.Deg2Rad) * _radius
            );
        }
    }
}