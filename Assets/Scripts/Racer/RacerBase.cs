using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class RacerBase : MonoBehaviour
{
	public RaceMatch match;

	public RacerStatistics statistics;
	public RacerMovement movement;
	public RacerAbilities abilities;
	public RacerModifiers modifiers;
	public RacerCurrValues tempValues;

	public bool IsOnTheMove { get; protected set; }

	public Renderer debugObject;

	private float roundTime;

	void Awake() {
		this.tempValues = ScriptableObject.CreateInstance<RacerCurrValues> ();
	}

	// Use this for initialization
	void Start () {
		movement.Init (this);
		abilities.Init (this);
		movement.SteeringReset.AddListener (InitTempValues);
		modifiers.ModifiersChanged.AddListener (InitTempValues);
		InitTempValues ();
	}

	protected void InitTempValues()
	{
		statistics.CopyValuesTo(tempValues.statistics);
		movement.CopyValuesTo(tempValues);
		modifiers.ModifyTempValues (tempValues);
	}

	// Update is called once per frame
	void Update () 
	{
		if (match != null) {
			var timer = Time.time;

			roundTime = timer % (match.actionPhaseLength + match.movePhaseLength);
			if (roundTime < match.actionPhaseLength) {
				if (IsOnTheMove) {
					IsOnTheMove = false;

					movement.EndProcessing ();
				}
			} else {
				if (!IsOnTheMove) {
					IsOnTheMove = true;

					movement.StartProcessing ();
				}
			}
			
			if (IsOnTheMove) {
				var t = (roundTime - match.actionPhaseLength) / match.movePhaseLength;

				movement.Process (t);
			}

			if (debugObject != null && debugObject.material != null) {
				debugObject.material.color = IsOnTheMove ? Color.green : Color.red;
			}
		}
	}
}
