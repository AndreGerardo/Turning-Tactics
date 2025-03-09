using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGameOverManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup GameOverCG;


    private void Awake()
    {
        BattleEvent.OnBattleLose += ShowGameOverPanel;
    }

    private void OnDestroy()
    {
        BattleEvent.OnBattleLose -= ShowGameOverPanel;
    }

    private void ShowGameOverPanel()
    {
        GameOverCG.DOFade(1f, UI_Constant.BaseAlphaAnimationDuration);
        GameOverCG.interactable = true;
        GameOverCG.blocksRaycasts = true;
    }

    public void ButtonRetry()
    {
        LevelManager.instance.LoadLevel(SceneList.ExploreScene);
    }

    public void ButtonQuitGame()
    {
        Application.Quit();
    }
}
