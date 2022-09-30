using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.TranslationSystem
{
    [CreateAssetMenu(fileName = "New TranslationData", menuName = "AllosiusDev/Translation Data")]
    public class TranslationData : ScriptableObject
    {
        #region UnityInspector

        [Tooltip("To be used to translate a simple text using its translation key (Function with ToTranslateObject Script)")]
        [SerializeField] public string translationKey;

        [Tooltip("To be used to translate groups of texts by putting each translation key between { } (Function with ToTranslateTextArea Script)")]
        [TextArea(5, 10)] [SerializeField] public string translationAreaKey;

        [Space]

        [SerializeField] public TypeDictionary typeDictionary;

        #endregion
    }
}
