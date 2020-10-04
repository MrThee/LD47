using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMover : MonoBehaviour
{
    public WorldParameters worldParameters;
    public MoverConfig moverConfig;
    public Transform armatureRoot;

    private float m_ring_space_t;
    private float m_lat_space_t;

    private List<(float,int)> m_encodedInputs;
    private int m_curInputIdx;

    // Start is called before the first frame update
    void Start()
    {
        m_encodedInputs = new List<(float, int)>();
    }

    [System.Serializable]
    public struct KickoffPacket {
        public List<(float,int)> inputStreamRef;
        public float latSpaceT;
    }

    public void Kickoff(KickoffPacket kickoff){
        m_encodedInputs.Clear();
        m_curInputIdx = 0;
        var inputStream = kickoff.inputStreamRef;
        foreach(var entry in inputStream){
            m_encodedInputs.Add(entry);
        }
        m_ring_space_t = 0f;
        m_lat_space_t = kickoff.latSpaceT;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(m_encodedInputs.Count == 0){
            return; // Can't do anything w/o inputs.
        }
        // Every fixed update, consume an input
        float dt = Time.fixedDeltaTime;
        float curInputX = ConsumeInput();

        // Ring Movement
        CalcOrientation(m_ring_space_t, m_lat_space_t, worldParameters, out var actorPos, out var actorRotation);
        transform.position = actorPos;
        transform.rotation = actorRotation;

        // # local roll
        float targetAngle = -moverConfig.maxSteerRoll * curInputX;
        float z = Mathf.LerpAngle(armatureRoot.localEulerAngles.z, targetAngle, dt*8f);
        // Debug.LogFormat("z: {0}", z);
        armatureRoot.localEulerAngles = new Vector3(
            armatureRoot.localEulerAngles.x, armatureRoot.localEulerAngles.y,
            z
        );

        float speed = moverConfig.defaultSpeed;
        float spaceDelta = speed * dt;
        float parameterDelta_t = worldParameters.CalcDeltaT(spaceDelta);
        m_ring_space_t = Mathf.Repeat(m_ring_space_t + parameterDelta_t, 1f);

        // #lateral deviation
        float latSpaceDelta = curInputX * dt * moverConfig.lateralSpeed;
        float lat_space_deltaT = latSpaceDelta / worldParameters.halfWidth;
        
        m_lat_space_t += lat_space_deltaT;
        m_lat_space_t = Mathf.Clamp(m_lat_space_t, -1f, 1f);
    }

    float ConsumeInput(){
        if(m_encodedInputs.Count == 0){
            return 0f;   
        }

        var curInput = m_encodedInputs[m_curInputIdx];
        var newState = (curInput.Item1, curInput.Item2-1);
        m_encodedInputs[m_curInputIdx] = newState;
        if(newState.Item2 == 0){
            // Stack fully consumed
            m_curInputIdx++;
            if(m_curInputIdx >= m_encodedInputs.Count){
                // Consumed all inputs!
                m_encodedInputs.Clear();
            }
        }
        return curInput.Item1;
    }

    // Moves the opposite direction
    static void CalcOrientation(float travelT, float horzDeviationT, WorldParameters worldParams,
        out Vector3 actorPosition, out Quaternion actorRotation)
    {
        // travelT ranges from [0,1]
        float ringAngle = 720f * travelT;
        float axialAngle = 360f * travelT;

        Quaternion ringRotation = Quaternion.AngleAxis(ringAngle, Vector3.left);
        Vector3 up = ringRotation * Vector3.up;
        Vector3 fwd = ringRotation * Vector3.back;
        Quaternion axialRotation = Quaternion.AngleAxis(axialAngle, Vector3.forward);

        actorRotation = ringRotation * axialRotation * Quaternion.AngleAxis(180f, Vector3.up);

        Vector3 majorOffset = up * worldParams.majorRadius;
        
        Vector3 localMinorOffset = (Vector3.up * worldParams.halfThickness) + (Vector3.right * worldParams.halfWidth * horzDeviationT);
        Vector3 minorOffset = actorRotation * localMinorOffset;

        actorPosition = majorOffset + minorOffset;
    }
}
