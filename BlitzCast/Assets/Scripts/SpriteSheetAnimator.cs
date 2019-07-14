using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Animates an image by incrementing the sprite frame from a sprite sheet.
/// The sprite used is based on the name and category; it is accessed through
/// the Resources folder, with the file path Resources/Textures/category/name.
/// Optionally, an Entity can be set, and the speed/state of the animation will
/// be based on the properties of the Entity.
/// <para>
/// Attach SpriteSheetAnimator onto a GameObject with an Image component.
/// Access this component and intialize with an Animatable, which contains all
/// of the properties needed to animate the image.
/// </para>
/// </summary>
public class SpriteSheetAnimator : MonoBehaviour
{

    /// <summary>
    /// The state of the Entity, to be based on property change events.
    /// </summary>
    public enum State
    {
        Idle,
        Action,
        Hurt
    }

    public float speed;

    private State state = State.Idle;
    private Image image;
    private GameTimer gameTimer;
    private Dictionary<State, Sprite[]> spritesDict;
    private Sprite[] currentSprites;
    private int frame;
    private float time;
    private Entity entity;

    private void Start()
    {
        image = GetComponent<Image>();
        gameTimer = FindObjectOfType<GameTimer>();
    }

    /// <summary>
    /// Fully initialize this SpriteSheetAnimator; get references to needed objects.
    /// </summary>
    public void Initialize(string name, string category, float speed, Entity entity)
    {
        this.speed = speed;
        this.entity = entity;

        currentSprites = new Sprite[] { };
        spritesDict = new Dictionary<State, Sprite[]>();
        foreach (State s in Enum.GetValues(typeof(State)))
        {
            string fp = string.Format("Textures/{0}/{1}-{2}", category, name.Replace(' ', '-').ToLower(), s.ToString().ToLower());
            Sprite[] sprites = Resources.LoadAll<Sprite>(fp);

            if (sprites == null || sprites.Length < 1)
            {
                string defaultPath = string.Format("Textures/{0}/default-{1}", category, s.ToString().ToLower());
                sprites = Resources.LoadAll<Sprite>(defaultPath);
            }

            spritesDict.Add(s, sprites);
        }
        spritesDict.TryGetValue(state, out currentSprites);
    }

    /// <summary>
    /// Copy the sprites of another SpriteSheetAnimator.
    /// </summary>
    public void Copy(SpriteSheetAnimator original)
    {
        speed = original.speed;
        spritesDict = original.GetSprites();
        spritesDict.TryGetValue(state, out currentSprites);
        time = 1f;
    }


    /// <summary>
    /// Called by Unity. Every frame this component is active, loop over the
    /// sprite frames based on the animate speed.
    /// </summary>
    void Update()
    {
        //TODO: if change state, change sprite set

        if (currentSprites.Length > 0)
        {
            time += gameTimer.deltaTime;

            if (time >= 1f / speed)
            {
                frame = frame + 1 >= currentSprites.Length ? 0 : frame + 1;
                image.sprite = currentSprites[frame];

                time = 0f;
            }
        }
    }

    public Dictionary<State, Sprite[]> GetSprites()
    {
        return spritesDict;
    }
}
