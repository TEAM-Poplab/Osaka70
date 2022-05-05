using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramesProfiler : MonoBehaviour
{
    public TMP_Text framesTextField;
    [SerializeField] int _cpuFrameRate;
    private int frameCount;
    [SerializeField] private float frameSampleRate = 0.1f;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    public int CPUFrameRate
    {
        get => _cpuFrameRate;
    }

    public int FrameCount
    {
        get => _cpuFrameRate;
    }

    // Start is called before the first frame update
    void Start()
    {
        frameCount = 0;
        stopwatch.Reset();
        stopwatch.Start();
    }

    private void LateUpdate()
    {
        FrameTimingManager.CaptureFrameTimings();

        ++frameCount;
        float elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;

        if (elapsedSeconds >= frameSampleRate)
        {
            int cpuFrameRate = (int)(1.0f / (elapsedSeconds / frameCount));

            _cpuFrameRate = cpuFrameRate;
            framesTextField.text = cpuFrameRate.ToString();

            // Reset timers.
            frameCount = 0;
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}
