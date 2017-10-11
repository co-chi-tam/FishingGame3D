using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragable : MonoBehaviour {

	public float power = 5f;
	public float angle = 45f;

	private Rigidbody2D ball;

//	void Update ()
//	{
//		if(Input.GetMouseButtonDown(0))
//		{
//			createBall();
//		}
//		else if(Input.GetMouseButtonUp(0))
//		{
//			throwBall();
//		}
		// when mouse button is pressed, cannon is rotated as per mouse movement and projectile trajectory path is displayed.
//		if(isPressed)
//		{
//			Vector3 vel = GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
//			float angle = Mathf.Atan2(vel.y,vel.x)* Mathf.Rad2Deg;
//			transform.eulerAngles = new Vector3(0,0,angle);
//			setTrajectoryPoints(startPos.position, vel / ball.mass);
//		}
//	}

	public virtual GameObject createBall(Rigidbody2D value, Vector3 startPos)
	{
		ball = Instantiate(value);
		ball.transform.position = startPos;
		ball.gameObject.SetActive(false);
		return ball.gameObject;
	}

	public virtual void setBall(Rigidbody2D value, Vector3 startPos) {
		ball = value;
		ball.transform.position = startPos;
		ball.gameObject.SetActive(false);
	}

	public virtual void throwBall() {
		this.throwBall (Vector3.zero, 30);
	}

	//---------------------------------------    
	// Following method gives force to the ball
	//---------------------------------------    
	public virtual Vector3[] throwBall(Vector3 offsetWind, int numOfTrajectoryPoints)
	{
		if (ball == null)
			return null;
		ball.gameObject.SetActive(true);    
		ball.bodyType 	= RigidbodyType2D.Dynamic;
		var forceAngle 	= Quaternion.AngleAxis (angle, Vector3.forward) * Vector3.right;
		var origin 		= ball.transform.position;
		var direction 	= origin + forceAngle + offsetWind; 
		var totalForce 	= GetForceFrom (origin, direction);
		ball.AddForce(totalForce, ForceMode2D.Impulse);
		var velocity 	= this.GetForceFrom(origin, direction);
//		float angle 	= Mathf.Atan2(velocity.y, velocity.x)* Mathf.Rad2Deg;
//		transform.eulerAngles = new Vector3(0,0,angle);
		return this.setTrajectoryPoints (origin, velocity, numOfTrajectoryPoints);
	}

	//---------------------------------------    
	// Following method returns force by calculating distance between given two points
	//---------------------------------------    
	private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
	}

	private Vector3[] setTrajectoryPoints(Vector3 pStartPosition , Vector3 pVelocity, int numOfTrajectoryPoints)
	{
		float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg*(Mathf.Atan2(pVelocity.y , pVelocity.x));
		float fTime = 0;
		var result = new Vector3[numOfTrajectoryPoints];
		fTime += 0.1f;
		for (int i = 0 ; i < numOfTrajectoryPoints ; i++)
		{
			float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx , pStartPosition.y + dy , 0f);
			result [i] = pos;
//			trajectoryPoints[i].transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);
			fTime += 0.1f;
		}
		return result;
	}

}
