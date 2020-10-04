using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFlame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float period = .15f;
        float t = Mathf.Sin(Time.timeSinceLevelLoad * Mathf.PI * 2f / period);
        float scale_t = 0.5f + 0.5f*t;
        float minScale = 0.85f;
        float maxScale = 1f;

        float scale = Mathf.Lerp(minScale, maxScale, scale_t);

        transform.localScale = Vector3.one * scale;
    }
}
