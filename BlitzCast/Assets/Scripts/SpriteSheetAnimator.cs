using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnimator : MonoBehaviour
{
    public Card card;

    private Image image;
    private GameTimer gameTimer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private int frame;
    private float time;


    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();
        sprites = Resources.LoadAll<Sprite>("Textures/Cards/" + card.spriteSheet.name);
        gameTimer = FindObjectOfType<GameTimer>();

        if (image == null)
        {
            Debug.LogWarning("No image component on " + gameObject.name);
        }
        if (sprites == null)
        {
            Debug.LogWarning("Sprite sheet not set on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= 1f / card.spriteAnimateSpeed)
        {
            frame = frame + 1 >= sprites.Length ? 0 : frame + 1;
            image.sprite = sprites[frame];

            time = 0f;
        }
    }
}
