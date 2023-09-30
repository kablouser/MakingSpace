using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private float speed = 1;

    // Update is called once per frame
    void Update()
    {
        Vector2 MovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float movementSpeed = Mathf.Clamp(MovementVector.magnitude, 0f, 1f);
        MovementVector.Normalize();
        rb.velocity = MovementVector * speed * movementSpeed;
    }
}
