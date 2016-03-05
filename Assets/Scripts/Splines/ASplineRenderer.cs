using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASplineRenderer : MonoBehaviour
{
	public TCBSplineGroup source;
	public float radius;
	public int targetLayer;

	protected ISplineSegment[] segments;

	public void Init(ISplineSegmentProvider segmentSource, float radius)
	{
		if (segmentSource != null)
		{
			this.segments = segmentSource.Segments;
			this.radius = radius;
			this.source = null;
			Init();
		}
	}

	public abstract void Init();

	public abstract void Cleanup();

	private void OnEnable()
	{
		Cleanup();
		if (source != null) // fill with set data to easier debug
		{
			segments = source.Segments;
		}
		Init();
	}

	private void OnDisable()
	{
		Cleanup();
	}
}