using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Ludiq;
using Bolt;

public class RealtimeNormcoreTourManager : RealtimeComponent<RealtimeNormcoreTourManagerModel>
{
    public TourManager tourManager;
    private Realtime core;
    private GameObject coreManager;
    private NormcoreStatusSaverManager coreStatusSaver;
    private RealtimeNormcoreStatus coreStatus;
    public UIManagerForUserMenuMRTKWithoutButtonsOsaka UIManager;
    public GameObject portalToStart;
    public GameObject playSpace;
    public GameObject navMeshIn;
    public GameObject navMeshOut;

    private void Start()
    {
        coreManager = GameObject.Find("NormcoreManager");
        core = coreManager.GetComponent<Realtime>();
        coreStatusSaver = coreManager.GetComponent<NormcoreStatusSaverManager>();
        coreStatus = coreManager.GetComponent<RealtimeNormcoreStatus>();
        //_uiManager = GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>();
        //playSpace = GameObject.Find("MixedRealityPlayspace");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FloorDidChange(RealtimeNormcoreTourManagerModel model, int floor)
    {
        if (core.clientID == coreStatus.guideID)
        {
            UIManager.TeleportTour(tourManager.GetTransformFromIndexAndFloor(15, floor));
        }
        if (core.clientID != coreStatus.guideID)
        {
            UIManager.TeleportTour(tourManager.GetTransformFromIndexAndFloor(core.clientID, floor));
            Debug.Log("Teleporting to new floor: " + tourManager.floorsVisitorsSpotsGroup[floor].transform.GetChild(core.clientID).position);
        }

        switch (floor)
        {
            case 5:               
                StartCoroutine(SetSunPosition(1.15f));  //This timer is the sum of WaitForSecond in UIManager.TeleportTour()
                break;
            default:
                StartCoroutine(ResetShadowDistance(1.15f));
                break;
        }
    }

    private void IsTourEndedDidChange(RealtimeNormcoreTourManagerModel model, bool enabled)
    {
        GameObject[] users = GameObject.FindGameObjectsWithTag("NormcorePlayerHead");
        if (enabled)
        {
            if (core.clientID != coreStatus.guideID)
            {
                CustomEvent.Trigger(GameObject.FindGameObjectWithTag("Fade"), "Exit", portalToStart);
                Debug.Log("ExitCalled");
            } else
            {
                StartCoroutine(ResetTourEnding(2f));
            }
            foreach(GameObject visitor in users)
            {
                CustomEvent.Trigger(core.gameObject, "UserExit", visitor.GetComponent<RealtimeView>().ownerIDInHierarchy);
            }
        }
    }

    private void TeleportDidChange(RealtimeNormcoreTourManagerModel model, bool enabled)
    {
        Debug.Log("Toggle: variable changed");
        if (coreManager.GetComponent<Realtime>().clientID != coreManager.GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            //GameObject fade = (GameObject)Variables.ActiveScene.Get("Fade");
            if (model.isTeleportEnabled)
            {
                //CustomEvent.Trigger(playSpace, "EnableTeleport");
                //Variables.Object(playSpace).Set("isTeleportAllowed", true);
                navMeshIn.SetActive(true);
                navMeshOut.SetActive(true);
                Debug.Log("Toggle: variable set to" + Variables.Object(playSpace).Get("isTeleportAllowed"));
            }
            else
            {
                //CustomEvent.Trigger(playSpace, "DisableTeleport");
                //Variables.Object(playSpace).Set("isTeleportAllowed", false);
                navMeshIn.SetActive(false);
                navMeshOut.SetActive(false);
                Debug.Log("Toggle: variable set to" + Variables.Object(playSpace).Get("isTeleportAllowed"));
            }
        }
        else Debug.Log("Toggle: guide ignores variable change");
    }

    protected override void OnRealtimeModelReplaced(RealtimeNormcoreTourManagerModel previousModel, RealtimeNormcoreTourManagerModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.floorDidChange -= FloorDidChange;
            previousModel.isTourEndedDidChange -= IsTourEndedDidChange;
            previousModel.isTeleportEnabledDidChange -=TeleportDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.floor = 0; //spawn point
                currentModel.isTourEnded = false;
                currentModel.isTeleportEnabled = false;
            }

            // Register for events
            currentModel.floorDidChange += FloorDidChange;
            currentModel.isTourEndedDidChange += IsTourEndedDidChange;
            currentModel.isTeleportEnabledDidChange += TeleportDidChange;
        }
    }

    public void SetFloor(int floor)
    {
        // Set the floor on the model
        // This will fire the floorDidChange event on the model
        if (core.clientID == coreStatus.guideID)
        {
            model.floor = floor;
        }
    }

    public void EndTour()
    {
        model.isTourEnded = true;
    }

    public void ChangeTeleport(bool enabled)
    {
        model.isTeleportEnabled = enabled;
    }

    public bool GetTeleportStatus()
    {
        return model.isTeleportEnabled;
    }

    /*
     * Register new user ID with index for fixed points travelling
     */
    public void RegisterNewVisitor()
    {
        //UserIDModel newUser = new UserIDModel();
        //newUser.userID = GetComponent<Realtime>().clientID;
        //newUser.userIDCurrentScene = "Osaka";
        //model.visitorsID.Add(newUser);
        tourManager.SetLocalVisitorIndex(core.clientID);
    }

    IEnumerator ResetTourEnding(float timer)
    {
        yield return new WaitForSeconds(timer);
        model.isTourEnded = false;
    }

    public void EnableTeleportLocally()
    {
        //CustomEvent.Trigger(playSpace, "EnableTeleport");
        //Variables.Object(playSpace).Set("isTeleportAllowed", true);
        navMeshIn.SetActive(true);
        navMeshOut.SetActive(true);
    }

    public void DisableTeleportLocally()
    {
        //CustomEvent.Trigger(playSpace, "DisableTeleport");
        //Variables.Object(playSpace).Set("isTeleportAllowed", false);
        navMeshIn.SetActive(false);
        navMeshOut.SetActive(false);
    }

    IEnumerator SetSunPosition(float timer)
    {
        yield return new WaitForSeconds(timer);
        QualitySettings.shadowDistance = 65;
        GameObject.Find("TimeManager").GetComponent<CustomLightManagerForOsaka>().SetSunRotation(new Vector3(280, -90, 0)); //Midnight
    }

    IEnumerator ResetShadowDistance(float timer)
    {
        yield return new WaitForSeconds(timer);
        QualitySettings.shadowDistance = 36;
    }
}
