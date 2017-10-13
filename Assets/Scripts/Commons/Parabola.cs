using UnityEngine;
using System;
using System.Collections;

public class Parabola : MonoBehaviour {

	[Header("Info")]
	[SerializeField]	private float m_FiringAngle = 45f;
	[SerializeField]	private float m_Gravity = 9.8f;
	[SerializeField]	private float m_DeltaSpeed = 1f;

	private Transform m_Transform;
	private bool m_OnActive;
	private WaitForFixedUpdate m_WaitForFixedUpdate;

	private void Awake ()
	{
		m_Transform = this.transform;
		m_OnActive = false;
		m_WaitForFixedUpdate = new WaitForFixedUpdate ();
	}

	public void JumpTo(Vector3 position, Action<float> processing = null, Action complete = null) {
		StartCoroutine(HandleJumpToPosition(position, processing, complete));
	}

	public IEnumerator HandleJumpToPosition(Vector3 position, Action<float> processing = null, Action complete = null) {
		if (m_OnActive == true) {
			yield break;
		}
		if (m_Transform == null) {
			yield break;
		}
		m_OnActive = true;
		var targetDistance = Vector3.Distance (m_Transform.position, position);
		var velocity = targetDistance / (Mathf.Sin (2 * m_FiringAngle * Mathf.Deg2Rad) / m_Gravity);
		var x = Mathf.Sqrt (velocity) * Mathf.Cos (m_FiringAngle * Mathf.Deg2Rad);
		var y = Mathf.Sqrt (velocity) * Mathf.Sin (m_FiringAngle * Mathf.Deg2Rad);
		var flightDuration = targetDistance / x;
		var direction = position - m_Transform.position;
		if (direction != Vector3.zero) {
			m_Transform.rotation = Quaternion.LookRotation (direction);
		}
		float elapseTime = 0;
		var deltaSpeed = Time.deltaTime * this.m_DeltaSpeed;
		while (elapseTime < flightDuration - Time.deltaTime)
		{
			if (this.m_Transform == null)
				break;
			this.m_Transform.Translate(0, (y - (m_Gravity * elapseTime)) * deltaSpeed, x * deltaSpeed, Space.Self);
			elapseTime += deltaSpeed;
			if (processing != null) {
				processing(elapseTime / flightDuration);
			}
			yield return m_WaitForFixedUpdate;
		}
		if (complete != null) {
			complete ();
		}
		m_OnActive = false;
	}
}
