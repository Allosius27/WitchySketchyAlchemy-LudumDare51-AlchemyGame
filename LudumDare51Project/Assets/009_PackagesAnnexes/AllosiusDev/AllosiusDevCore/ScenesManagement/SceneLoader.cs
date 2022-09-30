using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AllosiusDevUtilities.Audio;

namespace AllosiusDevCore
{
    public class SceneLoader : AllosiusDevUtilities.Singleton<SceneLoader>
    {
        #region Fields

        private LoadingScreen _loadingScreen;

        #endregion

        #region Properties

        #endregion

        #region UnityInspector

        public LoadingScreen loadingScreen;

        #endregion

        #region Events

        public event System.Action OnSceneChanged;

        #endregion

        #region Behaviour

        private void Start()
        {
            if (SceneManager.GetActiveScene().buildIndex == (int)(object)Scenes.BootScene)
            {
                ChangeScene(Scenes.MainMenu);
            }
        }

        public void ChangeScene(System.Enum _enum)
        {
            OnSceneChanged?.Invoke();
            SceneManager.LoadScene((int)(object)_enum);
        }

        public void ActiveLoadingScreen(SceneData _sceneData, float timeToWait)
        {
            Debug.Log("ActiveLoadingScreen !");

            _loadingScreen = Instantiate(loadingScreen, transform.position, transform.rotation);
            var _gui = GameObject.FindGameObjectWithTag("GUI");
            if (_gui != null)
            {
                _loadingScreen.transform.SetParent(_gui.transform);
            }
            else
            {
                Debug.LogWarning("Not GUI found");
            }

            StartCoroutine(LoadAsynchronously(_sceneData, timeToWait));
        }

        public IEnumerator LoadAsynchronously(SceneData _sceneData, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);

            AudioController.Instance.StopAllAmbients();
            Debug.Log("StopAllAmbients");

            AsyncOperation operation = SceneManager.LoadSceneAsync((int)(object)_sceneData.sceneToLoad);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                //_loadingScreen.slider.value = progress;
                _loadingScreen.uiProgressBar.SetFill(progress);
                _loadingScreen.progressText.text = (int)(progress * 100f) + "%";

                if (operation.progress >= 0.8f)
                {
                    Debug.Log("SceneChanged");
                    AudioController.Instance.StopAllMusics();
                    operation.allowSceneActivation = false;
                    yield return new WaitForSeconds(3f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }


        }

        #endregion
    }
}

