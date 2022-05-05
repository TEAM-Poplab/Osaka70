using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TourGuideClock : MonoBehaviour
{
    public TMP_Text minutes;
    public TMP_Text seconds;
    private float elapsedTime = 0;
    private bool isPaused = false;
    private float _minutes;
    private float _seconds;
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime * speed;
            _minutes = elapsedTime / 60f;
            _seconds = elapsedTime % 60f;

            if (Mathf.FloorToInt(_minutes) < 10 )
                minutes.text = string.Format("0{0} min", Mathf.FloorToInt(_minutes));
            else
                minutes.text = string.Format("{0} min", Mathf.FloorToInt(_minutes));

            if (Mathf.FloorToInt(_seconds) < 10)
                seconds.text = string.Format("0{0} sec", Mathf.FloorToInt(_seconds));
            else
                seconds.text = string.Format("{0} sec", Mathf.FloorToInt(_seconds));
        }
    }

    public void ResetStopwatch()
    {
        elapsedTime = 0;
    }
}
