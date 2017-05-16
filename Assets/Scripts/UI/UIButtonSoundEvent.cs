using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonSoundEvent : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{

	[SerializeField]
	private AudioClip buttonHighlightSound;
	[SerializeField]
	private AudioClip buttonPressSound;
	[SerializeField]
	private float highlightVolume = 0.3f;
	[SerializeField]
	private float pressVolume = 0.5f;

	private Button targetButton;


	void Start()
	{
		targetButton = gameObject.GetComponent<Button>();
		if (targetButton == null)
		{
			Debug.LogError("UIButtonSoundEvent is connected to an object that doesn't have a Button component!");
			this.enabled = false;
		}
		else
		{
			targetButton.onClick.AddListener( () => playPressSound() ); // play sound on click (button pressed)
		}
	}


	// Button highlighted (e.g. mouse over)
	public void OnPointerEnter( PointerEventData PEData )
	{
		if ( targetButton.interactable == true )
		{
			playHighlightSound();
		}
	}

	// Button selected (i.e. with a controller)
	public void OnSelect(BaseEventData eventData)
	{
		playHighlightSound();
	}

	private void playHighlightSound()
	{
		SoundFXPlayer.instance.playClipOnce(buttonHighlightSound, highlightVolume);
	}

	private void playPressSound()
	{
		SoundFXPlayer.instance.playClipOnce(buttonPressSound, pressVolume);
	}

}
