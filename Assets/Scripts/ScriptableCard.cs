// Put all our cards in the Resources folder
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game/ScriptableCard")]
public partial class ScriptableCard : ScriptableObject
{
    [SerializeField] string id = "";
    public string CardID { get { return this.id = Guid.NewGuid().ToString();} }

    [Header("Image")]
    public Sprite image; // Card image

    [Header("Properties")]
    public int cost;
    public int damage;
    public int life;


    [HideInInspector] public bool hasInitiative = false; // If our card has an INITIATIVE ability

    [Header("Description")]
    [SerializeField, TextArea(1, 100)] public string description;
    public string cardName;

    [Header("Card Prefab")]
    public GameObject cardPrefabHand;
    public GameObject cardPrefabField;

    
}