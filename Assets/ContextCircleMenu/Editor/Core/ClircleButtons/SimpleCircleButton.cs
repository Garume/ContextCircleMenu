using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public class SimpleCircleButton : CircleButton
    {
        private readonly Color _hoverColor = new(0.2745098f, 0.3764706f, 0.4862745f, 1.0f);
        private readonly Color _normalColor = new(0.02f, 0.02f, 0.02f, 0.8f);

        public SimpleCircleButton(string text, GUIContent icon, int section, Action onSelect,
            bool shouldCloseMenuAfterSelect = true) : base(text, icon, section, onSelect, shouldCloseMenuAfterSelect)
        {
        }

        protected override void ModifierButton(Button button, string text, GUIContent icon, int section)
        {
            button.style.paddingLeft = 8f;
            button.style.paddingRight = 8f;
            button.style.paddingTop = 4f;
            button.style.paddingBottom = 4f;
            button.style.flexDirection = FlexDirection.Row;
            button.style.borderTopLeftRadius = 4f;
            button.style.borderBottomLeftRadius = 4f;
            button.style.borderBottomRightRadius = 4f;
            button.style.borderTopRightRadius = 4f;
            button.style.flexGrow = 1;
            button.style.backgroundColor = _normalColor;
            button.text = "";

            var label = new Label
            {
                style =
                {
                    paddingBottom = 0f,
                    paddingLeft = 0f,
                    paddingRight = 0f,
                    paddingTop = 0f,
                    marginLeft = 5f,
                    marginRight = 5f,
                    flexGrow = 1
                },
                text = text
            };

            if (icon != null)
            {
                var image = new Image
                {
                    image = icon.image,
                    style =
                    {
                        width = 16f,
                        height = 16f,
                        flexShrink = 0
                    }
                };
                button.Add(image);
            }

            button.Add(label);

            if (section != -1)
            {
                var index = new Label
                {
                    text = section.ToString(),
                    style =
                    {
                        color = new Color(0.7f, 0.7f, 0.7f, 1.0f),
                        unityFontStyleAndWeight = FontStyle.Italic
                    }
                };
                button.Add(index);
            }
        }

        protected override void OnMouseEnter(Button button, MouseEnterEvent evt)
        {
            button.style.backgroundColor = _hoverColor;
        }

        protected override void OnMouseLeave(Button button, MouseLeaveEvent evt)
        {
            button.style.backgroundColor = _normalColor;
        }
    }
}