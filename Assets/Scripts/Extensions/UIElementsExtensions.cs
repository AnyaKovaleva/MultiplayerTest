using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Extensions
{
    public static class UIElementsExtensions
    {
        public static void MapFieldToUI<T>(this VisualElement element, out T field, string name = null)
            where T : VisualElement
        {
            field = element.Q<T>(name);
            if (field == null)
                Debug.LogError($"{typeof(T)} {name ?? ""} is null");
        }

        public static T MapFieldToUI<T>(this VisualElement element, string name = null) where T : VisualElement
        {
            T field = element.Q<T>(name);
            if (field == null)
                Debug.LogError($"{typeof(T)} {name ?? ""} is null");

            return field;
        }

        public static readonly string PLACEHOLDER_CLASS_NAME = "__placeholder";

        public static void SetPlaceholderText(this TextField textField, string placeholder,
            bool isPasswordField = false)
        {
            textField.RegisterCallback<FocusInEvent>(evt => ONFocusIn());
            textField.RegisterCallback<FocusOutEvent>(evt => ONFocusOut());

            textField.SetValueWithoutNotify(placeholder);
            textField.AddToClassList(PLACEHOLDER_CLASS_NAME);


            void ONFocusIn()
            {
                if (textField.ClassListContains(PLACEHOLDER_CLASS_NAME))
                {
                    textField.value = string.Empty;
                    textField.RemoveFromClassList(PLACEHOLDER_CLASS_NAME);
                    if (isPasswordField)
                    {
                        textField.isPasswordField = true;
                    }
                }
            }

            void ONFocusOut()
            {
                if (string.IsNullOrEmpty(textField.value))
                {
                    textField.SetValueWithoutNotify(placeholder);
                    textField.AddToClassList(PLACEHOLDER_CLASS_NAME);
                    if (isPasswordField)
                    {
                        textField.isPasswordField = false;
                    }
                }
            }
        }

        private static readonly string INCORRECT_INPUT_STYLE_CLASS_NAME = "incorrect-input";

        public static void SetValueCheck(this TextField textField, Func<string, bool> isValidValueCheck)
        {
            textField.RegisterCallback<FocusInEvent>(evt => ONFocusIn());
            textField.RegisterCallback<FocusOutEvent>(evt => ONFocusOut());


            void ONFocusIn()
            {
                if (textField.ClassListContains(INCORRECT_INPUT_STYLE_CLASS_NAME))
                {
                    textField.RemoveFromClassList(INCORRECT_INPUT_STYLE_CLASS_NAME);
                }
            }

            void ONFocusOut()
            {
                if (!textField.ClassListContains(PLACEHOLDER_CLASS_NAME) && !string.IsNullOrEmpty(textField.value))
                {
                    if (!isValidValueCheck(textField.value))
                    {
                        textField.AddToClassList(INCORRECT_INPUT_STYLE_CLASS_NAME);
                    }
                }
            }
        }

        public static bool CompareToString(this TextField textField, string stringValue)
        {
            if (textField.text == stringValue)
            {
                return true;
            }

            return false;
        }
    }
}