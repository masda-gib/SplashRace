using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class TouchSteerInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public int radius = 100;
	public string horizontalAxisName = "Horizontal1"; // The name given to the horizontal axis for the cross platform input
	public string verticalAxisName = "Vertical1"; // The name given to the vertical axis for the cross platform input
	public string inputButtonName = "SteerInput1";
	public Graphic visualizer;

	Vector3 m_StartPos;
	CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
	CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
	CrossPlatformInputManager.VirtualButton m_InputVirtualButton;
	RectTransform rt;

	void OnEnable()
	{
		CreateVirtualAxes();
	}

	void Start()
	{
		rt = GetComponent<RectTransform>();
		visualizer.color = Color.gray;
	}

	void UpdateVirtualAxes(Vector2 value)
	{
		var delta = value - rt.anchoredPosition;
		delta = Vector2.ClampMagnitude (delta, radius);

		if (visualizer != null) {
			visualizer.GetComponent<RectTransform>().anchoredPosition = delta;
		}

		delta /= radius;
		m_HorizontalVirtualAxis.Update(delta.x);
		m_VerticalVirtualAxis.Update(delta.y);
	}

	void CreateVirtualAxes()
	{
		// create new axes based on axes to use

		m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
		CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);

		m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
		CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);

		m_InputVirtualButton = new CrossPlatformInputManager.VirtualButton (inputButtonName);
		CrossPlatformInputManager.RegisterVirtualButton(m_InputVirtualButton);
	}


	public void OnDrag(PointerEventData data)
	{
		UpdateVirtualAxes(data.position);
	}


	public void OnPointerUp(PointerEventData data)
	{
		m_InputVirtualButton.Released ();
		visualizer.color = Color.gray;
	}


	public void OnPointerDown(PointerEventData data) 
	{
		UpdateVirtualAxes(data.position);
		m_InputVirtualButton.Pressed ();
		visualizer.color = Color.green;
	}

	void OnDisable()
	{
		m_HorizontalVirtualAxis.Remove();
		m_VerticalVirtualAxis.Remove();
		m_InputVirtualButton.Remove();
	}
}
