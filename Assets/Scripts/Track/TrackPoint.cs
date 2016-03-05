using UnityEngine;
using System.Collections;

public class TrackPoint : TCBSplinePoint 
{
	public TrackPointData data;

	public override void OnValidate ()
	{
		base.OnValidate ();
		data.Validate();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		var pts = CalcWorldPoints();
		for(int i = 0; i < pts.Length - 1; i++)
		{
			Gizmos.DrawLine(pts[i], pts[i + 1]);
		}

		Gizmos.color = Color.green;
		Vector3 pl = (pts.Length > 0) ? pts[0] : transform.position;
		Gizmos.DrawLine(pl, pl + transform.forward);

		Gizmos.color = Color.red;
		Vector3 pr = (pts.Length > 0) ? pts[pts.Length - 1] : transform.position;
		Gizmos.DrawLine(pr, pr + transform.forward);
	}

	public Vector3[] CalcLocalPoints()
	{
		return data.CalcLocalPoints();
	}

	public Vector3[] CalcWorldPoints()
	{
		return data.CalcWorldPoints(this.transform.position, this.transform.rotation);
	}
}
