using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AllosiusDevCore.DialogSystem
{
    public class NpcConversant : MonoBehaviour
    {

        #region Fields

        private bool mouseOver;

        private bool canTalk = true;

        private bool PlayerAround = false;

        #endregion

        #region Properties

        public NpcData NpcData => npcData;

        #endregion

        #region UnityInspector

        [Header("PNJ Information")]

        [SerializeField] private NpcData npcData;

        [SerializeField] private DialogueGraph npcDialogue;

        [SerializeField] private float timeBeforeRelaunchDialogue = 0.5f;

        #endregion

        #region Behaviour

        private void Awake()
        {
            
        }

        void OnEnable()
        {
            Debug.Log("NPC Conversant is active");
        }

        private void Update()
        {
            
        }

        public void SetCanTalk(bool value)
        {
            canTalk = value;
        }

        public IEnumerator ResetCanTalk()
        {
            canTalk = false;

            yield return new WaitForSeconds(timeBeforeRelaunchDialogue);

            canTalk = true;
        }

        public void StartDialog(PlayerConversant playerConversant)
        {
            if (npcDialogue == null || canTalk == false || playerConversant == null)
            {
                return;
            }

            if(playerConversant.canConvers)
            {
                Debug.Log("Player Start Dialogue");
                playerConversant.StartDialog(this, npcDialogue);
            }
            
        }


       

        #endregion
    }
}
