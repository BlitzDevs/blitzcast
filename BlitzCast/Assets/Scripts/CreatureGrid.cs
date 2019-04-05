﻿using System.Collections.Generic;
using UnityEngine;

public class CreatureGrid : MonoBehaviour {

    public Transform cellsParent;
    public Vector2Int size;
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesParent;
    public Dictionary<Vector2Int, CreatureCardManager> creatures;

    public List<GridCell> cells;

    // Start is called by Unity on first time this object is active
    void Start ()
    {
        creatures = new Dictionary<Vector2Int, CreatureCardManager>();

        // initialize our cells
        cells = new List<GridCell>();
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            GridCell cell = cellsParent.GetChild(i).GetComponent<GridCell>();
            cell.coordinates = new Vector2Int(i / size.y, i % size.y);
            cells.Add(cell);
            
        }
	}

    public void HighlightRC(Vector2Int rc, Color color)
    {
        if (rc.x < 0 ||
            rc.x > size.x ||
            rc.y < 0 ||
            rc.y > size.y)
        {
            return;
        }
        int index = (rc.x * size.y) + rc.y;
        cells[index].HighlightCell(color);
    }

    public List<CreatureCardManager> GetPlayerCreatures()
    {
        List<CreatureCardManager> creaturesList = new List<CreatureCardManager>();
        foreach (Transform child in playerCreaturesParent)
        {
            CreatureCardManager creatureCard = child.GetComponent<CreatureCardManager>();
            if (creatureCard != null)
            {
                creaturesList.Add(creatureCard);
            }
        }
        return creaturesList;
    }

    public List<CreatureCardManager> GetEnemyCreatures()
    {
        List<CreatureCardManager> creaturesList = new List<CreatureCardManager>();
        foreach (Transform child in enemyCreaturesParent)
        {
            CreatureCardManager creatureCard = child.GetComponent<CreatureCardManager>();
            if (creatureCard != null)
            {
                creaturesList.Add(creatureCard);
            }
        }
        return creaturesList;
    }

    public List<CreatureCardManager> GetAllCreatures()
    {
        List<CreatureCardManager> creatureList = GetPlayerCreatures();
        GetEnemyCreatures().AddRange(creatureList);
        return creatureList;
    }

    public CreatureCardManager GetCreature(Vector2Int location)
    {
        CreatureCardManager targetCreature;
        if (creatures.TryGetValue(location, out targetCreature))
        {
            return targetCreature;
        }
        return null;
    }
}
