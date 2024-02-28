using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    class AudioManager : CSharpSingleton<AudioManager>
    { 
        private AudioSource audioSource;

        public void Init()
        {
            audioSource = GameManager.Instance.MainCamera.GetComponent<AudioSource>();
        }
        public void UnInit()
        {

        }

        public void PlayAudio(string audioClipName)
        {
            ThreadManager.ExecuteUpdate(() => 
            {
                ResourceManager.Instance.Load(out AudioClip audioClip, audioClipName);
                audioSource.clip = audioClip;
                audioSource.Play();
            });
        }
    }
}
