using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnimator : MonoBehaviour
{
    public enum State
    {
        Idle,
        Action,
        Hurt
    }

    [Serializable]
    public struct Animatable {
        public Dictionary<State, Sprite[]> spritesDict;
        public float speed;
        public Entity entity;

        public Animatable(string name, string category, float speed, Entity entity)
        {
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
            this.speed = speed;
            this.entity = entity;
        }
    }

    public Animatable animatable;
    public State state = State.Idle;

    private Image image;
    private GameTimer gameTimer;
    [SerializeField] private Sprite[] currentSprites;
    [SerializeField] private int frame;
    private float time;


    public void Initialize(Animatable animatable)
    {
        this.animatable = animatable;

        image = GetComponent<Image>();
        gameTimer = FindObjectOfType<GameTimer>();

        animatable.spritesDict.TryGetValue(state, out currentSprites);

        if (image == null)
        {
            Debug.LogWarning("No image component on " + gameObject.name);
        }
        if (currentSprites == null)
        {
            Debug.LogWarning("Sprite sheet not set on " + gameObject.name);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //TODO: if change state, change sprite set

        if (currentSprites.Length > 0)
        {
            time += Time.deltaTime;

            if (time >= 1f / animatable.speed)
            {
                frame = frame + 1 >= currentSprites.Length ? 0 : frame + 1;
                image.sprite = currentSprites[frame];

                time = 0f;
            }
        }

    }
}
