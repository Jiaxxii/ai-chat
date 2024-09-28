using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Xiyu.Constant;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    public class WebsiteBox : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        public event UnityAction<Uri> OnWebsiteSubmit;


        public List<Uri> History { get; private set; }

        public Uri CurrentUrl => History.Last() ?? new Uri(GameConstant.DefaultNavigationPage);


        private void Start()
        {
            inputField.onSubmit.AddListener(url => OnWebsiteSubmitEventHandler(new Uri(url)));
        }

        public void SetWebsite(string url, bool triggerCallback = false) => SetWebsite(new Uri(url), triggerCallback);

        public void SetWebsite(Uri uri, bool triggerCallback = false)
        {
            inputField.text = uri.AbsoluteUri;
            if (triggerCallback)
                OnWebsiteSubmitEventHandler(uri);
        }


        private void OnWebsiteSubmitEventHandler(Uri uri)
        {
            History.Add(uri);
            OnWebsiteSubmit?.Invoke(uri);
        }
    }
}