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
        protected internal readonly CircleMenu Parent;

        private bool _alreadyInitialized;
        private IButtonFactory _buttonFactory;

        protected CircleButton[] ButtonElements;
        protected VisualElement[] UtilityElements;

        protected CircleMenu(string path, GUIContent icon, Action onSelected, CircleMenu parent, IButtonFactory factory,
            bool shouldCloseMenuAfterSelection = true)
        {
            Path = path;
            Icon = icon;
            OnSelected = onSelected;
            Parent = parent;
            ShouldCloseMenuAfterSelection = shouldCloseMenuAfterSelection;
            _buttonFactory = factory;
        }

        public List<CircleMenu> Children { get; } = new(8);
        public GUIContent Icon { get; }
        public string Path { get; }
        public bool ShouldCloseMenuAfterSelection { get; }
        public Action OnSelected { get; protected set; }

        internal ReadOnlySpan<VisualElement> BuildElements(ref ContextCircleMenuOption menuOption)
        {
            if (!_alreadyInitialized)
            {
                _buttonFactory ??= new ButtonFactory();
                var buttons = CreateButtons(_buttonFactory, ref menuOption);
                ButtonElements = ButtonElements == null ? buttons : ButtonElements.Concat(buttons).ToArray();
                UtilityElements = CreateUtilityElements(ref menuOption);

                OnInitialized(ref menuOption);
                _alreadyInitialized = true;
            }

            OnBuild();

            var pool = ArrayPool<VisualElement>.Shared;
            var buffer = pool.Rent(ButtonElements.Length + UtilityElements.Length);
            ButtonElements.CopyTo(buffer, 0);
            UtilityElements.CopyTo(buffer, ButtonElements.Length);
            var combinedSpan = new Span<VisualElement>(buffer, 0, ButtonElements.Length + UtilityElements.Length);
            pool.Return(buffer);
            return combinedSpan;
        }


        internal void PrepareButton(CircleButton button)
        {
            if (ButtonElements == null)
            {
                ButtonElements = new[] { button };
            }
            else
            {
                Array.Resize(ref ButtonElements, ButtonElements.Length + 1);
                ButtonElements[^1] = button;
            }
        }

        /// <summary>
        ///     Creates the buttons for the menu.
        /// </summary>
        /// <returns></returns>
        protected abstract CircleButton[]
            CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption);

        /// <summary>
        ///     Creates the utility elements for the menu.
        /// </summary>
        /// <returns></returns>
        protected virtual VisualElement[] CreateUtilityElements(ref ContextCircleMenuOption menuOption)
        {
            return Array.Empty<VisualElement>();
        }

        /// <summary>
        ///     Called when the menu is initialized.
        /// </summary>
        /// <param name="menuOption"></param>
        protected virtual void OnInitialized(ref ContextCircleMenuOption menuOption)
        {
        }

        /// <summary>
        ///     Called when the menu is built.
        /// </summary>
        protected virtual void OnBuild()
        {
        }
    }
}