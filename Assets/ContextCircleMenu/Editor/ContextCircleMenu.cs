using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    [InitializeOnLoad]
    public class CircularMenu
    {
        // Radial menu.
        private const int Radius = 100;

        private const KeyCode ActivationShortcutKey = KeyCode.A;

        // Scene view.
        private static SceneView _activeSceneView;
        private static int _activeSceneViewInstanceID;

        // VisualElements.
        private static VisualElement _sceneViewRoot;
        private static VisualElement _radialMenuRoot;
        private static readonly Vector2 RadialMenuSize = new(100, 100);
        private static int _currentlyHoveredSection = -1;
        private static readonly CircularMenuView RootCircularMenuView = new("root", new GUIContent(), () => { }, null);
        private static CircularMenuView _activeCircularMenuView;
        private static double _timeWhenRadialMenuOpened;

        // Mouse info.
        private static Vector2 _mousePositionWhenRadialMenuOpened;
        private static Vector2 _currentMousePosition;
        private static float _currentMouseAngle;

        // Colors
        private static readonly Color AnnulusColor = new(0.02f, 0.02f, 0.02f, 0.8f);
        private static readonly Color MouseAngleIndicatorBackgroundColor = new(0.01f, 0.01f, 0.01f, 1.0f);
        private static readonly Color MouseAngleIndicatorForegroundColor = Color.white;
        
        // Magic numbers



        static CircularMenu()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
            EditorApplication.update += OnEditorApplicationUpdate;

            SceneView.duringSceneGui -= OnDuringSceneGUI;
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        private static bool RadialMenuIsVisible => _radialMenuRoot?.style.display == DisplayStyle.Flex;

        private static void OnEditorApplicationUpdate()
        {
            // Get the currently active scene view.
            _activeSceneView = SceneView.currentDrawingSceneView
                ? SceneView.currentDrawingSceneView
                : SceneView.lastActiveSceneView;

            // Check if the scene view changed.
            if (_activeSceneView && _activeSceneView.GetInstanceID() != _activeSceneViewInstanceID)
            {
                _activeSceneViewInstanceID = _activeSceneView.GetInstanceID();
                RemovePreviousRadialMenu();
            }

            if (_radialMenuRoot != null || _sceneViewRoot != null || _activeSceneView == null) return;
            _sceneViewRoot = _activeSceneView.rootVisualElement;

            if (_sceneViewRoot != null) Initialize();
            else Debug.LogError("_activeSceneView.rootVisualElement was null");
        }

        private static void Initialize()
        {
            if (_radialMenuRoot != null) RemovePreviousRadialMenu();
            var methods = TypeCache.GetMethodsWithAttribute<CircularMenuAttribute>();

            // Create the root VisualElement that holds the radial menu.
            _radialMenuRoot = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    width = RadialMenuSize.x,
                    height = RadialMenuSize.y,
                    display = DisplayStyle.None, // initially hidden
                    marginBottom = 0.0f,
                    marginTop = 0.0f,
                    marginRight = 0.0f,
                    marginLeft = 0.0f,
                    paddingBottom = 0.0f,
                    paddingTop = 0.0f,
                    paddingRight = 0.0f,
                    paddingLeft = 0.0f,
                    alignItems = Align.Center,
                    alignContent = Align.Center,
                    justifyContent = Justify.Center
                }
            };

            // Draw the center mouse angle indicator.
            _radialMenuRoot.generateVisualContent -= DrawMouseAngleIndicator;
            _radialMenuRoot.generateVisualContent += DrawMouseAngleIndicator;

            // Create the radial menu for each method.
            RootCircularMenuView.Children.Clear();
            foreach (var method in methods)
            {
                var attribute =
                    (CircularMenuAttribute)method.GetCustomAttributes(typeof(CircularMenuAttribute), false).First();
                CreateRadialMenu(attribute.Path, attribute, method);
            }

            _activeCircularMenuView = RootCircularMenuView;

            // Add the radial menu root to the scene view root.
            _sceneViewRoot.Add(_radialMenuRoot);
        }

        private static void ShowRadialMenu(Vector2 position)
        {
            if (_radialMenuRoot == null) return;
            _radialMenuRoot.style.display = _radialMenuRoot.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            _radialMenuRoot.transform.position =
                position - new Vector2(RadialMenuSize.x * 0.5f, RadialMenuSize.y * 0.5f);
            RebuildRadialMenu();
            _activeSceneView.Repaint();
        }

        private static void RebuildRadialMenu()
        {
            // Remove all child elements.
            _radialMenuRoot.Clear();

            // If the radial menu item has a parent.
            if (_activeCircularMenuView.Parent != null)
            {
                // Add a label showing the current folder.
                _radialMenuRoot.Add(new Label(_activeCircularMenuView.Path)
                {
                    style =
                    {
                        marginBottom = RadialMenuSize.x * 0.5f + 5.0f,
                        fontSize = 10,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        color = Color.white,
                        textShadow = new TextShadow
                        {
                            offset = new Vector2(0.2f, 0.2f),
                            blurRadius = 0,
                            color = Color.black
                        }
                    }
                });

                // Add back button.
                _radialMenuRoot.Add(new CircularMenuButton("Back", new GUIContent(), -1,
                    () => SelectRadialMenuItem(_activeCircularMenuView.Parent)));
            }
            // If the menu item does not have a parent, 
            else
            {
                _radialMenuRoot.Add(new Label("")); // HACKY
            }

            // Add a button for each child of the radial menu item.
            var section = 1;
            foreach (var item in _activeCircularMenuView.Children)
            {
                _radialMenuRoot.Add(new CircularMenuButton(
                    item.Children.Count > 0 ? item.Path + "" : item.Path,
                    item.Icon,
                    _activeCircularMenuView.Children.Count - section,
                    item.OnRadialMenuItemSelected));
                section++;
            }

            // Move all buttons outwards using an animation.
            var i = 0;
            foreach (var item in _radialMenuRoot.Children().Where(c => c is CircularMenuButton))
            {
                item.transform.position = Vector3.zero;
                var targetPosition = Vector2.zero + GetCircleOffset(Radius, i, _radialMenuRoot.childCount - 1);
                item.experimental.animation.Position(targetPosition, 100);
                i++;
            }
        }

        private static void OnDuringSceneGUI(SceneView view)
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.Repaint:
                    _currentMousePosition = currentEvent.mousePosition;
                    break;
                // Allow clicking the left mouse button to move the location of the radial menu.
                case EventType.MouseDown when currentEvent.button == 0 && RadialMenuIsVisible:
                    ShowRadialMenu(_currentMousePosition);
                    break;
                // Show the radial menu when the activation shortcut is pressed and store the initial mouse position.
                case EventType.KeyDown when Event.current.keyCode == ActivationShortcutKey && !RadialMenuIsVisible:
                    _timeWhenRadialMenuOpened = EditorApplication.timeSinceStartup;
                    _mousePositionWhenRadialMenuOpened = _currentMousePosition;
                    ShowRadialMenu(_currentMousePosition);
                    break;
                // Update the radial menu when moving the mouse.
                case EventType.MouseMove when RadialMenuIsVisible:
                {
                    // Calculate the offset angle.
                    var referenceVector = new Vector2(0.0f, -1.0f);
                    var mouseVector = new Vector2(
                        _currentMousePosition.x - _mousePositionWhenRadialMenuOpened.x,
                        -_currentMousePosition.y + _mousePositionWhenRadialMenuOpened.y).normalized;
                    var angle = (float)(Math.Atan2(referenceVector.y, referenceVector.x) -
                                        Math.Atan2(mouseVector.y, mouseVector.x)) * (float)(180 / Math.PI);
                    if (angle < 0) angle += 360.0f;
                    _currentMouseAngle = angle;

                    // Calculate which section is being hovered over.
                    var sectionCount = _activeCircularMenuView.Children.Count;
                    if (_activeCircularMenuView.Parent != null) sectionCount++; // back button
                    var sectionPartAngle = 360.0f / sectionCount; // the part of a single section

                    int hoveredSection;
                    // HACKY: This code is kind of hacky and was a little bit of trial and error.
                    if (_activeCircularMenuView.Parent != null)
                        hoveredSection = Mathf.RoundToInt(angle / sectionPartAngle) % sectionCount - 1;
                    else
                        hoveredSection = (Mathf.RoundToInt(angle / sectionPartAngle) + sectionCount - 1) % sectionCount;

                    // If we moved the mouse to a new section.
                    if (hoveredSection != _currentlyHoveredSection)
                    {
                        // Go through all the buttons
                        var buttons = _radialMenuRoot.Children().Where(child => child is CircularMenuButton).ToList()
                            .Select(e => e as CircularMenuButton);
                        foreach (var button in buttons) button?.Hover(button.Section == hoveredSection);
                        _currentlyHoveredSection = hoveredSection;
                    }

                    _radialMenuRoot.MarkDirtyRepaint();
                    break;
                }
                // Select a radial menu item when the activation shortcut is released while the radial menu is visible.
                case EventType.KeyUp when Event.current.keyCode == ActivationShortcutKey && RadialMenuIsVisible:
                {
                    if (EditorApplication.timeSinceStartup - _timeWhenRadialMenuOpened > 0.2)
                    {
                        // Calculate the distance that the mouse has moved.
                        var mouseMoveDistance = Vector3.Distance(
                            EditorGUIUtility.PixelsToPoints(_mousePositionWhenRadialMenuOpened),
                            EditorGUIUtility.PixelsToPoints(_currentMousePosition));

                        // Require a minimum mouse move distance before a selection is triggered.
                        if (mouseMoveDistance > 15)
                        {
                            if (_currentlyHoveredSection == -1 && _activeCircularMenuView.Parent != null)
                                SelectRadialMenuItem(_activeCircularMenuView.Parent); // back button 
                            else
                                _activeCircularMenuView
                                    .Children[_activeCircularMenuView.Children.Count - _currentlyHoveredSection - 1]
                                    .OnRadialMenuItemSelected();
                        }
                        else
                        {
                            ShowRadialMenu(_currentMousePosition);
                        }
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RemovePreviousRadialMenu()
        {
            if (_radialMenuRoot is null) return;
            _radialMenuRoot.RemoveFromHierarchy();
            _radialMenuRoot = null;
        }

        private static Vector2 GetCircleOffset(float radius, float index, float numberOfSections)
        {
            var angle = index / numberOfSections * 360.0f;
            var offset = new Vector2
            {
                x = radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                y = radius * Mathf.Cos(angle * Mathf.Deg2Rad)
            };
            return offset;
        }

        private static void DrawMouseAngleIndicator(MeshGenerationContext context)
        {
            var position = new Vector2(RadialMenuSize.x * 0.5f, RadialMenuSize.y * 0.5f);
            var radius = RadialMenuSize.x * 0.1f;
            const float indicatorSizeDegrees = 70.0f;

            var painter = context.painter2D;
            painter.lineCap = LineCap.Butt;

            // Draw the annulus.
            painter.lineWidth = 8.0f;
            painter.strokeColor = AnnulusColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius, 0.0f, 360.0f);
            painter.Stroke();

            // Draw the mouse angle indicator background.
            painter.lineWidth = 8.0f;
            painter.strokeColor = MouseAngleIndicatorBackgroundColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius,
                _currentMouseAngle + 90.0f - indicatorSizeDegrees * 0.5f,
                _currentMouseAngle + 90.0f + indicatorSizeDegrees * 0.5f);
            painter.Stroke();

            // Draw the mouse angle indicator.
            painter.lineWidth = 4.0f;
            painter.strokeColor = MouseAngleIndicatorForegroundColor;
            painter.BeginPath();
            painter.Arc(new Vector2(position.x, position.y), radius,
                _currentMouseAngle + 90.0f - indicatorSizeDegrees * 0.5f,
                _currentMouseAngle + 90.0f + indicatorSizeDegrees * 0.5f);
            painter.Stroke();
        }

        private static void CreateRadialMenu(string path, CircularMenuAttribute attribute, MethodInfo method)
        {
            var pathSegments = path.Split('/');

            // Create the root radial menu view.
            var rootRadialMenuView = RootCircularMenuView;

            // Create the branch radial menus views.
            if (pathSegments.Length > 1)
                for (var i = 0; i < pathSegments.Length - 1; i++)
                {
                    var pathSegment = pathSegments[i];

                    // Look for an existing radial menu view with the same path.
                    var branchRadialMenuView = rootRadialMenuView.Children.Find(x => x.Path == pathSegment);

                    // Create a new one if it does not exist yet.
                    if (branchRadialMenuView is null)
                    {
                        branchRadialMenuView = new CircularMenuView(pathSegment,
                            EditorGUIUtility.IconContent(EditorIcons.FolderIcon),
                            () => SelectRadialMenuItem(branchRadialMenuView), rootRadialMenuView);
                        RootCircularMenuView.Children.Add(branchRadialMenuView);
                    }

                    rootRadialMenuView = branchRadialMenuView;
                }

            // Create the leaf radial menu view.
            var leafRadialMenuView = new CircularMenuView(pathSegments.Last(), attribute.Icon, () =>
            {
                ShowRadialMenu(_mousePositionWhenRadialMenuOpened);

                // Invoke the method that is linked to this leaf radial menu view.
                method.Invoke(null, null);
            }, null);
            rootRadialMenuView.Children.Add(leafRadialMenuView);
        }

        private static void SelectRadialMenuItem(CircularMenuView circularMenuView)
        {
            _activeCircularMenuView = circularMenuView;
            RebuildRadialMenu();
        }
    }
}