using AllosiusDevCore.TranslationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace AllosiusDevCore.DialogSystem
{
    public class PlayerConversant : MonoBehaviour
    {
        #region Fields

        DialogueGraph currentDialog;

        NpcConversant currentConversant = null;

        private bool isChoosing = false;
        private bool canNext = true;

        private List<Node> _currentStartNodes = new List<Node>();

        #endregion

        #region Properties

        public bool canConvers { get; set; }

        public bool canDialog { get; set; }

        public DialogueTextNode currentNode { get; set; }

        public DialogueGraph CurrentDialog => currentDialog;

        public NpcConversant CurrentConversant => currentConversant;

        #endregion

        #region Events

        public event Action onConversationUpdated;

        #endregion

        #region Behaviour

        private void Awake()
        {
            onConversationUpdated += UICanvasManager.Instance.DialogUi.UpdateUI;

            canDialog = true;

            canConvers = true;
        }

        /// <summary>
        /// Fonction permettant de vérifier si le joueur est actuellement en train de lire un dialogue
        /// </summary>
        /// <returns>vrai si il y a un dialogue et faux dans le cas contraire</returns>
        public bool IsActive()
        {
            return currentDialog != null;
        }

        /// <summary>
        /// Fonction permettant de récupérer le nom du Npc auquel le joueur est en train de parler
        /// </summary>
        /// <returns>nom du Npc actuellement sélectionné</returns>
        public string GetCurrentConversantName()
        {
            return currentConversant.NpcData.nameNpc;
        }

        /// <summary>
        /// Fonction permettant de vérifier si le dialogue est actuellement à une étape de choix offert au joueur
        /// </summary>
        /// <returns>vrai si le node actuel est un choix, faux sinon</returns>
        public bool IsChoosing()
        {
            return isChoosing;
        }

        /// <summary>
        /// Fonction permettant de récupérer le texte traduit correctement du node actuellement lu dans le dialogue (bool colorCode : variable servant à déterminer si on veut que les codes de couleur colorisant certains mots spécifiques des textes, soient lus ou ignorés (par défaut, ils sont lus))
        /// </summary>
        /// <returns>texte traduit</returns>
        public string GetKeyText(bool colorCode = true)
        {
            if (currentNode == null)
            {
                return "";
            }

            string playerMessage = LangueManager.Instance.Translate(currentNode.keyText, TypeDictionary.Dialogues, colorCode);

            string _text = playerMessage;
            if (currentNode.texteType == TexteType.Lower)
            {
                _text = playerMessage.ToLower();
            }
            else if (currentNode.texteType == TexteType.Upper)
            {
                _text = playerMessage.ToUpper();
            }

            return _text;
        }

        /// <summary>
        /// Fonction permettant de récupérer la liste des choix proposés au joueur dans le node actuel
        /// </summary>
        /// <returns>liste des choix du node actuellement lu</returns>
        public IEnumerable<DialogueTextNode> GetChoices()
        {
            return currentDialog.GetPlayerChoisingChildren(currentNode);
        }

        /// <summary>
        /// Fonction permettant de vérifier si il existe un node suivant celui actuellement lu, utilisé principalement pour savoir quand le dialogue est terminé et doit s'arrêter
        /// </summary>
        /// <returns>vrai si il y a une suite au node actuel, faux sinon</returns>
        public bool HasNext()
        {
            return currentDialog.GetAllChildren(currentNode).Count() > 0;
        }

        /// <summary>
        /// Fonction permettant de lancer le dialogue à l'intéraction avec un NPC par exemple, nécessite une référence vers le script du NPC avec lequel on interagit, ainsi que le data du dialogue à lire
        /// </summary>
        public void StartDialog(NpcConversant conversant, DialogueGraph dialogue)
        {
            Debug.Log("Start Dialog");

            canNext = true;

            currentDialog = dialogue;
            Debug.Log(currentDialog.name);

            if (conversant != null)
            {
                currentConversant = conversant;
                Debug.Log(currentConversant.name);
            }

            _currentStartNodes.Clear();

            for (int i = 0; i < currentDialog.nodes.Count; i++)
            {
                Debug.Log(currentDialog.nodes[i].name);
                if (currentDialog.nodes[i].GetInputsPorts().Count == 0)
                {
                    _currentStartNodes.Add(currentDialog.nodes[i]);
                }
            }

            SetNewCurrentNode();

            if(_currentStartNodes.Count <= 0)
            {
                currentNode = (DialogueTextNode)currentDialog.GetRootNode();
            }
        }

        /// <summary>
        /// Fonction spécifique à l'initialisation permettant de récupérer la liste des choix proposés au joueur au node de départ si il choisit d'un choix
        /// </summary>
        /// <returns>liste des choix du node de départ</returns>
        public IEnumerable<DialogueTextNode> GetPlayerChoisingChildren()
        {
            //Debug.Log("Get Player Choicsing Children");

            foreach (DialogueTextNode node in _currentStartNodes)
            {
                //Debug.Log(node.name);

                if (node.identityType == DialogueTextNode.IdentityType.Player)
                {
                    if (node.singleRead == false)
                    {
                        yield return node;
                    }
                    else if (node.singleRead && node.GetAlreadyRead() == false)
                    {
                        yield return node;
                    }

                }
            }
        }

        /// <summary>
        /// Fonction permettant de récupérer la liste des nodes possibles à affecter au NPC au lancement du dialogue
        /// </summary>
        /// <returns>liste des nodes de départ potentiels du dialogue du NPC actuellement lu</returns>
        public IEnumerable<DialogueTextNode> GetAiChildren()
        {
            //Debug.Log("Get AI Children");

            foreach (DialogueTextNode node in _currentStartNodes)
            {
                //Debug.Log(node.name);

                DialogueTextNode nodeChecked = currentDialog.GetRequiredNodes(node);
                if (nodeChecked != null)
                {
                    if (node.identityType == DialogueTextNode.IdentityType.NPC)
                    {
                        if (node.singleRead == false)
                        {
                            yield return node;
                        }
                        else if (node.singleRead && node.GetAlreadyRead() == false)
                        {
                            yield return node;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Fonction permettant de définir le choix sélectionné par le joueur en jeu comme étant le nouveau node avant de passer à la suite
        /// </summary>
        public void SelectChoice(DialogueTextNode chosenNode)
        {
            //Debug.Log("SelectChoice");
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        /// <summary>
        /// Fonction permettant de passer au node de dialogue suivant le node actuellement lu
        /// </summary>
        public void Next()
        {
            if(canNext)
                StartCoroutine(NextCoroutine());
        }


        public IEnumerator NextCoroutine()
        {
            Debug.Log("Next " + currentNode.name);

            canNext = false;

            if (canDialog == false)
            {
                yield break;
            }

            if (currentNode.hasGameActions)
            {
                //Debug.Log("Execute Game Actions");
                currentNode.gameActions.ExecuteGameActions();
            }

            yield return new WaitForSeconds(currentNode.timerBeforeNextNode);


            canDialog = true;

            currentNode.SetAlreadyReadValue(true);

            if (!HasNext())
            {
                //Debug.Log("Quit Dialog");
                Quit();
                yield break;
            }

            SetNewCurrentNode();
            canNext = true;

            //Debug.Log(currentNode.name);
        }

        /// <summary>
        /// Fonction permettant de définir le nouveau node actif selon l'état du dialogue (initialisation, choix, texte de NPC, etc...)
        /// </summary>
        private void SetNewCurrentNode()
        {
            int numPlayerResponses = 0;

            if (currentNode == null)
            {
                numPlayerResponses = GetPlayerChoisingChildren().Count();
                Debug.Log(numPlayerResponses);
            }
            else
            {
                numPlayerResponses = currentDialog.GetPlayerChoisingChildren(currentNode).Count();
                Debug.Log(numPlayerResponses);
            }

            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                onConversationUpdated.Invoke();
                return;
            }

            DialogueTextNode[] children = null;

            if (currentNode == null)
            {
                //Debug.Log("Init Node");
                children = GetAiChildren().ToArray();
                Debug.Log(children[0].name);
            }
            else
            {
                //Debug.Log("New Current Node");
                children = currentDialog.GetAiChildren(currentNode).ToArray();
                Debug.Log(children[0].name);
            }

            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            Debug.Log(randomIndex);

            currentNode = children[randomIndex];

            StartCoroutine(CoroutineEnterNodeActions());
        }

        /// <summary>
        /// Fonction permettant de lancer les game actions à jouer à l'apparition du node, si il en a dans son data
        /// </summary>
        private IEnumerator CoroutineEnterNodeActions()
        {
            if (currentNode.hasEnterNodeActions)
            {
                currentNode.enterNodeActions.ExecuteGameActions();
            }

            yield return new WaitForSeconds(currentNode.timerBeforeNextNode);

            onConversationUpdated.Invoke();
        }

        /// <summary>
        /// Fonction permettant de Quitter le dialogue et de retourner au jeu
        /// </summary>
        public void Quit()
        {
            Debug.Log("Quit Dialog");

            if(currentDialog != null)
                currentDialog.ResetDialogues();

            if (currentConversant != null)
            {
                StartCoroutine(currentConversant.ResetCanTalk());
            }

            currentDialog = null;

            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated.Invoke();
        }

        #endregion
    }
}
