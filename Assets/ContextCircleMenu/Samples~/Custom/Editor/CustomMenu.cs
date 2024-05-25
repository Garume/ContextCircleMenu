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
                {
                    var i1 = i;
                    builder.AddMenu($"Custom/a/Debug Test {i}",
                        EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                        () => Debug.Log($"custom/test{i1}"));
                }

                for (var i = 0; i < 6; i++)
                {
                    var i1 = i;
                    builder.AddMenu($"Custom/b/Debug Test {i}",
                        EditorGUIUtility.IconContent(EditorIcons.ConsoleInfoIcon2x),
                        () => Debug.Log($"custom/test{i1}"));
                }

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
        public CustomFolderCircleMenu(string path, IMenuControllable menu, CircleMenu parent, IButtonFactory factory) :
            base(path, menu, parent, factory)
        {
        }

        protected override VisualElement[] CreateUtilityElements(ref ContextCircleMenuOption menuOption)
        {
            var element = new VisualElement();
            var option = menuOption;
            element.generateVisualContent += context =>
            {
                var painter = context.painter2D;
                var buttonCount = ButtonElements.Length;
                for (var i = 0; i < buttonCount; i++)
                {
                    var angle = (float)i / buttonCount * 360f;
                    if (buttonCount % 2 == 1)
                        angle += 180f;
                    else
                        angle += 180f - 360f / buttonCount / 2;
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
        public CircleButton Create(CircleMenuAction menuAction, int section)
        {
            return new OnlyImageCircleButton(menuAction, section);
        }

        public CircleButton CreateBackButton(CircleMenuAction menuAction, int section)
        {
            menuAction.ActionName = "Back";
            menuAction.Icon = EditorGUIUtility.IconContent(EditorIcons.Back2x);
            return new OnlyImageCircleButton(menuAction, section);
        }
    }

    public class OnlyImageCircleButton : CircleButton
    {
        public OnlyImageCircleButton(CircleMenuAction menuAction, int section) : base(menuAction, section)
        {
        }

        protected override void ModifierButton(Button button, CircleMenuAction menuAction, int section)
        {
            var image = new Image
            {
                image = menuAction.Icon.image,
                style =
                {
                    width = 32f,
                    height = 32f,
                    flexShrink = 0
                },
                tooltip = menuAction.ActionName
            };

            button.Add(image);
        }
    }
}