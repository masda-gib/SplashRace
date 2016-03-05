using UnityEngine;
using System.Collections;

// https://de.wikipedia.org/wiki/Kubisch_Hermitescher_Spline#Kochanek-Bartels-Spline

[System.Serializable]
public class TCBSplineSegment : CatmullRomeSegment 
{
	// to be able to render meshes and calculate lanes correctly correctly
	protected Vector3 currUp;
	protected float twist;

	public static TCBSplineSegment From4Points (Vector3 prev, TCBSplinePoint start, TCBSplinePoint end, Vector3 next, bool normalized)
	{
		var t0 = (normalized) ? start.GetNormalizedTangent(prev, end.Position) : start.Tangent;
		var t1 = (normalized) ? end.GetNormalizedTangent(start.Position, next) : end.Tangent;

		t0 = start.GetOutTangent(t0, prev, end.Position);
		t1 = end.GetInTangent(t1, start.Position, next);

		return new TCBSplineSegment(start, t0, end, t1);
	}

	public TCBSplineSegment(TCBSplinePoint start, Vector3 startTangent, TCBSplinePoint end, Vector3 endTangent) 
		: base(start.Position, startTangent, end.Position, endTangent)
	{
		InitTwist(start, startTangent, end, endTangent);
	}

	public TCBSplineSegment(TCBSplinePoint start, TCBSplinePoint end) 
		: this(start, start.Tangent, end, end.Tangent)
	{
		InitTwist(start, start.Tangent, end, end.Tangent);
	}

	public override Vector3 InterpolateUpVector(float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		Vector3 tangent = Derivate(t);
		Quaternion rotDiff = Quaternion.FromToRotation(t0, tangent);
		Quaternion twistDiff = Quaternion.AngleAxis(twist * t, tangent);
		return twistDiff * rotDiff * currUp ;
	}

	private void InitTwist(TCBSplinePoint start, Vector3 startTangent, TCBSplinePoint end, Vector3 endTangent)
	{
		// this up for the given tangent
		var startTDiff = Quaternion.FromToRotation(start.Tangent, startTangent);
		currUp = startTDiff * start.Up;

		// the end point's up for the given tangent
		var endTDiff = Quaternion.FromToRotation(end.Tangent, endTangent);
		var endUp = endTDiff * end.Up;

		// where the end up should be without twist
		var tDiff = Quaternion.FromToRotation(startTangent, endTangent);
		var myEndUp = tDiff * currUp;

		// calculate twist
		var sameCrossDir = Vector3.Dot(Vector3.Cross(myEndUp, endUp), endTangent);
		sameCrossDir = Mathf.Sign(sameCrossDir);
		twist = Vector3.Angle(myEndUp, endUp) * sameCrossDir;
	}
}
