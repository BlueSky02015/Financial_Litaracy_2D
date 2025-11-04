using UnityEngine;

public class LaptopClickHandler : MonoBehaviour
{
    public void OnLaptopClicked()
    {
        // Let InteractableHandler open the canvas (via tag)
        // AND notify UIManager
        UIManager.instance.OnLaptopDesktopOpened();
    }
}