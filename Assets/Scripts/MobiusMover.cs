using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiusMover : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip laserClip;

    public bool positive = true;
    public WorldParameters worldParameters;
    public Prefabs prefabs;
    public MoverConfig moverConfig;
    public Transform armatureRoot;

    private float m_ring_space_t;
    public float lat_space_t { get; private set; }
    
    public float lastInput { get; private set; }
    public bool completedALoopLastUpdate { get; private set; }

    public static MobiusMover Inst { get; private set; }
    public static Vector2 PlayerParametricPosition => new Vector2(Inst.m_ring_space_t, Inst.lat_space_t);

    // Start is called before the first frame update
    void Awake()
    {
        Inst = this;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)){
            var bullet = Projectile.Instantiate(prefabs.laserBlast);
            float extra = worldParameters.CalcDeltaT( 0.15f );
            bullet.Kickoff(m_ring_space_t+extra, lat_space_t);
            AudioSource.PlayClipAtPoint(laserClip, transform.position);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        MovementUpdate(dt);
    }

    void MovementUpdate(float dt){
        float inputX = 0f;
        inputX += (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ? 1f : 0;
        inputX += (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ? -1f : 0;
        this.lastInput = inputX;

        // # ring movement
        CalcOrientation(m_ring_space_t, lat_space_t, worldParameters, out var actorPos, out var actorRotation);
        transform.position = actorPos;
        transform.rotation = actorRotation;

        // # local roll
        float targetAngle = -moverConfig.maxSteerRoll * inputX;
        float z = Mathf.LerpAngle(armatureRoot.localEulerAngles.z, targetAngle, dt*8f);
        // Debug.LogFormat("z: {0}", z);
        armatureRoot.localEulerAngles = new Vector3(
            armatureRoot.localEulerAngles.x, armatureRoot.localEulerAngles.y,
            z
        );

        float speed = moverConfig.defaultSpeed;
        float spaceDelta = speed * dt;
        float parameterDelta_t = worldParameters.CalcDeltaT(spaceDelta);
        {
            float oldRingSpaceT = m_ring_space_t;
            m_ring_space_t = Mathf.Repeat(m_ring_space_t + parameterDelta_t, 1f);
            completedALoopLastUpdate = m_ring_space_t < oldRingSpaceT;
        }

        // #lateral deviation
        float latSpaceDelta = inputX * dt * moverConfig.lateralSpeed;
        float lat_space_deltaT = latSpaceDelta / worldParameters.halfWidth;
        
        lat_space_t += lat_space_deltaT;
        lat_space_t = Mathf.Clamp(lat_space_t, -1f, 1f);
    }

    void CalcOrientation(float travelT, float horzDeviationT, WorldParameters worldParams,
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
