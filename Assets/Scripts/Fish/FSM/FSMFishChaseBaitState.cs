using UnityEngine;
using System.Collections;
using FSM;

public class FSMFishChaseBaitState : FSMBaseState {

	protected CSimpleFishController m_Controller;
	protected CSimpleBaitController m_Bait;

	public FSMFishChaseBaitState(IContext context) : base (context)
	{
		this.m_Controller = context as CSimpleFishController;
	}

	public override void StartState()
	{
		base.StartState ();
		this.m_Bait = this.m_Controller.GetBait ();
	}

	public override void UpdateState(float dt)
	{
		base.UpdateState (dt);
		this.m_Controller.ChaseBait (this.m_Bait, dt);
	}

	public override void ExitState()
	{
		base.ExitState ();
	}
}
