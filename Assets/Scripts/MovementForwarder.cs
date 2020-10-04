using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementForwarder : MonoBehaviour {

    public MobiusMover mobiusMover;
    public MirrorMover receiver;

    private List<(float, int)> m_inputBuffer;
    private float m_initialLatT;

    static MovementForwarder(){

    }

    void Start(){
        m_inputBuffer = new List<(float,int)>();
    }

    void FixedUpdate(){
        var curInput = mobiusMover.lastInput;
        
        if(m_inputBuffer.Count == 0){
            // Firsst input
            m_inputBuffer.Add((curInput, 1));
            m_initialLatT = mobiusMover.lat_space_t;
        } else {
            int lastItemIndex = m_inputBuffer.Count-1;
            var lastBufferInputTuple = m_inputBuffer[lastItemIndex];
            if(lastBufferInputTuple.Item1 == curInput){
                lastBufferInputTuple = (lastBufferInputTuple.Item1, lastBufferInputTuple.Item2+1);
                m_inputBuffer[lastItemIndex] = lastBufferInputTuple;
            } else {
                m_inputBuffer.Add((curInput, 1));
            }
        }

        if(mobiusMover.completedALoopLastUpdate){
            var kickoff = new MirrorMover.KickoffPacket() {
                inputStreamRef = m_inputBuffer,
                latSpaceT = m_initialLatT
            };
            receiver.Kickoff(kickoff);
        }
    }

}