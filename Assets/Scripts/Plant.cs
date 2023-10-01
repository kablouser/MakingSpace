using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plant : MonoBehaviour, IInteractable
{
    [HideInInspector]
    public Main main;

    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject focusIcon;
    [SerializeField]
    private Collider2D interactionCollider;

    [SerializeField]
    private BoxCollider2D plantCollider;
    [SerializeField]
    private GameObject grayOutSprite;
    [SerializeField]
    private Slider growBar;
    [SerializeField]
    private GameObject needWaterIcon;

    [SerializeField]
    private float timeToGrow = 60.0f;

    private float growingTimer = 0.0f;

    public float gowthSpeedMultiplier = 1.0f;

    private bool isGrowing = false;
    private bool canBeWatered = false;
    private bool hasBeenWatered = false;

    private bool isDead = false;

    private void Awake()
    {
        plantCollider.enabled = false;
        interactionCollider.enabled = false;
    }

    private void Update()
    {
        if(isGrowing)
        {
            growingTimer += Time.deltaTime * gowthSpeedMultiplier;
            growBar.value = growingTimer/timeToGrow;

            if(growingTimer >= timeToGrow)
            {
                //Plant dies
                if(!hasBeenWatered)
                {
                    isGrowing = false;
                    isDead = true;
                    needWaterIcon.SetActive(false);
                    return;
                }

                //Plant auto harvest
            }
            else if(growingTimer >= timeToGrow / 2)
            {
                if(!hasBeenWatered && !canBeWatered)
                {
                    canBeWatered = true;
                    needWaterIcon.SetActive(true);
                    interactionCollider.enabled = true;
                }
            }
        }
    }

    public void PlacedPlant()
    {
        main.plants.Add(this);

        spriteRenderer.sortingLayerName = "OnFloor";
        plantCollider.enabled = true;

        growBar.value = 0;
        growBar.gameObject.SetActive(true);

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

    public void Interact(CharacterController InteractingCharacter)
    {
        print("Interact with plant");
        if(isDead)
        {
            interactionCollider.enabled = false;
            Destroy(gameObject);
            return;
        }
        else if(canBeWatered && InteractingCharacter.isHoldingWater)
        {
            interactionCollider.enabled = false;
            hasBeenWatered = true;
            needWaterIcon.SetActive(false);
            InteractingCharacter.isHoldingWater = false;
            return;
        }
    }

    public void Focus()
    {
        if (focusIcon)
        {
            focusIcon.SetActive(true);
        }
    }

    public void Unfocus()
    {
        if(focusIcon)
        {
            focusIcon.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        main.plants.Remove(this);
    }
}
