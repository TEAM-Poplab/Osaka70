using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class FollowMeOnEnable : MonoBehaviour
{
    private bool alreadyConnected = false;
    private GameObject normcoreCore;

    private void Start()
    {
        normcoreCore = GameObject.Find("NormcoreManager");
    }

    private void OnEnable()
    {
        if (!alreadyConnected)
        {
            GetComponent<PressableButtonHoloLens2>().ButtonPressed.AddListener(normcoreCore.GetComponent<NavigationSync>().SetPosition);
            alreadyConnected = true;
        }
    }
}
