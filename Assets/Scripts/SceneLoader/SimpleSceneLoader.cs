using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


namespace ScreneLoader
{
    public class SimpleSceneLoader : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private Slider _slider;

        private void Start()
        {
            LoadSceneAsync(_sceneName);
        }

        private void LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.Log("Please set Scene Name in Scene Name");
                return;
            }

            StartCoroutine(LoadSceneAsyncCoroutine(sceneName.Trim()));
        }

        // In the future, change it to UniTask!
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                _slider.value = progress;
                yield return null;
            }
        }
    }
}
