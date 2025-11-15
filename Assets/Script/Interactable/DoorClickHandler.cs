using UnityEngine;

public class DoorClickHandler : MonoBehaviour
{
    public void OnLaptopClicked()
    {
        TutorialManager.instance.OnDoorClicked();
    }
}