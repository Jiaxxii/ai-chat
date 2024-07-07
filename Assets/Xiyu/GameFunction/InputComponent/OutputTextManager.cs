using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.GameFunction.InputComponent
{
    public class OutputTextManager : MonoBehaviour
    {
        private static OutputTextManager _instance;
        public static OutputTextManager Instance => _instance == null ? _instance = FindObjectOfType<OutputTextManager>() : _instance;

        // [Header("组件引用")] [SerializeField] private Image panel;
        [SerializeField] private TextMeshProUGUI outputText;

        [Header("参数")] [SerializeField] [Range(0.01F, 0.5F)]
        private float nextCharacterTime = 0.2F;


        private readonly StringBuilder _builderText = new(256);

        private Coroutine _currentPrintTextCoroutine;

        private Coroutine _loadingCoroutine;


        public void PrintTextAsync(string text, float nextCharTime = 0)
        {
            if (_currentPrintTextCoroutine != null)
            {
                StopCoroutine(_currentPrintTextCoroutine);
            }

            _currentPrintTextCoroutine = StartCoroutine(PrintTextCoroutine(text, nextCharTime));
        }

        public void PrintTextAsync(string text, float startWaitTime, float nextCharTime)
        {
            if (_currentPrintTextCoroutine != null)
            {
                StopCoroutine(_currentPrintTextCoroutine);
            }

            _currentPrintTextCoroutine = StartCoroutine(PrintTextCoroutine(text, startWaitTime, nextCharTime));
        }


        public IEnumerator PrintTextCoroutine(string text, float startWaitTime, float nextCharTime)
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
            }

            outputText.text = string.Empty;
            yield return null;
            yield return new WaitForSeconds(startWaitTime);
            yield return PrintTextCoroutine(text, nextCharTime);
        }

        public IEnumerator PrintTextCoroutine(string text, float nextCharTime = 0)
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
            }

            outputText.text = string.Empty;
            yield return null;
            var waitForSecond = new WaitForSeconds(nextCharTime == 0 ? nextCharacterTime : nextCharTime);
            // const string endText = "<color=red>...”";

            _builderText.Clear().Append('“');

            foreach (var character in text)
            {
                outputText.text = $"{_builderText.Append(character)}”";
                yield return waitForSecond;
            }

            outputText.text = _builderText.Append('”').ToString();
            _currentPrintTextCoroutine = null;
        }


        public void LoadingAsync(Func<bool> exit, string loadText = "………………", float nextCharTime = 0)
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
            }

            _loadingCoroutine = StartCoroutine(LoadingCoroutine(exit, loadText, nextCharTime));
        }

        private IEnumerator LoadingCoroutine(Func<bool> exit, string loadText = "………………", float nextCharTime = 0)
        {
            outputText.text = string.Empty;
            yield return null;
            var waitForSecond = new WaitForSeconds(nextCharTime == 0 ? nextCharacterTime : nextCharTime);

            _builderText.Clear();
            while (!exit.Invoke())
            {
                foreach (var character in loadText)
                {
                    outputText.text = $"{_builderText.Append(character)}";
                    yield return waitForSecond;
                }

                outputText.text = _builderText.Clear().ToString();
            }

            outputText.text = string.Empty;
            _loadingCoroutine = null;
        }
    }
}