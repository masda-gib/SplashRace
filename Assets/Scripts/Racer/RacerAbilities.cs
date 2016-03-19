using UnityEngine;
using System.Collections;

public class RacerAbilities : MonoBehaviour 
{
	protected RacerBase parent;

	public AbilityCard cardA;
	public AbilityCard cardB;
	public AbilityCard cardC;
	public AbilityCard cardD;

	public void Init (RacerBase parent) {
		this.parent = parent;
	}

	public bool ActivateCard (int index) {
		if (true) {
			DeactivateAll ();
			if (index >= 0 && index < 5) {
				switch (index) {
				case 1:
					ActivateCard(cardA);
					break;
				case 2:
					ActivateCard(cardB);
					break;
				case 3:
					ActivateCard(cardC);
					break;
				case 4:
					ActivateCard(cardD);
					break;
				}
				return true;
			}
		}
		return false;
	}

	public void DeactivateAll() {
		DeactivateCard(cardA);
		DeactivateCard(cardB);
		DeactivateCard(cardC);
		DeactivateCard(cardD);
	}

	protected void ActivateCard(AbilityCard card) {
		if (card != null) {
			card.Activate (parent);
		}
	}

	protected void DeactivateCard(AbilityCard card) {
		if (card != null) {
			card.Deactivate (parent);
		}
	}

	public bool GetActivatedState(AbilityCard card) {
		return card != null && card.IsActivated;
	}

	public void EndProcessing()
	{
	}
}
