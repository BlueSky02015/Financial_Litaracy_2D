using UnityEngine;

public class InteractableHandler : MonoBehaviour
{
    [System.Serializable]
    public class TagToCanvas
    {
        public string tag;
        public GameObject canvas;
    }

    [SerializeField] private TagToCanvas[] tagCanvasMappings;
    [SerializeField] private AudioManager audioManager;

    // Track current state
    private Transform currentlyActiveObject;
    private GameObject currentlyActiveCanvas;

    public void OnObjectClicked(Transform clickedObject)
    {
        if (clickedObject == null)
        {
            // Optional: close UI if clicked on empty space
            CloseCurrentCanvas();
            return;
        }

        // Find mapping for this object's tag
        TagToCanvas matchedMapping = null;
        foreach (var mapping in tagCanvasMappings)
        {
            if (clickedObject.CompareTag(mapping.tag))
            {
                matchedMapping = mapping;
                break;
            }
        }

        // If clicked object is NOT in our interactable list
        if (matchedMapping == null)
        {
            CloseCurrentCanvas();
            return;
        }

        // Case 1: Same object clicked again → toggle OFF
        if (clickedObject == currentlyActiveObject)
        {
            CloseCurrentCanvas();
            return;
        }

        // Case 2: Different object → close old, open new
        CloseCurrentCanvas();

        // Open new canvas
        if (matchedMapping.canvas != null)
        {
            SetCanvasVisible(matchedMapping.canvas, true);
            currentlyActiveObject = clickedObject;  
            currentlyActiveCanvas = matchedMapping.canvas;

            if (audioManager != null)
                audioManager.playSFX(audioManager.Click_SFX, 1.5f);

        }

        // Special handling for Laptop
        if (clickedObject.CompareTag("Laptop"))
        {
            TutorialManager.instance?.OnAppClicked("Laptop");
            matchedMapping.canvas.SetActive(true);

            // Notify UIManager
            if (UIManager.instance != null)
                UIManager.instance.OnLaptopDesktopOpened();
        }

        if (clickedObject.CompareTag("Door"))
        {
            TutorialManager.instance?.OnAppClicked("Door");
        }
    }

    public void CloseCurrentCanvas()
    {
        if (currentlyActiveCanvas != null)
        {
           SetCanvasVisible(currentlyActiveCanvas, false);
        }
        currentlyActiveObject = null;
        currentlyActiveCanvas = null;
    }


    void SetCanvasVisible(GameObject canvas, bool visible)
    {
        if (canvas == null) return;

        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
        else
        {
            // Fallback to SetActive if no CanvasGroup
            canvas.SetActive(visible);
        }
    }
}