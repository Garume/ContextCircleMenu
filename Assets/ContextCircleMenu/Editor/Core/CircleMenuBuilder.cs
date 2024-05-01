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
        private IFolderCircleMenuFactory _folderFactory;
        private ICircleMenuFactory _rootFactory;

        internal CircleMenu Build(IMenuControllable menu)
        {
            _rootFactory ??= new RootMenuFactory();
            _folderFactory ??= new FolderMenuFactory();

            var root = _rootFactory.Create();
            foreach (var factory in _factories)
            {
                var pathSegments = factory.PathSegments.SkipLast(1);
                var currentMenu = root;
                foreach (var pathSegment in pathSegments)
                {
                    var child = currentMenu.Children.Find(m => m.Path == pathSegment);
                    if (child == null)
                    {
                        child = _folderFactory.Create(pathSegment, menu, currentMenu);
                        currentMenu.Children.Add(child);
                    }

                    currentMenu = child;
                }

                currentMenu.Children.Add(factory.Create());
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
            AddMenu(new CircleMenuFactory(path, content, action));
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
    }
}