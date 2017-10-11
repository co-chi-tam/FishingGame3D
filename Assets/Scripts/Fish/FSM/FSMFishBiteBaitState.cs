using UnityEngine;
using System.Collections;
using FSM;

public class FSMFishBiteBaitState : FSMBaseState {

	protected CSimpleFishController m_Controller;

	public FSMFishBiteBaitState(IContext context) : base (context)
	{
		this.m_Controller = context as CSimpleFishController;
	}

	public override void StartState()
	{
		base.StartState ();
	}

	public override void UpdateState(float dt)
	{
		base.UpdateState (dt);
	}

	public override void ExitState()
	{
		base.ExitState ();
	}
}
