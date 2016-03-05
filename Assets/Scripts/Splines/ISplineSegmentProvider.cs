using UnityEngine;
using System.Collections;

public interface ISplineSegmentProvider
{
	ISplineSegment[] Segments { get; }
	ISplineSegment this[int index] { get; }
	float Length { get; }

	Placement GetPlacementAt(float angle, float distance, float height);
}
