using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AllosiusDevUtilities;
using System;
using AllosiusDevUtilities.Utils;

namespace AllosiusDevCore.TranslationSystem
{
    public class LangueManager : Singleton<LangueManager>
    {
        #region Events

        public event Action onLangageUpdated;

        #endregion

        #region UnityInspector

        [SerializeField] private LocalisationManager.Langage startLangage;

        #endregion

        #region Behaviour

        protected override void Awake()
        {
            base.Awake();

            ChangeLangage(startLangage);
        }

        public LocalisationManager.Langage GetCurrentLangage()
        {
            return LocalisationManager.currentLangage;
        }

        public void ChangeLangage(LocalisationManager.Langage newLangage)
        {
            LocalisationManager.SetCurrentLangage(newLangage);
            Debug.Log(LocalisationManager.currentLangage);
            if (onLangageUpdated != null)
                onLangageUpdated();
        }

        public string Translate(string key, TypeDictionary typeDictionary, bool colorCode = true)
        {
            string translatedText = LocalisationManager.GetLocalisedValue(key, typeDictionary);

            /*if (translatedText.Contains("[PLAYER]"))
            {
                translatedText = translatedText.Replace("[PLAYER]", GameManager.Instance.player.PlayerName);
            }*/
            

            return translatedText;
        }

        #endregion
    }
}
