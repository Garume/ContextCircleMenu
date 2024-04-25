using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public class ContextCircleMenu : VisualElement
    {
        private static readonly Color AnnulusColor = new(0.02f, 0.02f, 0.02f, 0.8f);
        private static readonly Color MouseAngleIndicatorBackgroundColor = new(0.01f, 0.01f, 0.01f, 1.0f);
        private static readonly Color MouseAngleIndicatorForegroundColor = Color.white;
        private readonly float _height;
        private readonly VisualElement _target;
        private readonly float _width;

        private float _currentMouseAngle;
        private Vector2 _mousePosition;
        private Vector2 _position;

        private CircleMenu _rootMenu;
        private CircleMenu _selectedMenu;

        public ContextCircleMenu(float width, float height, VisualElement target)
        {
            _width = width;
            _height = height;
            _target = target;

            style.position = Position.Absolute;
            style.width = width;
            style.height = height;
            style.display = DisplayStyle.None;
            style.marginBottom = 0.0f;
            style.marginTop = 0.0f;
            style.marginRight = 0.0f;
            style.marginLeft = 0.0f;
            style.paddingBottom = 0.0f;
            style.paddingTop = 0.0f;
            style.paddingRight = 0.0f;
            style.paddingLeft = 0.0f;
            style.alignItems = Align.Center;
            style.alignContent = Align.Center;
            style.justifyContent = Justify.Center;

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        public bool BlockMouseEvents { get; set; }


        public bool IsVisible => style.display == DisplayStyle.Flex;


        private void OnAttach(AttachToPanelEvent evt)
        {
            generateVisualContent += OnGenerateVisualContent;
            _target.RegisterCallback<MouseMoveEvent>(UpdateMousePosition);
            _target.RegisterCallback<ClickEvent>(OnClick);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            generateVisualContent -= OnGenerateVisualContent;
            _target.UnregisterCallback<MouseMoveEvent>(UpdateMousePosition);
            _target.UnregisterCallback<ClickEvent>(OnClick);
        }

        private void UpdateMousePosition(MouseMoveEvent evt)
        {
            _mousePosition = evt.localMousePosition;

            if (!IsVisible) return;
            var referenceVector = new Vector2(0f, -1f);
            var mouseVector = new Vector2(
                evt.mousePosition.x - _position.x,
                -evt.mousePosition.y + _position.y).normalized;
            var angle = (float)(Math.Atan2(referenceVector.y, referenceVector.x) -
                                Math.Atan2(mouseVector.y, mouseVector.x)) * (float)(180 / Math.PI);
            if (angle < 0) angle += 360.0f;
            _currentMouseAngle = angle;
            MarkDirtyRepaint();
        }

        private void OnClick(ClickEvent evt)
        {
            if (BlockMouseEvents) return;
            Hide();
        }


        public void Show()
        {
            if (IsVisible) return;

            style.display = DisplayStyle.Flex;
            _position = _mousePosition;
            transform.position = _position - new Vector2(_width * 0.5f, _height * 0.5f);
            Rebuild();
        }

        public void Hide()
        {
            if (!IsVisible) return;

            style.display = DisplayStyle.None;
        }

        public void Open(CircleMenu menu)
        {
            if (!IsVisible) return;

            _selectedMenu = menu;
            Rebuild();
        }

        public void Back()
        {
            if (_selectedMenu.Parent != null) Open(_selectedMenu.Parent);
        }

        public bool ForceSelectIfExistEnteredButton()
        {
            var button = Children().OfType<CircularButton>().FirstOrDefault(b => b.IsEntered);
            return button != null && button.ForceSelect();
        }

        private void Rebuild()
        {
            Clear();
            var elements = _selectedMenu.CreateElements();
            for (var i = 0; i < elements.Length; i++) Add(elements[i]);
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var position = new Vector2(_width * 0.5f, _height * 0.5f);
            var radius = _width * 0.1f;
            const float indicatorSizeDegrees = 70.0f;

            var painter = context.painter2D;
            painter.lineCap = LineCap.Butt;

            painter.lineWidth = 8.0f;
            painter.strokeColor = AnnulusColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius, 0.0f, 360.0f);
            painter.Stroke();

            painter.lineWidth = 8.0f;
            painter.strokeColor = MouseAngleIndicatorBackgroundColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius,
                _currentMouseAngle + 90.0f - indicatorSizeDegrees * 0.5f,
                _currentMouseAngle + 90.0f + indicatorSizeDegrees * 0.5f);
            painter.Stroke();

            painter.lineWidth = 4.0f;
            painter.strokeColor = MouseAngleIndicatorForegroundColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius,
                _currentMouseAngle + 90.0f - indicatorSizeDegrees * 0.5f,
                _currentMouseAngle + 90.0f + indicatorSizeDegrees * 0.5f);
            painter.Stroke();
        }


        public void CreateMenu(Action<CircleMenuBuilder> configureMenu)
        {
            var builder = new CircleMenuBuilder();
            configureMenu(builder);

            _rootMenu?.Children.Clear();
            _rootMenu = builder.Build(Open);
            _selectedMenu = _rootMenu;
        }
    }
}