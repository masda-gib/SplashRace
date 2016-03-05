using System.Collections;
using UnityEngine;
using System.Linq;

public class TrackSplineGroup : TCBSplineGroup
{
	public float trackRadius = 0; // radius of the track spline
	public float layerHeight = 0; // distance between jump/height layers
	public float contentsStartDistance = 0;
	public bool isMainTrack = false;

	public override Placement GetPlacementAt(float angle, float distance, float height)
	{
		Placement p = base.GetPlacementAt(angle, distance, Mathf.Max(height, 0));
		float centerToTrackSurface = Mathf.Clamp01(height + 1);
		p.position += p.up * trackRadius * centerToTrackSurface; // height -1 = spline center, height 0 = track surface
		return p;
	}

	public void CreateFinalizedTrackWithLength(float newLength, float startInfinityLength, float endInfinityLength)
	{
		// TODO: the additional points are now calculated based on .Tangent
		// It might be neccessary to be based on .GetInTangent and .GetOutTangent

		InitSegments(true);
		var tmpPoints = TCBPoints.ToList();
		var diff = newLength - this.Length;

		if (diff < -0.1f) // spline too long, allow some delta
		{
			var plc = GetPlacementAt(0, newLength, 0);
			var newP = CreatePoint("New End Point", plc.pointOnSpline, Quaternion.LookRotation(plc.forward, plc.up), true);

			var totalLength = 0f;
			for (int i = 0; i < segments.Length && totalLength < newLength; i++)
			{
				totalLength += segments[i].Length;
				if (totalLength >= newLength)
				{
					tmpPoints.RemoveRange(i + 1, tmpPoints.Count - i - 1);
				}
			}
			tmpPoints.Add(newP);
		}
		else if (diff > 0.1f) // spline too short, allow some delta
		{
			var lastP = tmpPoints[tmpPoints.Count - 1];
			var newP = CreatePoint("New End Point", lastP.transform.position + lastP.Tangent.normalized * diff, lastP.transform.rotation, true);
			tmpPoints.Add(newP);
		}

		var myP = tmpPoints[0];
		var newP2 = CreatePoint("Start Infinity Point", myP.transform.position - myP.Tangent.normalized * startInfinityLength, myP.transform.rotation, true);
		tmpPoints.Insert(0, newP2);

		myP = tmpPoints[tmpPoints.Count - 1];
		var newP3 = CreatePoint("End Infinity Point", myP.transform.position + myP.Tangent.normalized * endInfinityLength, myP.transform.rotation, true);
		tmpPoints.Add(newP3);

		this.points = tmpPoints.ToArray();

		contentsStartDistance = startInfinityLength;
		InitSegments(true);
	}

	protected TCBSplinePoint CreatePoint (string pointName, Vector3 position, Quaternion rotation, bool parentToMe)
	{
		var newGo = new GameObject(pointName);
		var newP = newGo.AddComponent<TCBSplinePoint>();
		if (parentToMe)
		{
			newGo.transform.parent = this.transform;
		}
		newGo.transform.position = position;
		newGo.transform.rotation = rotation;
		return newP;
	}

#if UNITY_EDITOR

	protected override void OnDrawGizmosSelected()
	{
		foreach (var seg in segments)
		{
			DrawSegment(seg, trackRadius, 0, Color.yellow);
			DrawSegment(seg, trackRadius, 90, Color.green);
			DrawSegment(seg, trackRadius, 180, Color.cyan);
			DrawSegment(seg, trackRadius, 270, Color.magenta);
		}
	}

#endif
}