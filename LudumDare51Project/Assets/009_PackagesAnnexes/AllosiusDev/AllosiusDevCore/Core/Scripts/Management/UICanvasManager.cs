using AllosiusDevCore.DialogSystem;
using AllosiusDevUtilities.Core.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AllosiusDevCore
{
    public class UICanvasManager : AllosiusDevUtilities.Singleton<UICanvasManager>
    {
        #region Fields

        private GameObject eventSystemCurrentObjectSelected;

        #endregion

        #region Properties

        public EventSystem EventSystem => eventSystem;

        public PageController PageController => pageController;

        public SettingsMenu SettingsMenu => settingsMenu;

        public PauseMenu PauseMenu => pauseMenu;

        public DialogueDisplayUI DialogUi => dialogUi;

        #endregion

        #region UnityInspector

        [SerializeField] private EventSystem eventSystem;

        [SerializeField] private PageController pageController;

        [SerializeField] private SettingsMenu settingsMenu;

        [SerializeField] private PauseMenu pauseMenu;

        [SerializeField] private DialogueDisplayUI dialogUi;

        #endregion

        #region Behaviour

        private void Update()
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                eventSystemCurrentObjectSelected = eventSystem.currentSelectedGameObject;
            }

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (eventSystemCurrentObjectSelected != null && eventSystem.currentSelectedGameObject == null)
                {
                    eventSystem.SetSelectedGameObject(eventSystemCurrentObjectSelected);
                }
            }
        }

        #endregion
    }
}

