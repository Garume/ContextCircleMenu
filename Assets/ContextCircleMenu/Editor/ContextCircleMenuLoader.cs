using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    [InitializeOnLoad]
    public class ContextCircleMenuLoader
    {
        private static SceneView _activeSceneView;
        private static int _activeSceneViewInstanceID;

        private static ContextCircleMenu _contextCircleMenu;
        private static readonly Vector2 RadialMenuSize = new(100, 100);


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

            _contextCircleMenu.CreateMenu(builder =>
            {
                var attributes = TypeCache.GetMethodsWithAttribute<CircularMenuAttribute>();
                foreach (var method in attributes)
                {
                    var attribute = method.GetCustomAttribute<CircularMenuAttribute>(false);
                    builder.AddMenu(attribute.Path, attribute, method);
                }
            });

            _activeSceneView.rootVisualElement.Add(_contextCircleMenu);
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