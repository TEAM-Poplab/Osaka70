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
using Microsoft.MixedReality.Toolkit.Utilities;
using Bolt;
using Ludiq;

public class UIManagerForUserMenuMRTKWithoutButtons : Singleton<UIManagerForUserMenuMRTKWithoutButtons>
{
    //[SerializeField] private Canvas _quitMenu;
    [SerializeField] private GameObject digitalClockObject;
    [SerializeField] private PressableButton digitalClockButton;
    [SerializeField] private GameObject[] clockButtons;
    [SerializeField] private GameObject dockObject;
    [SerializeField] private GameObject navigationObject;
    [SerializeField] private Vector3 dockPositionWhileHidden = Vector3.zero;
    [Tooltip("The show/hide labels button")] private GameObject labelsButton;
    [SerializeField] private GameObject geometryFreezButton;
    [SerializeField] private GameObject gridManager;
    private GameObject diagnostics = null;
    private bool wasAutoincreaseEnabled = true;

    private List<GameObject> dockedGameobjects = new List<GameObject>();
    private Vector3 dockPreviousPosition = Vector3.zero;
    private Vector3 dockPreviousScale = Vector3.one;
    private Vector3 guardianCenterPosition;
    private GameObject dockPlaceholder;
    private GameObject platformDock;
    private GameObject playspace;

    private DigitalClockManager dcm;

    private bool isUserMenuActive = false;
    private bool isDockActive = false;
    private bool isClockActive = false;
    private bool isNavigationActive = false;
    private bool areLabelsActive = false;
    private bool isGridManagerActive = false;

    [HideInInspector] public UnityEvent dockIsVisible = new UnityEvent();   //Used in PlatformLocomotion class to save dock position only when it becomes visible

    public bool IsUserMenuActive
    {
        get { return isUserMenuActive; }
        set { isUserMenuActive = value; }
    }

    public bool IsDockActive
    {
        get => isDockActive;
        private set => isDockActive = value;
    }

    public bool IsNavigationActive
    {
        get => isNavigationActive;
        private set => isNavigationActive = value;
    }

    public bool AreLabelsActive
    {
        get => areLabelsActive;
        private set => areLabelsActive = value;
    }

    public bool IsGridManagerActive
    {
        get => isGridManagerActive;
        private set => isGridManagerActive = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        guardianCenterPosition = GameObject.Find("GuardianCenter").transform.position;
        dockPlaceholder = GameObject.Find("DockPlaceholder");
        dcm = GameObject.Find("TimeManager").GetComponent<DigitalClockManager>();
        dockObject.transform.localPosition = dockPositionWhileHidden;
        platformDock = GameObject.Find("GuardianCenter/Dock/PlatformDockPosition");
        //platformDock.GetComponent<PlatformDockPosition>().onObjectPlatformed.AddListener(gameobject => {
        //    labelsButton.SetActive(true);
        //    labelsButton.transform.parent.GetComponent<GridObjectCollection>().UpdateCollection(); });
        //platformDock.GetComponent<PlatformDockPosition>().onObjectPlatformedMS.AddListener(gameobject => {
        //    labelsButton.SetActive(true);
        //    labelsButton.transform.parent.GetComponent<GridObjectCollection>().UpdateCollection(); });
        //platformDock.GetComponent<PlatformDockPosition>().onObjectUnplatformed.AddListener(() => {
        //    labelsButton.SetActive(false);
        //    labelsButton.transform.parent.GetComponent<GridObjectCollection>().UpdateCollection(); });
        //platformDock.GetComponent<PlatformDockPosition>().onObjectUnplatformedMS.AddListener(() => {
        //    labelsButton.SetActive(false);
        //    labelsButton.transform.parent.GetComponent<GridObjectCollection>().UpdateCollection(); });
        HideDock();
    }

    private void Update()
    {
        if (diagnostics == null)
        {
            diagnostics = GameObject.Find("MixedRealityPlayspace/Diagnostics");
            playspace = GameObject.Find("MixedRealityPlayspace");
            diagnostics.SetActive(false);
        }
    }

