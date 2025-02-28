﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingScript : MonoBehaviour
{
    public WheelScript[] wheels; //tu będą wszystkie nasze koła

    public float torque = 200; //moment obrotowy
    public float maxSteerAngle = 30; //maxymalny kąt wychylenia
    public float maxBrakeTorque = 500; //moment hamowania
    public float maxSpeed = 200; //max predkość
    public Rigidbody rb;
    public float currentSpeed; //aktualna prędkość

    public AudioSource engineSound;

    float rpm; //obroty na minutę
    int currentGear = 1; //aktualny bieg
    float currentGearPerc; //aktualny bieg wyrażony w procentach

    public GameObject backLights;
    public GameObject cameraTarget;



    public void Drive(float accel, float brake, float steer)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        if (brake != 0)
        {
            backLights.SetActive(true);
        }
        else
        {
            backLights.SetActive(false);
        }

        float thrustTorque = 0;

        currentSpeed = rb.velocity.magnitude * 3;
        if (currentSpeed < maxSpeed)
        {
            thrustTorque = accel * torque;
        }

        foreach (WheelScript wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = thrustTorque;
            if (wheel.frontWheel)
            {
                wheel.wheelCollider.steerAngle = steer;
            }
            else
            {
                wheel.wheelCollider.brakeTorque = brake;
            }

            Quaternion quat;
            Vector3 position;
            wheel.wheelCollider.GetWorldPose(out position, out quat);
            wheel.wheel.transform.position = position;
            wheel.wheel.transform.rotation = quat;
        }
    }

    public void EngineSound()
    {
        float gears = 5;
        float gearPerc = (1f / gears);
        currentSpeed = rb.velocity.magnitude * 3;
        float targetGearFactor = Mathf.InverseLerp(gearPerc * currentGear, gearPerc * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearsFactor = currentGear / (float)gears;
        rpm = Mathf.Lerp(gearsFactor, 1, currentGearPerc);
        float speedPerc = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGear = (1 / (float)gears) * (currentGear + 1);
        float downGear = (1 / (float)gears) * currentGear;

        if (currentGear > 0 && speedPerc < downGear) currentGear--;
        if (currentGear > upperGear && (currentGear < (gears - 1))) currentGear--;

        float pitch = Mathf.Lerp(1, 6, rpm);
        engineSound.pitch = Mathf.Min(6, pitch) * 0.25f;
    }
}
