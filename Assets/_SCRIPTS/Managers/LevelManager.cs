using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SceneList
{
    ExploreScene,
    CombatScene
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("LOADING SCREEN REFERENCE")]
    [SerializeField]
    private CanvasGroup loadingScreenCanvasGroup;

    private bool isLoading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(SceneList scene)
    {
        if (isLoading) return;
        loadingScreenCanvasGroup.gameObject.SetActive(true);

        StartCoroutine(LoadLevelAsync(scene.ToString()));
    }

    IEnumerator LoadLevelAsync(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        isLoading = true;

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            //loadingSlider.value = progressValue;
            //loadingText.SetText($"{progressValue * 100} %");
            yield return null;
        }

        loadingScreenCanvasGroup.DOFade(0f, 1f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                loadingScreenCanvasGroup.gameObject.SetActive(false);
                loadingScreenCanvasGroup.alpha = 1f;
                //loadingText.SetText("0 %");
                isLoading = false;
            });
    }

}