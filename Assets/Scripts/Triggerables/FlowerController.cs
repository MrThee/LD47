using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : Triggerable
{
    public AudioClip growClip;

    public WorldParameters worldParams;
    public Plant pfFlower;
    public List<Material> flowerMats = new List<Material>();
    public int subdivCount = 64;

    private List<Plant> m_flowers;

    void Start(){
        this.m_flowers = new List<Plant>(64);
        for(int i = 0; i < subdivCount; i++){
            float ring_t = (float)i / subdivCount;
            Material flowerMat = flowerMats[i % flowerMats.Count];
            bool evenRow = ((i&0x1) == 0);
            int perRow = evenRow ? 5 : 4;
            float delta = 2f / 5;
            float baseOffset = -1f + (evenRow ? (0.5f*delta) : delta );
            for(int f = 0; f < perRow; f++){
                float lat_t = baseOffset + (f*delta);
                worldParams.CalcOrientation(ring_t, lat_t, true, out var position, out var rotation);

                var inst = Plant.Instantiate(pfFlower, position, rotation, transform);
                Quaternion extraSpin = Quaternion.AngleAxis(180f, inst.transform.up);
                rotation = extraSpin * rotation;
                inst.transform.rotation = rotation;
                inst.transform.localScale = Vector3.zero;
                inst.kRenderer.sharedMaterial = flowerMat;
                m_flowers.Add(inst);
            }
        }
    }

    bool m_activated;
    float m_scaleSlider = 0f;

    public override void Trigger(){
        m_activated ^= true;
        var playerPoint = MobiusMover.Inst.transform.position;
        AudioSource.PlayClipAtPoint(growClip, playerPoint);
    }

    void FixedUpdate(){
        float targetScale = m_activated ? 1f : 0f;
        m_scaleSlider = Mathf.Lerp(m_scaleSlider, targetScale, 5f * Time.fixedDeltaTime);

        foreach(var flower in m_flowers){
            flower.transform.localScale = Vector3.one * m_scaleSlider;
        }
    }
}
