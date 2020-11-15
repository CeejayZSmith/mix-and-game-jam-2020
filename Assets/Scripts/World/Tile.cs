using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    [SerializeField] public GameObject m_renderer = null;

    public UnityEngine.Events.UnityEvent m_onPlayerLandedEvent = new UnityEngine.Events.UnityEvent();
    public delegate void CharacterConotrollerTileEvent(CharacterController cc);
    public event CharacterConotrollerTileEvent OnCharacterControllerLandedOnTile;
    public void OnPlayerLandedOnTile(CharacterController cc)
    {
        OnCharacterControllerLandedOnTile?.Invoke(cc);
        m_onPlayerLandedEvent.Invoke();
    }


}
