using System;
using UnityEngine;

namespace ContextCircleMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextCircleMenuAttribute : Attribute
    {
        public readonly GUIContent Icon;
        public readonly string Path;
        public readonly string IconPath;


        public ContextCircleMenuAttribute(string path) : this(path, new GUIContent())
        {
            
        }
        
        public ContextCircleMenuAttribute(string path, string iconPath) : this(path)
        {
            IconPath = iconPath;
        }

        public ContextCircleMenuAttribute(string path, GUIContent icon)
        {
            Path = path;
            Icon = icon;
        }
    }
}