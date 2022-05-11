using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludiq;
using Bolt;
using Normal.Realtime;

/// <summary>
/// This class handles a group of clients that visit Osaka as visitors: usually Osaka can be visited by a guide and a small amount of clients that are vleft free to move.
/// This class enables the presence of a alrge number of clients (up to 15) and allows to guide to easly manage them, locking their movement.
/// </summary>
public class TourManager : MonoBehaviour
{
    [Tooltip("Toggle to globally set the tour mode")] public bool isTourEnabled = false;
    public List<GameObject> floorsVisitorsSpotsGroup = new List<GameObject>();
    private List<List<Transform>> indexedSpotGroups = new List<List<Transform>>();
    private int _localVisitorIndex = 0;
    GameObject _normcore;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in floorsVisitorsSpotsGroup)
        {
            indexedSpotGroups.Add(IndexSpotGroup(obj));
        }
        _normcore = GameObject.Find("NormcoreManager");

        //Teleport for visitor has to be disabled at the very first beginning
        //StartCoroutine(DisableTeleport(1f));
        StartCoroutine(EnableGodVoice(0.5f));
    }

    private List<Transform> IndexSpotGroup(GameObject spotGroup)
    {
        List<Transform> indexedObject = new List<Transform>();
        int index = spotGroup.transform.childCount;

        for (int i = 0; i < index; i++)
        {
            indexedObject.Add(spotGroup.transform.GetChild(i));
            Debug.Log("Transform #" + i + ": " + indexedObject[i].position);
        }

        return indexedObject;
    }

    /// <summary>
    /// It returns the position inside a guest spot object of the specified spot
    /// </summary>
    /// <param name="index">The index of a single spot inside a group</param>
    /// <param name="floor">The group index of registered spot groups</param>
    /// <returns>The position of the index spot inside the floor group</returns>
    public Vector3 GetPositionFromIndexAndFloor(int index, int floor)
    {
        index = index > 15 ? 15 : index;
        index = index < 0 ? 0 : index;
        //return indexedSpotGroups[floor][index-1].position;
        return floorsVisitorsSpotsGroup[floor].transform.GetChild(index).position;
    }

    /// <summary>
    /// It returns the transform inside a guest spot object of the specified spot
    /// </summary>
    /// <param name="index">The index of a single spot inside a group</param>
    /// <param name="floor">The group index of registered spot groups</param>
    /// <returns>The transform of the index spot inside the floor group</returns>
    public Transform GetTransformFromIndexAndFloor(int index, int floor)
    {
        index = index > 15 ? 15 : index;
        index = index < 0 ? 0 : index;
        return floorsVisitorsSpotsGroup[floor].transform.GetChild(index);
    }

    public int GetLocalVisitorIndex()
    {
        return _localVisitorIndex;
    }

    public void SetLocalVisitorIndex(int index)
    {
        _localVisitorIndex = index;
    }

    IEnumerator DisableTeleport(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (_normcore.GetComponent<Realtime>().clientID != _normcore.GetComponent<RealtimeNormcoreStatus>().guideID)
        {
            GameObject _playSpace = GameObject.Find("MixedRealityPlayspace");
            CustomEvent.Trigger(_playSpace, "DisableTeleport");
            Variables.Object(_playSpace).Set("isTeleportAllowed", false);
        }
    }

    /// <summary>
    /// Allows guide to disable spatial audio for its own voice and be heard by all other clients whenever they are in Osaka
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    IEnumerator EnableGodVoice(float timer)
    {
        yield return new WaitForSeconds(timer);
        GameObject[] _heads = GameObject.FindGameObjectsWithTag("NormcorePlayerHead");
        foreach(GameObject head in _heads)
        {
            if (head.GetComponent<RealtimeView>().ownerIDInHierarchy == _normcore.GetComponent<RealtimeNormcoreStatus>().guideID)
            {
                head.GetComponent<AudioSource>().spatialize = false;
                head.GetComponent<AudioSource>().spatialBlend = 0;
            }
        }
    }
}
