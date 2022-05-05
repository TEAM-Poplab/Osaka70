using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Ludiq;
using Bolt;


public class TourTeleportController : RealtimeComponent<RealtimeNormcoreTourTeleportController>
{
    GameObject _normcoreManager;
    GameObject _playSpace;
    public GameObject _navMesh;
    public GameObject _navMeshOutside;

    // Start is called before the first frame update
    void Start()
    {
        _normcoreManager = GameObject.Find("NormcoreManager");
        _playSpace = GameObject.Find("MixedRealityPlayspace");
        Debug.Log("Toggle:" + _playSpace.name+_normcoreManager.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TeleportDidChange(RealtimeNormcoreTourTeleportController model, bool enabled)
    {
        Debug.Log("Toggle: variable changed");
        if (_normcoreManager.GetComponent<Realtime>().clientID != _normcoreManager.GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            //GameObject fade = (GameObject)Variables.ActiveScene.Get("Fade");
            if (!model.isTeleportEnabled)
            {
                //CustomEvent.Trigger(_playSpace, "EnableTeleport");
                //Variables.Object(_playSpace).Set("isTeleportAllowed", true);
                _navMesh.SetActive(true);
                _navMeshOutside.SetActive(true);
                Debug.Log("Toggle: variable set to"+ Variables.Object(_playSpace).Get("isTeleportAllowed"));
            } else
            {
                //CustomEvent.Trigger(_playSpace, "DisableTeleport");
                //Variables.Object(_playSpace).Set("isTeleportAllowed", false);
                _navMesh.SetActive(false);
                _navMeshOutside.SetActive(false);
                Debug.Log("Toggle: variable set to" + Variables.Object(_playSpace).Get("isTeleportAllowed"));
            }    
        } else Debug.Log("Toggle: guide ignores variable change");
    }

    protected override void OnRealtimeModelReplaced(RealtimeNormcoreTourTeleportController previousModel, RealtimeNormcoreTourTeleportController currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.isTeleportEnabledDidChange -= TeleportDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it, populate it with the current mesh renderer color.
            if (currentModel.isFreshModel)
            {
                currentModel.isTeleportEnabled = false; //spawn point
                //currentModel.position = coreManager.GetComponent<RealtimeAvatarManager>().localAvatar.gameObject.transform.position;
            }

            
            if (_normcoreManager.GetComponent<Realtime>().clientID != _normcoreManager.GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                //CustomEvent.Trigger(_playSpace, "DisableTeleport");
                //Variables.Object(_playSpace).Set("isTeleportAllowed", false);
                _navMesh.SetActive(false);
                _navMeshOutside.SetActive(false);
            }

            // Register for events so we'll know if the color changes later
            currentModel.isTeleportEnabledDidChange += TeleportDidChange;
        }
    }

    public int GetLocalClientID()
    {
        return _normcoreManager.GetComponent<Realtime>().clientID;
    }

    public bool isGuide()
    {
        return _normcoreManager.GetComponent<Realtime>().clientID == _normcoreManager.GetComponent<RealtimeNormcoreStatus>().guideID ? true : false;
    }

    public void ChangeTeleport(bool enabled)
    {
        model.isTeleportEnabled = enabled;
    }
}
