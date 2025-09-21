using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CharacterSpriteRendererController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> frameAnimationSprites;
    [SerializeField] int changeSpriteFrameSpan = 3;
    int frameCounter = 0;
    int currentSpriteFrameIndex = 0;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;
        if (frameCounter % changeSpriteFrameSpan == 0)
        {
            Sprite chageSprite = this.frameAnimationSprites[currentSpriteFrameIndex];
            spriteRenderer.sprite = chageSprite;
            currentSpriteFrameIndex++;
            if (currentSpriteFrameIndex >= this.frameAnimationSprites.Count)
            {
                currentSpriteFrameIndex = 0;
            }
            frameCounter = 0;
        }
    }
}
