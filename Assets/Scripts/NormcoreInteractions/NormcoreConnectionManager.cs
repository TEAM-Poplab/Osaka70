using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;

/// <summary>
/// The class handles any Normcore connection state change during playtime and responses accordingly to changes
/// </summary>
[RequireComponent(typeof(Realtime))]
public class NormcoreConnectionManager : MonoBehaviour
{
    [Header("Connection Events")]
    public UnityEvent onDisconnection = new UnityEvent();
    public UnityEvent onReconnection = new UnityEvent();
    public UnityEvent onReconnectionFailed = new UnityEvent();

    private Room room;
    private string roomName;

    private Coroutine onDisconnectionFromConnected = null;

    private void Start()
    {
        GetComponent<Realtime>().didConnectToRoom += NormcoreConnectionManager_didConnectToRoom;
    }

    private void NormcoreConnectionManager_didConnectToRoom(Realtime realtime)
    {
        room = realtime.room;
        roomName = room.name;

        room.connectionStateChanged += Room_connectionStateChanged;
    }

    private void Room_connectionStateChanged(Room room, Room.ConnectionState previousConnectionState, Room.ConnectionState connectionState)
    {
        Debug.LogError("From " + previousConnectionState + " to " + connectionState);
        if (connectionState == Room.ConnectionState.Disconnected)
        {
            if (previousConnectionState == Room.ConnectionState.Ready)
            {
                //onDisconnection.Invoke();
                if (onDisconnectionFromConnected == null)
                {
                    onDisconnectionFromConnected = StartCoroutine(TryReconnection(20, 1, roomName, GetComponent<Realtime>()));
                    onDisconnection.Invoke();
                }
            }
        }
        else if (connectionState == Room.ConnectionState.Error)
        {
            //onReconnectionFailed.Invoke();
        } else if (connectionState == Room.ConnectionState.Ready)
        {
            if (previousConnectionState == Room.ConnectionState.ConnectingToRoom)
            {
                //onReconnection.Invoke();
            }
        }
    }

    IEnumerator TryReconnection(int tryTime, float deltaTryTime, string roomName, Realtime realtime)
    {
        int timer = 0;
        yield return new WaitForSeconds(1f);

        while (timer < tryTime)
        {
            realtime.Connect(roomName);
            timer++;
            yield return new WaitForSeconds(deltaTryTime);
            if (realtime.room.connected)
            {
                onReconnection.Invoke();
                onDisconnectionFromConnected = null;
                yield break;
            }
        }

        onReconnectionFailed.Invoke();
        onDisconnectionFromConnected = null;
    }
}
