using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 15.0f;
    [SerializeField]
    private Animator armAnimControl;
    [SerializeField]
    private Animator camAnimControl;
    [SerializeField]
    private AudioSource footStep;
    [SerializeField]
    private AudioSource heavyBreathing;
    [SerializeField]
    private AudioSource normalBreathing;
    
    public bool isCrouching;

    private CharacterController playerControl;
    private float moveSpeed;
    private float footstepOffset, timeSinceFootstep;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        playerControl = GetComponent<CharacterController>();
        moveSpeed = speed;
        timeSinceFootstep = 0;
        footstepOffset = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceFootstep += Time.deltaTime;
        moveSpeed = speed;
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (normalBreathing.isPlaying)
            {
                if (!heavyBreathing.isPlaying)
                {
                    heavyBreathing.Play();
                    normalBreathing.Stop();
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (heavyBreathing.isPlaying)
            {
                if (!normalBreathing.isPlaying)
                {
                    normalBreathing.Play();
                    heavyBreathing.Stop();
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!Input.GetKey(KeyCode.S))
            {
                moveSpeed *= 1.5f;
                footstepOffset = 0.3f;
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed *= 0.35f;
            footstepOffset = 1f;
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
            footstepOffset = 0.75f;
            if (heavyBreathing.isPlaying)
            {
                if (!normalBreathing.isPlaying)
                {
                    normalBreathing.Play();
                    heavyBreathing.Stop();
                }
            }
        }

        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;
        Vector3 movement = new Vector3(moveX, 0, moveZ);

        movement = Vector3.ClampMagnitude(movement, moveSpeed);
        movement.y = Physics.gravity.y;

        if (Mathf.Abs(movement.x) > 0 || Mathf.Abs(movement.z) > 0)
        {
            armAnimControl.SetBool("moving", true);
        }
        else
        {
            armAnimControl.SetBool("moving", false);
            camAnimControl.SetFloat("moveSpeed", 0.0f);
        }

        if (moveSpeed > speed)
        {
            armAnimControl.SetFloat("animSpeed", 2.0f);
            camAnimControl.SetFloat("moveSpeed", 2.0f);
        }
        else if(moveSpeed < speed && moveSpeed > 0)
        {
            armAnimControl.SetBool("moving", false);
            camAnimControl.SetFloat("moveSpeed", 0.0f);
        }
        else
        {
            armAnimControl.SetFloat("animSpeed", 1.0f);
            if (Mathf.Abs(moveSpeed) > 0)
                camAnimControl.SetFloat("moveSpeed", 1.0f);
        }

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        playerControl.Move(movement);

        if(timeSinceFootstep > footstepOffset && (Mathf.Abs(movement.x) > 0 || Mathf.Abs(movement.z) > 0))
        {
            footStep.PlayOneShot(footStep.clip);
            timeSinceFootstep = 0;
        }
    }
    
}