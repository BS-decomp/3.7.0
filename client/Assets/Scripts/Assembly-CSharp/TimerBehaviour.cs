using System.Collections.Generic;
using UnityEngine;

public class TimerBehaviour : MonoBehaviour
{
	public List<vp_Timer.Handle> TimersList = new List<vp_Timer.Handle>();

	public virtual vp_Timer.Handle GetTimer()
	{
		for (int i = 0; i < TimersList.Count; i++)
		{
			if (!TimersList[i].Active)
			{
				TimersList.RemoveAt(i);
			}
		}
		vp_Timer.Handle handle = new vp_Timer.Handle();
		TimersList.Add(handle);
		return handle;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < TimersList.Count; i++)
		{
			if (TimersList[i].Active)
			{
				TimersList[i].Cancel();
			}
		}
	}

	public virtual void DeactiveAllTimers()
	{
		for (int i = 0; i < TimersList.Count; i++)
		{
			if (TimersList[i].Active)
			{
				TimersList[i].Cancel();
			}
		}
		TimersList.Clear();
	}
}
