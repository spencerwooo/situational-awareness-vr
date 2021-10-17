using System;
using System.Collections;
using UnityEngine;

public class GameWinTrigger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject winText;

    private void Awake()
    {
        winText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player) return;

        Debug.Log("Game won!");
        winText.SetActive(true);
    }
}