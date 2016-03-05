using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct TrackPointData
{
	public float width;
	public int curvature; // -100 .. 100

	public bool IsClosed
	{
		get { return Mathf.Abs (curvature) >= 100; }
	}

	public int CrossSectionPointNum
	{
		get { return 1 + 2 * CalcSideSegs(); }
	}
	
	public void Validate ()
	{
		curvature = Mathf.Clamp(curvature, -100, 100);
		width = Mathf.Max(width, 0.1f);
	}

	public Vector3[] CalcLocalPoints()
	{
		int segsPerSide = CalcSideSegs();
		var pList = new List<Vector3>();
		for (int i = -segsPerSide; i <= segsPerSide; i++)
		{
			pList.Add(CalcLocalPoint((float)i / segsPerSide));
		}
		return pList.ToArray();
	}
	
	public Vector3[] CalcWorldPoints(Vector3 position, Quaternion rotation)
	{
		var pts = CalcLocalPoints();
		for(int i = 0; i < pts.Length; i++)
		{
			//pts[i] = transform.TransformPoint(pts[i]);
			pts[i] = (rotation * pts[i]) + position;
		}
		return pts;
	}
	
	private Vector3 CalcLocalPoint(float p)
	{
		float angle = Mathf.Abs (curvature * 3.6f);
		if (angle < 1)
		{
			return Vector3.right * width * p * 0.5f;
		}
		else
		{
			float radius = Mathf.Rad2Deg * width / angle;
			float offset = radius * (curvature / 100.0f) * (curvature / 100.0f);
			float sign = Mathf.Sign(curvature);
			var quat = Quaternion.AngleAxis(angle * 0.5f * -p, Vector3.forward);
			var lPoint = (quat * Vector3.up * radius) + Vector3.up * (offset - radius);
			lPoint.y = lPoint.y * sign;
			return lPoint;
		}
	}
	
	private int CalcSideSegs()
	{
		return Mathf.FloorToInt(Mathf.Abs(curvature / 5.0f)) + 1;
	}
}
