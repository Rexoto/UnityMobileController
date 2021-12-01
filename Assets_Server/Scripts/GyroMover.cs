using UnityEngine;
public class GyroMover : MonoBehaviour
{
    Quaternion initialPos = Quaternion.identity;
    Quaternion gyroPos;

    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        if (initialPos == Quaternion.identity)
            ResetOrientation();

        gyroPos = Quaternion.Inverse(initialPos) * Input.gyro.attitude;
        gyroPos = new Quaternion(gyroPos.x, gyroPos.y, -gyroPos.z, -gyroPos.w);
        transform.localRotation = gyroPos;
    }

    public void ResetOrientation()
    {
        initialPos = Input.gyro.attitude;
    }
}
