/************************************************************************************
* 
* Class Purpose: it handles any event that occurs when the player enters and exits
*   the specific area (GHLoader version)
*
************************************************************************************/

using OVRTouchSample;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class Triggers : MonoBehaviour
{
    #region Properties and public fields
    [HideInInspector]
    public bool isActive;  //Property: it tells if the area has been activated by the player or not

    [Tooltip("The GHLoader object which handles the GH geometry loaded and modified in this area")]
    public GHLoader ghloaderScript;

    [Tooltip("The particle system activated with the thumb when it reveals")]
    public ParticleSystem particleSystemFX;

    [SerializeField, Tooltip("Base material for the area (when not active)")]
    private Material baseMaterial;  //Base material for the area (when not active)

    [SerializeField, Tooltip("Interaction material for the area (when the player is on it)")]
    private Material interactingMaterial;   //Interaction material for the area (when the player is on it)

    [SerializeField, Tooltip("The pinch slider (MRTK PinchSlider object) related to that area")]
    private PinchSlider pinchSlider; //The controller (handle + slider) related to that area

    // The FX for in and out transitions around the platform
    [Tooltip("The IN effect object around the platform")]
    public GameObject fxIn;
    [Tooltip("The OUT effect object around the platform")]
    public GameObject fxOut;
    [Tooltip("The player gameObject which will interact in the scene")]
    public GameObject player;
    [Tooltip("The Guardian effect object around the area")]
    public GameObject areaGuardian;

    [SerializeField, Tooltip("The track object of the PinchSlider")]
    private GameObject track;

    [SerializeField, Tooltip("The thumb object of the PinchSlider")]
    private GameObject handle;
    #endregion

    #region Fields
    private float _timerEnter = 0f;
    private float _timerExit = 0f;

    private bool _isLoaded;
    private bool _animationDone;
    private AudioSource _triggerSound;
    #endregion

    // Start is called before the first frame update. In it all variables needed are set
    void Start()
    {
        isActive = false;
        _isLoaded = false;
        _animationDone = false;
        _triggerSound = transform.Find("Sound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        /* OLD METHOD (kept for study purposes): This is needed to check when the player enters or leave the area.
         * Timer is activated when the player enters and if it arrives to 2s, the area interaction is activated
         * Area activation is also controlled by the isActive property, so I know if I have to start the TriggerEnters or the TriggerExit method.
         * Key concept: the two timers goes in opposite sense when activated, so they can control the final activation of the area without colliding
         */

        /*if (isActive && _timerEnter < 2)
        {
            _timerEnter += 2.0f * Time.deltaTime;
        }
        else if (isActive && _timerEnter > 2 && _timerExit == 0)
        {
            //Reset the FX cylinder rotation to its original value, before appling the new rotation. The other FX is always set to default at this point
            fxOut.transform.rotation = fxIn.transform.rotation;
            fxIn.transform.Rotate(new Vector3(0, 0, player.transform.eulerAngles.y)); //Facing the cylinder UV junction to be backward the player

            EnterBehavior();
            _timerExit = 2;
        }
        else if (!isActive && _timerExit > 0)
        {
            _timerExit -= 2.0f * Time.deltaTime;
        }
        else if (!isActive && _timerExit < 0 && _timerEnter == 0)
        {
            fxIn.transform.rotation = fxOut.transform.rotation;
            fxOut.gameObject.transform.Rotate(new Vector3(0, 0, player.transform.eulerAngles.y));

            ExitBehavior();
            _timerExit = 0;
        }*/

        /* This is needed to check when the player enters or leave the area.
         * The enter timer start anytime the player enters the trigger. If the player stays in it, and the area isn't loaded completely,
         * a couple of events may happen: after 2 seconds the geometry loads, and after another one the handle appears.
         * If the player leaves the area, the exit timer starts: if at least the geometry is loaded, after 2 seconds it will be hidden;
         * if the entire area is loaded, both geometry and slider need to be hidden. But if the player leaves the area accidently while the geometry has been loaded,
         * and suddently they come back in the area, no unload process is done because _animationDone and _isLoaded are checkmarks about the state of the geometry and slider.
         */

        if (isActive && _timerEnter < 3)
        {
            _timerEnter += 2.0f * Time.deltaTime;
        }

        if(_timerEnter > 2 && !_isLoaded) {
            if (_timerEnter >= 3 && !_isLoaded)
            {
                HandleEnabler(true);
                _isLoaded = true;
            } else if (!_animationDone)
            {
                //Reset the FX cylinder rotation to its original value, before appling the new rotation. The other FX is always set to default at this point
                fxOut.transform.rotation = fxIn.transform.rotation;
                fxIn.transform.Rotate(new Vector3(0, 0, player.transform.eulerAngles.y)); //Facing the cylinder UV junction to be backward the player

                EnterBehavior();
                _animationDone = true;
            }
        }

        if (!isActive && _timerExit < 3)
        {
            _timerExit += 2.0f * Time.deltaTime;
        }

        if (_timerExit > 2) {
            if (_timerExit >= 3 && _isLoaded)
            {
                HandleEnabler(false);
                _isLoaded = false;
            } else if (_animationDone)
            {
                fxIn.transform.rotation = fxOut.transform.rotation;
                fxOut.gameObject.transform.Rotate(new Vector3(0, 0, player.transform.eulerAngles.y));

                ExitBehavior();
                _animationDone = false;
            }
        }

        if (!fxIn.GetComponent<Animation>().isPlaying)
           fxIn.gameObject.SetActive(false);

        if (!fxOut.GetComponent<Animation>().isPlaying)
            fxOut.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  //Check that only the player can activate the file loading
        {
            isActive = true;
            _timerExit = 0;
            GetComponent<MeshRenderer>().material = interactingMaterial;    //Change the material to highlight the area while interacting with it

            if(!_animationDone)
            {
                _triggerSound.Play();
            }

            areaGuardian.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)  //Check that only the player unloads the meshes
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            _timerEnter = 0;
            GetComponent<MeshRenderer>().material = baseMaterial;   //Change the material to highlight the area while interacting with it
            
            if (_triggerSound.isPlaying)
                _triggerSound.Stop();

            //pinchSlider.EndInteraction();
            areaGuardian.SetActive(false);
        }
    }

    /********************************
     * Select and loading the specific GH file according
     * to the platform where the player enters in
     * and setting all the events for playing sounds and animations
     *******************************/
    private void EnterBehavior()
    {
        ghloaderScript.gameObject.SetActive(true);


        if (fxOut.GetComponent<Animation>().isPlaying)
        {
            fxOut.GetComponent<MeshRenderer>().material.SetFloat("DissolveAmount", 1f); //Force the out FX to be 0 in case we enter early and the animation has not endend yet
            fxOut.GetComponent<AudioSource>().Stop();
        }

        fxIn.gameObject.SetActive(true);
        fxIn.GetComponent<MeshRenderer>().material.SetFloat("DissolveAmount", 0f);    //Load the FX
        fxIn.GetComponent<Animation>().Play("FXFading");  //Play FX animation
        fxIn.GetComponent<AudioSource>().Play();
    }

    /********************************
     * Unloading any GH data structure loaded when
     * the player had entered the trigger zone
     * and setting all the events for playing sounds and animations
     *******************************/
    private void ExitBehavior()
    {
        ghloaderScript.gameObject.SetActive(false);


        if (fxIn.GetComponent<Animation>().isPlaying)
        {
            fxIn.GetComponent<MeshRenderer>().material.SetFloat("DissolveAmount", 1f); //Force the in FX to be 0 in case we enter early and the animation has not endend yet
            fxIn.GetComponent<AudioSource>().Stop();
        }

        fxOut.gameObject.SetActive(true);
        fxOut.GetComponent<MeshRenderer>().material.SetFloat("DissolveAmount", 0f); //Load the FX
        fxOut.GetComponent<Animation>().Play("FXFading");  //Play FX animation
        fxOut.GetComponent<AudioSource>().Play();
    }

    /********************************
     * DEPRECATED (kept for study purposes): used with coroutine usage verison
     * Enabling any components and behavior
     * related to the PinchSlider
     * @param {bool} status - The status which the components will be set to
     * @param {int} delay - Seconds of delay for the coroutine start
     *******************************/
    /*IEnumerator HandleEnabler(bool status, int delay)
    {
        yield return new WaitForSeconds(delay);

        handle.GetComponent<MeshRenderer>().enabled = status;

        if (status)
            particleSystemFX.Play();

        handle.GetComponent<AudioSource>().Play();
        track.GetComponent<MeshRenderer>().enabled = status;
    }*/

    /********************************
     * Enabling any components and behavior
     * related to the PinchSlider
     * @param {bool} status - The status which the components will be set to
     *******************************/
    void HandleEnabler(bool status)
    {
        handle.GetComponent<MeshRenderer>().enabled = status;
        handle.transform.Find("Outline").GetComponent<MeshRenderer>().enabled = status;

        if (status)
            particleSystemFX.Play();

        handle.GetComponent<AudioSource>().Play();
        track.GetComponent<MeshRenderer>().enabled = status;

        //In order to avoid the controller to keep following the player and interact "ghostly" with any other, RadialView (which ensure the object following the player) will be desabled
        pinchSlider.GetComponent<RadialView>().enabled = status;
    }
}
