using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="World Parameters", order = 0, fileName="World Params")]
public class WorldParameters : ScriptableObject
{
    public float majorRadius = 10f;
    public float halfThickness = 0.25f;
    public float halfWidth = 1f;

    public float defaultSpeed = 5f;

    public float CalcDeltaT(float worldSpaceDelta){
        float wholeTrackLength = 4 * Mathf.PI * (halfThickness + majorRadius);
        return worldSpaceDelta / wholeTrackLength;
    }

    public void CalcOrientation(float travelT, float horzDeviationT, bool positive,
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

        Vector3 majorOffset = up * this.majorRadius;
        
        Vector3 localMinorOffset = (Vector3.up * this.halfThickness) + (Vector3.right * this.halfWidth * horzDeviationT);
        Vector3 minorOffset = actorRotation * localMinorOffset;

        actorPosition = majorOffset + minorOffset;
    }

    public void CalcCenteredOrientation(float travelT, bool positive,
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

        Vector3 majorOffset = up * this.majorRadius;

        actorPosition = majorOffset;
    }

    public (float,float) GetParametricRingRange(float ringPosT, float minWorldOffset, float maxWorldOffset){
        float minDeltaT = CalcDeltaT(minWorldOffset);
        float maxDeltaT = CalcDeltaT(maxWorldOffset);

        return (ringPosT + minDeltaT, ringPosT + maxDeltaT);
    }


}
