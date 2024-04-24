using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContextCircleMenu.Editor
{
    public sealed class CircularButton : VisualElement
    {
        private readonly Button _button;
        private readonly Color _hoverColor = new(0.2745098f, 0.3764706f, 0.4862745f, 1.0f);
        private readonly Color _normalColor = new(0.02f, 0.02f, 0.02f, 0.8f);
        private readonly Action _onSelect;
        private readonly bool _shouldCloseMenuAfterSelect;
        public bool IsEntered;


        public CircularButton(string text, GUIContent icon, int section, Action onSelect,
            bool shouldCloseMenuAfterSelect = true)
        {
            _onSelect = onSelect;
            _shouldCloseMenuAfterSelect = shouldCloseMenuAfterSelect;

            style.position = Position.Absolute;
            style.alignItems = Align.Center;

            _button = new Button(onSelect)
            {
                style =
                {
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 4,
                    paddingBottom = 4,
                    flexDirection = FlexDirection.Row,
                    borderTopLeftRadius = 4.0f,
                    borderBottomLeftRadius = 4.0f,
                    borderBottomRightRadius = 4.0f,
                    borderTopRightRadius = 4.0f,
                    flexGrow = 1,
                    backgroundColor = _normalColor
                },
                text = ""
            };

            var label = new Label
            {
                style =
                {
                    paddingBottom = 0.0f,
                    paddingLeft = 0.0f,
                    paddingRight = 0.0f,
                    paddingTop = 0.0f,
                    marginLeft = 5.0f,
                    marginRight = 5.0f,
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
                        width = 16.0f,
                        height = 16.0f,
                        flexShrink = 0
                    }
                };
                _button.Add(image);
            }

            _button.Add(label);

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
                _button.Add(index);
            }

            Add(_button);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        internal bool ForceSelect()
        {
            _onSelect?.Invoke();
            return _shouldCloseMenuAfterSelect;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            IsEntered = true;
            _button.style.backgroundColor = _hoverColor;
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            IsEntered = false;
            _button.style.backgroundColor = _normalColor;
        }


        public static CircularButton CreateBackButton(Action back)
        {
            return new CircularButton("Back",
                EditorGUIUtility.IconContent(EditorIcons.Back2x), -1, back, false);
        }
    }
}