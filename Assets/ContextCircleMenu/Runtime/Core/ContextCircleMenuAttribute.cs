using System;
using UnityEditor;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextCircleMenuAttribute : Attribute
    {
        public readonly GUIContent Icon;
        public readonly string Path;


        public ContextCircleMenuAttribute(string path) : this(path, new GUIContent())
        {
            
        }
        
        public ContextCircleMenuAttribute(string path, string icon) : this(path, EditorGUIUtility.IconContent(icon))
        {
        }

        public ContextCircleMenuAttribute(string path, GUIContent icon)
        {
            Path = path;
            Icon = icon;
        }
    }
}