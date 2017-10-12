using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSimpleController : MonoBehaviour {

	#region Fields

	protected Transform m_Transform;

	#endregion

	#region Implementation Monobehaviour

	protected virtual void OnEnable () {

	}

	protected virtual void OnDisable () {

	}

	public virtual void Init() {

	}

	protected virtual void Awake () {
		this.m_Transform = this.transform;
	}

	protected virtual void Start () {
		
	}

	protected virtual void Update () {
		
	}

	protected virtual void LateUpdate () {

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

	#endregion

}
