using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CustomLineRenderer : MonoBehaviour {

	[SerializeField]	private ELineType m_LineType = ELineType.Straight;
	[SerializeField]	private Transform m_StartPoint;
	[SerializeField]	private Transform m_EndPoint;
	[SerializeField]	private Transform m_CurvePoint;

	private LineRenderer m_Line;

	private enum ELineType: byte {
		Straight 	= 0,
		Curve		= 1
	}

	void Awake () {
		this.m_Line = this.GetComponent<LineRenderer> ();
	}

	void LateUpdate () {
		if (this.m_StartPoint != null && this.m_EndPoint != null) {
			switch (this.m_LineType) {
			case ELineType.Curve:
				if (this.m_CurvePoint != null) {
					this.CurveLine (
						this.m_StartPoint.position, 
						this.m_CurvePoint.transform.position, 
						this.m_EndPoint.transform.position
					);
				}
				break;
			case ELineType.Straight:
			default:
				this.StraightLine();
				break;
			}
		}
	}

	private void StraightLine() {
		this.m_Line.positionCount = 2;
		this.m_Line.SetPosition (0, this.m_StartPoint.position);
		this.m_Line.SetPosition (1, this.m_EndPoint.position);
	}

	private void CurveLine(Vector3 point1, Vector3 point2, Vector3 point3) {
		var pointList = new Vector3[10];
		var segment = 1f / pointList.Length;
		var i = 0;
		for (var ratio = 0f; ratio <= 1f; ratio += segment) {
			var tangentPoint1 = Vector3.Lerp (point1, point2, ratio);
			var tangentPoint2 = Vector3.Lerp (point2, point3, ratio);
			var bezierPoint = Vector3.Lerp (tangentPoint1, tangentPoint2, ratio);
			pointList [i] = bezierPoint;
			i++;
		}
		this.m_Line.positionCount = pointList.Length;
		this.m_Line.SetPositions (pointList);
	}

}
