using System;
using UnityEngine;

[Serializable]
public class CatmullRomeSegment : ISplineSegment
{
	const float T_FACTOR = 0.5f;

	private float length;
	public float Length
	{
		get { return length; }
	}

	protected Vector3 xcurr;
	protected Vector3 xnext;

	protected Vector3 t0;
	protected Vector3 t1;

	public Vector3 StartPoint
	{
		get { return xcurr; }
	}

	public Vector3 EndPoint
	{
		get { return xnext; }
	}

	public CatmullRomeSegment(Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent)
	{
		xcurr = start;
		xnext = end;

		t0 = startTangent;
		t1 = endTangent;
		
		CalculateLength();
	}

	public Vector3 Interpolate(float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		double tau = t;
		double tau2 = tau * tau;
		double tau3 = tau2 * tau;

		double x = (2 * tau3 - 3 * tau2 + 1) * xcurr.x + (3 * tau2 - 2 * tau3) * xnext.x + (tau3 - 2 * tau2 + tau) * t0.x + (tau3 - tau2) * t1.x;
		double y = (2 * tau3 - 3 * tau2 + 1) * xcurr.y + (3 * tau2 - 2 * tau3) * xnext.y + (tau3 - 2 * tau2 + tau) * t0.y + (tau3 - tau2) * t1.y;
		double z = (2 * tau3 - 3 * tau2 + 1) * xcurr.z + (3 * tau2 - 2 * tau3) * xnext.z + (tau3 - 2 * tau2 + tau) * t0.z + (tau3 - tau2) * t1.z;

		return new Vector3((float)x, (float)y, (float)z);
	}

	// I have no idea what this method is for Oo
	public Vector3 Interpolate(float t, out Quaternion rot)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		double tau = t;
		double tau2 = tau * tau;
		double tau3 = tau2 * tau;

		double x = (2 * tau3 - 3 * tau2 + 1) * xcurr.x + (3 * tau2 - 2 * tau3) * xnext.x + (tau3 - 2 * tau2 + tau) * t0.x + (tau3 - tau2) * t1.x;
		double y = (2 * tau3 - 3 * tau2 + 1) * xcurr.y + (3 * tau2 - 2 * tau3) * xnext.y + (tau3 - 2 * tau2 + tau) * t0.y + (tau3 - tau2) * t1.y;
		double z = (2 * tau3 - 3 * tau2 + 1) * xcurr.z + (3 * tau2 - 2 * tau3) * xnext.z + (tau3 - 2 * tau2 + tau) * t0.z + (tau3 - tau2) * t1.z;

		rot = Quaternion.identity;

		return new Vector3((float)x, (float)y, (float)z);
	}

	public Vector3 Derivate(float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		double tau = t;
		double tau2 = tau * tau;

		double x = (6 * tau2 - 6 * tau) * xcurr.x + (6 * tau - 6 * tau2) * xnext.x + (3 * tau2 - 4 * tau + 1) * t0.x + (3 * tau2 - 2 * tau) * t1.x;
		double y = (6 * tau2 - 6 * tau) * xcurr.y + (6 * tau - 6 * tau2) * xnext.y + (3 * tau2 - 4 * tau + 1) * t0.y + (3 * tau2 - 2 * tau) * t1.y;
		double z = (6 * tau2 - 6 * tau) * xcurr.z + (6 * tau - 6 * tau2) * xnext.z + (3 * tau2 - 4 * tau + 1) * t0.z + (3 * tau2 - 2 * tau) * t1.z;

		return new Vector3((float)x, (float)y, (float)z);
	}
	
	public virtual Vector3 InterpolateUpVector(float t)
	{
		throw new NotImplementedException ();
	}

	protected float DerivationMagnitude(double t)
	{
		return Derivate((float)t).magnitude;
	}

	protected void CalculateLength()
	{
		length = Param2Arclength(1);
	}

	public float Param2Arclength (float t)
	{
		Integration integrator = new RombergIntegration();
		
		return integrator.Integrate(0f, t, DerivationMagnitude);
	}

	public float Arclength2Param (float arc)
	{
		Integration integrator = new RombergIntegration();
		float t = 0.5f;
		float l = 0;
		bool found = false;

		for (int i = 2; i < 20 && !found; i++)
		{
			l = integrator.Integrate(0f, t, DerivationMagnitude);

			found = (Mathf.Abs(arc - l) <= 0.0001f);
			if (!found)
			{
				t += (arc - l) > 0 ?  Mathf.Pow(0.5f, i)  : -Mathf.Pow(0.5f, i); 
			}
		}

		return t;
	}
}

