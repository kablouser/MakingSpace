using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject waterBucket;

    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float interactRadius = 1.5f;
    [SerializeField]
    private LayerMask interactLayer;

    private IInteractable currentFocus;

    public bool isHoldingWater = false;

    [Header("Audio")]
    public AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        //Water bucket
        if(waterBucket.activeInHierarchy != isHoldingWater)
        {
            waterBucket.SetActive(isHoldingWater);
        }

        //Movement
        Vector2 MovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float MovementSpeed = Mathf.Clamp(MovementVector.magnitude, 0f, 1f);
        MovementVector.Normalize();
        rb.velocity = MovementVector * speed * MovementSpeed;

        //Focus on closest interactable
        Collider2D[] InteractablesInRange = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactLayer);
        if(InteractablesInRange.Length > 0)
        {
            //Get closest interactable
            Collider2D ClosestInteractableCollider = InteractablesInRange[0];
            float ClosestDistance = interactRadius + 1;
            foreach(Collider2D InteractableCollider in InteractablesInRange)
            {
                float Distance = Vector2.Distance(InteractableCollider.transform.position, transform.position);
                if(Distance < ClosestDistance)
                {
                    ClosestInteractableCollider = InteractableCollider;
                    ClosestDistance = Distance;
                }
            }

            //Focus on closest interactable
            IInteractable ClosestInteractable = ClosestInteractableCollider.GetComponentInParent<IInteractable>();
            if(ClosestInteractable == null)
            {
                print("Missing IInteractable on " + ClosestInteractableCollider.name);
            }
            else if(currentFocus != ClosestInteractable)
            {
                UnfocusCurrent();
                currentFocus = ClosestInteractable;
                currentFocus.Focus();
            }
        }
        else
        {
            //No interactable in range
            UnfocusCurrent();
        }

        //Interact
        if(currentFocus != null && Input.GetKeyDown(KeyCode.E))
        {
            currentFocus.Interact(this);
        }
    }

    private void UnfocusCurrent()
    {
        if(currentFocus != null)
        {
            currentFocus.Unfocus();
            currentFocus = null;
        }
    }
}
