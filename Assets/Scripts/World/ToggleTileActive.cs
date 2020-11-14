using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTileActive : MonoBehaviour
{

    [SerializeField] private SpriteRenderer m_renderer = null;
    public Color m_activeColour = Color.white;
    public Color m_deactivatedColour = Color.black;
    private Collider2D m_collider = null;
    [SerializeField] 
    private bool m_active = true;

    public UnityEngine.Events.UnityEvent m_onEnabled = new UnityEngine.Events.UnityEvent();
    void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        SetTileActiveState(m_active);
    }

    public void SetTileActiveState(bool active)
    {
        m_active = active;

        m_collider.enabled = active;
        m_renderer.color = active ? m_activeColour : m_deactivatedColour;

        if(active == true)
        {
            m_onEnabled?.Invoke();
        }
    }
}
