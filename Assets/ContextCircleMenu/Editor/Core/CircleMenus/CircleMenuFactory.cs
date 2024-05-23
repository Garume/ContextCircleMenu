using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class CircleMenuFactory : ICircleMenuFactory
    {
        private readonly CircleMenuAction _circleMenuAction;

        public CircleMenuFactory(CircleMenuAction circleMenuAction)
        {
            PathSegments = circleMenuAction.Path.Split("/");
            _circleMenuAction = circleMenuAction;
        }

        public IEnumerable<string> PathSegments { get; }

        public CircleMenu Create(IButtonFactory factory)
        {
            return new LeafCircleMenu(_circleMenuAction, factory);
        }
    }

    public class AttributeCircleMenuFactory : ICircleMenuFactory
    {
        private readonly CircleMenuAction _circleMenuAction;

        public AttributeCircleMenuFactory(ContextCircleMenuAttribute attribute, MethodInfo method)
        {
            PathSegments = attribute.Path.Split("/");

            var icon = default(GUIContent);

            if (!string.IsNullOrEmpty(attribute.IconPath))
                icon = EditorGUIUtility.IconContent(attribute.IconPath);

            _circleMenuAction =
                new CircleMenuAction(attribute.Path,
                    action =>
                    {
                        method.Invoke(null, method.GetParameters().Length == 0 ? null : new object[] { action });
                    },
                    icon);
        }

        public IEnumerable<string> PathSegments { get; }

        public CircleMenu Create(IButtonFactory factory)
        {
            return new LeafCircleMenu(_circleMenuAction, factory);
        }
    }
    
    public class FilteredContextCircleMenuFactory : CircleMenuFactory
    {
        public Func<bool> Filter { get; }
        public FilteredContextCircleMenuFactory(CircleMenuAction circleMenuAction, Func<bool> filter) : base(circleMenuAction)
        {
            Filter = filter;
        }
    }
    

    public class FolderMenuFactory : IFolderCircleMenuFactory
    {
        public FolderCircleMenu Create(string path, IMenuControllable menu, CircleMenu parent, IButtonFactory factory)
        {
            return new FolderCircleMenu(path, menu, parent, factory);
        }
    }
}