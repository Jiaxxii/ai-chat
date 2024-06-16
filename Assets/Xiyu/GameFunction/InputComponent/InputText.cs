using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Xiyu.GameFunction.InputComponent
{
    public class InputText : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private TMP_InputField inputField;

        private static InputText _instance;
        public static InputText Instance => _instance == null ? _instance = FindObjectOfType<InputText>() : _instance;


        public InputText Active(bool active)
        {
            content.SetActive(active);
            return this;
        }


        public InputText SetFocus()
        {
            inputField.ActivateInputField();
            return this;
        }

        public InputText OnSubmitEventHandler(UnityAction<string> onSubmit)
        {
            inputField.onSubmit.AddListener(onSubmit);
            return this;
        }

        public InputText RemoveSubmitEventHandler(UnityAction<string> onSubmit)
        {
            inputField.onSubmit.RemoveListener(onSubmit);
            return this;
        }

        public InputText OnValueChangedEventHandler(UnityAction<string> onValueChanged)
        {
            inputField.onValueChanged.AddListener(onValueChanged);
            return this;
        }

        public InputText RemoveValueChangedEventHandler(UnityAction<string> onValueChanged)
        {
            inputField.onValueChanged.RemoveListener(onValueChanged);
            return this;
        }
    }
}