    /// <summary>
    /// Switch between reality on/off status.
    /// </summary>
    /// <remarks> It also controls the clock: if reality is set on, clock autoincrease is set true only if clock wasn't enabled, otherwise it will be stopped</remarks>
    public void RealityHandler()
    {
        switch (GameManager.Instance.LightStatus)
        {
            case GameManager.GameLight.DAY:
                GameManager.Instance.SetDaylight(false);

                //digitalClockObject.SetActive(false);
                if (isClockActive)
                {
                    //digitalClockObject.SetActive(false);
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (1)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                if (dcm.AutoIncrease)
                {
                    wasAutoincreaseEnabled = true;
                    dcm.StopAutoIncrease();
                } else
                {
                    wasAutoincreaseEnabled = false;
                    dcm.StopAutoIncrease();
                }
                //if (digitalClockObject.activeSelf)
                //{
                //    digitalClockButton.ButtonPressed.Invoke();
                //}
                break;
            case GameManager.GameLight.NIGHT:
                GameManager.Instance.SetDaylight(true);
                //if (isClockActive)
                //{
                //    digitalClockObject.SetActive(true);
                //    //dcm.StopAutoIncrease();
                //} else
                //{
                //    //dcm.StartAutoIncrease();
                //}
                if (wasAutoincreaseEnabled)
                {
                    dcm.StartAutoIncrease();
                }
                break;
        }
    }

    public void DockHandler()
    {
        switch (isDockActive)
        {
            case true:
                HideDock();
                //dockButtonText.color = new Color32(128, 128, 128, 255);
                break;
            case false:
                if (isNavigationActive)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (4)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                if(isClockActive && GameManager.Instance.LightStatus == GameManager.GameLight.DAY)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (1)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                ShowDock();
                //dockButtonText.color = new Color32(255, 255, 255, 255);
                break;
        }
    }

    public void ClockHandler()
    {
        switch (digitalClockObject.activeSelf)
        {
            case true:
                digitalClockObject.SetActive(false);
                isClockActive = false;
                //dcm.StartAutoIncrease();  //If disabled, when the clock is showed, autoincrease won't be stopped or manually started
                break;
            case false:
                if (isNavigationActive)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (4)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                if (isDockActive)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (2)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                digitalClockObject.SetActive(true);
                isClockActive = true;
                //dcm.StopAutoIncrease(); //If disabled, when the clock is showed, autoincrease won't be stopped or manually started
                break;
        }
    }

    public void ShowClockButtons()
    {
        foreach(GameObject button in clockButtons)
        {
            button.SetActive(true);
        }
    }

    public void HideClockButtons()
    {
        foreach (GameObject button in clockButtons)
        {
            button.SetActive(false);
        }
    }

    public void ExitHandler()
    {
        ScenesManager.Instance.LoadLevelSelectionScene();
        ScenesManager.Instance.ActivateScene();
    }

    private void HideDock()
    {
        dockObject.GetComponent<Orbital>().enabled = false;
        SliderManager.Instance.OnDockHidden();
        //dockPreviousPosition = dockObject.transform.localPosition;
        dockPreviousScale = dockObject.transform.localScale;
        //dockObject.transform.localPosition = guardianCenterPosition;
        dockObject.transform.position = dockPositionWhileHidden;
        //dockObject.transform.localScale = new Vector3(.01f, .01f, .01f);
        isDockActive = false;
    }

    private void ShowDock()
    {
        //dockObject.GetComponent<Orbital>().enabled = true;
        SliderManager.Instance.OnDockShowed();
        dockObject.transform.position = dockPlaceholder.transform.position;
        dockObject.transform.localScale = dockPreviousScale;
        isDockActive = true;
        //dockIsVisible.Invoke();
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

    public void NavigationHandler()
    {
        switch (navigationObject.activeSelf)
        {
            case true:
                navigationObject.SetActive(false);
                isNavigationActive = false;
                break;
            case false:
                if (isDockActive)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (2)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                if (isClockActive && GameManager.Instance.LightStatus == GameManager.GameLight.DAY)
                {
                    GameObject.Find("UserMenu/NearMenu2x4(Clone)/ButtonCollection/PressableButtonHoloLens2ToggleRadio_32x96(Clone) (1)").GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                navigationObject.SetActive(true);
                isNavigationActive = true;
                break;
        }
    }

    public void LabelsHandler()
    {
        switch(areLabelsActive)
        {
            case true:
                areLabelsActive = false;
                if (platformDock.GetComponent<PlatformDockPosition>().PlatformedObject != null)
                {
                    platformDock.GetComponent<PlatformDockPosition>().PlatformedObject.GetComponent<ValueLabelsVisualizer>().SetLabelsVisibility(false);
                    SliderManager.Instance.SetLabelVisibility(false);
                }
                platformDock.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().enabled = false;
                break;
            case false:
                areLabelsActive = true;
                if (platformDock.GetComponent<PlatformDockPosition>().PlatformedObject != null)
                {
                    platformDock.GetComponent<PlatformDockPosition>().PlatformedObject.GetComponent<ValueLabelsVisualizer>().SetLabelsVisibility(true);
                    SliderManager.Instance.SetLabelVisibility(true);
                }
                platformDock.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                break;
        }
    }

    public void FreezeGeometry()
    {
        if (platformDock.GetComponent<PlatformDockPosition>().PlatformedObject.transform.GetChild(0).childCount == 0)
        {
            GameObject.Find("GuardianCenter/ObjectPlaceholder").GetComponent<PlatformPlaceholder>().OnGeometryFreeze();
        } else
        {
            GameObject.Find("GuardianCenter/ObjectPlaceholderMS").GetComponent<PlatformPlaceholderMeshSequence>().OnGeometryFreeze();
        }
    }

    public void GridManagerHandler()
    {
        switch(isGridManagerActive)
        {
            case true:
                gridManager.SetActive(false);
                isGridManagerActive = false;
                break;
            case false:
                gridManager.SetActive(true);
                isGridManagerActive = true;
                break;
        }
    }

    /// <summary>
    /// Instantly teleport the player to selected position
    /// </summary>
    /// <param name="position">Position where teleport the player to</param>
    public void Teleport(Transform position)
    {
        StartCoroutine(TeleportCoroutine(position));
    }

    IEnumerator TeleportCoroutine(Transform position)
    {
        yield return new WaitForSeconds(0.5f);
        playspace.transform.position = position.position;
        CustomEvent.Trigger(playspace, "Teleport", position.position);
    }
}
