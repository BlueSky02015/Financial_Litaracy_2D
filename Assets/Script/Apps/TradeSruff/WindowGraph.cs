using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private RectTransform labelYTamplate;
    private float xSize = 50f;
    [SerializeField] private float yMaximum;
    [SerializeField] private float yMinimum;

    private List<GameObject> circleGameObjects = new List<GameObject>();
    private List<GameObject> lineGameObjects = new List<GameObject>();
    private List<RectTransform> labelYTransforms = new List<RectTransform>();

    private GameObject CreateCircle(Vector2 anchorPos)
    {
        GameObject circle = new GameObject("Circle", typeof(Image));
        circle.transform.SetParent(graphContainer, false);
        circle.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPos;
        rectTransform.sizeDelta = new Vector2(20, 20);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        circleGameObjects.Add(circle);
        return circle;
    }

    private void ShowGraph(List<int> valueList, Func<float, string> getAxislabelY = null)
    {
        if (getAxislabelY == null)
        {
            getAxislabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        float graphHeight = graphContainer.sizeDelta.y;
        GameObject lastCircle = null;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            // Calculate normalized position between yMinimum and yMaximum
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));

            if (lastCircle != null)
            {
                lineGameObjects.Add(CreateLine(lastCircle.GetComponent<RectTransform>().anchoredPosition,
                circleGameObject.GetComponent<RectTransform>().anchoredPosition));
            }
            lastCircle = circleGameObject;
        }

        // Label Y-Axis 
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelYTamplate);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-75, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = getAxislabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            labelYTransforms.Add(labelY);
        }
    }

    private GameObject CreateLine(Vector2 startPos, Vector2 endPos)
    {
        GameObject line = new GameObject("line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        RectTransform rect = line.GetComponent<RectTransform>();

        Image img = line.GetComponent<Image>();
        img.color = new Color(0.5f, 0.5f, 1f, 0.5f);

        Vector2 dir = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(distance, 3f);
        rect.anchoredPosition = startPos + dir * distance * 0.5f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        return line;
    }

    private void ClearGraph()
    {
        foreach (GameObject circle in circleGameObjects) Destroy(circle);
        foreach (GameObject line in lineGameObjects) Destroy(line);
        foreach (RectTransform labelY in labelYTransforms) Destroy(labelY.gameObject);

        circleGameObjects.Clear();
        lineGameObjects.Clear();
        labelYTransforms.Clear();
    }

    public void UpdateGraphData(List<int> valueList)
    {
        // Calculate new maximum and minimum for Y axis
        yMaximum = Mathf.Max(valueList.ToArray());
        yMinimum = Mathf.Min(valueList.ToArray());
        
        // Optionally, you might want to add some padding
        float padding = (yMaximum - yMinimum) * 0.1f;
        yMaximum += padding;
        yMinimum -= padding;
        
        // Ensure minimum doesn't go below zero if all values are positive
        if (yMinimum < 0 && valueList.TrueForAll(x => x >= 0))
        {
            yMinimum = 0;
        }
        
        ClearGraph();
        ShowGraph(valueList, (float _f) => "$" + Mathf.RoundToInt(_f));
    }
}