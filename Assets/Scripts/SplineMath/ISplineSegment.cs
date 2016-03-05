using System;
using UnityEngine;

public interface ISplineSegment
{
	Vector3 StartPoint { get; }
	Vector3 EndPoint { get; }

	float Length { get; }

	float Param2Arclength(float t);
	float Arclength2Param(float arc);

	Vector3 Interpolate(float t);  // 0 <= t <= 1
	Vector3 Interpolate(float t, out Quaternion rot);

	Vector3 Derivate(float t);

	Vector3 InterpolateUpVector(float t);
}
