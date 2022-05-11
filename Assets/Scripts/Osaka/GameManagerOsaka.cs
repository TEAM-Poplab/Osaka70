/************************************************************************************
* 
* Class Purpose: singleton class which controls any game related event in Osaka
*
************************************************************************************/

//using BeautifyEffect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using BeautifyEffect;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine.Serialization;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit;
using prvncher.MixedReality.Toolkit.Input.Teleport;
using Normal.Realtime;
using Bolt;
using Ludiq;


public class GameManagerOsaka : Singleton<GameManagerOsaka>, IMixedRealityTeleportHandler
{
    //[SerializeField] private Beautify beautifyScript;
    //[SerializeField] private BeautifyProfile profile;
    [SerializeField] private LoadingButtonMeshSequenceController _lightChangeButton;
    [SerializeField] private TextMeshProUGUI lightChangeButtonText;
    [SerializeField] private LoadingButtonMeshSequenceController _exitButton;

    [SerializeField]
    [FormerlySerializedAs("currentSkybox")]
    private Material daySkybox;

    [SerializeField]
    private Material nightSkybox;

    [SerializeField]
    private GameObject water;

    [SerializeField]
    private GameObject lightGameObject;

    public List<GameObject> meshes = new List<GameObject>();
    public Material realityOffMaterial;
    public Material realityOnMaterial;

    [SerializeField]
    private Transform guideSpawnPosition;
    [SerializeField]
    private List<Transform> spawnPositions = new List<Transform>();

    private bool isLightChangeButtonActive = false;

    private Realtime normcoreCoreRT;
    private RealtimeAvatarManager normcoreCoreRAM;
    private RealtimeNormcoreStatus normcoreCoreRNS;
    private RealtimeNormcoreTourManager normcoreRTM;
    private TourManager tourManager;
    private GameObject playspace;
    public bool IsLightChangeButtonActive
    {
        get { return isLightChangeButtonActive; }
        set { isLightChangeButtonActive = value; }
    }

    private bool isExitButtonActive = false;
    public bool IsExitButtonActive
    {
        get { return isExitButtonActive; }
        set { isExitButtonActive = value; }
    }

    //The camera which attach the Beautify effect to
    private GameObject _centerCameraAnchor;

    //The private status field
    private OsakaGameLight _osakaLightStatus;

    //The property related to the _status field, it defines the current game light status
    public OsakaGameLight OsakaLightStatus
    {
        get
        {
            return _osakaLightStatus;
        }
    }

    public enum OsakaGameLight
    {
        DAY,
        NIGHT
    }

    private void Awake()
    {
        _osakaLightStatus = OsakaGameLight.DAY;
        if(SceneManager.GetActiveScene().name == "OsakaMaterialsTest")
        {
            GameObject.Find("TimeManager").GetComponent<DigitalClockManager>().StartAutoIncrease();
        }

        //Normcore setting
        playspace = GameObject.Find("MixedRealityPlayspace");
        var normcore = GameObject.Find("NormcoreManager");
        tourManager = GameObject.Find("TourManager").GetComponent<TourManager>();
        normcoreCoreRT = normcore.GetComponent<Realtime>();
        normcoreCoreRAM = normcore.GetComponent<RealtimeAvatarManager>();
        normcoreCoreRNS = normcore.GetComponent<RealtimeNormcoreStatus>();
        normcoreRTM = normcore.GetComponent<RealtimeNormcoreTourManager>();
        normcoreCoreRT.didConnectToRoom += NormcoreCore_didConnectToRoom;
    }

    // Update is called once per frame
    void Update()
    {
        if (_centerCameraAnchor == null)
        {
            _centerCameraAnchor = GameObject.Find("MixedRealityPlayspace/MRTK-Quest_OVRCameraRig(Clone)/TrackingSpace/CenterEyeAnchor");
            //SetCameraWithBeautify(_centerCameraAnchor);
        }
    }

