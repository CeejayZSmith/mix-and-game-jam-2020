using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTileModule : MonoBehaviour
{
    public SpriteRenderer m_renderer = null;
        public Collider2D m_collider = null;
    public Color m_activeColor = Color.white;
    public Color m_deactiveColor  = Color.black;

    private bool m_isButtonDown = false;

    public UnityEngine.Events.UnityEvent m_onActiveEvent = new UnityEngine.Events.UnityEvent();

    public void ActivateButton(bool activeState)
    {
        if(activeState == true)
        {
            m_collider.enabled = false;
            m_renderer.color = m_activeColor;
            // only do once.
            if(m_isButtonDown == false)
            {
                m_onActiveEvent?.Invoke();
            }
        }
        else
        {
            m_collider.enabled = true;
            m_renderer.color = m_deactiveColor;
        }
        m_isButtonDown = activeState;
    }

}
