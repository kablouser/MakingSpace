using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWell : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Main main;

    [SerializeField]
    private GameObject focusIcon;

    public void Focus()
    {
        focusIcon.SetActive(true);
    }

    public void Interact(CharacterController InteractingCharacter)
    {
        if(!InteractingCharacter.isHoldingWater && main.water > 0)
        {
            InteractingCharacter.isHoldingWater = true;
            main.water--;
        }
        else if(InteractingCharacter.isHoldingWater)
        {
            InteractingCharacter.isHoldingWater = false;
            main.water++;
        }
    }

    public void Unfocus()
    {
        focusIcon.SetActive(false);
    }
}
