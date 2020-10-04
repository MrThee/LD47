using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static void Play(this Animation anim, AnimationClip clip){
        anim.Rewind();
        anim.clip = clip;
        anim.Play();
    }

}