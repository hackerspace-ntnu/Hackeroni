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
    private float previousScrollingSign = 0;

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
            var currentScrollingSign = Mathf.Sign(desiredPos - currentPos);

            if (previousScrollingSign == 0 || currentScrollingSign == previousScrollingSign) {
                var dist = Mathf.Abs(desiredPos - currentPos);
                scrollRect.horizontalNormalizedPosition += (desiredPos - currentPos) * Time.deltaTime * Mathf.Pow(dist, -0.6f) *2;
                previousScrollingSign = currentScrollingSign; 
            } else {
                scrollRect.horizontalNormalizedPosition = desiredPos;
                isDragging = true;
                previousScrollingSign = 0;
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
        previousScrollingSign = 0;
    }
}
