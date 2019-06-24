using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Arranges GridCell Objects using a GridLayout.
/// Also provides interface for cell highlighting and accessing creatures.
/// </summary>
public class CreatureGrid : MonoBehaviour {

    // in row, column format
    public Vector2Int size;

    // References to prefabs - Image with GridCell Component and Image with 
    public GameObject cellPrefab;
    public GameObject linePrefab;

    // Automatically arranges children in a grid
    // Defines cell size and 
    public GridLayoutGroup cellsGroup;
    public GridLayoutGroup horiLinesGroup;
    public GridLayoutGroup vertLinesGroup;
    //Hierarchy
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesParent;

    //Allows us to access creature based a location vector (row column)
    public Dictionary<Vector2Int, CreatureCardManager> creatures;
    //List of Cells on the board
    public List<GridCell> cells;

    /// <summary>
    /// Initialize this CreatureGrid; sets values and initiates cells
    /// </summary>
    /// <param name="size">
    /// The size (row column) of the CreatureGrid
    /// </param>
    public void Initialize(Vector2Int size)
    {
        this.size = size;
        RectTransform cellsGroupRect = cellsGroup.GetComponent<RectTransform>();

        //automatically determines offset between each cell
        // + 2 accounts for width of lines
        cellsGroupRect.sizeDelta = new Vector2(
            cellsGroup.cellSize.x * size.y + 1,
            cellsGroup.cellSize.y * size.x + 1
        );
        //automatically determines offset between each line (hori and vert)
        //2 is weight of line, size specifies distance between each line
        horiLinesGroup.cellSize = new Vector2(cellsGroupRect.rect.size.x, 1);
        vertLinesGroup.cellSize = new Vector2(1, cellsGroupRect.rect.size.y - 1);
        
        //Initializes Creature Dictionary
        creatures = new Dictionary<Vector2Int, CreatureCardManager>();

        // initialize our cells
        cells = new List<GridCell>();
        //iterate through rows/columns to give cells location vector
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

    /// <summary>
    /// Returns all Creatures under playerCreaturesParent
    /// May need to change when implementing networking
    /// </summary>
    /// <returns>
    /// returns based on playerCreaturesParent in Hierarchy
    /// </returns>
    public List<CreatureCardManager> GetPlayerCreatures()
    {
        List<CreatureCardManager> creaturesList = new List<CreatureCardManager>();
        //iterate through in hierarchy
        foreach (Transform child in playerCreaturesParent)
        {
            CreatureCardManager creatureCard = child.GetComponent<CreatureCardManager>();
            //error check
            if (creatureCard != null)
            {
                creaturesList.Add(creatureCard);
            }
        }
        return creaturesList;
    }
    /// <summary>
    /// Returns all Creatures under enemyCreaturesParent
    /// May need to change when implementing networking
    /// </summary>
    /// <returns>
    /// returns based on enemyCreaturesParent in hierarchy
    /// </returns>
    public List<CreatureCardManager> GetEnemyCreatures()
    {
        List<CreatureCardManager> creaturesList = new List<CreatureCardManager>();
        //iterate through in hierarchy
        foreach (Transform child in enemyCreaturesParent)
        {
            CreatureCardManager creatureCard = child.GetComponent<CreatureCardManager>();
            //error check
            if (creatureCard != null)
            {
                creaturesList.Add(creatureCard);
            }
        }
        return creaturesList;
    }

    /// <summary>
    /// Returns all Creatures on the Grid
    /// </summary>
    /// <returns>
    /// All Creatures contined in grid
    /// </returns>
    public List<CreatureCardManager> GetAllCreatures()
    {
        List<CreatureCardManager> creatureList = GetPlayerCreatures();
        GetEnemyCreatures().AddRange(creatureList);
        return creatureList;
    }

    /// <summary>
    /// Lookup using the Dictionary to get a creature at a given location
    /// </summary>
    /// <param name="location">
    /// Vector2Int in Row Column format indexed from 0
    /// </param>
    /// <returns>
    /// CreatureCardManager at the coordinates or null if nothing there
    /// </returns>
    public CreatureCardManager GetCreature(Vector2Int location)
    {
        CreatureCardManager targetCreature;
        //TryGetValue is weird - returns false if no location-targetCreature pair
        if (creatures.TryGetValue(location, out targetCreature))
        {
            return targetCreature;
        }
        return null;
    }

    /// <summary>
    /// Gets a cell at a row/column
    /// </summary>
    /// <param name="rc">
    /// Vector2Int in Row Column format indexed from 0
    /// </param>
    /// <returns>
    /// null if fails error check (out of bounds), GridCell if passes
    /// </returns>
    public GridCell GetCell(Vector2Int rc)
    {
        //error check to make sure coords we ask for are on grid
        if (rc.x < 0 ||
            rc.x >= size.x ||
            rc.y < 0 ||
            rc.y >= size.y)
        {
            return null;
        }
        //converts from row column to an index used in cells
        return cells[(rc.x * size.y) + rc.y];
    }
}
