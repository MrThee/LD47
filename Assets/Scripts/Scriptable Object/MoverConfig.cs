using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Mover Config", order = 0, fileName="Mover Config")]
public class MoverConfig : ScriptableObject {

    public float defaultSpeed = 5f;
    public float lateralSpeed = 1f;

    public float maxSteerRoll = 25f;

}