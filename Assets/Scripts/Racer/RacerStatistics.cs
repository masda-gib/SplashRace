using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "RacerStats", menuName = "SplashRace/RacerStatistics", order = 1)]
public class RacerStatistics : ScriptableObject {

	public int maxCards = 4;

	// Vector2[0] is steer radius, Vector2[1] is max speed
	public Vector2 handlingWater = new Vector2 (1.5f, 5);
	public Vector2 handlingLand = new Vector2 (1.2f, 4);
	public Vector2 handlingCurb = new Vector2 (1f, 1);

	public void CopyValuesTo(RacerStatistics target) {
		target.maxCards = this.maxCards;

		target.handlingWater = this.handlingWater;
		target.handlingLand = this.handlingLand;
		target.handlingCurb = this.handlingCurb;
	}

	public float GetSteerRadiusFor(int underground) {
		return handlingWater [0];
	}

	public float GetMaxSpeedFor(int underground) {
		return handlingWater [1];
	}
}
