using System;
using ContextCircleMenu.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
                builder.ConfigureButton(new CustomButtonFactory());
            };
        }
    }

    public class CustomButtonFactory : IButtonFactory
    {
        public CircleButton Create(string path, GUIContent icon, Action onSelected, int section,
            bool shouldCloseMenuAfterSelection)
        {
            return new OnlyImageCircleButton(path, icon, section, onSelected, shouldCloseMenuAfterSelection);
        }
    }

    public class OnlyImageCircleButton : CircleButton
    {
        public OnlyImageCircleButton(string text, GUIContent icon, int section, Action onSelect,
            bool shouldCloseMenuAfterSelect = true) : base(text, icon, section, onSelect, shouldCloseMenuAfterSelect)
        {
        }

        protected override void ModifierButton(Button button, string text, GUIContent icon, int section)
        {
            var image = new Image
            {
                image = icon.image,
                style =
                {
                    width = 16f,
                    height = 16f,
                    flexShrink = 0
                }
            };
            button.Add(image);
        }
    }
}