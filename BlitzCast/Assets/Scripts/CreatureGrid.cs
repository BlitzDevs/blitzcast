using System.Collections.Generic;
using UnityEngine;

public class CreatureGrid : MonoBehaviour {

    public List<GridCell> cells;
    public Vector2Int size;
    public Transform playerCreaturesParent;
    public Transform enemyCreaturesPlarent;
    private Dictionary<Vector2Int, CreatureCard> creatures;

	void Start () {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].coordinates = new Vector2Int(i / size.y, i % size.y);
        }
	}
}
