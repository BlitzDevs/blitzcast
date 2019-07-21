using UnityEngine;
using Mirror;
using System.Collections.Generic;

/// <summary>
/// The GameManager contains references to important game settings, components
/// and prefabs. It also has utility functions and handles the start and end of
/// the game.
/// </summary>
public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// Team; used to distinguish between players.
    /// </summary>
    public enum Team {
        Friendly, 
        Enemy,
        Neutral
    }

    // Game settings for players
    public int playerHealth = 100;
    public int cardHandSize = 4;

    // References to important components
    public Vector2Int creatureGridSize;
    public CreatureGrid creatureGrid;
    public DetailViewer detailViewer;
    public GameLogger gameLogger;
    public GameTimer timer;
    public PlayerManager localPlayerManager;
    public PlayerManager enemyPlayerManager;
    public Camera mainCamera;
    public Transform dragLocationParent;

    // Prefabs
    public GameObject circleTimerPrefab;
    public GameObject handSlotPrefab;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;
    public GameObject cardDisplayPrefab;
    public GameObject statusPrefab;

    /// <summary>
    /// Initialize CreatureGrid and PlayerManagers.
    /// </summary>
    [ClientRpc]
    public void RpcStartGame()
    {
        // Initialize CreatureGrid
        creatureGrid.Initialize(creatureGridSize);
        // Initialize Players
        localPlayerManager.Initialize(Team.Friendly, playerHealth, cardHandSize);
        enemyPlayerManager.Initialize(Team.Enemy, playerHealth, cardHandSize);
    }

    /// <summary>
    /// Create a new circle timer.
    /// </summary>
    public CircleTimer NewCircleTimer(Transform parent)
    {
        CircleTimer cTimer = null;
        GameObject timerObject = Instantiate(circleTimerPrefab, parent);
        cTimer = timerObject.GetComponent<CircleTimer>();
        cTimer.gameObject.SetActive(false);
        return cTimer;
    }

    /// <summary>
    /// Gets the player with the corresponding team.
    /// </summary>
    public PlayerManager GetPlayer(Team team)
    {
        return team == Team.Friendly ? localPlayerManager : enemyPlayerManager;
    }

}
