using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 startPosition;
    private GameObject player;

    [Range(-1f, 1f)]
    public float a, t;

    public float timeSinceStart = 0;

    [Header("Fitness")]
    public float overallFitness;
    public float proximityMultiplier = 1.4f;
    public float collisionMultiplier = 0.6f;
    public float speedModifier = 0.4f;
    public float sensorMultiplier = 0.1f;

    private float playerProximity;
    private float timeCompleted;
    private float numCollisions;
    private float sensorDistance = 15f;
    private float aSensor, bSensor, cSensor;
    private MazeGenerator mazeInfo;

    private NeuralNetwork net;
    private bool initialized = false;

    private void Awake()
    {
        mazeInfo = GameObject.FindWithTag("Maze").GetComponent<MazeGenerator>();
        transform.position = (new Vector3(6f * (mazeInfo.numColumns - 1), 3, 6f * (mazeInfo.numRows - 1)));
        startPosition = transform.position;
        player = GameObject.FindWithTag("Player");
    }

    public void Reset()
    {
        timeSinceStart = 0;
        timeCompleted = 0;
        numCollisions = 0;
        playerProximity = 0;
        overallFitness = 0;
        transform.position = startPosition;
    }

    private Vector3 input;
    public void Move(float v, float h)
    {
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.localEulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Structure")
            numCollisions++;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Structure")
            numCollisions++;
    }

    private void InputSensors()
    {
        Vector3 a = transform.forward + transform.right;
        Vector3 b = transform.forward;
        Vector3 c = transform.forward - transform.right;

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            aSensor = hit.distance / sensorDistance;
        }

        r.direction = b;
        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 75f;
        }

        r.direction = c;
        if (Physics.Raycast(r, out hit))
        {
            cSensor = hit.distance / sensorDistance;
        }
    }

    private void CalculateFitness()
    {
        playerProximity = Vector3.Distance(player.transform.position, transform.position);

        overallFitness = ((1 / playerProximity) * proximityMultiplier) + (timeCompleted * 0.4f) + ((1 / numCollisions) * collisionMultiplier)
            + (((aSensor + bSensor + cSensor) / 3) * sensorMultiplier);
    }

    private void FixedUpdate()
    {
        if (initialized)
        {
            InputSensors();
            // Neural network code here

            timeSinceStart += Time.deltaTime;
            //CalculateFitess();
            float[] inputs = new float[1];

            float angle = transform.eulerAngles.z % 360f;
            if (angle < 0f)
                angle += 360f;

            Vector2 deltaVector = (player.transform.position - transform.position).normalized;

            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
            rad *= Mathf.Rad2Deg;

            rad = rad % 360;
            if (rad < 0)
            {
                rad = 360 + rad;
            }
            rad = 90f - rad;
            if (rad < 0f)
            {
                rad += 360f;
            }
            rad = 360 - rad;
            rad -= angle;
            if (rad < 0)
                rad = 360 + rad;
            if (rad >= 180f)
            {
                rad = 360 - rad;
                rad *= -1f;
            }
            rad *= Mathf.Deg2Rad;
            inputs[0] = rad / (Mathf.PI);

            float[] output = net.FeedForward(inputs);

            a = 0.75f;
            t = output[0];
            Move(a, t);
            CalculateFitness();
            net.AddFitness(overallFitness);


            a = t = 0;
        }

    }

    public void Init(NeuralNetwork net, Transform player)
    {
        this.net = net;
        initialized = true;
    }

}
