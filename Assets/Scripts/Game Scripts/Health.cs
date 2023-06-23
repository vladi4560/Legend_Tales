using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class Health
{
    public int currentHealth;
    public int maxHealth;
    public TextMeshProUGUI healthText; // The UI Text object that displays the health
    public Image healthImg; // The UI Image object that displays the health In Circle

    public Health(TextMeshProUGUI hpText, Image hpCircle)
    {
        healthText = hpText;
        healthImg = hpCircle;
        maxHealth = 40;
        currentHealth = 5;
        UpdateHealthText();
    }


    private void UpdateHealthText()
    {
        // Update the text of the UI element to match the current and max health
        healthText.text = currentHealth + "/" + maxHealth;
        healthImg.fillAmount =(float) currentHealth/maxHealth;
    }

    public bool TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            return true; // if dead
        }
        return false;
    }

    // public void Heal(int healAmount) {
    //     currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    //     UpdateHealthText();
    // }
}
