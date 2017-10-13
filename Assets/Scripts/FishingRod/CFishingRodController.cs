using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CFishingRodController : CSimpleController, IFishingRodContext {

	#region Internal class

	[Serializable]
	public class CFishingRodData {

		[Header("Throw force")]
		[Range(0.1f, 1f)]
		public float maximumThrowForce		= 0f;
		public float maximumForceTimer		= 0f;
		public AnimationCurve throwForceCurve;

		[Header("Scroll force")]
		public float maximumScrollForce = 0f;

		public CFishingRodData ()
		{
			this.maximumThrowForce		= 0.5f;
			this.maximumForceTimer		= 3f;
			this.maximumScrollForce 	= 2f;
		}

	}

	#endregion

	#region Fields

	[Header("Data")]
	[SerializeField]	protected CFishingRodData m_Data;

	[Header("FSM")]
	[SerializeField]	protected TextAsset m_FSMText;
#if UNITY_EDITOR
	[SerializeField]	protected string m_FSMStateName;
#endif

	[Header("Animator")]
	[SerializeField]	protected Animator m_Animator;

	[Header("Bait")]
	[SerializeField]	protected GameObject m_BaitPoint;
	[SerializeField]	protected CSimpleBaitController m_Bait;
	[SerializeField]	protected LayerMask m_WaterLayerMask;

	public CFishingRodData data {
		get { return this.m_Data; }
	}

	protected bool m_IsBaitThrowed;
	protected float m_CurrentForce = 0f;

	protected FSMManager m_FSMManager;

	#endregion

	#region Implementation Monobehaviour

	protected override void Awake ()
	{
		base.Awake ();

		this.m_FSMManager = new FSMManager ();
		this.m_FSMManager.LoadFSM (this.m_FSMText.text);

		this.m_FSMManager.RegisterState ("RepairFishingState", 		new FSMRepairFishingState(this));
		this.m_FSMManager.RegisterState ("StartFishingState", 		new FSMStartFishingState(this));
		this.m_FSMManager.RegisterState ("FinishFishingState", 		new FSMFinishFishingState(this));

		this.m_FSMManager.RegisterCondition ("IsObjectActive",		this.IsObjectActive);
		this.m_FSMManager.RegisterCondition ("IsBaitThrowed",		this.IsBaitThrowed);
		this.m_FSMManager.RegisterCondition ("IsFishBite", 			this.IsFishBite);
		this.m_FSMManager.RegisterCondition ("IsFishingEnd",		this.IsFishingEnd);
	}

	protected override void Start ()
	{
		base.Start ();
	}

	protected override void Update ()
	{
		base.Update ();
		this.m_FSMManager.UpdateState (Time.deltaTime);
#if UNITY_EDITOR
		this.m_FSMStateName = this.m_FSMManager.currentStateName;
#endif
	}

	#endregion

	#region Main methods

	public virtual void RepairBait() {
		this.m_IsBaitThrowed = false;
		this.m_Bait.SetPosition (this.m_BaitPoint.transform.position);
		this.m_Bait.SetAnimation ("isFloating", 0);
		this.m_CurrentForce = 0f;
	}

	public virtual void RepairFishing(float dt) {
		if (Input.GetMouseButtonDown (0)) {
			this.m_CurrentForce = 0f;
			this.SetAnimation ("FishingStep", 1);
		}
		if (Input.GetMouseButton (0)) {
			this.m_CurrentForce += dt;
			this.m_Bait.SetPosition (this.m_BaitPoint.transform.position);
		}
		if (Input.GetMouseButtonUp (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo, this.m_WaterLayerMask)) {
				var throwForce = this.m_CurrentForce / this.m_Data.maximumForceTimer;
				var totalForce = this.m_Data.maximumThrowForce * this.m_Data.throwForceCurve.Evaluate (throwForce);
				var minimumRange = hitInfo.point;
				var baitPosition =  minimumRange * totalForce;
				baitPosition.y = 0f;
				this.m_Bait.ThrowBait (baitPosition, () => {
					this.m_Bait.SetAnimation ("isFloating", 1);
					this.m_Bait.AttractBait();
				});
				this.SetAnimation ("FishingStep", 2);
			}
			this.m_IsBaitThrowed = true;
			this.m_CurrentForce = 0f;
		}
	}

	public virtual void AttractBait(float dt) {
		if (Input.GetMouseButtonUp (0)) {
			this.m_Bait.SetAnimation ("IsAttractive");
			this.m_Bait.AttractBait();
		}
	}

	public virtual void ScrollBait(float dt) {
		var direction = this.GetFitPosition (this.m_BaitPoint.transform.position) - this.m_Bait.GetFitPosition ();
		var updatePosition = direction.normalized 
			* this.m_Data.maximumScrollForce 
			* dt;
		var movePosition = this.m_Bait.GetPosition () + updatePosition;
		this.m_Bait.SetPosition (movePosition);
		this.SetAnimation ("IsScrolling", true);
	}

	#endregion

	#region FSM

	public virtual bool IsBaitThrowed ()
	{
		return this.m_IsBaitThrowed;
	}

	public virtual bool IsFishBite ()
	{
		return false;
	}

	public virtual bool IsFishingEnd ()
	{
		var direction = this.GetFitPosition (this.m_BaitPoint.transform.position) - this.m_Bait.GetFitPosition ();
		return direction.sqrMagnitude < 0.1f;
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
