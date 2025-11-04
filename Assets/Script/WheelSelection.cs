using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelSelection : MonoBehaviour
{

    public Button[] buttons;
    public float radius = 200f;
    public Vector2 centeroffset = Vector2.zero;
    // Start is called before the first frame update    
    void Start()
    {
        ArrangeButtonsInCircle();
    }

    void OnValidate()
    {
        ArrangeButtonsInCircle();
    }

    void ArrangeButtonsInCircle()
    {
        if (buttons == null || buttons.Length == 0)
            return;
        float angleStep = 360f / buttons.Length;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians) * radius;
            float y = Mathf.Cos(radians) * radius;
            RectTransform buttontransform = buttons[i].GetComponent<RectTransform>();

            if (buttontransform != null)
            {
                buttontransform.anchoredPosition = new Vector2(x, y) + centeroffset;
                buttontransform.localEulerAngles = new Vector3(0, 0, -angle);

                foreach (RectTransform child in buttontransform)
                {
                    child.localEulerAngles = new Vector3(0, 0, angle);
                }
            }
        }
    }
}
