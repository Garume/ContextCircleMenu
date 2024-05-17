using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public abstract class CircleButton : VisualElement
    {
        private readonly Action _onSelect;
        private Button _button;

        protected CircleButton(string text, GUIContent icon, int section, Action onSelect)
        {
            _onSelect = onSelect;

            Section = section;

            Initialize(text, icon, section);
        }

        internal bool ShouldCloseMenuAfterSelection { get; set; } = true;

        public bool IsEntered { get; private set; }
        public int Section { get; private set; }
        private void Initialize(string text, GUIContent icon, int section)
        {
            style.position = Position.Absolute;
            style.alignItems = Align.Center;

            _button = new Button(_onSelect);
            ModifierButton(_button, text, icon, section);
            Add(_button);

            RegisterCallback<MouseEnterEvent>(InternalOnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(InternalOnMouseLeave);
        }

        protected abstract void ModifierButton(Button button, string text, GUIContent icon, int section);

        internal bool TryForceSelect()
        {
            _onSelect?.Invoke();
            return ShouldCloseMenuAfterSelection;
        }

        private void InternalOnMouseEnter(MouseEnterEvent evt)
        {
            IsEntered = true;
            OnMouseEnter(_button, evt);
        }

        protected virtual void OnMouseEnter(Button button, MouseEnterEvent evt)
        {
        }

        private void InternalOnMouseLeave(MouseLeaveEvent evt)
        {
            IsEntered = false;
            OnMouseLeave(_button, evt);
        }

        protected virtual void OnMouseLeave(Button button, MouseLeaveEvent evt)
        {
        }
    }
}