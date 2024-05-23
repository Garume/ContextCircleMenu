using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public sealed class CircleMenuBuilder
    {
        private readonly List<ICircleMenuFactory> _factories = new();
        private IButtonFactory _buttonFactory;
        private IFolderCircleMenuFactory _folderFactory;

        internal CircleMenu Build(IMenuControllable menu)
        {
            _folderFactory ??= new FolderMenuFactory();
            _buttonFactory ??= new ButtonFactory();

            CircleMenu root = _folderFactory.Create(string.Empty, menu, null, _buttonFactory);
            foreach (var factory in _factories)
            {
                var pathSegments = factory.PathSegments.SkipLast(1);
                var currentMenu = root;
                foreach (var pathSegment in pathSegments)
                {
                    var child = currentMenu.Children.Find(m => m.MenuAction.Path == pathSegment);
                    if (child == null)
                    {
                        child = _folderFactory.Create(pathSegment, menu, currentMenu, _buttonFactory);
                        var backMenuAction = new CircleMenuAction(pathSegment, _ => menu.Back(),
                            _ => CircleMenuAction.Status.Normal, EditorGUIUtility.IconContent(EditorIcons.Back2x));
                        var backButton = _buttonFactory.CreateBackButton(backMenuAction, -1);
                        backButton.ShouldCloseMenuAfterSelection = false;
                        child.PrepareButton(backButton);
                        currentMenu.Children.Add(child);
                    }

                    currentMenu = child;
                }

                currentMenu.Children.Add(factory.Create(_buttonFactory));
            }

            return root;
        }

        /// <summary>
        ///     Adds a menu from attribute.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="method"></param>
        public void AddMenu(ContextCircleMenuAttribute attribute, MethodInfo method)
        {
            AddMenu(new AttributeCircleMenuFactory(attribute, method));
        }

        /// <summary>
        ///     Adds a menu manually.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="action"></param>
        public void AddMenu(string path, GUIContent content, Action action)
        {
            var circleMenuAction = new CircleMenuAction(path, _ => action(), content);
            AddMenu(new CircleMenuFactory(circleMenuAction));
        }

        public void AddMenu(string path, Action<CircleMenuEventInformation> action,
            Func<CircleMenuEventInformation, CircleMenuAction.Status> statusCallback = null,
            GUIContent content = null)
        {
            var circleMenuAction = new CircleMenuAction(path, action, statusCallback, content);
            AddMenu(new CircleMenuFactory(circleMenuAction));
        }
        }

        /// <summary>
        ///     Adds a factory to the list of menu item factories.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the menu item.</param>
        public void AddMenu(ICircleMenuFactory factory)
        {
            _factories.Add(factory);
        }

        /// <summary>
        ///     Sets a custom factory for creating folder-like menu items, allowing for further customization of menu folders.
        /// </summary>
        /// <param name="factory">The factory to use for creating folder menu items.</param>
        public void ConfigureFolder(IFolderCircleMenuFactory factory)
        {
            _folderFactory = factory;
        }

        /// <summary>
        ///     Sets a custom factory for creating menu buttons, allowing for further customization of menu buttons.
        /// </summary>
        /// <param name="factory">The factory to use for creating menu buttons.</param>
        public void ConfigureButton(IButtonFactory factory)
        {
            _buttonFactory = factory;
        }
    }
}