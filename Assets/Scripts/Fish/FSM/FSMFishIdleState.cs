using UnityEngine;
using System.Collections;
using FSM;

public class FSMFishIdleState : FSMBaseState {

	protected CSimpleFishController m_Controller;

	public FSMFishIdleState(IContext context) : base (context)
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
		this.m_Controller.MoveAroundStartPoint (dt);
	}

	public override void ExitState()
	{
		base.ExitState ();
	}
}
