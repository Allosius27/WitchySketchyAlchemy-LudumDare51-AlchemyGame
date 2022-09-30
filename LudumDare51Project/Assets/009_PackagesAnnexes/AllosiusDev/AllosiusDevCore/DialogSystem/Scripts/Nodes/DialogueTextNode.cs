using AllosiusDevUtilities.Core;
using System.Collections;
using System.Collections.Generic;
//using BLINK.RPGBuilder.Logic;
//using BLINK.RPGBuilder.Managers;
using UnityEngine;
using XNode;

namespace AllosiusDevCore.DialogSystem
{
    [NodeWidth(600)]
    [NodeTint(46, 46, 46)]
    [CreateNodeMenu("Dialogue Text Node")]
    public class DialogueTextNode : Node
    {
        #region Fields

        private bool globalAlreadyRead;

        private bool alreadyRead;

        #endregion

        #region Properties

        public float timerBeforeNextNode { get; set; }

        #endregion

        #region UnityInspector

        [Input] public DialogueTextNode previousNode;
        [Output] public DialogueBaseNode nextNodes;

        public enum IdentityType { NPC, Player }
        [NodeEnum] [SerializeField] public IdentityType identityType;

        
        [NodeEnum] [SerializeField] public TexteType texteType;

        [SerializeField] public string keyText = "";

        [TextArea]
        [SerializeField] public string messagePreview = "";

        [SerializeField] public bool singleRead;

        [SerializeField] public bool hasEnterNodeActions;
        [SerializeField] public bool showEnterNodeActions;
        [SerializeField] public GameActions enterNodeActions;

        [SerializeField] public bool hasGameActions;
        [SerializeField] public bool showGameActions;
        [SerializeField] public GameActions gameActions;

        [SerializeField] public bool hasRequirements;
        [SerializeField] public bool showGameRequirements;
        [SerializeField] public GameRequirements gameRequirements;

        #endregion

        #region Functions

        public bool GetGlobalAlreadyRead()
        {
            return globalAlreadyRead;
        }

        public bool GetAlreadyRead()
        {
            return alreadyRead;
        }

        public void SetGlobalAlreadyRead(bool value)
        {
            globalAlreadyRead = value;
        }

        public void SetAlreadyReadValue(bool value)
        {
            alreadyRead = value;
        }

        public void GetPorts()
        {
            foreach (var item in Ports)
            {
                //Debug.Log(item.fieldName + " ");
                for (int i = 0; i < item.ConnectionCount; i++)
                {
                    //Debug.Log(item.GetConnection(i).fieldName + " " + item.GetConnection(i).node.name);
                }
            }
        }

        // GetValue should be overridden to return a value for any specified output port
        public override object GetValue(NodePort port)
        {

            // Get new a and b values from input connections. Fallback to field values if input is not connected
            DialogueTextNode previousNode = GetInputValue<DialogueTextNode>("previousNode", this.previousNode);

            // After you've gotten your input values, you can perform your calculations and return a value
            return previousNode;
        }



        #endregion
    }
}