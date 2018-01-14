using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common 
{
public class StateMachine : SingleInstance<StateMachine>
{
	static readonly int GAME_STATES_STACK_SIZE = 10;

	static gxGameState[]			m_pStateStack = new gxGameState[GAME_STATES_STACK_SIZE];
	static int						m_stateIndex;

	static QuickList<gxGameState>	m_pStatePushed = new QuickList<gxGameState>();
	static gxGameState	         	m_pStatePoped;

	public override void Initalize() 
	{
		m_stateIndex = -1;
		m_pStatePoped = null;
	}	

	public override void Deinitialization() 
	{ 
		//ClearStateStack();
	}

	public void ChangeState(gxGameState pState, bool destroyPrevious = true)
	{

		Common.UDebug.Assert(m_stateIndex >= 0);
		m_pStateStack[m_stateIndex].Release();
		if(destroyPrevious)
		{
			m_pStateStack[m_stateIndex] = null;
		}
		m_stateIndex--;
		
	
		if (pState == null)
		{
			Common.UDebug.Assert(false, "Tried to add a NULL state !!");
			ClearStateStack();
		}
		
		Common.UDebug.Assert(m_stateIndex < GAME_STATES_STACK_SIZE);
		
		m_stateIndex++;
		m_pStateStack[m_stateIndex] = pState;

		int result = pState.Initalize();
		if( result < 0)
		{
			Common.UDebug.Log(" state create failed with " + result);
			Common.UDebug.Assert(false , "Failed to create the current state!!!!");		
			ClearStateStack();
		}
		else
		{
			pState.Resume();
		}
	}


	public void Update(float timestamp)
	{
		if(m_pStatePoped != null)
		{
			m_pStatePoped.Pause();
			m_pStatePoped.Release();
			m_stateIndex--;

			if (m_stateIndex >= 0)
			{
				m_pStateStack[m_stateIndex].Resume();
			}
			ResetTouch();
			
			m_pStatePoped = null;
		}
		
		if(m_pStatePushed.Count > 0)
		{
			gxGameState pState = m_pStatePushed.Pop();
		
			if (pState == null)
			{
				Common.UDebug.Assert(false, "Tried to add a NULL state !!!");
				ClearStateStack();
			}
			
			Common.UDebug.Assert(m_stateIndex < GAME_STATES_STACK_SIZE);
			
			gxGameState oldState = CurrentState();
			if (oldState != null)
			{
				oldState.Pause();
			}
			m_stateIndex++;
			m_pStateStack[m_stateIndex] = pState;
			
			if(pState.Initalize() < 0)
			{		
				Common.UDebug.Assert(false , "Failed to create the current state!!!!");
				ClearStateStack();
			}
			else
			{
				pState.Resume();
				pState.SetLoading(true);
			}
			ResetTouch();
		}

		if (m_stateIndex < 0) 
		{
			return;
		}
		if (m_pStateStack[m_stateIndex].IsLoading())
		{
			m_pStateStack[m_stateIndex].SetLoading(m_pStateStack[m_stateIndex].Load());
		}
		else
		{
			m_pStateStack[m_stateIndex].Update(timestamp);
		}
	}
	
	public void PushState(gxGameState pState)
	{
		m_pStatePushed.Add(pState);
	}
	
	public void PopState(bool bResume)
	{
		Common.UDebug.Assert(m_stateIndex >= 0);
		m_pStatePoped = m_pStateStack[m_stateIndex];
		m_pStateStack [m_stateIndex] = null; //解除引用
	}
	
	void ClearStateStack()
	{
		while (m_stateIndex >= 0)
		{
			m_pStateStack[m_stateIndex].Pause();
			m_pStateStack[m_stateIndex].Release();
			m_pStateStack[m_stateIndex] = null;
			m_stateIndex--;
		}
		
		ResetTouch();
		
		while (m_pStatePushed.Count > 0) 
		{
			m_pStatePushed.Pop ();
		}
		m_pStatePoped = null;
	}

	bool IsStateOnStack(int stateKind)
	{
		for (int i = 0; i < m_stateIndex; i++)
		{
			if (m_pStateStack[i].IsKindOf(stateKind))
			{
				return true;
			}
		}
		return false;
	}
	
	public gxGameState CurrentState()
	{
		return (m_stateIndex >= 0) ? m_pStateStack[m_stateIndex] : null;
	}
	
	void ResetTouch()
	{
		
	}
}

}



