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
                builder.AddMenu("Custom/Debug Test 2", EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                    () => Debug.Log("custom/test2"));
                builder.AddMenu("Custom/Debug Test 3", EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                    () => Debug.Log("custom/test3"));

                for (var i = 0; i < 5; i++)
                    builder.AddMenu($"Custom/a/Debug Test {i}",
                        EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                        () => Debug.Log($"custom/test{i}"));

                for (var i = 0; i < 6; i++)
                    builder.AddMenu($"Custom/b/Debug Test {i}",
                        EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                        () => Debug.Log($"custom/test{i}"));

                builder.AddMenu("Debug Test", new GUIContent(), () => Debug.Log("test"));
                builder.AddMenu("Debug Test 2", EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                    () => Debug.Log("test2"));
                builder.ConfigureButton(new CustomButtonFactory());
                builder.ConfigureFolder(new CustomFolderMenuFactory());
            };
        }
    }

    public class CustomFolderMenuFactory : IFolderCircleMenuFactory
    {
        public FolderCircleMenu Create(string path, IMenuControllable menu, CircleMenu parent, IButtonFactory factory)
        {
            return new CustomFolderCircleMenu(path, menu, parent, factory);
        }
    }

    public class CustomFolderCircleMenu : FolderCircleMenu
    {
        public CustomFolderCircleMenu(string path, IMenuControllable menu, CircleMenu parent, IButtonFactory factory,
            int radius = 100) : base(path, menu, EditorGUIUtility.IconContent(EditorIcons.FolderIcon), parent, factory,
            radius)
        {
        }

        protected override VisualElement[] CreateButtons(IButtonFactory factory, ref ContextCircleMenuOption menuOption)
        {
            var buttons = new VisualElement[Children.Count];
            for (var index = 0; index < buttons.Length; index++)
            {
                var item = Children[index];
                buttons[index] =
                    factory.Create(
                        item.Path,
                        item.Icon,
                        item.OnSelected,
                        Children.Count - index,
                        item.ShouldCloseMenuAfterSelection);
            }

            return buttons;
        }

        protected override VisualElement[] CreateUtilityElements(ref ContextCircleMenuOption menuOption)
        {
            var element = new VisualElement();
            var option = menuOption;
            element.generateVisualContent += context =>
            {
                var painter = context.painter2D;

                for (var i = 0; i < GetButtonCount(); i++)
                {
                    var angle = (float)i / GetButtonCount() * 360f;
                    if (GetButtonCount() % 2 == 1)
                        angle += 180f;
                    else
                        angle += 180f - 360f / GetButtonCount() / 2;
                    var vector = new Vector2(
                        Mathf.Sin(Mathf.Deg2Rad * angle),
                        Mathf.Cos(Mathf.Deg2Rad * angle)).normalized;

                    var from = vector * 12f;
                    var to = vector * option.Radius * 1.5f;
                    painter.strokeColor = Color.black;
                    painter.lineWidth = 2f;
                    painter.BeginPath();
                    painter.MoveTo(from);
                    painter.LineTo(to);
                    painter.Stroke();
                }

                painter.BeginPath();
                painter.Arc(Vector2.zero, option.Radius * 1.5f, 0, 360f);
                painter.fillColor = new Color(0f, 0f, 0f, 0.2f);
                painter.Fill();

                painter.DrawCircle(Vector2.zero, option.Radius * 1.5f, 0, 360f, 5f, Color.gray);
            };
            return new[] { element };
        }
    }

    public class CustomButtonFactory : IButtonFactory
    {
        public CircleButton Create(string path, GUIContent icon, Action onSelected, int section,
            bool shouldCloseMenuAfterSelection)
        {
            return new OnlyImageCircleButton(path, icon, section, onSelected, shouldCloseMenuAfterSelection);
        }

        public CircleButton CreateBackButton(Action onBack)
        {
            return new OnlyImageCircleButton("Back", EditorGUIUtility.IconContent(EditorIcons.Back2x),
                -1, onBack, false);
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
                    width = 32f,
                    height = 32f,
                    flexShrink = 0
                },
                tooltip = text
            };

            button.Add(image);
        }
    }
}