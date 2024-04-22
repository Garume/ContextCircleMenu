using ContextCircleMenu.Editor;
using UnityEngine;

namespace ContextCircleMenu.Sample
{
    public class Menu
    {
        [CircularMenu("Example/Test", EditorIcons.AnimationRecord)]
        public static void TestMethod()
        {
            Debug.Log("TestMethod");
        }

        [CircularMenu("Example/Test2", EditorIcons.AnimationAnimated)]
        public static void TestMethod2()
        {
            Debug.Log("TestMethod2");
        }
    }
}