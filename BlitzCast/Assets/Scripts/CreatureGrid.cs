using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureGrid : MonoBehaviour {

    public Vector2Int size;

    public GameObject cellPrefab;
    public GameObject linePrefab;
    public GridLayoutGroup cellsGroup;
    public GridLayoutGroup horiLinesGroup;
    public GridLayoutGroup vertLinesGroup;
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesParent;

    public Dictionary<Vector2Int, CreatureCardManager> creatures;
    public List<GridCell> cells;


    public void Initialize(Vector2Int size)
    {
        this.size = size;
        RectTransform cellsGroupRect = cellsGroup.GetComponent<RectTransform>();

        cellsGroupRect.sizeDelta = new Vector2(
            cellsGroup.cellSize.x * size.y + 2,
            cellsGroup.cellSize.y * size.x + 2
        );
        horiLinesGroup.cellSize = new Vector2(cellsGroupRect.rect.size.x, 2);
        vertLinesGroup.cellSize = new Vector2(2, cellsGroupRect.rect.size.y);

        creatures = new Dictionary<Vector2Int, CreatureCardManager>();

        // initialize our cells
        cells = new List<GridCell>();
        for (int r = 0; r < size.x; r++)
        for (int c = 0; c < size.y; c++)
        {
            GameObject cellObject = Instantiate(cellPrefab, cellsGroup.transform);
            GridCell cell = cellObject.GetComponent<GridCell>();
            cell.grid = this;
            cell.coordinates = new Vector2Int(r, c);
            cells.Add(cell);
        }

        // create line objects
        for (int r = 0; r < size.x + 1; r++)
        {
            Instantiate(linePrefab, horiLinesGroup.transform);
        }
        for (int c = 0; c < size.y + 1; c++)
        {
            Instantiate(linePrefab, vertLinesGroup.transform);
        }
    }

    public void HighlightRC(Vector2Int rc, Color color)
    {
        GridCell temp = GetCell(rc);
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

    public GridCell GetCell(Vector2Int rc)
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
