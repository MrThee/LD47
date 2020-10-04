using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCycles : Triggerable
{   
    public UnityEngine.Animations.ParentConstraint cameraConstraint;
    public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private enum State {
        FromBack,
        FromSide,
        EaseBackAndForth,
        Orbit,
        _LAST
    }

    private State m_oldState;
    private State m_state;
    private float m_oldStateTimer;
    private float m_stateTimer;

    public override void Trigger(){
        var newState = m_state + 1;
        if(newState == State._LAST){
            newState = State.FromBack;
        }
        ChangeState(newState);
    }

    static (Vector3, Vector3) GetConstraints(State state, float timer){
        switch(state){
        default:
        case State.FromBack: {
            return (new Vector3(0f, 0.637f, -0.856f), new Vector3(24f, 0f, 0f));
        }
        case State.FromSide: {
            return (new Vector3(-0.3f, 0.637f, -0.856f), new Vector3(24f, 0f, 0f));
        }
        case State.EaseBackAndForth: {
            var a = (new Vector3(-0.3f, 0.22f, -0.856f), new Vector3(6.24f, 16f, 0f));
            var b = (new Vector3(0.25f, 0.3100004f, -0.18f), new Vector3(34.16005f, -19.41f, 0f));
            const float period = 20f;
            float t = timer / period;
            t = Mathf.Sin(t * 2 * Mathf.PI)*0.5f + 0.5f;
            return LerpConstraints(a,b, t);
        }
        case State.Orbit: {
            const float period = 20f;
            float t = timer / period;
            t = Mathf.Repeat(t, 1f);
            Vector3 spinThis = new Vector3(0f, 0.4f, -0.856f);
            Vector3 offset = Quaternion.AngleAxis(t * 360f, Vector3.up) * spinThis;
            float y = t * 360f;
            return (offset, new Vector3(24f, y, 0f));
        }
        }
    }

    static (Vector3, Vector3) LerpConstraints((Vector3, Vector3) a, (Vector3, Vector3) b, float t){
        Vector3 lerpedAngles = new Vector3(
            Mathf.LerpAngle(a.Item2.x, b.Item2.x, t),
            Mathf.LerpAngle(a.Item2.y, b.Item2.y, t),
            Mathf.LerpAngle(a.Item2.z, b.Item2.z, t)
        );
        return ( Vector3.Lerp(a.Item1, b.Item1, t), lerpedAngles );
    }

    const float CROSSFADE_PERIOD = 2f;

    void ChangeState(State newState){
        m_oldStateTimer = m_stateTimer;
        m_stateTimer = 0f;
        m_oldState = m_state;
        m_state = newState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var oldConstraints = GetConstraints(m_oldState, m_oldStateTimer);
        var curConstraints = GetConstraints(m_state, m_stateTimer);
        var blend = m_stateTimer / CROSSFADE_PERIOD;
        blend = blendCurve.Evaluate(blend);
        var usedThese = LerpConstraints(oldConstraints, curConstraints, blend);

        // Apply constraint
        cameraConstraint.SetTranslationOffset(0, usedThese.Item1);
        cameraConstraint.SetRotationOffset(0, usedThese.Item2);

        float dt = Time.fixedDeltaTime;
        m_oldStateTimer += dt;
        m_stateTimer += dt;
    }
}
