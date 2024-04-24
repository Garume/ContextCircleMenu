using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class AttributeCircleMenuFactory : ICircleMenuFactory
    {
        private readonly CircularMenuAttribute _attribute;
        private readonly MethodInfo _method;

        public AttributeCircleMenuFactory(string path, CircularMenuAttribute attribute, MethodInfo method)
        {
            PathSegments = path.Split("/");
            _attribute = attribute;
            _method = method;
        }

        public string[] PathSegments { get; }

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

        public string[] PathSegments { get; }

        public CircleMenu Create()
        {
            return new LeafCircleMenu(PathSegments.Last(), _content, _action);
        }
    }
}