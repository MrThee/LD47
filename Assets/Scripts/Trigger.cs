using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public List<Triggerable> triggerable = new List<Triggerable>();
    
    public void Activate(){
        foreach(var t in triggerable){ t.Trigger(); }
    }
}
