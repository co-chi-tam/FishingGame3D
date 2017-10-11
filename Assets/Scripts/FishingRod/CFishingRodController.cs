using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFishingRodController : CSimpleController {

	#region Internal class

	[Serializable]
	public class CFishingRodData {

		[Header("Fishing Rod")]
		[Range(0.1f, 1f)]
		public float maximumThrowForce		= 0f;
		public float maximumForceTimer		= 0f;
		public AnimationCurve throwForceCurve;

		public CFishingRodData ()
		{
			this.maximumThrowForce		= 0.5f;
			this.maximumForceTimer		= 3f;
		}

	}

	#endregion

	#region Fields

	[Header("Data")]
	[SerializeField]	protected CFishingRodData m_Data;

	[Header("Animator")]
	[SerializeField]	protected Animator m_Animator;

	[Header("Bait")]
	[SerializeField]	protected bool m_IsBaitThrowed;
	[SerializeField]	protected CSimpleBaitController m_Bait;
	[SerializeField]	protected LayerMask m_WaterLayerMask;

	public CFishingRodData data {
		get { return this.m_Data; }
	}

	protected float m_CurrentThrowForce = 0f;

	#endregion

	#region Implementation Monobehaviour

	protected override void Awake ()
	{
		base.Awake ();
		this.m_IsBaitThrowed = false;
	}

	protected override void Update ()
	{
		base.Update ();
		this.StartFishing (Time.deltaTime);
	}

	#endregion

	#region Main methods

	public virtual void StartFishing(float dt) {
		if (this.m_IsBaitThrowed == true)
			return;
		if (Input.GetMouseButtonDown (0)) {
			this.m_CurrentThrowForce = 0f;
			this.SetAnimation ("ScrollType", 1);
		}
		if (Input.GetMouseButton (0)) {
			this.m_CurrentThrowForce += dt;
		}
		if (Input.GetMouseButtonUp (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo, this.m_WaterLayerMask)) {
				var throwForce = this.m_CurrentThrowForce / this.m_Data.maximumForceTimer;
				var totalForce = this.m_Data.maximumThrowForce * this.m_Data.throwForceCurve.Evaluate (throwForce);
				Debug.Log (totalForce);
				var baitPosition = hitInfo.point * totalForce;
				this.m_Bait.ThrowBait (baitPosition, () => {
					Debug.Log ("Throwed");
				});
				this.SetAnimation ("ScrollType", 2);
			}
			this.m_IsBaitThrowed = true;
			this.m_CurrentThrowForce = 0f;
		}
	}

	#endregion

	#region Getter && Setter

	public virtual void SetAnimation (string name, object param = null)
	{
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
