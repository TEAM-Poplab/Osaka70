using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Normal.Realtime.Native;
using UnityEngine;
using UnityEngine.Audio;
using Normal.Realtime.Native;
using Microphone = UnityEngine.Microphone;

public class NormcoreAvatarReverbRoom : MonoBehaviour
{
    public AudioMixerGroup voiceMixerGroup;
    public AudioMixerGroup reverbMixerGroup;
    [HideInInspector] public float sourceVolume;

    private OculusMicrophoneDevice _microphone;
    private float[] _microphoneFrameData;
    private RealtimeAvatarVoice _avatarVoice;

    private AudioSource _originalAudioSource;
    private AudioSource _reverbAudioSource;

    private AudioClip _audioStream;
    private AudioOutput _audioOutput;
    
    private float[] _audioStreamData;

    public AudioSource ReverbAudioSource
    {
        get => _reverbAudioSource;
        private set => _reverbAudioSource = value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitVoiceMixer());
    }

    private IEnumerator InitVoiceMixer()
    {
        //int sourcePriority;
        yield return new WaitForSeconds(1f);
        
        _reverbAudioSource = transform.parent.gameObject.AddComponent<AudioSource>();
            
        //Remote user avatar in local client: it must have the Audio Source component
        _originalAudioSource = GetComponent<AudioSource>();
        if (_originalAudioSource != null)
        {
            //GetComponent<RealtimeAvatarVoice>().ConnectRemoteAudioStream();
            //_originalAudioSource = GetComponent<AudioSource>();
            _originalAudioSource.outputAudioMixerGroup = voiceMixerGroup;
            _audioStream = _originalAudioSource.clip;
            //sourcePriority = 128;
            sourceVolume = 1f;
        }
        //Local client avatar: it doesn't have the AudioSource component, and input from microphone should be get
        else
        {
            //_audioStream = Microphone.Start(null, true, 1, 48000);
            //sourcePriority = 128;
            sourceVolume = 1f;
        

            //// Check for Oculus native microphone device API
            //bool foundOculusMicrophoneDevice = false;
            //if (OculusMicrophoneDevice.IsOculusPlatformAvailable())
            //{
            //    foundOculusMicrophoneDevice = OculusMicrophoneDevice.IsOculusPlatformInitialized();
            //    if (!foundOculusMicrophoneDevice && Application.platform == RuntimePlatform.Android)
            //        Debug.LogWarning("Normcore: Oculus Platform SDK found, but it's not initialized. Oculus Quest native echo cancellation will be unavailable.");
            //}

            //if (foundOculusMicrophoneDevice)
            //{
                // Create Oculus microphone device
                //_microphone = new OculusMicrophoneDevice();
                //_microphone.Start();
                //int _microphoneSampleRate = 48000;
                //int _microphoneChannels = 1;
                //_microphoneFrameData = new float[_microphoneSampleRate / 100];
                //StartCoroutine(OculusMicrophoneInput());
            //}
        }


        _reverbAudioSource.clip = _audioStream;
        _reverbAudioSource.spatialBlend = 0;
        _reverbAudioSource.outputAudioMixerGroup = reverbMixerGroup;
        _reverbAudioSource.loop = true;
        //_reverbAudioSource.priority = sourcePriority;
        _reverbAudioSource.volume = 0;
        _reverbAudioSource.pitch = 1.0f;
        _reverbAudioSource.spatializePostEffects = true;
        _reverbAudioSource.enabled = true;
        while (!(Microphone.GetPosition(null) > 0)) {}
        _reverbAudioSource.Play();
    }

    IEnumerator OculusMicrophoneInput()
    {
        while(true)
        {
            _microphone.GetAudioData(_microphoneFrameData);
            yield return new WaitForSeconds(1);
            _audioStream.SetData(_microphoneFrameData, 48000 / 100);
        }

    }
}
