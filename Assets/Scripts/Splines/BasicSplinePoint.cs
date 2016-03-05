using UnityEngine;
using System.Collections;

public class BasicSplinePoint : MonoBehaviour
{
	public Vector3 Position
	{
		get { return transform.position; }
	}

	public Vector3 Forward
	{
		get { return transform.forward; }
	}
	
	public Vector3 Up
	{
		get { return transform.up; }
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos() 
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, .2f);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.color = Color.gray;
		Gizmos.DrawLine(transform.position, transform.position + transform.up);
	}
#endif
}