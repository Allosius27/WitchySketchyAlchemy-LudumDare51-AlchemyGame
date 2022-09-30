//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using UnityEngine;

namespace AllosiusDevUtilities {
    public abstract class InitializableScriptableObject : ScriptableObject, IInitializable {
        public abstract void Initialize();
    }
}
