using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AllosiusDevCore.Audio;
using AllosiusDevCore.TranslationSystem;
using AllosiusDevUtilities.Audio;

namespace AllosiusDevCore.DialogSystem
{
    public class DialogueDisplayUI : MonoBehaviour
    {
        #region Fields

        private RectTransform ThisRect;

        private PlayerConversant playerConversant;

        private List<GameObject> currentChoices = new List<GameObject>();

        private Coroutine writeTextCoroutine;
        private Coroutine animationPassTouchCoroutine;

        private bool canTurnNext = true;

        private bool updateButtons;
        private List<Button> currentChoicesButtons = new List<Button>();
        #endregion

        #region Properties

        public TextMeshProUGUI DialogueDisplayerText => dialogueDisplayerText;

        public List<GameObject> CurrentChoices => currentChoices;

        public Button NextButton => nextButton;

        public bool canInteract { get; set; }

        #endregion

        #region UnityInspector

        [SerializeField] private TextMeshProUGUI dialogueDisplayerText;
        [SerializeField] private RectTransform passTouch;

        [SerializeField] private TextMeshProUGUI dialogueNpcNameText;

        [SerializeField] private Transform choicesRoot;
        [SerializeField] private GameObject choicePrefab;

        [SerializeField] private Button nextButton;

        [SerializeField] private Color32 QuestionClassicColor;
        [SerializeField] private Color32 QuestionQuestColor;

        [SerializeField] private float delayAnimationText = 0.025f;

        [Header("Sounds")]
        [SerializeField] private AudioData sfxTextWrite;
        [SerializeField] private AudioData sfxValidate;

        #endregion

        #region Behaviour

        private void Awake()
        {
            ThisRect = gameObject.GetComponent<RectTransform>();

            canInteract = true;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void LateUpdate()
        {
            UpdateDialogueChoicesButtons();
        }


        private void UpdateDialogueChoicesButtons()
        {
            if (currentChoicesButtons.Count > 0 && updateButtons)
            {
                float HeightFinalBox = 30f;

                foreach (Button button in currentChoicesButtons)
                {
                    RectTransform hoverElementRect = button.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>();

                    button.GetComponent<RectTransform>().sizeDelta += hoverElementRect.sizeDelta;

                    HeightFinalBox += button.GetComponent<RectTransform>().sizeDelta.y;
                    ThisRect.sizeDelta = new Vector2(ThisRect.sizeDelta.x, HeightFinalBox);
                }
            }
        }

        private void UpdateButtonsListeners()
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => ButtonNext());
        }

        public void UpdateUI()
        {
            UpdateButtonsListeners();
            updateButtons = false;

            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }

            ThisRect.sizeDelta = new Vector2(ThisRect.sizeDelta.x, 96f);

            dialogueNpcNameText.text = playerConversant.GetCurrentConversantName();
            SetNPCNameWidth(playerConversant.GetCurrentConversantName());

            dialogueDisplayerText.gameObject.SetActive(!playerConversant.IsChoosing());
            choicesRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (writeTextCoroutine != null)
            {
                StopCoroutine(writeTextCoroutine);
            }

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                writeTextCoroutine = StartCoroutine(WriteText(playerConversant.GetKeyText(false)));

