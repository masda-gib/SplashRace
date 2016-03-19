using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class AbilityInput : MonoBehaviour
{
	public string abilityAButtonName = "AbilityA1";
	public Toggle buttonA;
	public Toggle buttonB;
	public Toggle buttonC;
	public Toggle buttonD;

	CrossPlatformInputManager.VirtualButton m_AbilityAVirtualButton;

	// Use this for initialization
	void Start () {
		buttonA.onValueChanged.AddListener (OnButtonAChanged);
	}

	void OnEnable()
	{
		CreateVirtualAxes();
	}

	void CreateVirtualAxes()
	{
		// create new axes based on axes to use

		m_AbilityAVirtualButton = new CrossPlatformInputManager.VirtualButton (abilityAButtonName);
		CrossPlatformInputManager.RegisterVirtualButton(m_AbilityAVirtualButton);
	}

	protected void OnButtonAChanged(bool state) {
		m_AbilityAVirtualButton.Pressed ();
		m_AbilityAVirtualButton.Released ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
