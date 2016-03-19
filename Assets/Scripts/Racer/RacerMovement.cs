using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class RacerMovement : MonoBehaviour 
{
	protected RacerBase parent;

	public Vector3 steerInput;
	public Vector3 moveStart;
	public Vector3 moveVector;

	public Vector3 SteerVector { get { return ConvertToSteerVector(steerInput); } }

	public Vector3 MoveStart { get { return parent.tempValues.moveStart; } }
	public Vector3 MoveVector { get { return parent.tempValues.moveVector; } }
	public Vector3 MoveEnd { get { return MoveStart + MoveVector; } }

	public UnityEvent SteeringReset;

	public void FillTempValues(RacerCurrValues tempValues) {
		tempValues.moveStart = moveStart;
		tempValues.moveVector = moveVector;
	}

	public void Init (RacerBase parent) {
		this.parent = parent;
		this.moveStart = transform.position;
	}

	public Vector3 ConvertToSteerVector(Vector3 input)
	{
		return input * parent.tempValues.steerRadius;
	}

	public void SetSteerInput(Vector3 input)
	{
		this.steerInput = input;
	}

	public void Process(float t)
	{
		parent.transform.position = MoveStart + (MoveVector * t);
	}

	public void StartProcessing()
	{
	}

	public void EndProcessing()
	{
		transform.transform.position = this.MoveEnd;

		var oldMoveVec = this.MoveVector;
		this.moveStart = this.MoveEnd;
		var addMove = this.transform.rotation * SteerVector;
		this.moveVector = Vector3.ClampMagnitude(oldMoveVec + addMove, parent.tempValues.maxSpeed);

		this.steerInput = Vector3.zero;
		this.SteeringReset.Invoke ();

		var lookQuat = (this.MoveVector.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(this.MoveVector) : this.transform.rotation;
		parent.transform.rotation = lookQuat;
	}
}
