/************************************************************************************
* 
* Class Purpose: singleton class which controls any UI related events
*
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit;
using Normal.Realtime;
using UnityEngine.SceneManagement;

public class UIManagerForUserMenuMRTKLoadingScene : Singleton<UIManagerForUserMenuMRTKLoadingScene>
{
    //[SerializeField] private Canvas _quitMenu;
    private GameObject diagnostics = null;
    private Vector3 guardianCenterPosition;

    public GameObject GuideButton;

    private GameObject normcoreManager;

    private bool osakaLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        guardianCenterPosition = GameObject.Find("GuardianCenter").transform.position;
        normcoreManager = GameObject.Find("NormcoreManager");
        normcoreManager.GetComponent<Realtime>().didConnectToRoom += UIManagerForUserMenuMRTKLoadingScene_didConnectToRoom;
    }

    private void UIManagerForUserMenuMRTKLoadingScene_didConnectToRoom(Realtime realtime)
    {
        //if guideID != -1 it means the user connected to a room where other users are visiting osaka
        if (normcoreManager.GetComponent<RealtimeNormcoreStatus>().guideID != -1)
        {
            GuideMenuHandler(false);
        }
        if (!osakaLoaded)
        {
            GameObject.Find("ScenesManager").GetComponent<ScenesManager>().LoadLevel("Osaka");
            osakaLoaded = true;
        }
        //If the guide crashes, and then comes back to the app, but someone is still in Osaka, the guide is the guide again and the secret menu should be visibile again ONLY for them
        if (normcoreManager.GetComponent<Realtime>().clientID == normcoreManager.GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            GuideMenuHandler(true);
        }
    }

    private void Update()
    {
        if (diagnostics == null)
        {
            diagnostics = GameObject.Find("MixedRealityPlayspace/Diagnostics");
            diagnostics.SetActive(false);
        }
    }

    public void DiagnosticHandler()
    {
        switch (diagnostics.activeSelf)
        {
            case true:
                diagnostics.SetActive(false);
                break;
            case false:
                diagnostics.SetActive(true);
                break;
        }
    }

    public void GuideMenuHandler(bool status)
    {
        GuideButton.SetActive(status);
    }

    /// <summary>
    /// This method assign the guide authority to the user who confirmed the right password
    /// </summary>
    public void GuideMenuOnPress()
    {
        var manager = GameObject.Find("NormcoreManager");

        if (manager.GetComponent<RealtimeNormcoreStatus>().guideID == -1)
        {
            manager.GetComponent<RealtimeNormcoreStatus>().SetGuideID(manager.GetComponent<Realtime>().clientID);
            //manager.GetComponent<RealtimeNormcoreStatus>().SetIsGuide(true);
        }

        //if (manager.GetComponent<Realtime>().clientID == manager.GetComponent<RealtimeNormcoreStatus>().guideID)
        //{
        //    manager.GetComponent<RealtimeNormcoreStatus>().SetIsGuide(true);
        //}
    }

    //public void ReSetGuideButton()
    //{
    //    GuideButton.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<PressableButtonHoloLens2>().ButtonPressed.AddListener(
    //        () => GameObject.Find("ScenesManager").GetComponent<ScenesManager>().LoadLevel("Osaka"));
    //}

    /// <summary>
    /// This method assigns listeners to the event fired when the input password is right, allowing the activation of scene switching to main Osaka scene
    /// </summary>
    public void ReSetGuideButton()
    {
        GuideButton.GetComponent<PasswordManager>().onPasswordIsRight.AddListener(() => GameObject.Find("MixedRealityPlayspace").GetComponent<Animator>().SetTrigger("Ascend"));
        GuideButton.GetComponent<PasswordManager>().onPasswordIsRight.AddListener(() => GameObject.Find("Fade").GetComponent<Animator>().SetTrigger("LoadingOsaka"));
        GuideButton.GetComponent<PasswordManager>().onPasswordIsRight.AddListener(() => GameObject.Find("GuardianCenter/Platform").GetComponent<Animator>().SetTrigger("Ascend"));
        GuideButton.GetComponent<PasswordManager>().onPasswordIsRight.AddListener(() => GameObject.Find("GuardianCenter/OOBSphere").GetComponent<Animator>().SetTrigger("Ascend"));
    }
}
