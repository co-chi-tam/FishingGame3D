using UnityEngine;
using System.Collections;
using FSM;

public class FSMRepairFishingState : FSMBaseState{

	protected CFishingRodController m_Controller;

	public FSMRepairFishingState(IContext context) : base (context)
	{
		this.m_Controller = context as CFishingRodController;
	}

	public override void StartState()
	{
		base.StartState ();
	}

	public override void UpdateState(float dt)
	{
		base.UpdateState (dt);
		this.m_Controller.RepairFishing (dt);
	}

	public override void ExitState()
	{
		base.ExitState ();
	}
}
