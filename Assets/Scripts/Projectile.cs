using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool positive = true;
    public WorldParameters worldParameters;
    public MoverConfig moverConfig;

    private float m_ring_space_t;
    private float m_lat_space_t;

    private float m_stateTimer;

    private static int? s_triggerLayerMask;

    void Start(){
        s_triggerLayerMask = s_triggerLayerMask ?? LayerMask.GetMask("Hittable");
    }

    public void Kickoff(float ringSpaceT, float latSpaceT){
        m_ring_space_t = ringSpaceT;
        m_lat_space_t = latSpaceT;
        CalcOrientation(m_ring_space_t, m_lat_space_t, positive, worldParameters, out var actorPos, out var actorRotation);
        transform.position = actorPos;
        transform.rotation = actorRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if(MaybeHitSomething(dt)){
            Destroy();
            return;
        }
        MovementUpdate(dt);
        m_stateTimer += dt;

        if(m_stateTimer > 1f){
            Destroy();
        }
    }

    bool MaybeHitSomething(float dt){
        CalcOrientation(m_ring_space_t, m_lat_space_t, positive, worldParameters, out var finalPos, out _);
        Vector3 initPos = transform.position;
        Vector3 rayDir = (finalPos - initPos);
        float rayLength = rayDir.magnitude;

        if(Physics.SphereCast(initPos, 0.125f, rayDir, out RaycastHit hit, rayLength, s_triggerLayerMask.Value, QueryTriggerInteraction.Collide)){
            if(hit.collider.TryGetComponent(out Trigger trig)){
                trig.Activate();
                Destroy();
                return true;
            }
        }
        return false;
    }

    void MovementUpdate(float dt){
        // # ring movement
        CalcOrientation(m_ring_space_t, m_lat_space_t, positive, worldParameters, out var actorPos, out var actorRotation);
        // TODO: spherecast
        transform.position = actorPos;
        transform.rotation = actorRotation;

        float speed = moverConfig.defaultSpeed;
        float spaceDelta = speed * dt;
        float parameterDelta_t = worldParameters.CalcDeltaT(spaceDelta);
        m_ring_space_t = Mathf.Repeat(m_ring_space_t + parameterDelta_t, 1f);

        // # Lateral deviation

        // float latSpaceDelta = inputX * dt * moverConfig.lateralSpeed;
        // float lat_space_deltaT = latSpaceDelta / worldParameters.halfWidth;
        
        // m_lat_space_t += lat_space_deltaT;
        // m_lat_space_t = Mathf.Clamp(m_lat_space_t, -1f, 1f);
    }

    void Destroy(){
        Destroy(this.gameObject);
    }

    static void CalcOrientation(float travelT, float horzDeviationT, bool positive, WorldParameters worldParams,
        out Vector3 actorPosition, out Quaternion actorRotation)
    {
        // travelT ranges from [0,1]
        float ringAngle = 720f * travelT;
        float axialAngle = 360f * travelT;

        Quaternion ringRotation = Quaternion.AngleAxis(ringAngle, (positive ? Vector3.right : Vector3.left));
        Vector3 up = ringRotation * Vector3.up;
        Vector3 fwd = ringRotation * (positive ? Vector3.forward : Vector3.back);
        Quaternion axialRotation = Quaternion.AngleAxis(axialAngle, (positive ? Vector3.back : Vector3.forward));

        actorRotation = ringRotation * axialRotation * (positive ? Quaternion.identity : Quaternion.AngleAxis(180f,Vector3.up));

        Vector3 majorOffset = up * worldParams.majorRadius;
        
        Vector3 localMinorOffset = (Vector3.up * worldParams.halfThickness) + (Vector3.right * worldParams.halfWidth * horzDeviationT);
        Vector3 minorOffset = actorRotation * localMinorOffset;

        actorPosition = majorOffset + minorOffset;
    }
}