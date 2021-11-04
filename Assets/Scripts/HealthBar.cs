using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private float fillDuration = 0.5f;

    public void ChangeHealth(float fillAmmount)
    {
        healthBarImage.DOFillAmount(fillAmmount, fillDuration);
    }
}