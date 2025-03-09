using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    [Header("PROMPT CONFIGURATION")]
    [SerializeField, TextArea] protected string promptText;

    public virtual void ShowPrompt()
    {
        PromptEvent.OnShowPrompt?.Invoke(promptText);
        CharacterEvent.OnSetPlayerIdleState?.Invoke(true);
    }

}

public static class PromptEvent
{
    public static Action<string> OnShowPrompt;
}
