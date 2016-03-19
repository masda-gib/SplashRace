using UnityEngine;
using System.Collections;

public class AbilityCard : MonoBehaviour 
{
	public virtual CardType MyType { get { return CardType.ACTIVATABLE_DISPOSABLE; } }

	public int MaxCharges { get; set; }
	public int CurrentCharges { get; set; }
	public bool IsActivated { get; set; }

	public enum CardType
	{
		PERMANENT = 0, 
		PERMANENT_IF_NO_ACTIVE = 1, 
		ACTIVATABLE_RECHARGE = 2, 
		ACTIVATABLE_DISPOSABLE = 3
	}

	public virtual void Install(RacerBase parent)
	{
	}

	public virtual void Deinstall(RacerBase parent)
	{
	}

	public virtual void Activate(RacerBase parent)
	{
		IsActivated = true;
	}

	public virtual void Deactivate(RacerBase parent)
	{
		IsActivated = false;
	}

	public virtual void EndTurn(RacerBase parent)
	{
	}

}
