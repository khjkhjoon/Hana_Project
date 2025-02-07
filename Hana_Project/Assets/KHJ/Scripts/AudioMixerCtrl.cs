using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.KHJ
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer m_AudioMixer;
        [SerializeField] private Slider m_MusicMasterSlider;

        private void Awake()
        {
            m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        public void SetMasterVolume(float volume)
        {
            m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }
    }
}