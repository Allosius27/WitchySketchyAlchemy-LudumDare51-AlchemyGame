
using AllosiusDevUtilities;
using AllosiusDevUtilities.Audio;
using UnityEngine;

namespace AllosiusDevCore {

    namespace Audio {

        public class TestAudio : MonoBehaviour
        {

            //public AudioController audioController;

            public AudioData sfxTest01;
            public AudioData sfxTest02;

            public AudioData musicTest01;


#region Unity Functions
#if UNITY_EDITOR
            private void Update() 
            {
                if (Input.GetKeyUp(KeyCode.T)) {
                    AudioController.Instance.PlayAudio(musicTest01, null, true);
                }
                if (Input.GetKeyUp(KeyCode.G)) {
                    AudioController.Instance.StopAudio(musicTest01, null, true);
                }
                if (Input.GetKeyUp(KeyCode.B)) {
                    AudioController.Instance.RestartAudio(musicTest01, null, true);
                }
                if (Input.GetKeyUp(KeyCode.Y)) {
                    AudioController.Instance.PlayAudio(sfxTest01, null, false);
                }
                if (Input.GetKeyUp(KeyCode.H)) {
                    AudioController.Instance.StopAudio(sfxTest01, null, false);
                }
                if (Input.GetKeyUp(KeyCode.N)) {
                    AudioController.Instance.RestartAudio(sfxTest01, null, false);
                }
            }
#endif
#endregion
        }
    }
}
