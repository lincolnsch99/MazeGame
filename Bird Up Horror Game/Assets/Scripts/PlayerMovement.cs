using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 15.0f;

    private CharacterController playerControl;
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        playerControl = GetComponent<CharacterController>();
        moveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed *= 1.5f;

        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;
        Vector3 movement = new Vector3(moveX, 0, moveZ);

        movement = Vector3.ClampMagnitude(movement, moveSpeed);
        movement.y = Physics.gravity.y;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        playerControl.Move(movement);

    }
}