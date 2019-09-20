using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField]
    private Text timerDisplay;
    private DontDestroy PersistentData;
    private int timeLeft;
    private float timeElapsed;
    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        timeLeft = PersistentData.countdownTime;
        timerDisplay.text = timeLeft.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > 1)
        {
            timeLeft -= 1;
            timeElapsed = 0;
            timerDisplay.text = timeLeft.ToString();
            StartCoroutine(Shake(0.2f, 5f));
        }
        if(timeLeft < 0)
        {
            timerDisplay.text = "0";
            PersistentData.GameLost();
        }
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 curPos = transform.localPosition;
        float timer = 0.0f;

        while(timer < duration)
        {
            float randX = Random.Range(-1f, 1f) * magnitude;
            float randY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(curPos.x + randX, curPos.y + randY, curPos.z);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = curPos;
    }
}
