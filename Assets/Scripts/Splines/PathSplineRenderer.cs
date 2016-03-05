using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSplineRenderer : ASplineRenderer
{
	// mesh topology
	public int sides = 4;

	public float maxPartLength = 2;
	public float maxPartCurve = 0.1f;
	public float angleOffset = 0;

	// surface & materials
	public bool facesInward = false;

	public int uvTiling = 1;
	public float uvStretch = 1;
	public Material[] materials;
	public bool continuousMats;

	private GameObject[] meshes;
	private float accuracy = 20; // higher accuray -> more steps to check if one of the maxValues has occured
	private float circumference;

	public override void Init()
	{
		if (enabled)
		{
			InitMeshes();
		}
	}

	public override void Cleanup()
	{
		if (meshes != null)
		{
			foreach (var m in meshes)
			{
				if (m != null)
				{
					if (Application.isPlaying)
					{
						GameObject.Destroy(m.GetComponent<MeshFilter>().sharedMesh);
						GameObject.Destroy(m);
					}
					else
					{
						GameObject.DestroyImmediate(m);
					}
				}
			}
		}
		meshes = null;
	}

	protected virtual void CreateIndices(Mesh m, int vertRows, int parts)
	{
		List<int>[] indices = new List<int>[materials.Length];
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] = new List<int>();
		}

		if (vertRows > 1 && materials.Length > 0)
		{
			int faceRows = vertRows - 1;

			// TODO: maybe swith loops for parts and rows to create trianglestrips along the spline
			for (int p = 0; p < parts; p++) // connection between 2 cross sections
			{
				for (int s = 0; s < faceRows; s++) // connection between 2 vertex rows
				{
					int mat = 0;
					if (continuousMats) // try to split up the tube in materials.Length sections of materials
					{
						mat = Mathf.FloorToInt(s / (float)faceRows * materials.Length);
					}
					else // just cycle through each face row
					{
						mat = s % materials.Length;
					}

					//int baseIndex = (p * faceRows + s) * 6; // 6 = number of points for 2 triangles (1 quad)
					int leftVert = p * vertRows + s;
					int rightVert = p * vertRows + s + 1;

					if (facesInward)
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
		var outward = Vector3.Cross(tangent, up).normalized;

		float sideAngle = 360 / sides;
		Vector3 nT = tangent.normalized;
		Vector4 nTang = new Vector4(nT.x, nT.y, nT.z, 1);

		Vector3[] verts = new Vector3[sides + 1];
		Vector3[] norms = new Vector3[sides + 1];
		Vector4[] tangs = new Vector4[sides + 1];
		Vector2[] uvs = new Vector2[sides + 1];
		for (int s = 0; s < sides; s++)
		{
			var rQuat = Quaternion.AngleAxis(s * sideAngle + angleOffset, tangent.normalized);
			var out2 = rQuat * outward;
			verts[s] = pos + radius * out2;
			norms[s] = (facesInward) ? -out2 : out2;
			tangs[s] = nTang;
			uvs[s] = new Vector2(s / (float)sides * uvTiling, t * uvTiling / circumference / uvStretch);

			if (s == 0) // set first vertex in ring also as last but with different uv
			{
				verts[sides] = verts[0];
				norms[sides] = norms[0];
				tangs[sides] = nTang;
				uvs[sides] = new Vector2(uvTiling, t * uvTiling / circumference / uvStretch);
			}
		}

		vertList.AddRange(verts);
		normalList.AddRange(norms);
		tangentList.AddRange(tangs);
		uvList.AddRange(uvs);

		return sides + 1;
	}

	private void InitMeshes()
	{
		circumference = 2 * Mathf.PI * radius;
		uvStretch = Mathf.Max(uvStretch, 0.01f);

		if (segments != null)
		{
			float prevLengths = 0;
			meshes = new GameObject[segments.Length];
			for (int i = 0; i < segments.Length; i++)
			{
				Mesh m = InitSegmentMesh(segments[i], prevLengths);

				GameObject mGo = new GameObject("Segment_" + i);
				mGo.transform.parent = this.transform;
				mGo.layer = targetLayer;

				var mf = mGo.AddComponent<MeshFilter>();
				mf.mesh = m;

				var mr = mGo.AddComponent<MeshRenderer>();
				mr.materials = this.materials;

				meshes[i] = mGo;

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
}