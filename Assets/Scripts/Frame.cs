using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : Triggerable
{
    public Transform spinThisChild;

    private bool m_enabled;
    private float m_scaleSlider;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public override void Trigger(){
        m_enabled ^= true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float targetScale = m_enabled ? 1f : 0f;
        m_scaleSlider = Mathf.Lerp(m_scaleSlider, targetScale, Time.fixedDeltaTime * 4f);
        transform.localScale = Vector3.one * m_scaleSlider;

        float period = 10f;
        float z_t = Time.timeSinceLevelLoad / period;
        float zAngle = z_t * 360f;
        spinThisChild.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zAngle);
    }
}
