using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class AttributeCircleMenuFactory : ICircleMenuFactory
    {
        private readonly ContextCircleMenuAttribute _attribute;
        private readonly MethodInfo _method;

        public AttributeCircleMenuFactory(string path, ContextCircleMenuAttribute attribute, MethodInfo method)
        {
            PathSegments = path.Split("/");
            _attribute = attribute;
            _method = method;
        }

        public IEnumerable<string> PathSegments { get; }

        public CircleMenu Create()
        {
            return new LeafCircleMenu(PathSegments.Last(), _attribute.Icon, () => _method.Invoke(null, null));
        }
    }

    public class CircleMenuFactory : ICircleMenuFactory
    {
        private readonly Action _action;
        private readonly GUIContent _content;

        public CircleMenuFactory(string path, GUIContent content, Action action)
        {
            PathSegments = path.Split("/");
            _content = content;
            _action = action;
        }

        public IEnumerable<string> PathSegments { get; }

        public CircleMenu Create()
        {
            return new LeafCircleMenu(PathSegments.Last(), _content, _action);
        }
    }

    public class RootMenuFactory : ICircleMenuFactory
    {
        public IEnumerable<string> PathSegments => null;

        public CircleMenu Create()
        {
            return new RootCircleMenu();
        }
    }

    public class FolderMenuFactory : IFolderCircleMenuFactory
    {
        public int Radius { get; set; } = 100;

        public CircleMenu Create(string path, IMenuControllable menu, CircleMenu parent)
        {
            return new FolderCircleMenu(path, menu.Open, menu.Back, parent, Radius);
        }
    }
}