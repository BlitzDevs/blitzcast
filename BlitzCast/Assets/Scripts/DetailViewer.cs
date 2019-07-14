using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Shows the detailed view of a selected object.
/// </summary>
public class DetailViewer : MonoBehaviour
{

    // References to display components
    [SerializeField] private GameObject display;
    [SerializeField] private CardDisplayer cardDisplayer;
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        Set(null);
    }


    /// <summary>
    /// Set detail view info.
    /// </summary>
    public void Set(GameObject thing)
    {
        if (thing == null)
        {
            display.SetActive(false);
            return;
        }

        display.SetActive(true);

        CardManager cardManager = thing.GetComponent<CardManager>();
        if (cardManager)
        {
            cardDisplayer.Set(cardManager);
        }
    }



}