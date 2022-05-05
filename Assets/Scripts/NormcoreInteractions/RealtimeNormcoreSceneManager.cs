using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;

public class RealtimeNormcoreSceneManager : RealtimeComponent<RealtimeNormcoreSceneManagerModel>
{
    public int osakaConnectedUsers = 0;
    public int loadingSceneConnectedUsers = 0;
    public int loadingScenePostOsakaConnectedUsers = 0;

    private UIManagerForUserMenuMRTKWithoutButtonsOsaka UIManagerOsaka;

    [SerializeField]
    private string _currentScene;

    [SerializeField]
    private string _previousScene;

    public string currentScene
    {
        get => _currentScene;
    }

    public string previousScene
    {
        get => _previousScene;
    }

    public int OsakaConnectedUsers
    {
        get => model.osakaConnectedUSers;
    }

    public int LoadingScreenMainConnectedUsers
    {
        get => model.loadingScreenMainConnectedUsers;
    }

    public int LoadingScreenSecondaryConnectedUsers
    {
        get => model.loadingScreenSecondaryConnectedUsers;
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        GetComponent<Realtime>().didConnectToRoom += RealtimeNormcoreSceneManager_didConnectToRoom;
        GetComponent<Realtime>().didDisconnectFromRoom += RealtimeNormcoreSceneManager_didDisconnectFromRoom;
        GetComponent<RealtimeAvatarManager>().avatarDestroyed += RealtimeNormcoreSceneManager_avatarDestroyed;
    }

    private void RealtimeNormcoreSceneManager_didDisconnectFromRoom(Realtime realtime)
    {
        if (currentScene == "Osaka")
        {
            GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetButtonsVisibility(false);
        }
    }

    private void RealtimeNormcoreSceneManager_avatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {     
        if (RemoveUserSceneByID(avatar.realtimeView.ownerIDInHierarchy)) {
            if (avatar.realtimeView.ownerIDInHierarchy == GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                GetComponent<RealtimeNormcoreStatus>().SetGuideID(-1);
                GetComponent<RealtimeNormcoreStatus>().SetIsGuide(false);
            }
        }
    }

    private void RealtimeNormcoreSceneManager_didConnectToRoom(Realtime realtime)
    {
        AddUserSceneSelf();

        if (_currentScene == "LoadingScene")
        {
            AddUserSelfToQueue();
            IncreaseMainLoadingSceneUsers();
        } else if (_currentScene == "Osaka")
        {
            IncreaseOsakaUsers();
        }
    }

