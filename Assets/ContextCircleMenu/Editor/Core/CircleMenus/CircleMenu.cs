using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <summary>
    ///     Represents a menu item in the circle menu.
    /// </summary>
    public abstract class CircleMenu
    {
        private readonly int _radius;
        protected internal readonly CircleMenu Parent;

        private bool _alreadyInitialized;

        private VisualElement[] _buttonElements;
        private IButtonFactory _buttonFactory;
        private Vector3[] _buttonPositions;
        private VisualElement _preparedElement;
        private VisualElement[] _utilityElements;

        protected CircleMenu(string path, GUIContent icon, Action onSelected, CircleMenu parent, IButtonFactory factory,
            int radius = 100,
            bool shouldCloseMenuAfterSelection = true)
        {
            Path = path;
            Icon = icon;
            OnSelected = onSelected;
            Parent = parent;
            _buttonFactory = factory;
            _radius = radius;
            ShouldCloseMenuAfterSelection = shouldCloseMenuAfterSelection;
        }

        public List<CircleMenu> Children { get; } = new(8);
        public GUIContent Icon { get; }
        public string Path { get; }
        public bool ShouldCloseMenuAfterSelection { get; }
        public Action OnSelected { get; protected set; }

        internal ReadOnlySpan<VisualElement> CreateElements(ref ContextCircleMenuOption menuOption)
        {
            if (!_alreadyInitialized)
            {
                _buttonFactory ??= new ButtonFactory();
                var buttons = CreateButtons(_buttonFactory, ref menuOption);
                _buttonElements = _buttonElements == null ? buttons : _buttonElements.Concat(buttons).ToArray();
                _utilityElements ??= CreateUtilityElements(ref menuOption);

                _buttonPositions = new Vector3[_buttonElements.Length];
                for (var i = 0; i < _buttonElements.Length; i++)
                    _buttonPositions[i] = GetPositionForIndex(i, _buttonElements.Length);
                _alreadyInitialized = true;
            }

            for (var i = 0; i < _buttonElements.Length; i++)
            {
                var button = _buttonElements[i];
                button.transform.position = Vector3.zero;
                var to = _buttonPositions[i];
                button.experimental.animation.Position(to, 100);
            }

            var pool = ArrayPool<VisualElement>.Shared;
            var buffer = pool.Rent(_buttonElements.Length + _utilityElements.Length);
            _buttonElements.CopyTo(buffer, 0);
            _utilityElements.CopyTo(buffer, _buttonElements.Length);
            var combinedSpan = new Span<VisualElement>(buffer, 0, _buttonElements.Length + _utilityElements.Length);
            pool.Return(buffer);
            return combinedSpan;
        }

        internal void PrepareButton(CircleButton button)
        {
            if (_buttonElements == null)
            {
                _buttonElements = new VisualElement[] { button };
            }
            else
            {
                Array.Resize(ref _buttonElements, _buttonElements.Length + 1);
                _buttonElements[^1] = button;
            }
        }

        /// <summary>
        ///     Gets the number of buttons in the menu.
        /// </summary>
        /// <returns></returns>
        public int GetButtonCount()
        {
            return _buttonElements.Length;
        }

        /// <summary>
        ///     Creates the buttons for the menu.
        /// </summary>
        /// <returns></returns>
        protected abstract VisualElement[]
            CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption);

        /// <summary>
        ///     Creates the utility elements for the menu.
        /// </summary>
        /// <returns></returns>
        protected virtual VisualElement[] CreateUtilityElements(ref ContextCircleMenuOption menuOption)
        {
            return Array.Empty<VisualElement>();
        }

        private Vector3 GetPositionForIndex(float index, float totalCount)
        {
            var angle = index / totalCount * 360f;
            return new Vector2(
                Mathf.Sin(angle * Mathf.Deg2Rad) * _radius,
                Mathf.Cos(angle * Mathf.Deg2Rad) * _radius
            );
        }
    }
}