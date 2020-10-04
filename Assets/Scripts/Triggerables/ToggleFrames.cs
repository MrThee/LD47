using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFrames : Triggerable {
    public AudioClip growClip;
    public AudioClip shrinkClip;

    public WorldParameters worldParams;
    public Frame framePrefab;

    private List<Frame> m_frames;

    private bool m_active;

    void Start(){
        int subdivCount = 6;
        m_frames = new List<Frame>(subdivCount-1);
        for(int i = 1; i < subdivCount; i++){
            float ringT = (float)i / subdivCount;
            ringT *= 0.5f;

            worldParams.CalcCenteredOrientation(ringT, true, out var position, out var rotation);
            var newFrame = Frame.Instantiate(framePrefab, position, rotation, this.transform);
            m_frames.Add(newFrame);
        }
    }

    public override void Trigger(){
        m_active ^= true;
        foreach(var frame in m_frames){
            frame.Trigger();
        }
        var playerPoint = MobiusMover.Inst.transform.position;
        if(m_active){
           AudioSource.PlayClipAtPoint(growClip, playerPoint);
        } else {
            AudioSource.PlayClipAtPoint(shrinkClip, playerPoint);
        }
    }

}