using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Attach to the player GameObject as a component. 
 * Triggers the planet end UI when player hits a trigger with the tag 'EndTrigger' or 
 * when the player presses the 'Esc' key.
*/
public class PlanetEndController : MonoBehaviour
{

    [SerializeField]
    private GameObject planetUI;
    [SerializeField]
    private Button yesButton;
    [SerializeField]
    private Button noButton;

    private bool isUIActive = false;

	private void Start()
    {
        findPlanetUI();
        findYesNoButtons();
        if (yesButton)
        {
            yesButton.onClick.AddListener(clickYes);
        }
        if (noButton)
        {
            noButton.onClick.AddListener(clickNo);
        }
        disableUI();
	}
	
	private void Update()
    {
        if (isUIActive)
        {
            handleUIInput();
        }
	}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EndTrigger") && !isUIActive)
        {
            enableUI();
        }
    }


    private void enableUI()
    {
        Time.timeScale = 0f;
        planetUI.SetActive(true);
        isUIActive = true;
    }

    private void disableUI()
    {
        Time.timeScale = 1;
        planetUI.SetActive(false);
        isUIActive = false;
    }

    private void handleUIInput()
    {
        float vertical = Input.GetAxis("Vertical");
        if (vertical < 0)
        {
            clickYes();
        }
        else if (vertical > 0)
        {
            clickNo();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            clickYes();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            clickNo();
        }
    }

    private void clickYes()
    {
        disableUI();
        LevelManager.loadLevel("Space");
    }

    private void clickNo()
    {
        disableUI();
    }


    private void findPlanetUI()
    {
        if (planetUI != null) // UI ok
        {
            return;
        }

        planetUI = GameObject.FindGameObjectWithTag("PlanetUI");
        if (planetUI != null)
        {
            return;
        }

        planetUI = GameObject.Find("PlanetUI");
        if (planetUI == null)
        {
            Debug.LogError("PlanetUI GameObject is not attached to PlanetEndController script and can't be found.");
        }
    }

    // Call after finding planetUI
    private void findYesNoButtons()
    {
        if ((yesButton != null && noButton != null) || planetUI == null)
        {
            // already ok or planetUI not found
            return;
        }

        Button[] buttons = planetUI.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; ++i) // find yes/no buttons
        {
            string bName = buttons[i].name.ToLower();
            if (yesButton == null && (bName == "yesbutton" || bName == "yes button" || bName == "yes" || bName == "yes_button" || bName == "yes-button"))
            {
                yesButton = buttons[i];
            }
            else if (noButton == null && (bName == "nobutton" || bName == "no button" || bName == "no" || bName == "no_button" || bName == "no-button"))
            {
                noButton = buttons[i];
            }

            if (noButton != null && yesButton != null) // found both, stop looping
            {
                break;
            }
        }

        if (noButton == null)
        {
            Debug.LogError("PlanetEndController can't find noButton under '" + planetUI.name + "' GameObject.");
        }
        if (yesButton == null)
        {
            Debug.LogError("PlanetEndController can't find yesButton under '" + planetUI.name + "' GameObject.");
        }
    }

}
