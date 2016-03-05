using System.Collections;
using UnityEngine;

// https://de.wikipedia.org/wiki/Kubisch_Hermitescher_Spline#Kochanek-Bartels-Spline

public class TCBSplinePoint : BasicSplinePoint
{
	public float scale = 1;
	public float tension;
	public float continuity;
	public float bias;

	public Vector3 Tangent
	{
		get { return Forward * scale; }
	}

	public virtual void OnValidate()
	{
		scale = Mathf.Max(0, scale);
		tension = Mathf.Clamp(tension, -1, 1);
		continuity = Mathf.Clamp(continuity, -1, 1);
		bias = Mathf.Clamp(bias, -1, 1);
	}

	public void Normalize(Vector3 prev, Vector3 next, bool onlyScale)
	{
		Vector3 t0 = GetNormalizedTangent(prev, next);
		if (!onlyScale)
		{
			Quaternion rot = Quaternion.LookRotation(t0.normalized, Up);
			this.transform.rotation = rot;
		}
		scale = t0.magnitude;
	}

	public Vector3 GetNormalizedTangent(Vector3 prev, Vector3 next)
	{
		return 0.5f * ((Position - prev) + (next - Position));
	}

	private Vector3 GetBaseTangent(Vector3 tangent, Vector3 prev, Vector3 next)
	{
		float _B0 = Mathf.Clamp(bias, -1, 1);
		if (_B0 < 0)
		{
			Vector3 prevDiff = (Position - prev);
			tangent = Vector3.Slerp(tangent, prevDiff, -_B0);
		}
		if (_B0 > 0)
		{
			Vector3 nextDiff = (next - Position);
			tangent = Vector3.Slerp(tangent, nextDiff, _B0);
		}
		
		float _T0 = Mathf.Clamp(tension, -1, 1);
		tangent = (1 - _T0) * tangent;
		return tangent;
	}

	public Vector3 GetOutTangent(Vector3 tangent, Vector3 prev, Vector3 next)
	{
		tangent = GetBaseTangent(tangent, prev, next);
		tangent = Vector3.ClampMagnitude(tangent, (Position - next).magnitude); // to avoid strange zig-zag paths

		if (continuity != 0)
		{
			float _C0 = Mathf.Clamp(continuity, -1, 1);
			var tC = (1 + _C0) * (Position - prev) + (1 - _C0) * (next - Position);
			var tN = (Position - prev) + (next - Position);
			var rot = Quaternion.FromToRotation(tN, tC);
			tangent = rot * tangent;
		}

		return tangent;
	}

	public Vector3 GetInTangent(Vector3 tangent, Vector3 prev, Vector3 next)
	{
		tangent = GetBaseTangent(tangent, prev, next);
		tangent = Vector3.ClampMagnitude(tangent, (Position - prev).magnitude); // to avoid strange zig-zag paths

		if (continuity != 0)
		{
			float _C0 = Mathf.Clamp(continuity, -1, 1);
			var tC = (1 - _C0) * (Position - prev) + (1 + _C0) * (next - Position);
			var tN = (Position - prev) + (next - Position);
			var rot = Quaternion.FromToRotation(tN, tC);
			tangent = rot * tangent;
		}

		return tangent;
	}
	
	public Vector3 GetOutTangent(Vector3 prev, Vector3 next)
	{
		return GetOutTangent(Tangent, prev, next);
	}
	
	public Vector3 GetInTangent(Vector3 prev, Vector3 next)
	{
		return GetInTangent(Tangent, prev, next);
	}
	
	#if UNITY_EDITOR
	protected override void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + Tangent);
		Gizmos.color = Color.gray;
		Gizmos.DrawLine(transform.position, transform.position + transform.up);
	}
	#endif
}