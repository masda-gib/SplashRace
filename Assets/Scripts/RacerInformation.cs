using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RacerInformation : MonoBehaviour 
{
	public float actionPhaseLength = 1;
	public float movePhaseLength = 2;
	public float steerRadius = 1;
	public float maxSpeed = 4;

	public bool IsOnTheMove { get; protected set; }
	public Vector3 MoveStart { get; protected set; }
	public Vector3 MoveVector { get; protected set; }
	public Vector3 MoveEnd { get { return MoveStart + MoveVector; } }

	public Vector3 SteerVector { get; protected set; }

	public UnityEvent SteeringReset;
}
