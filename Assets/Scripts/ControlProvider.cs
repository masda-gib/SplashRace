using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlProvider : MonoBehaviour 
{
	public RacerBase racer;
	public string inputId = "1";
	public bool confirmSteering = true;
	public bool resetSteerInput = true;

	public Vector3 CurrentSteerInput { get; private set; }
	public Vector3 CurrentSteering
	{
		get { return racer.movement.ConvertToSteerVector (CurrentSteerInput); }
	}
	public Vector3 ConfirmedSteering 
	{
		get { return (racer != null) ? racer.movement.SteerVector : Vector3.zero; } 
	}
	public bool IsGettingSteerInput { get; private set; }
	public int SelectedCardId { get; private set; }

	// Use this for initialization
	void Awake () 
	{
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Touch);
	}

	void Start()
	{
		if (racer != null) {
			racer.movement.SteeringReset.AddListener (this.OnSteeringReset);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		CurrentSteerInput = new Vector3 (CrossPlatformInputManager.GetAxis ("Horizontal" + inputId), 0, CrossPlatformInputManager.GetAxis ("Vertical" + inputId));
		IsGettingSteerInput = CrossPlatformInputManager.GetButton ("SteerInput" + inputId);

		if (!confirmSteering || IsGettingSteerInput) {
			if (racer != null) {
				racer.movement.SetSteerInput (CurrentSteerInput);
			}
		}

		var abilityAToggle = CrossPlatformInputManager.GetButtonUp ("AbilityA" + inputId);
		if (abilityAToggle) {
			if (racer.abilities.GetActivatedState (racer.abilities.cardA)) {
				racer.abilities.DeactivateAll ();
			} else {
				racer.abilities.ActivateCard (1);
			}
		}
	}

	protected void OnSteeringReset()
	{
		if (resetSteerInput) {
			CurrentSteerInput = Vector3.zero;
		}
	}
}
