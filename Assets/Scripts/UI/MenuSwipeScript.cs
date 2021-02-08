using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuSwipeScript : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    // Start is called before the first frame update
    private ScrollRect scrollRect;
    private RectTransform rect;
    private int currentIndex = 0;
    private int childCount;
    private bool isDragging = false;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging == false) {
            var desiredPos = CalculateDesiredScrollPos();
            var currentPos = scrollRect.horizontalNormalizedPosition;
            var dist = Mathf.Abs(desiredPos - currentPos);
            if (dist > 0.0001) {
                scrollRect.horizontalNormalizedPosition += (desiredPos - currentPos) * Time.deltaTime * Mathf.Pow(dist, -0.6f) *2;
            } else {
                scrollRect.horizontalNormalizedPosition = desiredPos;
            }
        }
    }

    public void OnRectTransformDimensionsChange() {
        if (scrollRect == null || rect == null)
            return;

        var content = scrollRect.content.GetComponent<RectTransform>();
        var width = rect.rect.width;
        childCount = content.childCount;
        
        for(int index = 0; index < childCount; index++) {
            var child = content.GetChild(index).GetComponent<RectTransform>();
            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, width * index, width);
        }
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * childCount);
        scrollRect.horizontalNormalizedPosition = CalculateDesiredScrollPos();
    }

    public float CalculateDesiredScrollPos() {
        currentIndex = Mathf.Clamp(currentIndex, 0, childCount-1);
        return 1f/(childCount - 1) *currentIndex;
    }

    public void OnEndDrag(PointerEventData data) {
        if (data.position.x - data.pressPosition.x > 0) {
            currentIndex--;
        } else {
            currentIndex++;
        }
        currentIndex = Mathf.Clamp(currentIndex, 0, childCount-1);
        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData data) {
        isDragging = true;
    }
}
