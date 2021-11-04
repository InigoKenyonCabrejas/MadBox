using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarView : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private Image playerHealthImage;
    [SerializeField] private float animDuration = 0.5f;

    private Tweener fillTween;

    private void Awake()
    {
        characterData.HealthChangedAction += OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if(fillTween != null)
        {
            fillTween.Kill();
        }
        
        fillTween = playerHealthImage.DOFillAmount((float)currentHealth / (float)maxHealth, animDuration);
        fillTween.OnKill(() =>
        {
            fillTween = null;
        });
    }
}
