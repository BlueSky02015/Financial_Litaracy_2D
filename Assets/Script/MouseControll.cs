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

    void Awake()
    {
        ChangeCursor(CursorNormal);
        Cursor.lockState = CursorLockMode.Confined;
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
                audioManager.playSFX(audioManager.Click_SFX, 1.5f);
                Debug.Log("Hit " + objectHit.name);
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
                Debug.Log("Exit " + previousHoverObject.name);
            }
            if (nextHoverObject != null)
            {   
                ChangeCursor(CursorHover);
                Debug.Log("Enter " + nextHoverObject.name);
            }
        }
    }
    
    void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }
}
