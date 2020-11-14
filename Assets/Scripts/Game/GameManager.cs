using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance {get => instance;}

    public PlayerAccountTracker m_playerAccountTracker = new PlayerAccountTracker();
    public HUDView m_hudView = null;

    public void Awake()
    {
        instance = this;
        m_playerAccountTracker = new PlayerAccountTracker();
    } 
    
    public void Update()
    {
        m_hudView.RefreshDataUI(m_playerAccountTracker);
    }


}
