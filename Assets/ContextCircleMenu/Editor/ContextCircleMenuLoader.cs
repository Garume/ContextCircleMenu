using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    /// <summary>
    ///     Initializes and manages the Context Circle Menu within the Unity Scene View.
    /// </summary>
    [InitializeOnLoad]
    public static class ContextCircleMenuLoader
    {
        private static SceneView _activeSceneView;
        private static int _activeSceneViewInstanceID;

        private static ContextCircleMenu _contextCircleMenu;
        private static readonly Vector2 RadialMenuSize = new(100, 100);

        private static Action<CircleMenuBuilder> _onBuild;


        static ContextCircleMenuLoader()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }


        private static void Update()
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

            if (_contextCircleMenu != null || _activeSceneView == null) return;

            if (_activeSceneView.rootVisualElement != null) Initialize();
            else Debug.LogError("_activeSceneView.rootVisualElement was null");
        }

        private static void Initialize()
        {
            if (_contextCircleMenu != null) RemovePreviousRadialMenu();
            _contextCircleMenu =
                new ContextCircleMenu(RadialMenuSize.x, RadialMenuSize.y, 100f, _activeSceneView.rootVisualElement);

            if (_onBuild == null)
                _contextCircleMenu.CreateMenu(builder =>
                {
                    var attributes = TypeCache.GetMethodsWithAttribute<ContextCircleMenuAttribute>();
                    foreach (var method in attributes)
                    {
                        var attribute = method.GetCustomAttribute<ContextCircleMenuAttribute>(false);
                        builder.AddMenu(attribute, method);
                    }
                });
            else
                _contextCircleMenu.CreateMenu(_onBuild);

            _activeSceneView.rootVisualElement.Add(_contextCircleMenu);
        }


        /// <summary>
        ///     Event that allows customization of the Context Circle Menu construction.
        /// </summary>
        /// <remarks>
        ///     This event provides a mechanism to extend or modify the content of the Context Circle Menu
        ///     dynamically at runtime. It's invoked during the initialization phase of the menu in the Scene View.
        ///     Subscribers can add custom menu items or modify existing ones by manipulating the CircleMenuBuilder
        ///     instance provided in the event arguments.
        ///     Example Usage:
        ///     ContextCircleMenuLoader.OnBuild += builder =>
        ///     {
        ///     // Adds a nested menu item under "Custom" with an action to log "custom/test" to the console
        ///     builder.AddMenu("Custom/Debug Test", new GUIContent(), () => Debug.Log("custom/test"));
        ///     // Adds another top-level menu item with an action to log "test"
        ///     builder.AddMenu("Debug Test", new GUIContent(), () => Debug.Log("test"));
        ///     };
        ///     This example demonstrates how developers can add items that perform actions like logging to the console,
        ///     but it could also be used to trigger any method reflecting more complex functionality.
        /// </remarks>
        public static event Action<CircleMenuBuilder> OnBuild
        {
            add => _onBuild += value;
            remove => _onBuild -= value;
        }


        /// <summary>
        ///     Shortcut that toggles the visibility of the Context Circle Menu based on keyboard input.
        /// </summary>
        [ClutchShortcut("Context Circle Menu/Show Menu", typeof(SceneView), KeyCode.A)]
        public static void ToggleMenuClutch(ShortcutArguments args)
        {
            _contextCircleMenu.BlockMouseEvents = args.stage switch
            {
                ShortcutStage.Begin => true,
                ShortcutStage.End => false,
                _ => _contextCircleMenu.BlockMouseEvents
            };

            if (_contextCircleMenu.IsVisible)
            {
                if (args.stage == ShortcutStage.End && _contextCircleMenu.TryForceSelect())
                    _contextCircleMenu.Hide();
            }
            else
            {
                _contextCircleMenu.Show();
            }
        }

        private static void RemovePreviousRadialMenu()
        {
            if (_contextCircleMenu == null) return;
            _contextCircleMenu.RemoveFromHierarchy();
            _contextCircleMenu = null;
        }
    }
}