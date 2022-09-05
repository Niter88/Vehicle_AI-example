using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAI : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    float rotationSpeed = 35f; //in degrees per second

    [SerializeField]
    float walkSpeed = 5f;


    [SerializeField]
    float leftSensorD = 0f;

    [SerializeField]
    float rightSensorD = 0f;

    [SerializeField]
    float midleSensorD = 0f;

    const float maxVisionDist = 3f;

    const float maxProxDist = 3f;

    RaycastHit hit;

    Vector3 leftEDirection = new Vector3(-1f, 0f, 1f);
    Vector3 rightEDirection = new Vector3(1f, 0f, 1f);

    float lastMovedTimeStamp = 0f;

    [Header("GFX")]
    [SerializeField]
    MeshRenderer[] sensorEyesMR;

    [SerializeField]
    Material greenLight;

    [SerializeField]
    Material redLight;

    bool[] sensorFree = { true, true, true };
    bool[] calcSensorFree = { true, true, true };


    // Update is called once per frame
    void Update()
    {
        Wander();
        
    }

    private void Wander()
    {
        LookAround();

        if (leftSensorD < maxProxDist ||
            midleSensorD < maxProxDist ||
            rightSensorD < maxProxDist)
        {
            FaceOutwards();
        }
        else
        {
           
            WalkForward();

        }
    }

    private void FaceOutwards()
    {
        if ((Time.realtimeSinceStartup - lastMovedTimeStamp) > 20f)
        {
            StuckBeyondRepair();
        }

        if ((Time.realtimeSinceStartup - lastMovedTimeStamp) > 3f)
        {
            //haven't been able to move due to sensors logic, unstuck it
            transform.Rotate(Vector3.up, 180f);
            return;
        }

        if (midleSensorD < leftSensorD &&
            midleSensorD < rightSensorD)
        {
            //midle is the nearest
            if (leftSensorD < rightSensorD)
            {
                //left sensor is the nearest
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else
            {
                //right sensor is the nearest
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
        } 
        else if (leftSensorD < rightSensorD)
        {
            //left sensor is the nearest
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else
        {
            //right sensor is the nearest
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }

    private void WalkForward()
    {
        transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        lastMovedTimeStamp = Time.realtimeSinceStartup;
    }

    private void LookAround()
    {
        leftSensorD = EyeLook(leftEDirection);
        midleSensorD = EyeLook(Vector3.forward);
        rightSensorD = EyeLook(rightEDirection);

        PresentObstructionState();
    }

    private float EyeLook(Vector3 direction)
    {
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, maxVisionDist, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
            return hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * maxVisionDist, Color.green);
            return maxVisionDist;
        }
    }

    private void PresentObstructionState()
    {
        calcSensorFree[0] = (rightSensorD >= maxProxDist);
        calcSensorFree[1] = (midleSensorD >= maxProxDist);
        calcSensorFree[2] = (leftSensorD >= maxProxDist);

        for (int i = 0; i < 3; i++)
        {
            if (sensorFree[i] != calcSensorFree[i])
            {
                sensorFree[i] = calcSensorFree[i];
                sensorEyesMR[i].material = GetEyeMaterial(sensorFree[i]);
            }
        }
    }

    private Material GetEyeMaterial(bool isFree)
    {
        if (isFree)
            return greenLight;
        else
            return redLight;
    }

    private void StuckBeyondRepair()
    {
        Debug.Log(transform.name + " Stuck Beyond Repair. Replacing...");
        StaticEvents.CallSpawnNewEntity();
        Destroy(this.gameObject);
    }
}
