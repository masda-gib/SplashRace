using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class RacerModifiers : MonoBehaviour {

	protected List<RacerModifier> mods;

	public UnityEvent ModifiersChanged;

	void Awake() {
		mods = new List<RacerModifier> ();
	}

	public bool AddModifier(RacerModifier mod) {
		mods.Add (mod);
		ModifiersChanged.Invoke ();
		return true;
	}

	public bool RemoveModifier(RacerModifier mod) {
		var success = mods.Remove (mod);
		ModifiersChanged.Invoke ();
		return success;
	}

	public void ModifyTempValues(RacerCurrValues tempValues) {
		foreach (var m in mods) {
			m.ModifyTempValues (tempValues);
		}
	}
}
