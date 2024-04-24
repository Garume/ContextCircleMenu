using System;
using UnityEditor;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CircularMenuAttribute : Attribute
    {
        public GUIContent Icon;
        public string Path;


        public CircularMenuAttribute(string path, string icon) : this(path, EditorGUIUtility.IconContent(icon))
        {
        }

        public CircularMenuAttribute(string path, GUIContent icon)
        {
            Path = path;
            Icon = icon;
        }
    }
}