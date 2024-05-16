using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    /// <summary>
    ///     Represents a circular context menu that can be dynamically constructed and controlled within the Unity Editor.
    /// </summary>
    public class ContextCircleMenu : VisualElement, IMenuControllable
    {
        private const float IndicatorSizeDegrees = 70.0f;
        private static readonly Color AnnulusColor = new(0.02f, 0.02f, 0.02f, 0.8f);
        private static readonly Color MouseAngleIndicatorBackgroundColor = new(0.01f, 0.01f, 0.01f, 1.0f);
        private static readonly Color MouseAngleIndicatorForegroundColor = Color.white;
        private readonly float _height;
        private readonly VisualElement _target;
        private readonly float _width;

        private float _currentMouseAngle;
        private Vector2 _mousePosition;
        private Vector2 _position;

        private CircleMenu _selectedMenu;

        /// <summary>
        ///     Initializes a new instance of the ContextCircleMenu with specified dimensions and target.
        /// </summary>
        /// <param name="width">Width of the menu.</param>
        /// <param name="height">Height of the menu.</param>
        /// <param name="target">The UI element in which the menu will appear.</param>
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

        /// <summary>
        ///     Determines whether mouse events are blocked by the menu.
        /// </summary>
        public bool BlockMouseEvents { get; set; }


        public bool IsVisible => style.display == DisplayStyle.Flex;

        /// <summary>
        ///     Displays the menu and repositions it based on the current mouse position.
        /// </summary>
        public void Show()
        {
            if (IsVisible) return;

            style.display = DisplayStyle.Flex;
            _position = _mousePosition;
            transform.position = _position - new Vector2(_width * 0.5f, _height * 0.5f);
            Rebuild();
        }

        /// <summary>
        ///     Hides the menu.
        /// </summary>
        public void Hide()
        {
            if (!IsVisible) return;

            style.display = DisplayStyle.None;
        }

        /// <summary>
        ///     Opens a specified menu within the context circle menu.
        /// </summary>
        /// <param name="menu">The menu to display.</param>
        public void Open(CircleMenu menu)
        {
            if (!IsVisible) return;

            _selectedMenu = menu;
            Rebuild();
        }

        /// <summary>
        ///     Returns to the parent menu if available.
        /// </summary>
        public void Back()
        {
            if (_selectedMenu.Parent != null) Open(_selectedMenu.Parent);
        }


        private void OnAttach(AttachToPanelEvent evt)
        {
            generateVisualContent += OnGenerateVisualContent;
            _target.RegisterCallback<MouseMoveEvent>(UpdateMousePosition, TrickleDown.TrickleDown);
            _target.RegisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            generateVisualContent -= OnGenerateVisualContent;
            _target.UnregisterCallback<MouseMoveEvent>(UpdateMousePosition, TrickleDown.TrickleDown);
            _target.UnregisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
        }

        private void UpdateMousePosition(MouseMoveEvent evt)
        {
            _mousePosition = evt.localMousePosition;

            if (!IsVisible) return;
            var referenceVector = new Vector2(0f, -1f);
            var mouseVector = new Vector2(
                _mousePosition.x - _position.x,
                -_mousePosition.y + _position.y).normalized;
            _currentMouseAngle = Vector2.SignedAngle(mouseVector, referenceVector);

            MarkDirtyRepaint();
        }

        private void OnClick(ClickEvent evt)
        {
            if (BlockMouseEvents) return;
            Hide();
        }

        /// <summary>
        ///     Force selection of the active button in the menu.
        /// </summary>
        public bool TryForceSelect()
        {
            var button = Children().OfType<CircleButton>().FirstOrDefault(b => b.IsEntered);
            return button != null && button.TryForceSelect();
        }

        public bool TryForceEnterByMousePosition()
        {
            var anglePerRegion = 360f / (Children().Count() - 1);

            var currentAngle = _currentMouseAngle % 360;
            if (currentAngle < 0) currentAngle += 360;

            var region = (int)(currentAngle / anglePerRegion);
            if (region <= 0) region = Children().Count() - 1;

            var otherButtons = Children().OfType<CircleButton>().Where(x => x.Section != region);
            foreach (var otherButton in otherButtons)
            {
                var mouseLeaveEvent = MouseLeaveEvent.GetPooled();
                mouseLeaveEvent.target = otherButton;
                otherButton.SendEvent(mouseLeaveEvent);
            }

            var button = Children().OfType<CircleButton>().FirstOrDefault(x => x.Section == region);
            if (button == null) return true;
            var mouseEnterEvent = MouseEnterEvent.GetPooled();
            mouseEnterEvent.target = button;
            button.SendEvent(mouseEnterEvent);
            return false;
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

            var startAngle = _currentMouseAngle + 90.0f - IndicatorSizeDegrees * 0.5f;
            var endAngle = _currentMouseAngle + 90.0f + IndicatorSizeDegrees * 0.5f;

            var painter = context.painter2D;
            painter.lineCap = LineCap.Butt;

            painter.DrawCircle(position, radius, 0f, 360.0f, 8.0f, AnnulusColor);
            painter.DrawCircle(position, radius, startAngle, endAngle, 8.0f, MouseAngleIndicatorBackgroundColor);
            painter.DrawCircle(position, radius, startAngle, endAngle, 4.0f, MouseAngleIndicatorForegroundColor);
        }

        /// <summary>
        ///     Configures and builds the menu based on a provided configuration action.
        /// </summary>
        /// <param name="configureMenu">
        ///     An action delegate that configures the menu. This delegate is responsible for defining
        ///     the structure and behavior of the menu items within the context menu.
        /// </param>
        /// <remarks>
        ///     This method is used to dynamically construct the menu content at runtime. It allows for flexible modification
        ///     and extension of the menu items by external code. The method receives an action delegate that takes a
        ///     <see cref="CircleMenuBuilder" /> as a parameter, which is used to add, configure, and organize menu items.
        ///     The <see cref="CircleMenuBuilder" /> provides methods such as <code>AddMenu</code> which are used to define each
        ///     item's
        ///     label, icon, and the action it triggers when selected. This design allows for the encapsulation of the menu's
        ///     construction logic, making the <see cref="ContextCircleMenu" /> class highly modular and customizable.
        ///     Example usage:
        ///     <code>
        /// contextCircleMenu.CreateMenu(builder => {
        ///     builder.AddMenu("File/Open", new GUIContent(), () => { Debug.Log("Open clicked"); });
        ///     builder.AddMenu("File/Save", new GUIContent(), () => { Debug.Log("Save clicked"); });
        /// });
        /// </code>
        ///     This example configures the menu to have "Open" and "Save" options under a "File" submenu, with each menu item
        ///     triggering a logging action when clicked.
        /// </remarks>
        public void CreateMenu(Action<CircleMenuBuilder> configureMenu)
        {
            var builder = new CircleMenuBuilder();
            configureMenu(builder);

            _selectedMenu = builder.Build(this);
        }
    }
}