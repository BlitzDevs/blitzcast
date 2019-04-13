using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager
{
    public Entity entity;

    [SerializeField] private TMP_Text cardHealthText;
    [SerializeField] private TMP_Text cardActionValueText;
    [SerializeField] private TMP_Text cardActionTimeText;

    [SerializeField] private RectTransform gridDisplayRect;
    [SerializeField] private RectTransform gridStatusesParent;
    [SerializeField] private TMP_Text gridHealthText;
    [SerializeField] private TMP_Text gridActionValueText;
    [SerializeField] private TMP_Text gridSpeedText;
    [SerializeField] private CircleTimer actionTimer;

    private int actionValue;
    private int actionTime;

    private Vector2Int coordinates;
    private CreatureCard creatureCard;

    private Vector2 spriteSize;
    private Vector2 sizeOffset;


    // Initialize is our own function which is called by HandSlot
    public override void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        base.Initialize(card, slot, player);

        creatureCard = (CreatureCard) card;

        // set text on card
        actionValue = creatureCard.cardBehavior.actionValue;
        actionTime = creatureCard.actionTime;
        cardHealthText.text = creatureCard.health.ToString();
        cardActionValueText.text = actionValue.ToString();
        cardActionTimeText.text = actionTime.ToString();

        spriteSize = new Vector2(
            sprite.rectTransform.rect.width * creatureCard.size.y,
            sprite.rectTransform.rect.height * creatureCard.size.x
        );
        sizeOffset = new Vector2(
            sprite.rectTransform.rect.width / 2 * (creatureCard.size.y - 1),
            -sprite.rectTransform.rect.height / 2 * (creatureCard.size.x - 1)
        );

        sprite.rectTransform.sizeDelta = spriteSize;
        castingSpriteParent.sizeDelta = spriteSize;

        gridStatusesParent.sizeDelta = new Vector2(spriteSize.x, gridStatusesParent.rect.y);
        gridDisplayRect.sizeDelta = spriteSize;
        gridDisplayRect.gameObject.SetActive(false);
    }

    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        if (target != null)
        {
            //set color/transparency
            sprite.color = new Color(0, 0, 0, 0.5f);
            //snap to target
            castingSpriteParent.transform.position = target.transform.position;

            foreach (GameObject targetObject in GetCastTargets(target))
            {
                Highlightable highlightable = targetObject.gameObject.GetComponent<Highlightable>();
                previewHighlightables.Add(highlightable);
                highlightable.Highlight(card.color);
            }
        }
        else
        {
            sprite.color = card.color;
        }

        castingSpriteParent.transform.localPosition += (Vector3) sizeOffset;
    }

    public override GameObject GetCastLocation()
    {
        // Get first GridCell under cursor
        GameObject cellObject = gameManager.GetFirstUnderCursor<GridCell>();
        GridCell cell = cellObject != null ?
            cellObject.GetComponent<GridCell>() : null;

        if (cell != null &&
            cell.coordinates.x >= grid.size.x / 2 &&
            cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
            cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x)
        {
            for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(
                    cell.coordinates.x + r, cell.coordinates.y + c);
                if (grid.creatures.ContainsKey(rc))
                {
                    return null;
                }
            }
            return cellObject;
        }
        return null;
    }

    public override List<GameObject> GetCastTargets(GameObject target)
    {
        List<GameObject> targets = new List<GameObject>();
        GridCell cell = target.GetComponent<GridCell>();
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(
                cell.coordinates.x + r, cell.coordinates.y + c);
            targets.Add(grid.GetCell(rc).gameObject);
        }
        return targets;
    }

    protected override IEnumerator CastTimer(GameObject target)
    {
        // add creature location to grid
        foreach (GameObject cellObject in GetCastTargets(target))
        {
            GridCell c = cellObject.GetComponent<GridCell>();
            grid.creatures.Add(c.coordinates, this);
        }

        return base.CastTimer(target);
    }

    public override void Cast(GameObject location)
    {
        GridCell cell = location.GetComponent<GridCell>();
        coordinates = cell.coordinates;

        gameObject.layer = LayerMask.NameToLayer("Creatures");

        // Turn CreatureCard into Creature on grid
        gameObject.name = card.cardName;
        // set creature rect size (parent GameObject)
        RectTransform creatureRect = gameObject.GetComponent<RectTransform>();
        creatureRect.sizeDelta = spriteSize;
        // move out of the hierarchy
        castingSpriteParent.transform.SetParent(transform);
        castingSpriteParent.transform.localPosition = Vector3.zero;
        transform.SetParent(grid.playerCreaturesParent);
        //transform.position = target.transform.position + sizeOffset;
        transform.position = location.transform.position;
        transform.localPosition += (Vector3) sizeOffset;

        // Enable Grid Creature Display
        gridDisplayRect.gameObject.SetActive(true);
        gridHealthText.text = creatureCard.health.ToString();
        gridActionValueText.text = actionValue.ToString();
        actionTimer.entity = entity;
        actionTimer.StartTimer(actionTime);

        // disable card display
        SetTint(new Color(0f, 0f, 0f, 0f));
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        // enable sprite display
        sprite.gameObject.SetActive(true);
        // set color/transparency to normal
        sprite.color = card.color;

        //create entity 
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(creatureCard.health, 1f, new List<Entity.Status>(),
            gridStatusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;

        // Start action timer coroutine
        StartCoroutine(DoAction());
    }


    public override void DestroySelf()
    {
        //delete creature locations from CreatureGrid
        for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(coordinates.x + r, coordinates.y + c);
                grid.creatures.Remove(rc);
            }

        base.DestroySelf();
    }

    private IEnumerator DoAction()
    {
        while (entity.Health > 0)
        {
            if (actionTimer.IsComplete())
            {
                //raise event to let entity know
                entity.ActionEvent += entity.OnDoAction;

                switch (card.cardBehavior.action)
                {
                    case Card.Action.Damage:
                        Debug.Log(card.cardName + " deal damage");
                        break;
                    case Card.Action.Heal:
                        Debug.Log(card.cardName + " heals");
                        break;
                    case Card.Action.Destroy:
                        Debug.Log(card.cardName + " destroys");
                        break;
                    default:
                        Debug.LogWarning("Action not implemented");
                        break;
                }
                actionTimer.StartTimer(creatureCard.actionTime);
            }
            yield return null;
        }

        // when health <= 0
        DestroySelf();
    }

    // added onto event OnHealthChange of entity
    public void SetHealthDisplay(int hp)
    {
        gridHealthText.text = hp.ToString();
    }

    // added onto event OnSpeedChange of entity
    public void SetSpeedDisplay(float s)
    {
        // if speed is ~1f
        if (Mathf.Abs(s - 1) < Mathf.Epsilon)
        {
            gridSpeedText.text = "";
        }
        else
        {
            gridSpeedText.text = "x" + (Mathf.Round(s * 10) / 10f).ToString();
        }
    }
}
