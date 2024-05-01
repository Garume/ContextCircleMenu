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

        public void AddMenu(ContextCircleMenuAttribute attribute, MethodInfo method)
        {
            AddMenu(new AttributeCircleMenuFactory(attribute, method));
        }

        public void AddMenu(string path, GUIContent content, Action action)
        {
            AddMenu(new CircleMenuFactory(path, content, action));
        }

        public void AddMenu(ICircleMenuFactory factory)
        {
            _factories.Add(factory);
        }

        public void ConfigureFolder(IFolderCircleMenuFactory factory)
        {
            _folderFactory = factory;
        }
    }
}