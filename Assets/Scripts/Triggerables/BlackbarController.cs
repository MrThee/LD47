using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class BlackbarController : Triggerable
{
    public RectTransform topRect;
    public RectTransform bottomRect;

    private bool m_enabled;

    public override void Trigger(){
        m_enabled ^= true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float targetValue = m_enabled ? 0f : -64f;
        float delta = Time.fixedDeltaTime * 64f;
        topRect.anchoredPosition = new Vector2(
            0f, 
            Mathf.MoveTowards(topRect.anchoredPosition.y, -targetValue, delta)
        );
        bottomRect.anchoredPosition = new Vector2(
            0f,
            Mathf.MoveTowards(bottomRect.anchoredPosition.y, targetValue, delta)
        );
    }
}
