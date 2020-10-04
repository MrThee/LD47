using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleParticleSystem : Triggerable
{
    public ParticleSystem kParticleSystem;

    private bool m_enabled = false;
    // Start is called before the first frame update

    public override void Trigger(){
        m_enabled ^= true;
        if(m_enabled){
            kParticleSystem.Play();
        } else {
            kParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
