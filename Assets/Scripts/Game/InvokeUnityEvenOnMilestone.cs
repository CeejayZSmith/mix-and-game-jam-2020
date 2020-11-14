using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeUnityEvenOnMilestone : MonoBehaviour
{
    public GameManager.Milestone m_milestoneToAchieve = GameManager.Milestone.InitialMilestone;
    public UnityEngine.Events.UnityEvent m_onMilestoneAchievedEvent = new UnityEngine.Events.UnityEvent();
    public void OnMilestoneAchieved(GameManager.Milestone milestone)
    {
        if(milestone == m_milestoneToAchieve)
        {
            m_onMilestoneAchievedEvent?.Invoke();
        }
    }

    public void OnEnable()
    {
        GameManager.Instance.OnMilestoneAchieved += OnMilestoneAchieved;
    }
    public void OnDisable()
    {
        GameManager.Instance.OnMilestoneAchieved -= OnMilestoneAchieved;
    }

}

