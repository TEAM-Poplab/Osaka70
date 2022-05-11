using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Ludiq;
using Bolt;

/// <summary>
/// This Normcore class manages all variables and behaviours related to the tour system for Osaka (when users are a large number). Normcore model: <see cref="RealtimeNormcoreTourManagerModel"/>
/// </summary>
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
    }

    /// <summary>
    /// Invoked when the floor variable in the model changes and changes some settings according to specific teleport location (for particular scenery)
    /// </summary>
    /// <param name="model"></param>
    /// <param name="floor"></param>
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

    /// <summary>
    /// Invoked when the guide ends the tour: since users cannot use the exit portal as in the free motion experience, they are forced to exit
    /// </summary>
    /// <param name="model"></param>
    /// <param name="enabled"></param>
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

    /// <summary>
    /// Invoked when the teleport system is enabled or disabled by the guide for the visitors, allowing them to freely move aroud the scene
    /// </summary>
    /// <param name="model"></param>
    /// <param name="enabled"></param>
    private void TeleportDidChange(RealtimeNormcoreTourManagerModel model, bool enabled)
    {
        Debug.Log("Toggle: variable changed");
        if (coreManager.GetComponent<Realtime>().clientID != coreManager.GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            if (model.isTeleportEnabled)
            {
                navMeshIn.SetActive(true);
                navMeshOut.SetActive(true);
                Debug.Log("Toggle: variable set to" + Variables.Object(playSpace).Get("isTeleportAllowed"));
            }
            else
            {
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

    /// <summary>
    /// Set the floor to teleport all visitors and the guide to
    /// </summary>
    /// <param name="floor">Floor index</param>
    public void SetFloor(int floor)
    {
        // Set the floor on the model
        // This will fire the floorDidChange event on the model
        if (core.clientID == coreStatus.guideID)
        {
            model.floor = floor;
        }
    }

    /// <summary>
    /// Tells to visitors to exit the experience, while the guide stays in Osaka waiting for other visitors
    /// </summary>
    public void EndTour()
    {
        model.isTourEnded = true;
    }

    /// <summary>
    /// Set the teleport status for all visitors (guide has always teleport enabled)
    /// </summary>
    /// <param name="enabled"></param>
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

    /// <summary>
    /// Coroutine to reset the isTourEnded variable aftar all visitors has exited
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    IEnumerator ResetTourEnding(float timer)
    {
        yield return new WaitForSeconds(timer);
        model.isTourEnded = false;
    }

    public void EnableTeleportLocally()
    {
        navMeshIn.SetActive(true);
        navMeshOut.SetActive(true);
    }

    public void DisableTeleportLocally()
    {
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
