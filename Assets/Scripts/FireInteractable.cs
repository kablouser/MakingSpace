using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject focusIcon;

    [SerializeField]
    private AudioClip putOutFireSFX;

    public void Focus()
    {
        focusIcon.SetActive(true);
    }

    public void Interact(CharacterController InteractingCharacter)
    {
        if(InteractingCharacter.isHoldingWater)
        {
            InteractingCharacter.audioSource.PlayOneShot(putOutFireSFX);
            InteractingCharacter.isHoldingWater = false;
            gameObject.SetActive(false);
        }
    }

    public void Unfocus()
    {
        focusIcon.SetActive(false);
    }
}
