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
    [SerializeField] private CardDisplayer cardDisplayer;

    /// <summary>
    /// Set detail view info.
    /// </summary>
    public void Set()
    {
       Debug.Log("set detail view info");
    }



}