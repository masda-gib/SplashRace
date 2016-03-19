using UnityEngine;
using System.Collections;

public class RacerCameraFollow : MonoBehaviour 
{
	public RacerBase racerBase;
	public float behind = 4;
	public float up = 1;
	public float speed = 1;

	Quaternion lookQuat = Quaternion.identity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		var mv = racerBase.movement.MoveVector;
		lookQuat = (mv.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(mv) : lookQuat;
		var offset = Vector3.forward * -1 * behind + Vector3.up * up;
		offset = lookQuat * offset;

		this.transform.position = Vector3.Lerp(this.transform.position, racerBase.transform.position + offset, Time.deltaTime * speed);
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookQuat, Time.deltaTime * speed);
	}
}
