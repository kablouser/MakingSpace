using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    //Need to add a childed collider with with the layer set to interactable to the game object that you want
    //To be able to interact with
    public void Interact(CharacterController InteractingCharacter);
    public void Focus();
    public void Unfocus();
}
