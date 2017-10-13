using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSimpleBaitController : CSimpleController {

	#region Internal class

	[Serializable]
	public class CBaitData {
		
		[Header("Bait")]
		public float baitRange		= 0f;
		public float baitTimer		= 0f;
		public float baitValue		= 0f;

		public CBaitData ()
		{
			this.baitRange		= 10f;
			this.baitTimer		= 10f;
			this.baitValue		= 1f;
		}

	}

	#endregion

	#region Fields

	[Header("Data")]
	[SerializeField]	protected CBaitData m_Data;
	[Header("Fish attract")]
	[SerializeField]	protected LayerMask m_FishLayerMask;
	[SerializeField]	protected CSimpleFishController m_CurrentFishController;
	[Header("Parabolla")]
	[SerializeField]	protected Parabola m_Parabolla;
	[Header("Animator")]
	[SerializeField]	protected Animator m_Animator;

	public CBaitData data {
		get { return this.m_Data; }
	}

	#endregion

	#region Main methods

	public virtual void ThrowBait(Vector3 position, Action complete) {
		this.m_Parabolla.JumpTo (position, null, complete);
	}

	public virtual void AttractBait() {
		if (this.m_CurrentFishController == null) {
			var inRangeFishs = Physics.OverlapSphere (this.GetFitPosition (), this.m_Data.baitRange, this.m_FishLayerMask);
			if (inRangeFishs.Length > 0) {
				var random = UnityEngine.Random.Range (0, inRangeFishs.Length);
				var fish = inRangeFishs [random];
				this.m_CurrentFishController = fish.GetComponent<CSimpleFishController> ();
			}
		}
		if (this.m_CurrentFishController != null) {
			if (this.m_CurrentFishController.GetBait() == null) {
				this.m_CurrentFishController.SetBait (this);
			} 
			this.m_CurrentFishController.SetBaitRatio (this.m_Data.baitValue);
		}
	}

	#endregion

	#region Getter && Setter

	public override void SetAnimation (string name, object param = null)
	{
		base.SetAnimation (name, param);
		if (this.m_Animator == null)
			return;
		if (param is int) {
			this.m_Animator.SetInteger (name, (int)param);
		} else if (param is bool) {
			this.m_Animator.SetBool (name, (bool)param);
		} else if (param is float) {
			this.m_Animator.SetFloat (name, (float)param);
		} else if (param == null) {
			this.m_Animator.SetTrigger (name);
		}
	}

	#endregion

}
