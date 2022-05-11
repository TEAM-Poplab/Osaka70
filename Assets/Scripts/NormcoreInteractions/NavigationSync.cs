using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// This Normcore class is used to sync the teleport location that is changed by the guide and forces all connected clients to move to the selected position. Normcore model: <see cref="NavigationSyncModel"/>
/// </summary>
public class NavigationSync : RealtimeComponent<NavigationSyncModel>
{
    public List<Transform> floors = new List<Transform>();
    private Realtime core;
    private GameObject coreManager;
    private NormcoreStatusSaverManager coreStatusSaver;
    private RealtimeNormcoreStatus coreStatus;

    private void Start()
    {
        coreManager = GameObject.Find("NormcoreManager");
        core = coreManager.GetComponent<Realtime>();
        coreStatusSaver = coreManager.GetComponent<NormcoreStatusSaverManager>();
        coreStatus = coreManager.GetComponent<RealtimeNormcoreStatus>();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    /// <summary>
    /// Properly set component variables at scene switching
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1 == SceneManager.GetSceneByName("Osaka"))
        {
            var tmpFloors = GameObject.FindGameObjectsWithTag("Floors");

            floors.Clear();

            foreach(GameObject floor in tmpFloors)
            {
                floors.Add(floor.transform);
            }
        }     
    }

    /// <summary>
    /// Called when the floor variable in the model changes
    /// </summary>
    /// <param name="model"></param>
    /// <param name="floor"></param>
    private void FloorDidChange(NavigationSyncModel model, int floor)
    {
        if (core.clientID != coreStatus.guideID)
        {
            UIManagerForUserMenuMRTKWithoutButtonsOsaka.Instance.Teleport(floors[floor]);
        }
    }
    /// <summary>
    /// Called when the position variable in the model changes (position variables stores the location of the guide client)
    /// </summary>
    /// <param name="model"></param>
    /// <param name="position"></param>
    private void PositionDidChange(NavigationSyncModel model, Vector3 position)
    {
        Debug.LogWarning("Position is going to change after guide with ID " + coreStatus.guideID + "called Follow me");
        if (core.clientID != coreStatus.guideID)
        {
            Debug.LogWarning("Position did change after guide called Follow me");
            GameObject tmpLocation = new GameObject();
            tmpLocation.transform.position = position;
            GameObject.Find("UIManager").GetComponent<UIManagerForUserMenuMRTKWithoutButtonsOsaka>().Teleport(tmpLocation.transform, tmpLocation);
        }
    }

    protected override void OnRealtimeModelReplaced(NavigationSyncModel previousModel, NavigationSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.floorDidChange -= FloorDidChange;
            previousModel.positionDidChange -= PositionDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it, populate it with the current mesh renderer color.
            if (currentModel.isFreshModel)
            {
                currentModel.floor = 5; //spawn point
                //currentModel.position = coreManager.GetComponent<RealtimeAvatarManager>().localAvatar.gameObject.transform.position;
            }

            // Register for events so we'll know if the color changes later
            currentModel.floorDidChange += FloorDidChange;
            currentModel.positionDidChange += PositionDidChange;
        }
    }

    /// <summary>
    /// It sets the new floor to teleport to
    /// </summary>
    /// <param name="floor">Index of floor to teleport to</param>
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
    /// Set the position to the guide's current lcoation and forces all clients to teleport where the guide is
    /// </summary>
    public void SetPosition()
    {
        Debug.LogWarning("Called SetPosition by guide with ID: " + coreStatus.guideID);
        if (core.clientID == coreStatus.guideID)
        {
            model.position = coreManager.GetComponent<RealtimeAvatarManager>().localAvatar.gameObject.transform.position;
        }
    }
}
