using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LinearProxySegment : ISplineSegment
{
	private ISplineSegment segment;
	private List<float> times = new List<float>();
	private List<Vector3> waypoints = new List<Vector3>();
	private List<Quaternion> waypointRots = new List<Quaternion>();

	private int count;

	public Vector3 StartPoint
	{
		get { return waypoints[0]; }
	}

	public Vector3 EndPoint
	{
		get { return waypoints[count - 3]; }
	}

	public float Length
	{
		get { return segment.Length; }
	}

	public LinearProxySegment(int t_steps, ISplineSegment segment, Vector3 up)
	{
		this.segment = segment;

		float step = 1f / t_steps;

		if (step * t_steps < 1.0f)
			t_steps++;

		for (int i = 0; i <= t_steps; i++)
		{
			float t = Math.Min(1f, step * i);
			Vector3 x = segment.Interpolate(t);
			times.Add(t);
			waypoints.Add(x);

			// Calculate rotation
			Vector3 d = ((CatmullRomeSegment)segment).Derivate(t);

			Quaternion rot = Quaternion.identity;
			if (d != Vector3.zero)
			{
				rot = Quaternion.LookRotation(d, up);
			}
			waypointRots.Add(rot);
		}

		count = times.Count;
	}

	public Vector3 Interpolate(float t)
	{
		if (t <= 0)
			return waypoints[0];

		if (t >= 1f)
			return waypoints[count - 1];

		t = Mathf.Clamp(t, 0f, 1f);
		for (int i = 0; i < count - 1; i++)
		{
			if (times[i + 1] > t)
			{
				return waypoints[i] + ((t - times[i]) / (times[i + 1] - times[i])) * (waypoints[i + 1] - waypoints[i]);
			}
		}

		return waypoints[count - 1];
	}

	public Vector3 Interpolate(float t, out Quaternion rot)
	{
		if (t <= 0)
		{
			rot = waypointRots[0];
			return waypoints[0];
		}

		if (t >= 1f)
		{
			rot = waypointRots[count - 1];
			return waypoints[count - 1];
		}

		t = Mathf.Clamp(t, 0f, 1f);
		for (int i = 0; i < count - 1; i++)
		{
			if (times[i + 1] > t)
			{
				rot = Quaternion.Slerp(waypointRots[i], waypointRots[i + 1], (t - times[i]) / (times[i + 1] - times[i]));
				return waypoints[i] + ((t - times[i]) / (times[i + 1] - times[i])) * (waypoints[i + 1] - waypoints[i]);
			}
		}

		rot = waypointRots[count - 1];
		return waypoints[count - 1];
	}

	public float Param2Arclength (float t)
	{
		return t * Length;
	}
	
	public float Arclength2Param (float arc)
	{
		return arc / Length;
	}

	public Vector3 Derivate (float t)
	{
		throw new NotImplementedException ();
	}
	
	public Vector3 InterpolateUpVector(float t)
	{
		throw new NotImplementedException ();
	}
}

