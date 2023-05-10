using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private Image MaxHealthBar;
    [SerializeField] private Image CurrentHealthBar;

    public void Start()
    {
        MaxHealthBar.fillAmount = healthController.currentHealth/10;
    }

    public void Update()
    {
        CurrentHealthBar.fillAmount = healthController.currentHealth / 10;
    }
}

