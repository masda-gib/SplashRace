using UnityEngine;
using System.Collections;

public class RacerBase : RacerInformation 
{
	public Renderer debugObject;

	public void SetSteerInput(Vector3 input)
	{
		this.SteerVector = ConvertToSteerVector(input);
	}

	public Vector3 ConvertToSteerVector(Vector3 input)
	{
		return input * this.steerRadius;
	}

	private float roundTime;

	// Use this for initialization
	void Start () {
		MoveStart = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		var timer = Time.time;

		roundTime = timer % (actionPhaseLength + movePhaseLength);
		if (roundTime < actionPhaseLength) 
		{
			if (IsOnTheMove) 
			{
				transform.transform.position = MoveEnd;

				var oldMoveVec = MoveVector;
				MoveStart = MoveEnd;
				var addMove = this.transform.rotation * this.SteerVector;
				MoveVector = Vector3.ClampMagnitude(oldMoveVec + addMove, maxSpeed);
				var lookQuat = (MoveVector.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(MoveVector) : this.transform.rotation;
				this.transform.rotation = lookQuat;

				IsOnTheMove = false;

				this.SteerVector = Vector3.zero;
				this.SteeringReset.Invoke ();
			}
		}
		else 
		{
			if (!IsOnTheMove) 
			{
				IsOnTheMove = true;
			}
		}

		if (debugObject != null && debugObject.material != null) 
		{
			debugObject.material.color = IsOnTheMove ? Color.green : Color.red;
		}
			
		if (IsOnTheMove) 
		{
			var t = (roundTime - actionPhaseLength) / movePhaseLength;
			transform.transform.position = MoveStart + (MoveVector * t);
		}
	}
}
