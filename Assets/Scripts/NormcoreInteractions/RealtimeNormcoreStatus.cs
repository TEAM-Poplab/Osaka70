using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RealtimeNormcoreStatus : RealtimeComponent<RealtimeNormcoreStatusModel>
{
    private Realtime realtime;
    private int _guideID = -1;
    private Coroutine cooldown = null;

    public int guideID
    {
        get => _guideID;
    }

    [Space(7)]
    [Tooltip("Events fired by the client when they are about to connect to Osaka scene")] public UnityEvent onGuideIsReady = new UnityEvent();
    [Tooltip("Events fired by the client when new incoming clients are about to connect to Osaka scene")] public UnityEvent onNewUsersConnecting = new UnityEvent();
    [Tooltip("Events fired by the client when the guide is about to connect back to Osaka scene")] public UnityEvent onGuideIsConnecting = new UnityEvent();

    private void Start()
    {
        realtime = GetComponent<Realtime>();
    }
    protected override void OnRealtimeModelReplaced(RealtimeNormcoreStatusModel previousModel, RealtimeNormcoreStatusModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.isGuideDidChange -= CurrentModel_isGuideDidChange;
            previousModel.guideIsReadyDidChange -= CurrentModel_guideIsReadyDidChange;
            previousModel.guideIDDidChange -= CurrentModel_guideIDDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it,
            if (currentModel.isFreshModel)
            {
                currentModel.isGuide = false;
                currentModel.guideIsReady = false;
                currentModel.guideID = -1;
            }

            currentModel.isGuideDidChange += CurrentModel_isGuideDidChange;
            currentModel.guideIsReadyDidChange += CurrentModel_guideIsReadyDidChange;
            currentModel.guideIDDidChange += CurrentModel_guideIDDidChange;

            UpdateGuideID();
            if (_guideID == -1)
            {
                if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
                {
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(true);
                }
                else if (SceneManager.GetActiveScene().name == "Osaka")
                {
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(true);
                }
                Debug.Log("CurrentModel_isGuideDidChange: secret menu endabled");
            }
            else
            {
                if (realtime.clientID != _guideID)
                {
                    if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
                    {
                        //After an user typed the right password, they gain the guide privileges and any other user won't dislay the password menu while waiting
                        GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(false);
                    }
                    else if (SceneManager.GetActiveScene().name == "Osaka")
                    {
                        //Event to announce other players in Osaka that guide is coming back
                        onGuideIsConnecting.Invoke();
                        GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(false);
                    }
                    Debug.Log("CurrentModel_isGuideDidChange: secret menu hidden");
                }
            }
        }
    }

    private void CurrentModel_guideIDDidChange(RealtimeNormcoreStatusModel model, int value)
    {
        UpdateGuideID();

        if (value == -1)
        {
            if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(true);
            }
            else if (SceneManager.GetActiveScene().name == "Osaka")
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(true);
            }
            Debug.Log("CurrentModel_isGuideDidChange: secret menu endabled");
        } else
        {
            if (realtime.clientID != value)
            {
                if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
                {
                    //After an user typed the right password, they gain the guide privileges and any other user won't dislay the password menu while waiting
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(false);
                }
                else if (SceneManager.GetActiveScene().name == "Osaka")
                {
                    //Event to announce other players in Osaka that guide is coming back
                    onGuideIsConnecting.Invoke();
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(false);
                }
                Debug.Log("CurrentModel_isGuideDidChange: secret menu hidden");
            }
            else if (realtime.clientID == value)
            {   
                //Event to announce other players in Osaka that guide is coming back
                onGuideIsConnecting.Invoke();
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(false);
            } else
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().UpdateOsakaUsers(GetComponent<RealtimeNormcoreSceneManager>().osakaConnectedUsers);
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().UpdateWaitingUsers(GetComponent<RealtimeNormcoreSceneManager>().loadingSceneConnectedUsers + GetComponent<RealtimeNormcoreSceneManager>().loadingScenePostOsakaConnectedUsers);
            }
        }
    }

    private void CurrentModel_guideIsReadyDidChange(RealtimeNormcoreStatusModel model, bool value)
    {
        if (realtime.clientID != model.guideID && value)
        {
            if (GetComponent<RealtimeNormcoreSceneManager>().currentScene == "Osaka")
                onNewUsersConnecting.Invoke();
            if (GetComponent<RealtimeNormcoreSceneManager>().currentScene == "LoadingScene" || GetComponent<RealtimeNormcoreSceneManager>().currentScene == "LoadingScenePostOsaka")
                if (cooldown == null)
                    cooldown = StartCoroutine(CooldownBeforeEnter(1, 1, realtime.clientID*1f));
        }
    }

    /// <summary>
    /// Invoked when the guide has been assigned or revoked, in order to immeditely hide or show specific menus...
    /// </summary>
    /// <param name="model"></param>
    /// <param name="value"></param>
    private void CurrentModel_isGuideDidChange(RealtimeNormcoreStatusModel model, bool value)
    {
        if (value)
        {
            if (realtime.clientID != model.guideID)
            {
                if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
                {
                    //After an user typed the right password, they gain the guide privileges and any other user won't dislay the password menu while waiting
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(false);
                } else if (SceneManager.GetActiveScene().name == "Osaka")
                {
                    //Event to announce other players in Osaka that guide is coming back
                    onGuideIsConnecting.Invoke();
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(false);
                }
                Debug.Log("CurrentModel_isGuideDidChange: secret menu hidden");
            }
        } else
        {
            if (realtime.clientID != model.guideID)
            {
                if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "LoadingScenePostOsaka")
                {
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(true);
                }
                else if (SceneManager.GetActiveScene().name == "Osaka")
                {
                    GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SecretMenuHandler(true);
                }
                Debug.Log("CurrentModel_isGuideDidChange: secret menu endabled");
            }
        }
    }

    public void SetGuideID(int id)
    {
        model.guideID = id;
        _guideID = id;
        Debug.Log($"SetGuideID: guideID changed. New ID is {_guideID}");
    }

    private void UpdateGuideID()
    {
            _guideID = model.guideID;   
    }

    /// <summary>
    /// Guide is ready is used when the recall button is press to trigger the entrance of any waiting user
    /// </summary>
    public void SetGuideIsReady()
    {
        if (model.guideID != -1)
        {
            model.guideIsReady = true;
        }
    }

    public void ResetGuideIsReady()
    {
        if (model.guideID != -1)
        {
            model.guideIsReady = false;
        }
    }

    /// <summary>
    /// Is guide is used to determine if any user is a guide, i.e, in the room a guide has been assigned
    /// </summary>
    /// <returns></returns>
    public bool GetIsGuide() => model.isGuide;
    public void SetIsGuide(bool val) => model.isGuide = val;

    public void ReconnectEvent()
    {
        onGuideIsReady.AddListener(() => GameObject.Find("MixedRealityPlayspace").GetComponent<Animator>().SetTrigger("Ascend"));
        onGuideIsReady.AddListener(() => GameObject.Find("Fade").GetComponent<Animator>().SetTrigger("LoadingOsaka"));
        onGuideIsReady.AddListener(() => GameObject.Find("GuardianCenter/Platform").GetComponent<Animator>().SetTrigger("Ascend"));
        onGuideIsReady.AddListener(() => GameObject.Find("GuardianCenter/OOBSphere").GetComponent<Animator>().SetTrigger("Ascend"));
        //onGuideIsReady.AddListener(() => GameObject.Find("ScenesManager").GetComponent<ScenesManager>().LoadLevel("Osaka"));
        //onGuideIsReady.AddListener(GameObject.Find("ScenesManager").GetComponent<ScenesManager>().ActivateScene);
    }

    IEnumerator CooldownBeforeEnter(int totalTimer, int timeStep = 1, float delayBeforeStart = 0)
    {
        AudioSource aus = GameObject.Find("Countdown source").GetComponent<AudioSource>();
        int integer = 0;
        //We noticed that when there are multiple users, user counter is not properly updated because of concurrent access to same variable in the datastore, so we put a delay different for each user before they enter
        yield return new WaitForSeconds(delayBeforeStart);
        while (integer < totalTimer)
        {
            //tick code
            if (aus.isPlaying)
            {
                aus.Stop();
            }
            aus.Play();

            integer += 1;
            yield return new WaitForSeconds(timeStep);
        }
        onGuideIsReady.Invoke();
        cooldown = null;
    }
}
