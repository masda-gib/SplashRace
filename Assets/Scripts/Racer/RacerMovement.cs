using UnityEngine;
using System.Collections;

public class RacerMovement : MonoBehaviour 
{
	public Vector3 MoveStart { get; protected set; }
	public Vector3 MoveVector { get; protected set; }
	public Vector3 MoveEnd { get { return MoveStart + MoveVector; } }

	// Use this for initialization
	void Start () {
		MoveStart = transform.position;

	}

	public void Process(RacerBase parent, float t)
	{
		parent.transform.position = MoveStart + (MoveVector * t);
	}

	public void StartProcessing(RacerBase parent)
	{
	}

	public void EndProcessing(RacerBase parent)
	{
		transform.transform.position = this.MoveEnd;

		var oldMoveVec = this.MoveVector;
		this.MoveStart = this.MoveEnd;
		var addMove = this.transform.rotation * parent.SteerVector;
		this.MoveVector = Vector3.ClampMagnitude(oldMoveVec + addMove, parent.maxSpeed);
		var lookQuat = (this.MoveVector.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(this.MoveVector) : this.transform.rotation;
		this.transform.rotation = lookQuat;
	}
}
