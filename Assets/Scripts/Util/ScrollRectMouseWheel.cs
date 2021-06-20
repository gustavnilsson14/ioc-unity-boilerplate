using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectMouseWheel : MonoBehaviour
{
    public float sensitivity = 1;
    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0)
            return;
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar.value += (scroll / scrollRect.content.sizeDelta.y) * sensitivity;
        scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value,0,1);
    }
}
