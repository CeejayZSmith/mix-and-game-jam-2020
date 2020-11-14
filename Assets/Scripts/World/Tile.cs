using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    [SerializeField] public GameObject m_renderer = null;

    public UnityEngine.Events.UnityEvent m_onPlayerLandedEvent = new UnityEngine.Events.UnityEvent();
    public void OnPlayerLandedOnTile()
    {
        m_onPlayerLandedEvent.Invoke();
    }
}
