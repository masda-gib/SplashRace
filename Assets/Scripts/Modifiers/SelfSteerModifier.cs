using UnityEngine;
using System.Collections;

public class SelfSteerModifier : RacerModifier {
	public float ratio;

	public SelfSteerModifier(float ratio) {
		this.ratio = ratio;
	}

	public override void ModifyTempValues (RacerCurrValues tempValues)
	{
		tempValues.steerRadius *= ratio;
	}
}
