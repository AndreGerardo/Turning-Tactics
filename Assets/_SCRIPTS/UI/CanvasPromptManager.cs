using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class CanvasPromptManager : MonoBehaviour
{

    [Header("PROMPT REFERENCE")]
    [SerializeField] private CanvasGroup promptGroupCG;
    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        PromptEvent.OnShowPrompt += ShowPrompt;
    }

    private void OnDestroy()
    {
        PromptEvent.OnShowPrompt -= ShowPrompt;
    }

    private void ShowPrompt(string text)
    {
        Debug.Log(text);
        promptText.SetText(text);


        CharacterEvent.OnSetPlayerIdleState?.Invoke(true);

        promptGroupCG.DOFade(1f, UI_Constant.BaseAlphaAnimationDuration);
        promptGroupCG.DOFade(0f, UI_Constant.BaseAlphaAnimationDuration)
            .SetDelay(UI_Constant.UI_PROMPT_ANIMATION_DURATION)
            .OnComplete(() =>
            {
                CharacterEvent.OnSetPlayerIdleState?.Invoke(false);
            });


    }

}
