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

        [CircularMenu("Example/Test3", EditorIcons.AnimationRecord)]
        public static void TestMethod3()
        {
            Debug.Log("TestMethod3");
        }

        [CircularMenu("Example/Test4", EditorIcons.AnimationRecord)]
        public static void TestMethod4()
        {
            Debug.Log("TestMethod4");
        }

        [CircularMenu("Example/Test5", EditorIcons.AnimationRecord)]
        public static void TestMethod5()
        {
            Debug.Log("TestMethod5");
        }


        [CircularMenu("Example/Scene/Test", EditorIcons.AnimationRecord)]
        public static void TestMethod6()
        {
            Debug.Log("TestMethod6");
        }
    }
}