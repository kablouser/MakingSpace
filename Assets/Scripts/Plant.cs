using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject grayOutSprite;

    [SerializeField]
    private float timeToGrow = 60.0f;

    private float growingTimer = 0.0f;

    private bool isGrowing = false;
    private bool hasBeenWatered = false;

    private bool isDead = false;

    private void Update()
    {
        if(isGrowing)
        {
            growingTimer += Time.deltaTime;

            if(growingTimer >= timeToGrow)
            {
                //Plant dies
                if(!hasBeenWatered)
                {
                    isGrowing = false;
                    isDead = true;
                    return;
                }

                //Plant auto harvest
            }
            else if(growingTimer >= timeToGrow / 2)
            {
                if(!hasBeenWatered)
                {

                }
            }
        }
    }

    public void PlacedPlant()
    {
        spriteRenderer.sortingLayerName = "OnFloor";
        isGrowing = true;
    }

    public void SetGrayedOut(bool grayOut)
    {
        if(grayOut)
        {
            grayOutSprite.SetActive(true);
        }
        else
        {
            grayOutSprite.SetActive(false);
        }
    }
}
