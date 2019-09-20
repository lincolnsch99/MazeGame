using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private int maxStamina;
    [SerializeField]
    private Slider staminaDisplay;
    
    public bool isCrouching;
    public float curStamina;
    public float staminaDrain;
    public float staminaRegen;

    private CharacterController playerControl;
    private DontDestroy PersistentData;
    private float moveSpeed;
    private float footstepOffset, timeSinceFootstep;
    // Start is called before the first frame update
    void Start()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        Cursor.visible = false;
        playerControl = GetComponent<CharacterController>();
        moveSpeed = speed;
        timeSinceFootstep = 0;
        footstepOffset = 0.75f;
        curStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PersistentData.PAUSED)
        {
            if (pauseScreen.activeSelf)
                pauseScreen.SetActive(false);
            timeSinceFootstep += Time.deltaTime;
            moveSpeed = speed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!Input.GetKey(KeyCode.S) && curStamina > 0)
                {
                    moveSpeed *= 1.5f;
                    footstepOffset = 0.3f;
                    curStamina -= staminaDrain * Time.deltaTime;
                    if (normalBreathing.isPlaying)
                    {
                        if (!heavyBreathing.isPlaying)
                        {
                            heavyBreathing.Play();
                            normalBreathing.Stop();
                        }
                    }
                }
            }
            else
                curStamina += staminaRegen * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                moveSpeed *= 0.35f;
                footstepOffset = 1f;
                isCrouching = true;
                if (heavyBreathing.isPlaying)
                {
                    if (!normalBreathing.isPlaying)
                    {
                        normalBreathing.Play();
                        heavyBreathing.Stop();
                    }
                }
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
            movement.y = 0;

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
            else if (moveSpeed < speed && moveSpeed > 0)
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
            staminaDisplay.value = 1f * (curStamina / (float)maxStamina);

            if (timeSinceFootstep > footstepOffset && (Mathf.Abs(movement.x) > 0 || Mathf.Abs(movement.z) > 0))
            {
                footStep.PlayOneShot(footStep.clip);
                timeSinceFootstep = 0;
            }
        }
        else
        {
            if(!pauseScreen.activeSelf)
                pauseScreen.SetActive(true);
        }
    }
    
}