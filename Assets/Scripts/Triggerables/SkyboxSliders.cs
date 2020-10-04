using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSliders : Triggerable
{
    [System.Serializable]
    public class Entry {

        public Color lightColor;
        public Color skyColor;
        public Color groundColor;
        public Color ambientColor;
        public Color trackColor;
    }
    public Light directionalLight;
    public Material skyboxMaterialInstance;
    public Material trackColor;
    public List<Entry> entries = new List<Entry>();

    private int m_targetEntryIndex;

    private int m_skyTintID;
    private int m_groundColorID;
    private int m_colorID;

    void Start(){
        m_skyTintID = Shader.PropertyToID("_SkyTint");
        m_groundColorID = Shader.PropertyToID("_GroundColor");
        m_colorID = Shader.PropertyToID("_BaseColor");
    }

    public override void Trigger(){
        m_targetEntryIndex = (m_targetEntryIndex+1) % entries.Count;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var curEntry = entries[m_targetEntryIndex];

        var mat = skyboxMaterialInstance;
        var lerpAmt = Time.deltaTime * 7f;

        directionalLight.color = Color.Lerp( directionalLight.color, curEntry.lightColor, lerpAmt );
        mat.SetColor( m_skyTintID, Color.Lerp(mat.GetColor(m_skyTintID), curEntry.skyColor, lerpAmt) ) ;
        mat.SetColor( m_groundColorID, Color.Lerp(mat.GetColor(m_groundColorID), curEntry.groundColor, lerpAmt) ) ;
        RenderSettings.ambientLight = Color.Lerp( RenderSettings.ambientLight, curEntry.ambientColor, lerpAmt );
        trackColor.SetColor( m_colorID, Color.Lerp(trackColor.GetColor(m_colorID), curEntry.trackColor, lerpAmt) );
    }
}
