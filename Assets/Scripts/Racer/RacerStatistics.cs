using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacerStatistics : MonoBehaviour 
{
	public float actionPhaseLength = 1; // has to be race info
	public float movePhaseLength = 2; // has to be race info

	public int maxCards = 4;

	public float steerRadius = 1;
	public float maxSpeed = 4;

	public void FillTempValues(RacerCurrValues tempValues) {
		tempValues.steerRadius = steerRadius;
		tempValues.maxSpeed = maxSpeed;
	}
}
