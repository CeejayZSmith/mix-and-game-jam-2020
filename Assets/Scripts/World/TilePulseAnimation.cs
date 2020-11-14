using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePulseAnimation : MonoBehaviour
{
    public bool scaleYOnly = true;
    public float m_squishScale = 0.7f;
    public float m_normalScale = 1.0f;
    public float m_animationLength = 0.2f;
    public Vector3 m_scaleFrom = new Vector3();

    private Tile m_tile = null;

    public void Awake()
    {
        m_tile = GetComponent<Tile>();
    }

    public void DoAnimation()
    {
        this.StartCoroutine(AnimationCoroutine());
    }

    IEnumerator AnimationCoroutine()
    {
        float timeAtStart = Time.unscaledTime;
        while(timeAtStart + m_animationLength > Time.unscaledTime)
        {
            float t = (Time.unscaledTime - timeAtStart) / m_animationLength;
            float maxPoint = 0.6f;
            float midT = t > maxPoint ? (t-maxPoint)/(1-maxPoint)  : (t / maxPoint);

            float scale = m_normalScale;
            if(t < maxPoint)
            {
                scale = Mathf.Lerp(scale, m_squishScale, midT);
            }
            else
            {
                scale = Mathf.Lerp(m_squishScale, m_normalScale, midT);
            }

            

            Vector3 scaleVec = new Vector3(scaleYOnly? 1.0f : scale, scale, 1.0f); 
            m_tile.m_renderer.transform.localScale = scaleVec;
            // assumes local position should be zero without scaling.
            m_tile.m_renderer.transform.localPosition = ((1-scale) * m_scaleFrom)/m_tile.transform.localScale.magnitude;
            yield return null;
        }
        yield break;
    }
}
