using System.Collections.Generic;
using UnityEngine;

public class CreatureGrid : MonoBehaviour {

    public Transform cellsParent;
    public Vector2Int size;
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesParent;
    public Dictionary<Vector2Int, CreatureCardManager> creatures;

    public List<GridCell> cells;

	void Start () {
        cells = new List<GridCell>();
        foreach (Transform child in cellsParent)
        {
            cells.Add(child.GetComponent<GridCell>());
        }

        creatures = new Dictionary<Vector2Int, CreatureCardManager>();
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].coordinates = new Vector2Int(i / size.y, i % size.y);
        }
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
