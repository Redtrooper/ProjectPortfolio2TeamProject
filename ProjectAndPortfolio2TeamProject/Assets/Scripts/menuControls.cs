using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuControls : MonoBehaviour
{
    [SerializeField] List<GameObject> interactableList = new List<GameObject>();
    [SerializeField] List<Image> backgroundImageList = new List<Image>();

    public bool freezeInput = false;
    private int currentlySelectedItem;

    private void Start()
    {
        currentlySelectedItem = interactableList.Count - 1;
        foreach (Image image in backgroundImageList)
        {
            image.enabled = false;
        }
        backgroundImageList[currentlySelectedItem].enabled = true;
    }

    private void Update()
    {
        if (!freezeInput)
        {
            changeSelectedItem();
            GameObject currentlySelectedInteractable = interactableList[currentlySelectedItem];
            Button selectedButton = null;
            Slider selectedSlider = null;
            Toggle selectedToggle = null;
            if (Input.GetButtonDown("Submit"))
            {

                if (currentlySelectedInteractable.TryGetComponent<Button>(out selectedButton))
                    selectedButton.onClick.Invoke();
                else if (currentlySelectedInteractable.TryGetComponent<Toggle>(out selectedToggle))
                    selectedToggle.isOn = !selectedToggle.isOn;
            }
            if (currentlySelectedInteractable.TryGetComponent<Slider>(out selectedSlider))
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    selectedSlider.value += 1;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    selectedSlider.value -= 1;
                }
            }
        }
    }

    private void changeSelectedItem()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentlySelectedItem < interactableList.Count - 1)
        {
            backgroundImageList[currentlySelectedItem].enabled = false;
            currentlySelectedItem++;
            backgroundImageList[currentlySelectedItem].enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentlySelectedItem > 0)
        {
            backgroundImageList[currentlySelectedItem].enabled = false;
            currentlySelectedItem--;
            backgroundImageList[currentlySelectedItem].enabled = true;
        }
    } 
}
