using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AllosiusDevCore.TranslationSystem
{
    /// <summary>
    /// Classe permettant de charger une liste d'objet facilement transférable dans un objet de type Dictionary pour s'affranchr de la limitation du serializer d'unity
    /// </summary>
    [Serializable]
    public class Dico
    {
        [SerializeField]
        public string key = "";
        [SerializeField]
        public string value = "";

        public Dico(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// Classe permettant de charger le dictionnaire à l'aide de la serialization présente dans unity
    /// </summary>
    [Serializable]
    public class JSONLoader
    {
        /// <summary>
        /// On passe par une liste (le fichier json doit être écrit de telle sorte qu'il corresponde aux variables [SerializeField] présent dans cette classe
        /// </summary>
        [SerializeField]
        private List<Dico> dictionnaire;

        /// <summary>
        /// Fonction permettant de charger la classe JSONLoader à partir d'un fichier présent à un endroit donné avec un code langue à définir
        /// </summary>
        /// <param name="codeLangue">Code de la langue, correspond au nom du fichier sans le type (.json)</param>
        /// <returns>Returne un objet JSONLoader chargé avec la liste présente dans le fichier JSON correspondant au code langue</returns>
        public static JSONLoader LoadJSON(string codeLangue, TypeDictionary typeDictionary)
        {
            //string path = Application.streamingAssetsPath + "/" + codeLangue + ".json";
            //string json = File.ReadAllText(path, Encoding.UTF8);

            //string path = "";
            string json = "";
            
            //json = Resources.Load<TextAsset>(string.Format("Traduction/{0}", codeLangue)).text;

            if (typeDictionary == TypeDictionary.Dialogues)
            {
                codeLangue += "_Dialogues";
                //path = Application.streamingAssetsPath + "/Traduction/Dialogues/" + codeLangue + ".json";
                //json = File.ReadAllText(path, Encoding.UTF8);
                json = Resources.Load<TextAsset>("Traduction/Dialogues/" + codeLangue).text;
                //json = Resources.Load<TextAsset>(string.Format("Traduction/Dialogues/{0}", codeLangue)).text;
            }
            else if(typeDictionary == TypeDictionary.GeneralsUI)
            {
                codeLangue += "_GeneralsUI";
                //path = Application.streamingAssetsPath + "/Traduction/GeneralsUI/" + codeLangue + ".json";
                //json = File.ReadAllText(path, Encoding.UTF8);
                json = Resources.Load<TextAsset>("Traduction/GeneralsUI/" + codeLangue).text;
                //json = Resources.Load<TextAsset>(string.Format("Traduction/GeneralsUI/{0}", codeLangue)).text;
            }
            else if (typeDictionary == TypeDictionary.Quests)
            {
                codeLangue += "_Quests";
                //path = Application.streamingAssetsPath + "/Traduction/Quests/" + codeLangue + ".json";
                //json = File.ReadAllText(path, Encoding.UTF8);
                json = Resources.Load<TextAsset>("Traduction/Quests/" + codeLangue).text;
                //json = Resources.Load<TextAsset>(string.Format("Traduction/Quests/{0}", codeLangue)).text;
            }

            //Debug.Log(codeLangue);
            //Debug.Log(json);
            return JsonUtility.FromJson<JSONLoader>(json);
        }

        /// <summary>
        /// Permet si la liste est bien chargée de récupérer la liste dictionnaire dans le format optimisé pour ce type de tâche : Dictionary
        /// </summary>
        /// <returns>Objet de type dictionary</returns>
        public Dictionary<string, string> GetDictionaryValues(Dictionary<string, string> oldDictionary)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary = oldDictionary;

            foreach(Dico dico in dictionnaire)
            {
                dictionary.Add(dico.key, dico.value);
            }

            /*foreach(var item in dictionary)
            {
                Debug.Log(item.Key + " " + item.Value);
            }*/

            return dictionary;
        }
    }
}
