using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CSimpleFishController : CSimpleController, IFishContext {

	#region Internal class

	[Serializable]
	public class CFishData {
		[Header("Move")]
		public float minMoveSpeed	= 0f;
		public float maxMoveSpeed	= 0f;
		public AnimationCurve moveCurve;
		public float aroundRange	= 0f;

		[Header("Rotation")]
		public float rotationSpeed 	= 0f;
		public AnimationCurve rotationCurve;

		[Header("Bait")]
		public float minimumBaitValue 	= 0f;
		public AnimationCurve chaseBaitCurve;

		public CFishData ()
		{
			this.minMoveSpeed	= 5f;
			this.maxMoveSpeed	= 10f;
			this.aroundRange	= 5f;
			this.rotationSpeed 	= 10f;
			this.minimumBaitValue = 2f;
		}

	}

	#endregion

	#region Fields

	[Header("Data")]
	[SerializeField]	protected CFishData m_Data;
	[Header("Bait")]
	[SerializeField]	protected CSimpleBaitController m_Bait;
	[Header("FSM")]
	[SerializeField]	protected TextAsset m_FSMText;
#if UNITY_EDITOR
	[SerializeField]	protected string m_FSMStateName;
#endif

	protected float m_MoveDetailCurve 		= 0f;
	protected float m_RotationDetailCurve 	= 0f;
	protected float m_CurrentAxis 			= 0f;

	protected Vector3 m_StartPos;
	protected Vector3 m_CurrentTargetPos;

	protected float m_ChaseBaitTimer 		= 10f;
	[SerializeField]	protected float m_ChaseBaitRatio 		= 10f;
	protected float m_MaxBaitRatio 			= 10f;

	protected FSMManager m_FSMManager;

	public CFishData data {
		get { return this.m_Data; }
	}

	#endregion

	#region Implementation Monobehaviour

	protected override void Awake() {
		base.Awake ();

		this.m_FSMManager = new FSMManager ();
		this.m_FSMManager.LoadFSM (this.m_FSMText.text);

		this.m_FSMManager.RegisterState ("FishIdleState", 		new FSMFishIdleState(this));
		this.m_FSMManager.RegisterState ("FishChaseBaitState", 	new FSMFishChaseBaitState(this));
		this.m_FSMManager.RegisterState ("FishBiteBaitState", 	new FSMFishBiteBaitState(this));

		this.m_FSMManager.RegisterCondition ("HaveBait", 		this.HaveBait);
		this.m_FSMManager.RegisterCondition ("IsBiteBail", 		this.IsBiteBail);
		this.m_FSMManager.RegisterCondition ("IsBailTimer",		this.IsBailTimer);
	}

	protected override void Start() {
		base.Start ();
		this.m_StartPos = this.m_Transform.position;
		this.m_CurrentTargetPos = this.m_Transform.position;
	}

	protected override void Update() {
		base.Update ();
		this.m_FSMManager.UpdateState (Time.deltaTime);
#if UNITY_EDITOR
		this.m_FSMStateName = this.m_FSMManager.currentStateName;
#endif
	}

	#endregion

	#region Main methods

	public virtual void ChaseBait(CSimpleBaitController bait, float dt) {
		this.MoveAroundTarget (bait.GetPosition (), this.m_ChaseBaitRatio, dt);
		this.m_ChaseBaitTimer -= dt;
//		this.m_ChaseBaitRatio += dt;
	}

	public virtual void BiteBait(CSimpleBaitController bait, float dt) {
		this.m_Transform.position = bait.GetPosition ();
	}

	public virtual void MoveFollow(Vector3 targetPos, float dt) {
		var direction = targetPos - this.m_Transform.position;
		var lookRotation = Quaternion.Lerp (
			this.m_Transform.rotation, 
			Quaternion.LookRotation (direction), 
			this.m_Data.rotationCurve.Evaluate (this.m_RotationDetailCurve)
		); 
		this.m_Transform.rotation = lookRotation;
		this.m_RotationDetailCurve += dt;
		this.MoveForward (dt);
	}

	public virtual void MoveAroundStartPoint(float dt) {
		this.MoveAroundTarget (this.m_StartPos, this.data.aroundRange, dt);
	}

	public virtual void MoveAroundTarget(Vector3 targetPos, float radius, float dt) {
		var direction = this.m_CurrentTargetPos - this.m_Transform.position;
		if (direction.sqrMagnitude > 0.1f) {
			this.MoveFollow (this.m_CurrentTargetPos, dt);
		} else {
			var randomInside = UnityEngine.Random.insideUnitCircle;
			var randomNewPos = randomInside * radius;
			var currentNewPos = targetPos;
			currentNewPos.x += randomNewPos.x;
			currentNewPos.y = this.m_Transform.position.y;
			currentNewPos.z += randomNewPos.y;
			this.m_CurrentTargetPos = currentNewPos;
			this.m_RotationDetailCurve = 0f;
		}
	}

	public virtual void MoveForward(float dt) {
		var forward = this.m_Transform.forward;
		var moveSpeed = this.GetMoveSpeed ();
		var movePosition = forward.normalized * moveSpeed * dt;
		var moveCurve = this.m_Transform.position + movePosition;
		moveCurve.y = this.m_Transform.position.y;
		this.m_Transform.position = moveCurve;
		this.m_MoveDetailCurve = (this.m_MoveDetailCurve + dt) % 1f;
	}

	public virtual void TurnObject(float axis, float dt) {
		this.m_CurrentAxis = this.m_Transform.rotation.eulerAngles.y;
		this.m_CurrentAxis += axis * this.m_Data.rotationSpeed * dt;
		this.m_Transform.rotation = Quaternion.AngleAxis (this.m_CurrentAxis, Vector3.up);
		this.m_RotationDetailCurve += dt;
	}

	#endregion

	#region FSM

	public virtual bool HaveBait ()
	{
		return this.m_Bait != null;
	}

	public virtual bool IsBiteBail ()
	{
		return this.m_ChaseBaitRatio <= this.m_Data.minimumBaitValue;
	}

	public virtual bool IsBailTimer ()
	{
		return this.m_ChaseBaitTimer > 0f;
	}

	#endregion

	#region Getter && Setter

	public virtual void SetBaitRatio(float value) {
		var segment = this.m_ChaseBaitRatio / this.m_MaxBaitRatio;
		var newRatio = this.m_Data.chaseBaitCurve.Evaluate (segment);
		var newValue = value * newRatio;
		this.m_ChaseBaitRatio -= newValue;
		this.m_ChaseBaitTimer = this.m_MaxBaitRatio;
	}

	public virtual void SetBait(CSimpleBaitController value) {
		this.m_Bait 			= value;
		this.m_ChaseBaitTimer	= value.data.baitTimer;
		this.m_ChaseBaitRatio 	= value.data.baitTimer;
		this.m_MaxBaitRatio 	= value.data.baitTimer;
	}

	public virtual CSimpleBaitController GetBait() {
		return this.m_Bait;
	}

	public virtual float GetMoveSpeed() {
		return (this.m_Data.maxMoveSpeed - this.m_Data.minMoveSpeed) 
			* this.m_Data.moveCurve.Evaluate (this.m_MoveDetailCurve) 
			+ this.m_Data.minMoveSpeed;
	}

	#endregion

}
