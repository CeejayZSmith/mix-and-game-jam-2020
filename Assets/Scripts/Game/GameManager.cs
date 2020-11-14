using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float m_yValueToDieFrom = -15.0f;
    private static GameManager instance = null;
    public static GameManager Instance {get => instance;}


    public GameObject m_playerSpawnPoint = null;
    public PlayerAccountTracker m_playerAccountTracker = new PlayerAccountTracker();
    public HUDView m_hudView = null;

    public CharacterController m_player = null;

    public void Awake()
    {
        instance = this;
        m_playerAccountTracker = new PlayerAccountTracker();
        RespawnPlayer();
    } 
    
    public void Update()
    {
        m_hudView.RefreshDataUI(m_playerAccountTracker);
    }

    private void FixedUpdate()
    {
        if(m_player.transform.position.y < m_yValueToDieFrom)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        m_player.transform.position = m_playerSpawnPoint.transform.position;
    }


}
