using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public abstract class CircleButton : VisualElement
    {
        private Button _button;
        private bool _shouldCloseMenuAfterSelection = true;

        protected CircleButton(CircleMenuAction menuAction, int section)
        {
            CircleMenuAction = menuAction;
            Section = section;
            Initialize();
        }

        internal CircleMenuAction CircleMenuAction { get; }

        internal bool ShouldCloseMenuAfterSelection
        {
            get => _shouldCloseMenuAfterSelection && CircleMenuAction.CurrentStatus == CircleMenuAction.Status.Normal;
            set => _shouldCloseMenuAfterSelection = value;
        }

        public bool IsEntered { get; internal set; }
        public int Section { get; }

        private void Initialize()
        {
            style.position = Position.Absolute;
            style.alignItems = Align.Center;

            _button = new Button(CircleMenuAction.Execute);
            ModifierButton(_button, CircleMenuAction, Section);
            Add(_button);

            RegisterCallback<MouseEnterEvent>(InternalOnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(InternalOnMouseLeave);
        }

        protected abstract void ModifierButton(Button button, CircleMenuAction menuAction, int section);

        internal bool TryForceSelect()
        {
            CircleMenuAction.Execute();
            return ShouldCloseMenuAfterSelection && CircleMenuAction.CurrentStatus == CircleMenuAction.Status.Normal;
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


        internal void UpdateStatus(CircleMenuEventInformation information)
        {
            CircleMenuAction.UpdateStatus(information);
            var enabled = CircleMenuAction.CurrentStatus == CircleMenuAction.Status.Normal;
            _button.SetEnabled(enabled);
        }
    }
}