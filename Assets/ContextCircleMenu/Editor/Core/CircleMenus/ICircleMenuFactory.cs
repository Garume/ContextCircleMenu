using System.Collections.Generic;

namespace ContextCircleMenu.Editor
{
    public interface ICircleMenuFactory
    {
        public IEnumerable<string> PathSegments { get; }
        public CircleMenu Create(IButtonFactory factory);
    }

    public interface IFolderCircleMenuFactory
    {
        public FolderCircleMenu Create(string path, IMenuControllable menu, CircleMenu parent, IButtonFactory factory);
    }
}