using UnityEngine;
using System.Collections;

public class SelfSteerChangeCard : AbilityCard 
{
	public float ratio = 2;

	public override CardType MyType { get { return CardType.ACTIVATABLE_DISPOSABLE; } }

	RacerModifier activeMod;

	public override void Activate (RacerBase parent)
	{
		activeMod = new SelfSteerModifier (ratio);
		parent.modifiers.AddModifier(activeMod);
		base.Activate (parent);
	}

	public override void Deactivate (RacerBase parent)
	{
		base.Deactivate (parent);
		if (activeMod != null) {
			parent.modifiers.RemoveModifier (activeMod);
			activeMod = null;
		}
	}
}
