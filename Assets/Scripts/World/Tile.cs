using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject renderer = null;

    public UnityEngine.Events.UnityEvent m_onPlayerLandedEvent = new UnityEngine.Events.UnityEvent();
    public void OnPlayerLandedOnTile()
    {
        m_onPlayerLandedEvent.Invoke();
    }
}