    /// <summary>
    /// Properly set Normcore when a client successfully connects to a room. It places the client to proper spawn location
    /// </summary>
    /// <param name="realtime"></param>
    public void NormcoreCore_didConnectToRoom(Realtime realtime)
    {
        if (tourManager.isTourEnabled)
        {
            if (normcoreCoreRT.clientID != normcoreCoreRNS.guideID)
            {
                if (!normcoreRTM.GetTeleportStatus())
                {
                    normcoreRTM.DisableTeleportLocally();
                }
            }
            Vector3 displacement;
            if (normcoreCoreRT.clientID == normcoreCoreRNS.guideID)
            {
                displacement = tourManager.GetPositionFromIndexAndFloor(15, 0);
            } else
            {
                displacement = tourManager.GetPositionFromIndexAndFloor(normcoreCoreRT.clientID, 0);
            }           
            CustomEvent.Trigger(playspace, "Teleport", displacement);
            //StartCoroutine(DisableTeleportAfterSpawing(1f));
        } else
        {
            Vector3 displacement;
            if (normcoreCoreRT.clientID == normcoreCoreRNS.guideID)
            {
                displacement = guideSpawnPosition.position;
            }
            else
            {
                if (normcoreCoreRT.clientID < spawnPositions.Count)
                {
                    displacement = spawnPositions[normcoreCoreRT.clientID].position;
                }
                else
                {
                    displacement = spawnPositions[normcoreCoreRT.clientID % spawnPositions.Count].position;
                }
            }
            playspace.transform.position = displacement;
            CustomEvent.Trigger(playspace, "Teleport", displacement);
        }
    }

    /*
     * Attaching and setting the Beautify effect on the main camera which is created on runtime
     * @param {GameObject} cameraGO - The camera which the effect will be applied to
     */
    private void SetCameraWithBeautify(GameObject cameraGO)
    {
        //if (!cameraGO.GetComponent<Beautify>())
        //    cameraGO.AddComponent<Beautify>();

        //Beautify bf = cameraGO.GetComponent<Beautify>();
        //bf.profile = profile;
        //bf.quality = BEAUTIFY_QUALITY.BestPerformance;
    }

    #region TeleportSystem
    public void OnEscalatorEnter()
    {
        CoreServices.TeleportSystem.Disable();
        CoreServices.TeleportSystem.RegisterHandler<IMixedRealityTeleportHandler>(this);
    }

    public void OnEscalatorExit()
    {
        CoreServices.TeleportSystem.Enable();
        CoreServices.TeleportSystem.UnregisterHandler<IMixedRealityTeleportHandler>(this);
    }

    public void OnTeleportCanceled(TeleportEventData eventData)
    {

    }

    public void OnTeleportCompleted(TeleportEventData eventData)
    {

    }

    public void OnTeleportRequest(TeleportEventData eventData)
    {
        GameObject teleportPrefabRight = GameObject.Find("CustomTeleportPointer Right");
        GameObject teleportPrefabLeft = GameObject.Find("CustomTeleportPointer Left");

        if (teleportPrefabRight)
        {
            teleportPrefabRight.SetActive(false);
            CoreServices.TeleportSystem.RaiseTeleportCanceled(teleportPrefabRight.GetComponent<CustomTeleportPointer>(), teleportPrefabRight.GetComponent<CustomTeleportPointer>().TeleportHotSpot);
        } else
        {
            teleportPrefabLeft.SetActive(false);
            CoreServices.TeleportSystem.RaiseTeleportCanceled(teleportPrefabLeft.GetComponent<CustomTeleportPointer>(), teleportPrefabLeft.GetComponent<CustomTeleportPointer>().TeleportHotSpot);
        }
    }

    public void OnTeleportStarted(TeleportEventData eventData)
    {

    }
    #endregion

    IEnumerator DisableTeleportAfterSpawing(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (normcoreCoreRT.clientID != normcoreCoreRNS.guideID)
        {
            if (!normcoreRTM.GetTeleportStatus())
            {
                normcoreRTM.DisableTeleportLocally();
            }
        }
    }
}
