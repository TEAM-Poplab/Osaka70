using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTJ.Alembic;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(AlembicStreamPlayer))]
public class TimelineController : MonoBehaviour
{
    private AlembicStreamPlayer alembicPlayerScript;

    private void Start()
    {
        alembicPlayerScript = GetComponent<AlembicStreamPlayer>();
    }

    public void OnValueChange(SliderEventData eventData)
    {
        alembicPlayerScript.currentTime = Mathf.Lerp((float)alembicPlayerScript.startTime, (float)alembicPlayerScript.endTime, eventData.NewValue);
    }
}
