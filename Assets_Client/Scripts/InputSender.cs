using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleInputNamespace;

public class InputSender : MonoBehaviour
{
    public bool Accelerometer;
    public bool Gyroscope;
    private Gyroscope _gyro;
    public GameObject referenceObject;

    //public List<Selectable> inputs = new List<Selectable>();
    public Joystick joystick;

    LiteNetClient client;

    Quaternion origin = Quaternion.identity;

    // The initials orientation
    private int initialOrientationX;
    private int initialOrientationY;
    private int initialOrientationZ;

    // Use this for initialization
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _gyro = Input.gyro;
        _gyro.enabled = true;

        //origin = Input.gyro.attitude;
        client = LiteNetClient.instance;

        // Save the firsts values
        //initialOrientationX = (int)Input.gyro.rotationRateUnbiased.x;
        //initialOrientationY = (int)Input.gyro.rotationRateUnbiased.y;
        //initialOrientationZ = (int)-Input.gyro.rotationRateUnbiased.z;
    }

    // Update is called once per frame
    void Update()
    {
        JoystickPacket jp = new JoystickPacket
        {
            x = joystick.xAxis.value,
            y = joystick.yAxis.value
        };
        client.SendPacketToServer(jp);
        if (Accelerometer)
        {
            AccelerometerPacket ap = new AccelerometerPacket
            {
                x = Input.acceleration.x
            };
            client.SendPacketToServer(ap);
        }

        if (Gyroscope)
        {
            GyroscopePacket gp = new GyroscopePacket
            {
                x = referenceObject.transform.rotation.x,
                y = referenceObject.transform.rotation.y,
                z = referenceObject.transform.rotation.z,
                w = referenceObject.transform.rotation.w
            };
            client.SendPacketToServer(gp);
        }

        //if (Gyroscope)
        //{
        //x = 0.98f * (x + Input.gyro.attitude.eulerAngles.x * 0.01f) + 0.02f * Input.acceleration.x;
        //y = 0.98f * (y + Input.gyro.attitude.eulerAngles.y * 0.01f) + 0.02f * Input.acceleration.y;
        //z = 0.98f * (z + Input.gyro.attitude.eulerAngles.z * 0.01f) + 0.02f * Input.acceleration.z;

        //ahrs.Update(Input.gyro.rotationRateUnbiased.z, Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.x, Input.acceleration.z, Input.acceleration.y, Input.acceleration.x);

        //Vector3 rotation = new Vector3(initialOrientationX - Input.gyro.rotationRateUnbiased.x, initialOrientationY -Input.gyro.rotationRateUnbiased.y, initialOrientationZ + Input.gyro.rotationRateUnbiased.z);
        //Vector3 acceleration = rotation = new Vector3(initialOrientationX - Input.acceleration.x, initialOrientationY - Input.acceleration.y, initialOrientationZ + Input.acceleration.z);
        //rotation = Vector3.Slerp(rotation, acceleration, 0.5f);
        //CorrectValueByThreshold(ref rotation);
        //Quaternion rotation = Input.gyro.attitude * Quaternion.Inverse(origin);
        //Quaternion rotation = Quaternion.LookRotation(Input.gyro.gravity, new Vector3(Mathf.Cos(Input.compass.magneticHeading), 0f, Mathf.Sin(Input.compass.magneticHeading))) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);

        //if (Quaternion.Angle(rotation, Quaternion.Inverse(origin) * Input.gyro.attitude) > 1)
        //{
        //    rotation = Quaternion.Inverse(origin) * Input.gyro.attitude;
        //}

        //    GyroscopePacket gp = new GyroscopePacket
        //    {
        //        x = rotation.x,
        //        y = rotation.y,
        //        z = rotation.z,
        //        //w = rotation.w
        //    };
        //    client.SendPacketToServer(gp);
        //}
    }

    private void CorrectValueByThreshold(ref Vector3 a_vDeltaRotation, float a_fXThreshold = 1e-1f, float a_fYThreshold = 1e-2f, float a_fZThreshold = 1e-1f)
    {

        //  Debug.Log(a_quatDelta.x + " " + a_quatDelta.y + " " + a_quatDelta.z );

        a_vDeltaRotation.x = Mathf.Abs(a_vDeltaRotation.x) < a_fXThreshold ? 0.0f : a_vDeltaRotation.x + a_fXThreshold;
        a_vDeltaRotation.y = Mathf.Abs(a_vDeltaRotation.y) < a_fYThreshold ? 0.0f : a_vDeltaRotation.y + a_fYThreshold;
        a_vDeltaRotation.z = Mathf.Abs(a_vDeltaRotation.z) < a_fZThreshold ? 0.0f : a_vDeltaRotation.z + a_fZThreshold;
    }

    public void SendButton(string Name)
    {
        ButtonPacket bp = new ButtonPacket
        {
            name = Name
        };
        client.SendPacketToServer(bp);
    }

    [Serializable]
    public class Inputs
    {
        public Inputs input;
    }

    [Serializable]
    public class NSlider : Inputs
    {
        public Slider button;
    }
}

