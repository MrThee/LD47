using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSwitch : Triggerable {

    public AudioClip hitClip;

    public WorldParameters worldParams;
    
    [Range(0,1)]
    public float ringPosition;
    [Range(-1,1)]
    public float lateralPosition;
    
    public Animation kAnimation;
    public Renderer kRenderer;
    public Collider kTriggerCollider;

    public AnimationClip idleClip;
    public AnimationClip activatedClip;

    public List<Material> activationStateMaterials = new List<Material>();

    private float m_scaleSlider;
    private int m_activationState;

    void Start(){
        // Position this correctly
        worldParams.CalcOrientation(ringPosition, lateralPosition, true, 
            out var worldPos, out var worldRot
        );
        transform.SetPositionAndRotation(worldPos, worldRot);
        m_scaleSlider = 0f;
        transform.localScale = Vector3.zero;
    }

    void FixedUpdate(){
        if(kAnimation.isPlaying == false){
            this.kAnimation.Play(idleClip);
            this.kTriggerCollider.enabled = true;
        }

        // Scale by proximity
        Vector2 playerPos = MobiusMover.PlayerParametricPosition;
        bool playersInRange = InParametricRange(playerPos.x);
        float targetScale = playersInRange ? 1 : 0;

        float oldScale = m_scaleSlider;
        m_scaleSlider = Mathf.Lerp(m_scaleSlider, targetScale, Time.fixedDeltaTime * 12f);
        this.transform.localScale = Vector3.one * m_scaleSlider;

        if(oldScale < 0.05f && 0.05f <= m_scaleSlider){
            this.kTriggerCollider.enabled = true;
        } else if(oldScale > 0.05f && 0.05f >= m_scaleSlider){
            this.kTriggerCollider.enabled = false;
        }
    }

    public override void Trigger(){
        var playerPos = MobiusMover.Inst.transform.position;
        AudioSource.PlayClipAtPoint(hitClip, playerPos);
        this.kAnimation.Stop();
        this.kAnimation.Play(activatedClip);
        int stateCount = activationStateMaterials.Count;
        m_activationState = (m_activationState+1) % stateCount;
        this.kRenderer.sharedMaterial = activationStateMaterials[m_activationState];
        this.kTriggerCollider.enabled = false;
    }

    public bool InParametricRange(float playerRingPos){
        const float minDelta = -8.0f;
        const float maxDelta = -1.0f;
        var (min,max) = worldParams.GetParametricRingRange(ringPosition, minDelta, maxDelta);

        if(min < max){
            return (min < playerRingPos && playerRingPos < max);
        } else {
            // Wrapped around the ring
            return (min < playerRingPos || playerRingPos < max);
        }
    }

}