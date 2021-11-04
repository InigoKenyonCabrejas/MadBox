using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageFloater : MonoBehaviour
{
    [SerializeField] private float originYOffset = 2f;
    [SerializeField] private float floatAmmount;
    [SerializeField] private float floatDuration;
    [SerializeField] private TextMeshProUGUI damageText;
    
    public Action<DamageFloater> FinishedFloatAction;
    
    public void DisplayDamage(string text, Vector3 pos)
    {
        damageText.text = text;
        pos.y += originYOffset;
        transform.position = pos;
        gameObject.SetActive(true);
        
        transform.DOMoveY(transform.position.y + floatAmmount, floatDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
            FinishedFloatAction?.Invoke(this);
        });
    }
}
