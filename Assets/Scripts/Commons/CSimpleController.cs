using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSimpleController : MonoBehaviour {

	#region Fields

	protected Transform m_Transform;
	protected bool m_ObjectActive;

	#endregion

	#region Implementation Monobehaviour

	protected virtual void OnEnable () {
		this.m_ObjectActive = true;
	}

	protected virtual void OnDisable () {
		this.m_ObjectActive = false;
	}

	public virtual void Init() {

	}

	protected virtual void Awake () {
		this.m_Transform = this.transform;
		this.m_ObjectActive = true;
	}

	protected virtual void Start () {
		
	}

	protected virtual void Update () {
		
	}

	protected virtual void LateUpdate () {

	}

	#endregion

	#region FSM

	public virtual bool IsObjectActive() {
		return this.m_ObjectActive;
	}

	#endregion

	#region Getter && Setter

	public virtual Vector3 GetFitPosition() {
		var result = this.m_Transform.position;
		result.y = 0f;
		return result;
	}

	public virtual Vector3 GetFitPosition(Vector3 position) {
		var result = position;
		result.y = 0f;
		return result;
	}

	public virtual Vector3 GetPosition() {
		return this.m_Transform.position;
	}

	public virtual void SetPosition(Vector3 value) {
		this.m_Transform.position = value;
	}

	public virtual void SetAnimation (string name, object param = null)
	{
		
	}

	#endregion

}
