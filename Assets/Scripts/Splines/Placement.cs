using UnityEngine;

public struct Placement
{
	/// <summary>
	/// Direction away from Spline.
	/// </summary>
	public Vector3 up;

	/// <summary>
	/// Tangent on Spline on this point.
	/// </summary>
	public Vector3 forward;

	/// <summary>
	/// Position of this placement object.
	/// </summary>
	public Vector3 position;

	/// <summary>
	/// Corresponding point on the spline for this position.
	/// </summary>
	public Vector3 pointOnSpline;

	public void PlaceTransform(Transform transform)
	{
		PlaceTransform(transform, false);
	}

	public void PlaceTransform(Transform transform, bool onSpline)
	{
		if (forward == Vector3.zero)
		{
			Debug.LogError("Cannot place transform " + transform.name + ". Deactivating.");
			transform.gameObject.SetActive(false);
		}
		transform.position = (onSpline) ? pointOnSpline : position;
		if (forward.magnitude != 0 && up.magnitude != 0)
		{
			transform.rotation = Quaternion.LookRotation(forward, up);
		}
	}
}