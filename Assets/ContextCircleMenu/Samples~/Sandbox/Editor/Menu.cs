using ContextCircleMenu.Editor;
using UnityEditor;
using UnityEngine;

namespace ContextCircleMenu.Sandbox
{
    public class Menu
    {
        [ContextCircleMenu("Debug Test", EditorIcons.ConsoleInfoIcon)]
        public static void TestMethod()
        {
            Debug.Log("TestMethod");
        }

        [ContextCircleMenu("Instantiate/Cube", EditorIcons.PreMatCube)]
        public static void InstantiateCube(CircleMenuEventInformation information)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Debug.Log($"Position: {information.Position}");
            HandleUtility.PlaceObject(information.Position, out var position, out var normal);
            cube.transform.localPosition = position;
        }

        [ContextCircleMenu("Instantiate/Sphere", EditorIcons.PreMatSphere)]
        public static void InstantiateSphere(CircleMenuEventInformation information)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Debug.Log($"Position: {information.Position}");
            HandleUtility.PlaceObject(information.Position, out var position, out var normal);
            sphere.transform.position = Vector3.zero;
        }

        [ContextCircleMenu("SceneView/Shaded", "d_Shaded")]
        public static void ChangeShadingModeToShaded()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured);
        }

        [ContextCircleMenu("SceneView/Wireframe", "d_Shaded")]
        public static void ChangeShadingModeToWireframe()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe);
        }

        [ContextCircleMenu("SceneView/Shaded Wireframe", "d_Shaded")]
        public static void ChangeShadingModeToShadedWireframe()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.cameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.TexturedWire);
        }

        [ContextCircleMenu("SceneView/Toggle 2D", EditorIcons.SceneView2D)]
        public static void Toggle2D()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.in2DMode = !sceneView.in2DMode;
        }

        [ContextCircleMenu("SceneView/Toggle Gizmos", EditorIcons.GizmosToggle)]
        private static void ToggleGizmos()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.drawGizmos = !sceneView.drawGizmos;
        }
    }
}