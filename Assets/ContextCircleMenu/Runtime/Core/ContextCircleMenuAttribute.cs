using System;

namespace ContextCircleMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ContextCircleMenuAttribute : Attribute
    {
        public readonly string IconPath;
        public readonly string Path;

        public ContextCircleMenuAttribute(string path, string iconPath = "")
        {
            Path = path;
            IconPath = iconPath;
        }
    }
}