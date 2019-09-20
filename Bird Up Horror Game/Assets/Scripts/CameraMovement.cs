using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float minVert = -45.0f;
    [SerializeField]
    private float maxVert = 45.0f;

    public enum RotationAxis
    {
        MouseX,
        MouseY
    }

    public RotationAxis axis = RotationAxis.MouseX;
    public float sensitivityH = 10.0f;
    public float sensitivityV = 10.0f;
    private float rotationX = 0;
    private DontDestroy PersistentData;

    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PersistentData.PAUSED)
        {
            if (axis == RotationAxis.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityH, 0);
            }
            else if (axis == RotationAxis.MouseY)
            {
                rotationX -= Input.GetAxis("Mouse Y") * sensitivityV;
                rotationX = Mathf.Clamp(rotationX, minVert, maxVert);
                float rotationY = transform.localEulerAngles.y;
                transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
            }
        }
    }
}
