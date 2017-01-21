using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0 , sendInterval = 0.033f)]
public class PlayerSync : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    [SyncVar]
    private Quaternion syncRot;

    [SerializeField]
    Transform myTransform;
   
    private float lerpRate;
    [SerializeField]
    private float normalLerpRate = 10f;
    [SerializeField]
    private float fasterLerpRate = 20f;
    private Vector3 lastPos;
    private float posThreshold = 0.5f;

    private Quaternion lastRot;
    private float rotThreshold = 5f;


    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLerp = false;
    private float closeDist = 0.2f;

    private void Awake()
    {
        lerpRate = normalLerpRate;
    }

    private void Update()
    {
        LerpPosition();
        LerpRotation();
    }

    // Update is called once per frame
    

	// Update is called once per frame
	void FixedUpdate () {
        TransmitPosition();
        TransmitRotation();       
	}

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, syncRot, Time.deltaTime * lerpRate);
                  
        }
    }

  

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (shouldLerp)
            {
                if (useHistoricalLerp)
                {
                    HistoricalLerp();
                }
                else
                {
                    NormalLerp();
                }
            }
        }
    }

    void NormalLerp()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    void HistoricalLerp()
    {
        if (syncPosList.Count > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if ((myTransform.position-syncPosList[0]).sqrMagnitude < (closeDist * closeDist))
            {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count > 10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }
        }
    }
    [ClientCallback]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }
    [Command]
    void CmdProvideRotationToServer(Quaternion rot)
    {
        syncRot = rot;
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && (myTransform.position - lastPos).sqrMagnitude > (posThreshold * posThreshold))
        {
            if ((myTransform.position - lastPos).sqrMagnitude < 25f)
            {
                shouldLerp = true;
                CmdProvidePositionToServer(myTransform.position);
                lastPos = myTransform.position;
            }
            else
            {
                shouldLerp = false;
            }
        }
    }

    bool shouldLerp = true;
    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer && Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
        {
            CmdProvideRotationToServer(myTransform.rotation);
        }
    }

}
