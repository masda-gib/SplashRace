using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackSegmentGenerator : MonoBehaviour 
{
	public float minStep = 1;
	public float maxStep = 10;
	public float maxBend = 0.3f;
	public float maxWidthDiff = 5;
	public int maxPointDiff = 1;
	public TrackPoint tpPrefab;
	public Material trackMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateMesh(TrackPoint p0, TrackPoint p1, TCBSplineSegment spline)
	{
		float length = spline.Length;
		
		int steps = Mathf.FloorToInt(length / minStep);
		float stepLength = (steps > 1) ? length / steps : length;

		var points = new List<TrackPoint>();
		points.Add(p0);

		float lastDist = 0;
		var lastPoint = p0;

		for (int s = 1; s < steps; s++) // while (dist < length && myMinStep > 0 && length > 0)
		{
			var dist = s * stepLength;
			var t = dist / length;

			var currStep = dist - lastDist;
			var currDir = spline.Derivate(t).normalized;
			var currUp = spline.InterpolateUpVector(t);
			var currBend = 1 - Vector3.Dot(lastPoint.transform.forward, currDir);
			currBend = Mathf.Max(currBend, 1 - Vector3.Dot(lastPoint.transform.up, currUp));
			var currWidth = Mathf.Lerp(p0.data.width, p1.data.width, t);
			var currCurve = Mathf.Lerp(p0.data.curvature, p1.data.curvature, t);
			var currPointNum = Mathf.Lerp(p0.data.CrossSectionPointNum, p1.data.CrossSectionPointNum, t);

			bool setNew = currStep > maxStep;
			setNew |= currBend > maxBend;
			setNew |= Mathf.Abs(lastPoint.data.width - currWidth) > maxWidthDiff;
			setNew |= Mathf.Abs(lastPoint.data.CrossSectionPointNum - currPointNum) > maxPointDiff;
			setNew &= length - dist > (stepLength * 0.5f); // rounding error check at end of spline

			if(setNew)
			{
				var pos = spline.Interpolate(t);
				var rot = Quaternion.LookRotation(currDir, currUp);
				var newPoint = GameObject.Instantiate<TrackPoint>(tpPrefab);

				newPoint.transform.position = pos;
				newPoint.transform.rotation = rot;
				newPoint.data.width = currWidth;
				newPoint.data.curvature = Mathf.RoundToInt(currCurve);

				points.Add(newPoint);

				lastDist = dist;
				lastPoint = newPoint;
			}
		}

		points.Add(p1);

		GenerateMesh(points);
	}
	
	void GenerateMesh (List<TrackPoint> points)
	{
		var mesh = new Mesh();
		var verts = new List<Vector3>();
		var tris = new List<int>();

		int lastNum = 0;
		for(int i = 0; i < points.Count; i++)
		{
			var wPts = points[i].CalcWorldPoints();
			AddToMesh (wPts, ref verts, ref tris, lastNum);
			lastNum = wPts.Length;
		}
		
		mesh.vertices = verts.ToArray();
		mesh.SetTriangles(tris.ToArray(), 0);
		mesh.RecalculateNormals();
		
		var segObj = new GameObject("Segment");
		var mf = segObj.AddComponent<MeshFilter>();
		mf.sharedMesh = mesh;
		
		var mr = segObj.AddComponent<MeshRenderer>();
		mr.material = trackMaterial;
		
		var mc = segObj.AddComponent<MeshCollider>();
		mc.sharedMesh = mesh;
	}

	void AddToMesh (Vector3[] wPts, ref List<Vector3> verts, ref List<int> tris, int lastNum)
	{
		var firstOld = verts.Count - lastNum;
		var firstNew = verts.Count;
		var pts1 = (firstOld >= 0 && lastNum > 0) ? verts.GetRange(firstOld, lastNum).ToArray() : new Vector3[0];
		var pts2 = wPts;

		verts.AddRange(pts2);
		
		if(pts1.Length > 0 && pts2.Length > 0)
		{
			float hiCount = Mathf.Max(pts1.Length, pts2.Length);
			
			for (int i = 0; i < hiCount - 1; i++)
			{
				int i10 = firstOld + Mathf.FloorToInt((i / hiCount) * pts1.Length);
				int i20 = firstNew + Mathf.FloorToInt((i / hiCount) * pts2.Length);
				int i11 = firstOld + Mathf.FloorToInt(((i + 1) / hiCount) * pts1.Length);
				int i21 = firstNew + Mathf.FloorToInt(((i + 1) / hiCount) * pts2.Length);
				
				if (i10 != i11)
				{
					AddTri(i11, i10, i20, verts, ref tris);
				}
				if (i20 != i21)
				{
					AddTri(i20, i21, i11, verts, ref tris);
				}
			}
		}
	}
	
	void AddTri (int p0, int p1, int p2, List<Vector3> verts, ref List<int> tris)
	{
		tris.Add(p0);
		tris.Add(p1);
		tris.Add(p2);
	}
}
