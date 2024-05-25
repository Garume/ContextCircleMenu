using System;
using UnityEngine;

namespace ContextCircleMenu.Editor
{
    public class CircleMenuAction
    {
        [Flags]
        public enum Status
        {
            Normal,
            Disabled
        }

        private readonly Func<CircleMenuEventInformation, Status> _actionStatusCallback;

        private CircleMenuEventInformation _information;

        public CircleMenuAction(string path, Action<CircleMenuEventInformation> action,
            Func<CircleMenuEventInformation, Status> status = null, GUIContent icon = null)
        {
            Path = path;
            ActionName = path.Split("/")[^1];
            ActionCallback = action;
            _actionStatusCallback = status;
            Icon = icon;
        }

        public CircleMenuAction(string path, Action<CircleMenuEventInformation> action, GUIContent icon)
            : this(path, action, AlwaysEnabled, icon)
        {
        }

        public string Path { get; }
        public string ActionName { get; set; }
        public Action<CircleMenuEventInformation> ActionCallback { get; internal set; }
        public GUIContent Icon { get; set; }
        public Status CurrentStatus { get; private set; }

        public void UpdateStatus(CircleMenuEventInformation information)
        {
            _information = information;
            CurrentStatus = _actionStatusCallback?.Invoke(information) ?? Status.Disabled;
        }

        public void Execute()
        {
            if (CurrentStatus == Status.Disabled)
                return;
            ActionCallback?.Invoke(_information);
        }

        public static Status AlwaysEnabled(CircleMenuEventInformation _)
        {
            return Status.Normal;
        }

        public static Status AlwaysDisabled(CircleMenuEventInformation _)
        {
            return Status.Disabled;
        }
    }
}