using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormcoreAvatarReverbRoomTrigger : MonoBehaviour
{
    public NormcoreAvatarReverbRoom avatarReverbRoomComponent;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("Enters ReverbZone");
        if (other.CompareTag("ReverbZone"))
        {
            avatarReverbRoomComponent.ReverbAudioSource.volume = avatarReverbRoomComponent.sourceVolume;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.LogError("Exits ReverbZone");
        if (other.CompareTag("ReverbZone"))
        {
            avatarReverbRoomComponent.ReverbAudioSource.volume = 0;
        }
    }
}
