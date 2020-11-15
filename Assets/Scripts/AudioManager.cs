using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance = null;

    public static AudioManager Instance {get => instance; }

    public void Awake()
    {
        instance = this;
    }
    public AudioSource m_source = null;

    public AudioClip m_upgradeClip = null;
    public AudioClip m_upgradeMaxPlayers = null;
    public AudioClip m_unlockPlatformNoice = null;
    public AudioClip m_coinGatherNoise= null;
    public void PlayUpgradeSound()
    {
        m_source.PlayOneShot(m_upgradeClip);
    }

    public void UpgradeMaxPlayers()
    {
        m_source.PlayOneShot(m_upgradeMaxPlayers);
    }

    public void PlayPlatformPurchase()
    {
        m_source.PlayOneShot(m_unlockPlatformNoice);
    }

    public void PlayCoinGatherNoise()
    {
        m_source.PlayOneShot(m_coinGatherNoise);
    }


}
