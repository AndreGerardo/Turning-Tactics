using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHealthBarUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup healthBarCG;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthBarText;

    public void SetCurrentHealthBar(int currentHP, int maxHP)
    {
        healthBarSlider.value = currentHP * 1f / maxHP;
        healthBarText.SetText($"{currentHP} / {maxHP}");
    }

    public void Show()
    {
        healthBarCG.alpha = 1f;
    }

    public void Hide()
    {
        healthBarCG.alpha = 0f;
    }

}