                UICanvasManager.Instance.EventSystem.SetSelectedGameObject(nextButton.gameObject);
            }

        }

        private void SetNPCNameWidth(string Name)
        {
            int NumOfChar = Name.Length;
            dialogueNpcNameText.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2((NumOfChar * 14f) + 10f, dialogueNpcNameText.transform.parent.GetComponent<RectTransform>().sizeDelta.y);
        }



        private void BuildChoiceList()
        {
            foreach (Transform item in choicesRoot)
            {
                Destroy(item.gameObject);
            }

            currentChoices.Clear();
            currentChoicesButtons.Clear();



            foreach (DialogueTextNode choice in playerConversant.GetChoices())
            {
                GameObject _choiceInstance = Instantiate(choicePrefab, choicesRoot);
                currentChoices.Add(_choiceInstance);

                var _textComp = _choiceInstance.GetComponentInChildren<TextMeshProUGUI>();

                var _textTranslate = _choiceInstance.GetComponentInChildren<ToTranslateObject>();
                _textTranslate.SetTranslationKey(choice.keyText, TypeDictionary.Dialogues, true, choice.texteType);

                if (choice.hasRequirements)
                {
                    for (int i = 0; i < choice.gameRequirements.requirementsList.Count; i++)
                    {
                        if (choice.gameRequirements.requirementsList[i].requirementType == RequirementType.HasQuest
                            || choice.gameRequirements.requirementsList[i].requirementType == RequirementType.QuestState)
                        {
                            _textComp.color = QuestionQuestColor;
                        }
                    }
                }
                else
                {
                    _textComp.color = QuestionClassicColor;
                }

                Button button = _choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    ButtonSelectChoice(choice);
                });

                currentChoicesButtons.Add(button);
            }

            updateButtons = true;

            for (int i = 0; i < currentChoices.Count; i++)
            {
                Button button = currentChoices[i].GetComponent<Button>();

                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnUp = button.navigation.selectOnUp;
                newNav.selectOnDown = button.navigation.selectOnDown;
                newNav.selectOnLeft = button.navigation.selectOnLeft;
                newNav.selectOnRight = button.navigation.selectOnRight;

                if (i > 0)
                {
                    newNav.selectOnUp = currentChoices[i - 1].GetComponent<Selectable>();
                }

                if (i < currentChoices.Count - 1)
                {
                    newNav.selectOnDown = currentChoices[i + 1].GetComponent<Selectable>();
                }

                button.navigation = newNav;

            }

            UICanvasManager.Instance.EventSystem.SetSelectedGameObject(currentChoices[0]);
        }

        private void ButtonNext()
        {
            if(canInteract)
            {
                if (canTurnNext)
                {
                    AudioController.Instance.PlayAudio(sfxValidate);
                    playerConversant.Next();

                    if (animationPassTouchCoroutine != null)
                    {
                        StopCoroutine(animationPassTouchCoroutine);
                    }
                    passTouch.gameObject.SetActive(false);
                }
                else
                {
                    canTurnNext = true;

                    if (writeTextCoroutine != null)
                    {
                        StopCoroutine(writeTextCoroutine);

                        EndTextWrite(playerConversant.GetKeyText());
                    }
                }
            }
        }

        private void ButtonSelectChoice(DialogueTextNode choice)
        {
            AudioController.Instance.PlayAudio(sfxValidate);
            playerConversant.SelectChoice(choice);
        }

        private IEnumerator WriteText(string text)
        {
            canTurnNext = false;

            string _text = text;

            dialogueDisplayerText.text = "";

            for (int i = 0; i < _text.Length; i++)
            {
                dialogueDisplayerText.text += _text[i];

                if (_text[i] != ' ')
                {
                    AudioController.Instance.PlayAudio(sfxTextWrite);
                }

                yield return new WaitForSeconds(delayAnimationText);
            }

            EndTextWrite(playerConversant.GetKeyText());
        }

        private void EndTextWrite(string text)
        {
            dialogueDisplayerText.text = text;

            canTurnNext = true;

            if (animationPassTouchCoroutine != null)
            {
                StopCoroutine(animationPassTouchCoroutine);
            }
            animationPassTouchCoroutine = StartCoroutine(AnimationPassText());
        }

        IEnumerator AnimationPassText()
        {
            if (canTurnNext)
            {
                passTouch.gameObject.SetActive(true);

                //passTouch.anchoredPosition = new Vector2(passTouch.anchoredPosition.x, passTouch.anchoredPosition.y + 2.5f);
                yield return new WaitForSeconds(0.5f);
                //passTouch.anchoredPosition = new Vector2(passTouch.anchoredPosition.x, passTouch.anchoredPosition.y - 2.5f);
                yield return new WaitForSeconds(0.5f);

                StartCoroutine(AnimationPassText());
            }
            else
            {
                passTouch.gameObject.SetActive(false);
            }


        }

        #endregion

    }
}
