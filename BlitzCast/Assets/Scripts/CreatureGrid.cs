using System.Collections.Generic;
using UnityEngine;

public class CreatureGrid : MonoBehaviour {

    public Vector2Int size;
    public GameObject cellPrefab;
    public GameObject linePrefab;
    public Transform cellsParent;
    public Transform horizontalLinesParent;
    public Transform verticalLinesParent;
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesParent;
    public Dictionary<Vector2Int, CreatureCardManager> creatures;

    public List<GridCell> cells;

    public void Initialize(Vector2Int size)
    {
        this.size = size;

        creatures = new Dictionary<Vector2Int, CreatureCardManager>();

        // initialize our cells
        cells = new List<GridCell>();
        //for (int i = 0; i < cellsParent.childCount; i++)
        //{
        //    GridCell cell = cellsParent.GetChild(i).GetComponent<GridCell>();
        //    cell.coordinates = new Vector2Int(i / size.y, i % size.y);
        //    cells.Add(cell);
        //}
        for (int r = 0; r < size.x; r++)
        for (int c = 0; c < size.y; c++)
        {
            GameObject cellObject = Instantiate(cellPrefab, cellsParent);
            GridCell cell = cellObject.GetComponent<GridCell>();
            cell.coordinates = new Vector2Int(r, c);
            cells.Add(cell);
        }

        for (int r = 0; r < size.x + 1; r++)
        {
            GameObject lineObject = Instantiate(linePrefab, horizontalLinesParent);
        }
        for (int c = 0; c < size.y + 1; c++)
        {
            GameObject lineObject = Instantiate(linePrefab, verticalLinesParent);
        }
    }

    public void HighlightRC(Vector2Int rc, Color color)
    {
        GridCell temp = GetCellRC(rc);
        if (temp != null)
        {
            temp.Highlight(color);
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

    public GridCell GetCellRC(Vector2Int rc)
    {
        if (rc.x < 0 ||
            rc.x >= size.x ||
            rc.y < 0 ||
            rc.y >= size.y)
        {
            return null;
        }
        return cells[(rc.x * size.y) + rc.y];
    }
}
