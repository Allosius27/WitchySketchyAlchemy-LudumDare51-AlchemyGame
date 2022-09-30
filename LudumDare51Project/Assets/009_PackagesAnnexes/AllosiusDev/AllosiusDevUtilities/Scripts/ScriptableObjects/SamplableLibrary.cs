//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AllosiusDevUtilities {
    [CreateAssetMenu(fileName = "Library", menuName = "AllosiusDev/Base Library")]
    public class SamplableLibrary : InitializableScriptableObject {
        public static List<Object> Library => staticLibrary;
        private static List<Object> staticLibrary;
        public List<Object> library;
        [SerializeField] private Object sampleType;

        private System.Type Type => sampleType.GetType();

        public override void Initialize() {
#if UNITY_EDITOR
            library.Clear();
            string[] assets = AssetDatabase.FindAssets($"t:{Type.FullName}");
            foreach(string asset in assets) {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                Object item = AssetDatabase.LoadAssetAtPath(path, Type);
                library.Add(item);
                //Debug.Log($"{item} Initialized");
            }
            staticLibrary = library;
#endif
        }
    }
}