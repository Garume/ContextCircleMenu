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

        internal CircleMenu Build(Action<CircleMenu> open)
        {
            CircleMenu root = new RootCircleMenu();
            foreach (var factory in _factories)
            {
                var pathSegments = factory.PathSegments.SkipLast(1);
                var currentMenu = root;
                foreach (var pathSegment in pathSegments)
                {
                    var child = currentMenu.Children.Find(m => m.Path == pathSegment);
                    if (child == null)
                    {
                        child = new FolderCircleMenu(pathSegment,
                            () => open(child), () => open(child.Parent),
                            currentMenu);
                        currentMenu.Children.Add(child);
                    }

                    currentMenu = child;
                }

                currentMenu.Children.Add(factory.Create());
            }

            return root;
        }

        public void AddMenu(string attributePath, CircularMenuAttribute attribute, MethodInfo method)
        {
            _factories.Add(new AttributeCircleMenuFactory(attributePath, attribute, method));
        }

        public void AddMenu(string path, GUIContent content, Action action)
        {
            _factories.Add(new CircleMenuFactory(path, content, action));
        }

        public void AddMenu(ICircleMenuFactory factory)
        {
            _factories.Add(factory);
        }
    }
}