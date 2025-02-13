using Dolls.Health;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private DollHealth dollHealth;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthCountTMP;

    private int _dollMaxHealth;

    private void Awake()
    {
        _dollMaxHealth = dollHealth.GetMaxHealth();
    }

    private void OnEnable()
    {
        dollHealth.CurrentHealth.OnChange += CurrentHealthOnOnChange;
    }

    private void OnDisable()
    {
        dollHealth.CurrentHealth.OnChange -= CurrentHealthOnOnChange;
    }

    private void UpdateFillImage(int currentHealth)
    {
        healthFillImage.fillAmount = (float)currentHealth / _dollMaxHealth;
    }

    private void CurrentHealthOnOnChange(int prev, int next, bool asServer)
    {
        UpdateHealthText(next);
        UpdateFillImage(next);
    }

    private void UpdateHealthText(int currentHealth)
    {
        healthCountTMP.text = $"{currentHealth} / {_dollMaxHealth}";
    }
}