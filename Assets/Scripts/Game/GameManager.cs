using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float m_yValueToDieFrom = -15.0f;
    private static GameManager instance = null;
    public static GameManager Instance {get => instance;}

    public enum Milestone
    {
        InitialMilestone = 0,
        LastMilestone = 1,
    }

    private Milestone m_currentMileStone = Milestone.InitialMilestone;
    public GameObject m_playerSpawnPoint = null;
    public PlayerAccountTracker m_playerAccountTracker = new PlayerAccountTracker();
    public HUDView m_hudView = null;

    public CharacterController m_player = null;

    public delegate void MilestoneEvent(Milestone milstone);
    public event MilestoneEvent OnMilestoneAchieved;
    public void OnEnable()
    {
        instance = this;
        m_playerAccountTracker = new PlayerAccountTracker();
        RespawnPlayer();
    } 
    
    public void Update()
    {
        m_hudView.RefreshDataUI(m_playerAccountTracker);

        if(m_currentMileStone != Milestone.LastMilestone)
        {
            if(NextMilestoneTargetAmount() <= m_playerAccountTracker.MoneyAmount)
            {
                OnMilestoneAchieved?.Invoke(m_currentMileStone);
                m_currentMileStone = ((Milestone)((int)m_currentMileStone + 1));
            }
        }
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

    public int NextMilestoneTargetAmount()
    {
        switch(m_currentMileStone)
        {
            case Milestone.InitialMilestone:
                return 10;
            case Milestone.LastMilestone:
                return 1000000;
        }

        return 0;
    }
}