    protected override void OnRealtimeModelReplaced(RealtimeNormcoreSceneManagerModel previousModel, RealtimeNormcoreSceneManagerModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.loadingScreenMainConnectedUsersDidChange -= CurrentModel_loadingSceneMainConnectedUsersDidChange;
            previousModel.loadingScreenSecondaryConnectedUsersDidChange -= CurrentModel_loadingScreenSecondaryConnectedUsersDidChange;
            previousModel.osakaConnectedUSersDidChange -= CurrentModel_osakaConnectedUSersDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it,
            if (currentModel.isFreshModel)
            {
                currentModel.loadingScreenMainConnectedUsers = 0;   //The user who starts the connection
                currentModel.loadingScreenSecondaryConnectedUsers = 0;
                currentModel.osakaConnectedUSers = 0;
            }

            currentModel.loadingScreenMainConnectedUsersDidChange += CurrentModel_loadingSceneMainConnectedUsersDidChange;
            currentModel.loadingScreenSecondaryConnectedUsersDidChange += CurrentModel_loadingScreenSecondaryConnectedUsersDidChange;
            currentModel.osakaConnectedUSersDidChange += CurrentModel_osakaConnectedUSersDidChange;

            //currentModel.loadingScreenMainConnectedUsers = currentModel.loadingScreenMainConnectedUsers + 1; //At the beginning, users will always be in the loading scene

            osakaConnectedUsers = currentModel.osakaConnectedUSers;
            loadingSceneConnectedUsers = currentModel.loadingScreenMainConnectedUsers;
            loadingScenePostOsakaConnectedUsers = currentModel.loadingScreenSecondaryConnectedUsers;

            _currentScene = SceneManager.GetActiveScene().name;
        }
    }

    private void CurrentModel_osakaConnectedUSersDidChange(RealtimeNormcoreSceneManagerModel model, int value)
    {
        Debug.Log("New user connected to LoadingScreen scene! " + value + " users currently connected.");
        osakaConnectedUsers = value;
        if (realtime.clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            if (_currentScene == "Osaka")
            {
                UIManagerOsaka.UpdateOsakaUsers(osakaConnectedUsers);
            }
        }
    }

    private void CurrentModel_loadingScreenSecondaryConnectedUsersDidChange(RealtimeNormcoreSceneManagerModel model, int value)
    {
        Debug.Log("New user connected to LoadingScreen scene! " + value + " users currently connected.");
        loadingScenePostOsakaConnectedUsers = value;

        //Check if the user count in the loading scene is > 0 and if current user is the guide
       if (realtime.clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            if (_currentScene == "Osaka")
            {
                UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);

                if (loadingScenePostOsakaConnectedUsers + loadingSceneConnectedUsers > 0)
                {
                    GetComponent<RealtimeNormcoreStatus>().ResetGuideIsReady();
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(true);
                }
                else
                {
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(false);
                    ClearQueue();
                }
            }
       }        
    }

    private void CurrentModel_loadingSceneMainConnectedUsersDidChange(RealtimeNormcoreSceneManagerModel model, int value)
    {
        Debug.Log("New user connected to Osaka scene! " + value + " users currently connected.");
        loadingSceneConnectedUsers = value;

        //Check if the user count in the loading scene is > 0 and if current user is the guide
        if (realtime.clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            if (_currentScene == "Osaka")
            {
                UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);

                if (loadingScenePostOsakaConnectedUsers + loadingSceneConnectedUsers > 0)
                {
                    GetComponent<RealtimeNormcoreStatus>().ResetGuideIsReady();
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetRecallButtonVisbility(true);
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(true);
                }
                else
                {
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetRecallButtonVisbility(false);
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(false);
                    ClearQueue();
                }
            }
        }
    }

    /// <summary>
    /// Handles the proper setting of normcore manager and any related setting when the current scene changes
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        _previousScene = _currentScene;
        _currentScene = arg1.name;

        UpdateUserSceneSelf();

        if (_currentScene == "Osaka")
        {
            UIManagerOsaka = GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>();
            IncreaseOsakaUsers();

            //Setting tour manager for normcore
            GetComponent<RealtimeNormcoreTourManager>().UIManager = UIManagerOsaka;
            GetComponent<RealtimeNormcoreTourManager>().portalToStart = GameObject.Find("PortalToStart");
            GetComponent<RealtimeNormcoreTourManager>().playSpace = GameObject.Find("MixedRealityPlayspace");
            GetComponent<RealtimeNormcoreTourManager>().navMeshIn = GameObject.Find("NavMesh");
            GetComponent<RealtimeNormcoreTourManager>().navMeshOut = GameObject.Find("NavMeshOutside");

            GetComponent<RealtimeNormcoreTourManager>().tourManager = GameObject.Find("TourManager").GetComponent<TourManager>();
            GetComponent<RealtimeNormcoreTourManager>().RegisterNewVisitor();

            if (_previousScene == "LoadingScene")
            {
                DecreaseMainLoadingSceneUsers(); 
                if (GetComponent<Realtime>().clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
                {
                    RemoveUserSelfFromQueue();
                }
            }
            
            if (_previousScene == "LoadingScenePostOsaka")
            {
                DecreasePostLoadingSceneUsers();
            }

            GameObject.Find("GameManager").GetComponent<GameManagerOsaka>().NormcoreCore_didConnectToRoom(GetComponent<Realtime>());    //It sets the spawn position according to user ID and users number
            if (GetComponent<Realtime>().clientID == GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetButtonsVisibility(true);
                if (loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers > 0)
                {
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetRecallButtonVisbility(true);
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(true);
                }
                else
                {
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetRecallButtonVisbility(false);
                    //TODO: renable when fixed
                    //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(false);
                }
            }
            else
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetButtonsVisibility(false);
                //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetRecallButtonVisbility(false);
                //TODO: renable when fixed
                //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().SetNewRecallFollomeButtonVisbility(false);

                //Tour Manager addition for managing large groups of visitors in a time-limited and space-defined experience
                GetComponent<RealtimeNormcoreTourManager>().tourManager = GameObject.Find("TourManager").GetComponent<TourManager>();
                if (GetComponent<RealtimeNormcoreTourManager>().tourManager.isTourEnabled)
                {

                }
            }
        }

        if (_currentScene == "LoadingScenePostOsaka")
        {
            IncreasePostLoadingSceneUsers();
            AddUserSelfToQueue();

            GetComponent<RealtimeNormcoreStatus>().ReconnectEvent();

            if (_previousScene == "Osaka")
            {
                DecreaseOsakaUsers();
            }

            if (GetComponent<Realtime>().clientID != GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().GuideMenuHandler(false);
            }
            else
            {
                //GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKLoadingScene>().ReSetGuideButton();
            }

            GameObject.Find("ScenesManager").GetComponent<ScenesManager>().LoadLevel("Osaka");
        }
    }

    public void DecreaseOsakaUsers()
    {
        if (osakaConnectedUsers >= 1)
        {
            model.osakaConnectedUSers = osakaConnectedUsers - 1;
            //osakaConnectedUsers--;
        }            
        //UIManagerOsaka.UpdateOsakaUsers(osakaConnectedUsers);
    }

    public void IncreaseOsakaUsers()
    {
        model.osakaConnectedUSers = osakaConnectedUsers + 1;
        //osakaConnectedUsers++;
        //UIManagerOsaka.UpdateOsakaUsers(osakaConnectedUsers);
    }

    public void DecreaseMainLoadingSceneUsers()
    {
        if (loadingSceneConnectedUsers >= 1)
        {
            model.loadingScreenMainConnectedUsers = loadingSceneConnectedUsers - 1;
            //loadingSceneConnectedUsers--;
        }
        //UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);

    }

    public void IncreaseMainLoadingSceneUsers()
    {
        model.loadingScreenMainConnectedUsers = loadingSceneConnectedUsers + 1;
        //loadingSceneConnectedUsers++;
        //UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);
    }

    public void DecreasePostLoadingSceneUsers()
    {
        if (loadingScenePostOsakaConnectedUsers >= 1)
        {
            model.loadingScreenSecondaryConnectedUsers = loadingScenePostOsakaConnectedUsers - 1;
            //loadingScenePostOsakaConnectedUsers--;
        }
        //UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);
    }

    public void IncreasePostLoadingSceneUsers()
    {
        model.loadingScreenSecondaryConnectedUsers = loadingScenePostOsakaConnectedUsers + 1;
        //loadingScenePostOsakaConnectedUsers++;
        //UIManagerOsaka.UpdateWaitingUsers(loadingSceneConnectedUsers + loadingScenePostOsakaConnectedUsers);
    }

    private void AddUserSceneSelf()
    {
        UserIDModel newUser = new UserIDModel();
        newUser.userID = GetComponent<Realtime>().clientID;
        newUser.userIDCurrentScene = _currentScene;
        model.usersCurrentScene.Add((uint) GetComponent<Realtime>().clientID, newUser);
    }

    private void RemoveUserSceneSelf()
    {
        model.usersCurrentScene.Remove((uint)GetComponent<Realtime>().clientID);
    }

    private bool RemoveUserSceneByID(int id)
    {
        try
        {
            UserIDModel user = model.usersCurrentScene[(uint)id];
            switch (user.userIDCurrentScene)
            {
                case "Osaka":
                    DecreaseOsakaUsers();
                    break;
                case "LoadingScene":
                    DecreaseMainLoadingSceneUsers();
                    break;
                case "LoadingScenePostOsaka":
                    DecreasePostLoadingSceneUsers();
                    break;
                default:
                    break;
            }
            model.usersCurrentScene.Remove((uint)id);
            return true;
        }
        catch (KeyNotFoundException e)
        {
            //User already removed from model
            return false;
        }
    }

    public string GetSceneFromUserID(int id)
    {
        var sceneName = model.usersCurrentScene[(uint)id].userIDCurrentScene;
        return sceneName;
    }

    private void UpdateUserSceneSelf()
    {
        model.usersCurrentScene[(uint)GetComponent<Realtime>().clientID].userIDCurrentScene = _currentScene;
    }

    private void AddUserSelfToQueue()
    {
        UserIDModel newUser = new UserIDModel();
        newUser.userID = GetComponent<Realtime>().clientID;
        newUser.userIDCurrentScene = _currentScene;
        model.aboutToConnectQueue.Add(newUser);
    }

    public List<int> UsersQueue()
    {
        List<int> users = new List<int>();
        foreach (UserIDModel el in model.aboutToConnectQueue)
        {
            users.Add(el.userID);
        }
        return users;
    }

    private void ClearQueue()
    {
        foreach(UserIDModel el in model.aboutToConnectQueue)
        {
            model.aboutToConnectQueue.Remove(el);
        }
    }

    private void RemoveUserSelfFromQueue()
    {
        foreach (UserIDModel el in model.aboutToConnectQueue)
        {
            if (el.userID == GetComponent<Realtime>().clientID)
            {
                model.aboutToConnectQueue.Remove(el);
            }
        }
    }
}
