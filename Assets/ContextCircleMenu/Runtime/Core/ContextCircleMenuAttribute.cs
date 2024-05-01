using System;

namespace ContextCircleMenu
{
    /// <summary>
    ///     Attribute to add an item to the Context Circle Menu.
    /// </summary>
    /// <remarks>
    ///     Apply this attribute to a method to add it as an item in the Context Circle Menu. The 'path' parameter
    ///     defines the location and name of the item within the menu. Optionally, an 'iconPath' can be specified
    ///     to set an icon for the menu item. Use the 'EditorIcons' static class to specify icons.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ContextCircleMenuAttribute : Attribute
    {
        public readonly string IconPath;
        public readonly string Path;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextCircleMenuAttribute" /> class with the specified path and
        ///     optional icon path.
        /// </summary>
        /// <param name="path">The path under which the menu item will be listed in the Context Circle Menu.</param>
        /// <param name="iconPath">The optional path to the icon for the menu item.</param>
        public ContextCircleMenuAttribute(string path, string iconPath = "")
        {
            Path = path;
            IconPath = iconPath;
        }
    }
}