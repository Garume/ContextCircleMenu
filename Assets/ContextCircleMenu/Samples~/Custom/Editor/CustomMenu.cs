using ContextCircleMenu.Editor;
using UnityEditor;
using UnityEngine;

namespace ContextCircleMenu.Custom
{
    public static class CustomMenu
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            ContextCircleMenuLoader.OnBuild += builder =>
            {
                builder.AddMenu("Custom/Debug Test", new GUIContent(), () => Debug.Log("custom/test"));
                builder.AddMenu("Debug Test", new GUIContent(), () => Debug.Log("test"));
            };
        }
    }
}