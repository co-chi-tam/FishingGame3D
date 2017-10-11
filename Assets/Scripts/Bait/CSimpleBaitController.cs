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
	[Header("Parabolla")]
	[SerializeField]	protected Parabola m_Parabolla;

	public CBaitData data {
		get { return this.m_Data; }
	}

	#endregion

	#region Main methods

	public virtual void ThrowBait(Vector3 position, Action complete) {
		this.m_Parabolla.JumpTo (position, null, complete);
	}

	#endregion

}
