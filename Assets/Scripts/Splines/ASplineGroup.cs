using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASplineGroup : MonoBehaviour, ISplineSegmentProvider
{
	public abstract ISplineSegment[] Segments
	{
		get;
	}

	public abstract BasicSplinePoint[] Points
	{
		get;
	}

	public virtual float Length
	{
		get;
		protected set;
	}

	public ISplineSegment this[int index]
	{
		get
		{
			return Segments[index];
		}
	}

	protected virtual void Awake()
	{
		InitSegments(true);
	}

	public abstract void InitSegments(bool force);
	public abstract Placement GetPlacementAt(float angle, float distance, float height);
}