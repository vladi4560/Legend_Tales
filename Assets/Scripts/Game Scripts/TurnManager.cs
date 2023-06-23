using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class TurnManager : MonoBehaviour
{
    // To keep track of whose turn it is
    public static TurnManager Instance { get; private set; }
    public bool myTurn = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


