using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;

public class NormcoreStatusSaverManager : MonoBehaviour
{
    //public GameObject currentSceneNormcoreManager;

    private bool isGuide = false;

    private bool osakaLoaded;
    private int internalClientID;
    
    public int clientID
    {
        get => internalClientID;
        private set => internalClientID = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        GetComponent<Realtime>().didConnectToRoom += NormcoreStatusSaverManager_didConnectToRoom;
    }

    /// <summary>
    /// Handles the proper setting of normcore manager and any related setting when the current scene changes
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1 == SceneManager.GetSceneByName("Osaka"))
        {
            GameObject.Find("GameManager").GetComponent<GameManagerOsaka>().NormcoreCore_didConnectToRoom(GetComponent<Realtime>());
            if (GetComponent<Realtime>().clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetButtonsVisibility(true);
            } else
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetButtonsVisibility(true);
            }
        }

        if (arg1.name == "LoadingScene")
        {
            if (GetComponent<Realtime>().clientID != GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(false);
            }         
        }
    }

    private void NormcoreStatusSaverManager_didConnectToRoom(Realtime realtime)
    {
        internalClientID = realtime.clientID;
    }
}
