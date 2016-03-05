using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackSplineRenderer : ASplineRenderer
{
	public float maxPartLength = 2;
	public float maxPartCurve = 0.1f;
	public float skipStartLength;
	public float skipEndLength;
	public Material material;
	public bool flipFaces;

	public SortedList<float, int[]> sortedPoints;
	public GameObject[] Meshes { get; protected set; }

	private float accuracy = 20; // higher accuray -> more steps to check if one of the maxValues has occured

	public override void Init ()
	{
		if (enabled)
		{
			InitMeshes();
		}
	}

	public override void Cleanup ()
	{
		if (Meshes != null)
		{
			foreach (var m in Meshes)
			{
				if (m != null)
				{
					if (!Application.isEditor)
					{
						GameObject.Destroy(m);
					}
					else
					{
						GameObject.DestroyImmediate(m);
					}
				}
			}
		}
		Meshes = null;
	}
	
	private void InitMeshes()
	{
		if (segments != null)
		{
			float prevLengths = 0;
			Meshes = new GameObject[segments.Length];
			for (int i = 0; i < segments.Length; i++)
			{
				Mesh m = InitSegmentMesh(segments[i], prevLengths);
				
				GameObject mGo = new GameObject("Segment_" + i);
				mGo.transform.parent = this.transform;
				mGo.layer = targetLayer;
				
				var mf = mGo.AddComponent<MeshFilter>();
				mf.mesh = m;
				
				var mr = mGo.AddComponent<MeshRenderer>();
				mr.material = this.material;

				var mc = mGo.AddComponent<MeshCollider>();
				mc.sharedMesh = m;

				Meshes[i] = mGo;
				
				prevLengths += segments[i].Length;
			}
		}
	}
	
	private Mesh InitSegmentMesh(ISplineSegment seg, float prevLengths)
	{
		List<Vector3> vertList = new List<Vector3>();
		List<Vector3> normalList = new List<Vector3>();
		List<Vector4> tangentList = new List<Vector4>();
		List<Vector2> uvList = new List<Vector2>();
		
		float lastLength = -1000;
		Vector3 lastDir = Vector3.zero;
		int parts = 0;
		int vertRows = 0;
		
		if (seg != null)
		{
			int numParts = Mathf.CeilToInt(seg.Length / maxPartLength);
			float step = 1 / (float)numParts / accuracy;
			for (float s = 0; s < 1; s += step)
			{
				float arcLength = seg.Param2Arclength(s);
				Vector3 tangent = seg.Derivate(s);
				
				if ((arcLength - lastLength >= maxPartLength) ||
				    (1 - Vector3.Dot(lastDir, tangent.normalized) >= maxPartCurve))
				{
					lastLength = arcLength;
					lastDir = tangent.normalized;
					
					CreateCrossSectionVerts(
						seg.Interpolate(s), 
						tangent, 
						seg.InterpolateUpVector(s), 
						prevLengths + seg.Param2Arclength(s), 
						ref vertList, ref normalList, ref tangentList, ref uvList);
					parts++;
				}
			}
			
			vertRows = CreateCrossSectionVerts(
				seg.Interpolate(1), 
				seg.Derivate(1), 
				seg.InterpolateUpVector(1), 
				prevLengths + seg.Length, 
				ref vertList, ref normalList, ref tangentList, ref uvList);
		}
		
		Vector3[] verts = vertList.ToArray();
		Vector3[] normals = normalList.ToArray();
		Vector4[] tangents = tangentList.ToArray();
		Vector2[] uv0 = uvList.ToArray();
		
		Mesh m = new Mesh();
		m.name = "SegmentMesh";
		m.vertices = verts;
		m.normals = normals;
		m.tangents = tangents;
		m.uv = uv0;
		
		CreateIndices(m, vertRows, parts);
		
		return m;
	}
	
	protected virtual void CreateIndices(Mesh m, int vertRows, int parts)
	{
		List<int>[] indices = new List<int>[1];
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] = new List<int>();
		}
		
		if (vertRows > 1)
		{
			int faceRows = vertRows - 1;
			
			// TODO: maybe swith loops for parts and rows to create trianglestrips along the spline
			for (int p = 0; p < parts; p++) // connection between 2 cross sections
			{
				for (int s = 0; s < faceRows; s++) // connection between 2 vertex rows
				{
					int mat = 0;

					//int baseIndex = (p * faceRows + s) * 6; // 6 = number of points for 2 triangles (1 quad)
					int leftVert = p * vertRows + s;
					int rightVert = p * vertRows + s + 1;
					
					if (flipFaces)
					{
						indices[mat].Add(leftVert);
						indices[mat].Add(leftVert + vertRows);
						indices[mat].Add(rightVert + vertRows);
						
						indices[mat].Add(leftVert);
						indices[mat].Add(rightVert + vertRows);
						indices[mat].Add(rightVert);
					}
					else
					{
						indices[mat].Add(leftVert);
						indices[mat].Add(rightVert + vertRows);
						indices[mat].Add(leftVert + vertRows);
						
						indices[mat].Add(leftVert);
						indices[mat].Add(rightVert);
						indices[mat].Add(rightVert + vertRows);
					}
				}
			}
		}
		
		m.subMeshCount = indices.Length;
		for (int i = 0; i < indices.Length; i++)
		{
			m.SetTriangles(indices[i].ToArray(), i);
		}
	}
	
	protected virtual int CreateCrossSectionVerts(
		Vector3 pos, Vector3 tangent, Vector3 up, float t, 
		ref List<Vector3> vertList, ref List<Vector3> normalList, ref List<Vector4> tangentList, ref List<Vector2> uvList)
	{
		Vector3 side = Vector3.Cross(tangent, Vector3.up).normalized;
		Vector3 nT = tangent.normalized;
		Vector4 nTang = new Vector4(nT.x, nT.y, nT.z, 1);
		
		Vector3[] verts = new Vector3[3];
		Vector3[] norms = new Vector3[3];
		Vector4[] tangs = new Vector4[3];
		Vector2[] uvs = new Vector2[3];
		for (int s = 0; s <= 2; s++)
		{
			int offset = s -1;
			verts[s] = pos + radius * side * offset;
			norms[s] = (flipFaces) ? -Vector3.up : Vector3.up;
			tangs[s] = nTang;
			uvs[s] = new Vector2 (0.5f + offset * 0.5f, t);
		}
		
		vertList.AddRange(verts);
		normalList.AddRange(norms);
		tangentList.AddRange(tangs);
		uvList.AddRange(uvs);
		
		return 3;
	}
}
