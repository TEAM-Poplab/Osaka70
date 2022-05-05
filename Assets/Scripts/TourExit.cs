using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using Ludiq;
using Bolt;
using UnityEngine.Events;

public class TourExit : RealtimeComponent<RealtimeNormcoreTourExit>
{
    public GameObject portalToStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void IsTourEndedDidChange(RealtimeNormcoreTourExit model, bool enabled)
    {
       if (enabled)
        {
            CustomEvent.Trigger(GameObject.FindGameObjectWithTag("Fade"), "Exit", portalToStart);
            Debug.Log("ExitCalled");
        }
    }

    protected override void OnRealtimeModelReplaced(RealtimeNormcoreTourExit previousModel, RealtimeNormcoreTourExit currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.isTourEndedDidChange -= IsTourEndedDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.isTourEnded = false;
            }

            currentModel.isTourEndedDidChange += IsTourEndedDidChange;
        }
    }

    public void EndTour()
    {
        model.isTourEnded = true;
    }

    public bool IsGuide()
    {
        return GameObject.Find("NormcoreManager").GetComponent<Realtime>().clientID == GameObject.Find("NormcoreManager").GetComponent<RealtimeNormcoreStatus>().guideID;
    }
}
