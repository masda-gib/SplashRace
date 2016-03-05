using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RacerBase : RacerInformation 
{
	public bool IsOnTheMove { get; protected set; }
	public Vector3 SteerVector { get; protected set; }

	private RacerMovement movement;

	public RacerMovement Movement {
		get {
			if (movement == null) {
				movement = GetComponent<RacerMovement> ();
			}
			return movement;
		}
	}

	public UnityEvent SteeringReset;

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
				IsOnTheMove = false;

				movement.EndProcessing (this);

				this.SteerVector = Vector3.zero;
				this.SteeringReset.Invoke ();
			}
		}
		else 
		{
			if (!IsOnTheMove) 
			{
				IsOnTheMove = true;

				movement.StartProcessing (this);
			}
		}
			
		if (IsOnTheMove) 
		{
			var t = (roundTime - actionPhaseLength) / movePhaseLength;

			movement.Process (this, t);
		}

		if (debugObject != null && debugObject.material != null) 
		{
			debugObject.material.color = IsOnTheMove ? Color.green : Color.red;
		}
	}
}
