using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCBSplineGroup : ASplineGroup
{
	public enum NormalizationMode
	{
		NONE,
		SKIP_ENDS,
		ALL
	}

	public TCBSplinePoint[] points;
	public bool dynamicUpdate;
	public NormalizationMode normalizeMode;

	protected TCBSplineSegment[] segments;
	public override ISplineSegment[] Segments
	{
		get
		{
			InitSegments(dynamicUpdate);
			return segments;
		}
	}

	public override BasicSplinePoint[] Points 
	{
		get { return TCBPoints; }
	}

	public TCBSplinePoint[] TCBPoints
	{
		get 
		{
			// collect only non-null entries
			List<TCBSplinePoint> validPoints = new List<TCBSplinePoint>();
			if (points != null)
			{
				foreach (var p in points)
				{
					if (p != null)
					{
						validPoints.Add(p);
					}
				}
			}
			return validPoints.ToArray(); 
		}
	}

	public override Placement GetPlacementAt(float angle, float distance, float height)
	{
		int i = 0;
		bool found = false;
		InitSegments(dynamicUpdate);
		for (; i < segments.Length && !found; i++)
		{
			if (distance < segments[i].Length)
			{
				found = true;
			}
			else if (i < segments.Length - 1)
			{
				distance -= segments[i].Length;
			}
		}

		ISplineSegment segment = segments[i - 1];

		float interpolateToDistance = segment.Arclength2Param(distance);
		Placement placement = new Placement();
		placement.pointOnSpline = segment.Interpolate(interpolateToDistance);
		placement.forward = segment.Derivate(interpolateToDistance);
		placement.up = Quaternion.AngleAxis(angle, placement.forward) * segment.InterpolateUpVector(interpolateToDistance);
		placement.position = placement.pointOnSpline + placement.up * height;
		return placement;
	}
	
	protected Vector3 CalcOffsetPoint(TCBSplineSegment segment, float t, float angle, float dist)
	{
		Vector3 p = segment.Interpolate(t);
		if (dist > 0)
		{
			Vector3 tangent = segment.Derivate(t).normalized;
			Vector3 up = segment.InterpolateUpVector(t);
			Quaternion rot = Quaternion.AngleAxis(angle, tangent);
			Vector3 offset = dist * (rot * up);
			p += offset;
		}
		return p;
	}

	private Vector3 GetMissingPoint (BasicSplinePoint point, Vector3 otherPos)
	{
		var diff = otherPos - point.Position;
		return point.Position - diff;
	}

	public void NormalizePoint(TCBSplinePoint p, bool onlyScale)
	{
		var pts = TCBPoints;
		int i = System.Array.IndexOf(pts, p);
		if (i >= 0)
		{
			Vector3 prevPos = (i > 0) ? pts[i - 1].Position : GetMissingPoint(pts[i], pts[i + 1].Position);
			Vector3 nextPos = (i < pts.Length - 1) ? pts[i + 1].Position : GetMissingPoint(pts[i], pts[i - 1].Position);
			
			pts[i].Normalize(prevPos, nextPos, onlyScale);
		}
	}
	
	public override void InitSegments(bool force)
	{
		var pts = TCBPoints;
		if (segments == null || force || segments.Length != pts.Length - 1)
		{
			segments = new TCBSplineSegment[(pts.Length > 0) ? pts.Length - 1 : 0];

			if (pts.Length > 1)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					var isEndPoint = !((i > 0) && (i < pts.Length - 1));
					var skipNormalize = (normalizeMode == NormalizationMode.NONE) || (normalizeMode == NormalizationMode.SKIP_ENDS && isEndPoint);

					NormalizePoint(pts[i], skipNormalize);
				}
				
				for (int i = 0; i < segments.Length; i++)
				{
					Vector3 prevPos = (i > 0) ? 
						pts[i - 1].Position : 
						GetMissingPoint(pts[i], pts[i + 1].Position);
					Vector3 nextPos = (i < segments.Length - 1) ? 
						pts[i + 2].Position : 
						GetMissingPoint(pts[i + 1], pts[i].Position);

					segments[i] = TCBSplineSegment.From4Points(prevPos, pts[i], pts[i + 1], nextPos, false);
				}
			}
		}
		CalcTotalDistance();
	}
	
	private void CalcTotalDistance()
	{
		Length = 0;
		foreach (TCBSplineSegment s in segments)
		{
			Length += s.Length;
		}
	}

#if UNITY_EDITOR

	protected virtual void OnDrawGizmos()
	{
		InitSegments(dynamicUpdate);
		if (segments != null)
		{
			foreach (var seg in segments)
			{
				DrawSegment(seg, 0, 0, Color.blue);
				DrawUpVec(seg, 0, Color.red);
			}
			if (segments.Length > 0)
			{
				DrawUpVec(segments[segments.Length - 1], 1, Color.red);
			}
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if (segments != null)
		{
			foreach (var seg in segments)
			{
				float inc = 0.1f;
				for (float t = inc; t < 1.0f; t += inc) // skips the first one
				{
					DrawUpVec(seg, t, Color.red);
				}
			}
		}
	}

	protected void DrawSegment(TCBSplineSegment segment, float offsetDistance, float offsetAngle, Color splineColor)
	{
		Vector3 last = CalcOffsetPoint(segment, 0, offsetAngle, offsetDistance);
		float inc = 0.05f;
		for (float t = 0f; t < 1.0f; t += inc)
		{
			Gizmos.color = splineColor;
			Vector3 cur = CalcOffsetPoint(segment, t + inc, offsetAngle, offsetDistance);
			Gizmos.DrawLine(last, cur);

			last = cur;
		}
	}

	protected void DrawUpVec(TCBSplineSegment segment, float t, Color upVecColor)
	{
		Vector3 cur = segment.Interpolate(t);
		Gizmos.color = upVecColor;
		Vector3 up = segment.InterpolateUpVector(t);
		Gizmos.DrawLine(cur, cur + up);
	}

	protected void DrawTangent(TCBSplineSegment segment, float t, Color tangentColor)
	{
		Vector3 cur = segment.Interpolate(t);
		Gizmos.color = tangentColor;
		Gizmos.DrawLine(cur, cur + 0.5f * segment.Derivate(0));
	}

#endif
}