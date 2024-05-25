using UnityEditor;

namespace ContextCircleMenu.Editor
{
    public class ButtonFactory : IButtonFactory
    {
        public CircleButton Create(CircleMenuAction menuAction, int section)
        {
            return new SimpleCircleButton(menuAction, section);
        }

        public CircleButton CreateBackButton(CircleMenuAction menuAction, int section)
        {
            menuAction.ActionName = "Back";
            menuAction.Icon = EditorGUIUtility.IconContent(EditorIcons.Back2x);
            return new SimpleCircleButton(menuAction, section);
        }
    }
}