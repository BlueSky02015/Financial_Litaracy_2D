using UnityEngine;

public class LaptopClickHandler : MonoBehaviour
{
    public void OnLaptopClick()
    {
        UIManager.instance.OnLaptopDesktopOpened();
    }
}