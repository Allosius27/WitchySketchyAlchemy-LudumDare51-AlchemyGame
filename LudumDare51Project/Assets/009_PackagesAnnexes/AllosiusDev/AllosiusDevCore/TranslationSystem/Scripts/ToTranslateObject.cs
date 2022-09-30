using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AllosiusDevCore.TranslationSystem
{
    public class ToTranslateObject : MonoBehaviour
    {
        #region Fields

        private string translationKey;

        private TexteType texteType;

        #endregion

        #region Properties

        public TextMeshProUGUI TextToTranslate => textToTranslate;

        public string TranslationKey => translationKey;

        public TypeDictionary typeDictionary { get; set; }

        #endregion

        #region UnityInspector

        [SerializeField] private TextMeshProUGUI textToTranslate;

        [SerializeField] private TranslationData translationData;

        #endregion

        #region Behaviour

        private void Awake()
        {
            LoadTranslationData();
        }

        public void LoadTranslationData()
        {
            if (translationData != null)
            {
                translationKey = translationData.translationKey;
                typeDictionary = translationData.typeDictionary;
            }
        }

        void Start()
        {
            LangueManager.Instance.onLangageUpdated += Translation;
            Translation();
        }

        public void SetTranslationKey(string value, TypeDictionary newTypeDictionary, bool automaticTranslation = true, TexteType newTextType = TexteType.Default)
        {
            translationKey = value;
            typeDictionary = newTypeDictionary;
            texteType = newTextType;

            if (automaticTranslation)
                Translation();
        }

        [ContextMenu("Translation")]
        public void Translation()
        {
            if (translationKey == null)
            {
                Debug.LogWarning("translation key is null");
                return;
            }
            string text = GetCorrectText();
            textToTranslate.text = text;

            if(texteType == TexteType.Lower)
            {
                textToTranslate.text = textToTranslate.text.ToLower();
            }
            else if (texteType == TexteType.Upper)
            {
                textToTranslate.text = textToTranslate.text.ToUpper();
            }
        }

        public string GetCorrectText()
        {
            string text = LangueManager.Instance.Translate(translationKey, typeDictionary);
            Debug.Log(text);
            return text;
        }

        private void OnDestroy()
        {
            LangueManager.Instance.onLangageUpdated -= Translation;
        }

        #endregion
    }
}

public enum TexteType { Default, Lower, Upper }
