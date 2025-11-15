using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseControll : MonoBehaviour
{
    Vector3 mousePos;
    RaycastHit2D hit;
    Ray MouseRay;
    Transform objectHit;
    Transform previousHoverObject, nextHoverObject;
    [SerializeField] private Texture2D CursorNormal;
    [SerializeField] private Texture2D CursorHover;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private InteractableHandler interactableHandler;


    void Awake()
    {
        ChangeCursor(CursorNormal);
        // Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        mousePos = Input.mousePosition;
        MouseRay = Camera.main.ScreenPointToRay(mousePos);
        clickedObject();
        HoverObject();

    }

    void clickedObject()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hit = Physics2D.Raycast(MouseRay.origin, MouseRay.direction);
            objectHit = hit ? hit.collider.transform : null;

            if (objectHit != null)
            {
                TutorialTrigger();
                audioManager.playSFX(audioManager.Click_SFX, 1.5f);

                IInteractable interactable = objectHit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnClicked();
                }

                if (interactableHandler != null)
                {
                    interactableHandler.OnObjectClicked(objectHit);
                }
            }
        }
    }

    void HoverObject()
    {
        previousHoverObject = nextHoverObject;
        hit = Physics2D.Raycast(MouseRay.origin, MouseRay.direction);
        nextHoverObject = hit ? hit.collider.transform : null;

        if (previousHoverObject != nextHoverObject)
        {
            if (previousHoverObject != null)
            {
                ChangeCursor(CursorNormal);
            }
            if (nextHoverObject != null)
            {
                ChangeCursor(CursorHover);
            }
        }
    }

    void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }

    void TutorialTrigger()
    {
        // 🔍 DEBUG: Check if TutorialManager exists
        if (TutorialManager.instance == null)
        {
            Debug.LogError("TutorialManager is NULL!");
        }
        else if (TutorialManager.instance.IsTutorialActive())
        {
            Debug.Log("Tutorial is ACTIVE");
            Debug.Log($"Current tutorial step: {TutorialManager.instance.currentStep}");

            if (objectHit.CompareTag("Laptop"))
            {
                Debug.Log("Laptop clicked → calling OnLaptopClicked()");
                TutorialManager.instance.OnLaptopClicked();
            }
            else if (objectHit.CompareTag("Door"))
            {
                Debug.Log("Door clicked → calling OnDoorClicked()");
                TutorialManager.instance.OnDoorClicked();
            }
        }
        else
        {
            Debug.Log("Tutorial is NOT active");
        }
    }
}
