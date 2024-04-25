using System;
using System.Collections.Generic;

namespace ContextCircleMenu.Editor
{
    public interface ICircleMenuFactory
    {
        public IEnumerable<string> PathSegments { get; }
        public CircleMenu Create();
    }

    public interface IFolderCircleMenuFactory
    {
        public CircleMenu Create(string path, Action onSelected, Action onBack, CircleMenu parent);
    }
}