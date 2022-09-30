using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AllosiusDevCore.TranslationSystem
{
    public class ToTranslateTextArea : MonoBehaviour
    {
        #region Properties

        public TextMeshProUGUI TextToTranslate => textToTranslate;

        #endregion

        #region UnityInspector

        [SerializeField] private TextMeshProUGUI textToTranslate;

        [TextArea(5, 10)] [SerializeField] private string translationKey;

        [SerializeField] public TypeDictionary typeDictionary;


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
                translationKey = translationData.translationAreaKey;
                typeDictionary = translationData.typeDictionary;
            }
        }

        void Start()
        {
            LangueManager.Instance.onLangageUpdated += Translation;
            //Translation();
        }

        public void SetTranslationKey(string value, TypeDictionary newTypeDictionary, bool automaticTranslation = true)
        {
            translationKey = value;
            typeDictionary = newTypeDictionary;

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
        }

        public string GetCorrectText()
        {
            List<string> keys = new List<string>();
            string temp = "";

            string text = translationKey;
            text = text.Replace("{", "");
            text = text.Replace("}", "");

            for (int i = 0; i < translationKey.Length; i++)
            {
                if (temp.Length >= 1)
                {
                    temp += translationKey[i];

                    if (translationKey[i] == '}')
                    {
                        temp = temp.Remove(0, 1);
                        temp = temp.Remove(temp.Length - 1, 1);
                        keys.Add(temp);
                        temp = "";
                    }
                }

                if (translationKey[i] == '{')
                {
                    temp = "";
                    temp += translationKey[i];
                }

            }

            for (int i = 0; i < keys.Count; i++)
            {
                //Debug.Log(keys[i]);
                string translation = LangueManager.Instance.Translate(keys[i], typeDictionary);
                //Debug.Log(translation);
                text = text.Replace(keys[i], translation);
            }

            //Debug.Log(text);
            return text;
        }

        private void OnDestroy()
        {
            LangueManager.Instance.onLangageUpdated -= Translation;
        }

        #endregion
    }
}
