using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]
public class Mana {
    public int currentMana;
    public int maxMana;
    public TextMeshProUGUI manaText; // The UI Text object that displays the mana

    public Mana(TextMeshProUGUI manaText) {
        this.manaText = manaText;
        currentMana = 0;
        maxMana = 0;
        UpdateManaText();
    }

    private void UpdateManaText() {
        // Update the text of the UI element to match the current and max mana
        manaText.text = currentMana + "/" + maxMana;
    }

    public void StartTurn() {
        maxMana = Mathf.Min(maxMana + 1, 10);
        currentMana = maxMana;
        UpdateManaText();
    }

    public bool UseMana(int cost) {
        if (cost <= currentMana) {
            currentMana -= cost;
            UpdateManaText();
            return true;
        }
        else {
            return false;
        }
    }
}