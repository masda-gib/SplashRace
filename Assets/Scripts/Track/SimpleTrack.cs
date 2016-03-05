using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleTrack : MonoBehaviour 
{
	public TrackSegmentGenerator generator;
	public TCBSplineGroup spline;

	public bool generate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(generate)
		{
			generate = false;

			for(int j = 0; j < spline.TCBPoints.Length - 1; j++)
			{
				generator.GenerateMesh((TrackPoint)spline.TCBPoints[j], (TrackPoint)spline.TCBPoints[j + 1], (TCBSplineSegment)spline.Segments[j]);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for(int i = 0; i < spline.TCBPoints.Length; i++)
		{
			if(i < spline.TCBPoints.Length - 1)
			{
				Gizmos.DrawLine(spline.TCBPoints[i].transform.position, spline.TCBPoints[i + 1].transform.position);
			}
		}
	}
}
