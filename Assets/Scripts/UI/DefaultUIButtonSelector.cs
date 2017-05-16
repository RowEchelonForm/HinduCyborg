using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DefaultUIButtonSelector : MonoBehaviour
{

	private EventSystem uiEventSystem;
	private GameObject defaultSelection;


	// Use this for initialization
	void Start()
	{
		if ( !initEventSys() )
		{
			return;
		}
		deselectIfNoControllers();
	}

	void Update()
	{
		// Selects button if nothing is selected and getting input from vertical/horizontal axis
		if ( uiEventSystem.currentSelectedGameObject == null && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) )
		{
			uiEventSystem.SetSelectedGameObject( defaultSelection );
		}
	}

	private bool initEventSys()
	{
		uiEventSystem = gameObject.GetComponent<EventSystem>();
		if (uiEventSystem == null)
		{
			Debug.LogError("DefaultUIButtonSelector is connected to an object that doesn't have an EventSystem component!");
			this.enabled = false;
			return false;
		}
		defaultSelection = uiEventSystem.firstSelectedGameObject;
		return true;
	}

	private void deselectIfNoControllers()
	{
		string[] joysticks = Input.GetJoystickNames();
		if ( joysticks.Length <= 0 ) // no controllers
		{
			uiEventSystem.firstSelectedGameObject = null;
			return;
		}

		bool controllerConnected = false;
		for (int i = 0; i < joysticks.Length; ++i) // there can be empty 'left-overs'
		{
			if (joysticks[i] != "") // "" as name means not connected
			{
				controllerConnected = true;
			}
		}
		if (!controllerConnected)
		{
			uiEventSystem.firstSelectedGameObject = null;
		}
	}
	

}
