using UnityEngine;
using System.Collections;

public class RacerCurrValues : ScriptableObject 
{
	public RacerStatistics statistics;
	public Vector3 moveStart;
	public Vector3 moveVector;

	void OnEnable() {
		this.statistics = ScriptableObject.CreateInstance<RacerStatistics> ();
	}
}
