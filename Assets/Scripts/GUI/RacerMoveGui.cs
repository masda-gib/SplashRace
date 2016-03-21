using UnityEngine;
using System.Collections;

public class RacerMoveGui : MonoBehaviour 
{
	public ControlProvider cp;
	public Transform moveStartMarker;
	public Transform moveTargetAreaMarker;
	public Transform steerInputMarker;
	public Transform steerInputVectorMarker;
	public Transform steeringVectorMarker;
	public bool useNextMove;
	public float heightOffset = -0.15f;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (cp != null && cp.racer != null) {
			var r = cp.racer;
			var nextTarget = r.movement.MoveEnd + r.movement.MoveVector;

			if (moveStartMarker != null) {
				moveStartMarker.position = r.movement.MoveEnd + (Vector3.up * heightOffset);
			}

			if (moveTargetAreaMarker != null) {
				moveTargetAreaMarker.position = nextTarget + (Vector3.up * heightOffset);
				moveTargetAreaMarker.localScale = new Vector3 (r.tempValues.statistics.GetSteerRadiusFor(0) * 2, 1, r.tempValues.statistics.GetSteerRadiusFor(0) * 2);
			}

			var endMarker = SetMoveMarker (steerInputVectorMarker, cp, false);
			SetMoveMarker (steeringVectorMarker, cp, true);

			if (steerInputMarker != null) 
			{
				steerInputMarker.position = endMarker;
			}
		}
	}

	private Vector3 SetMoveMarker(Transform marker, ControlProvider cp, bool isSet) 
	{
		var r = cp.racer;
		var steer = (isSet) ? cp.ConfirmedSteering : cp.CurrentSteering;
		steer = cp.racer.transform.rotation * steer;
		var begin = (useNextMove) ? r.movement.MoveEnd + r.movement.MoveVector + steer : r.movement.MoveEnd;
		var v = r.movement.MoveEnd + r.movement.MoveVector + steer - r.movement.MoveEnd;
		if (marker != null) 
		{
			var vm = v.magnitude;
			marker.position = begin  + 0.5f * v;
			marker.localScale = new Vector3 (1, 1, vm);
			marker.rotation =  (vm > 0.001f) ? Quaternion.LookRotation (v) : Quaternion.identity;
		}
		return begin + v;
	}
}
