using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
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
                new ContextCircleMenu(RadialMenuSize.x, RadialMenuSize.y, _activeSceneView.rootVisualElement);

            if (_onBuild == null)
                _contextCircleMenu.CreateMenu(builder =>
                {
                    var attributes = TypeCache.GetMethodsWithAttribute<ContextCircleMenuAttribute>();
                    foreach (var method in attributes)
                    {
                        var attribute = method.GetCustomAttribute<ContextCircleMenuAttribute>(false);
                        builder.AddMenu(attribute.Path, attribute, method);
                    }
                });
            else
                _contextCircleMenu.CreateMenu(_onBuild);

            _activeSceneView.rootVisualElement.Add(_contextCircleMenu);
        }

        public static event Action<CircleMenuBuilder> OnBuild
        {
            add => _onBuild += value;
            remove => _onBuild -= value;
        }


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
                if (args.stage == ShortcutStage.End && _contextCircleMenu.ForceSelectIfExistEnteredButton())
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