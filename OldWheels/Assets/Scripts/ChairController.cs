using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}

public class ChairController : MonoBehaviour {

    public List<AxleInfo> axleInfos; //info about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    float motor, steering;
    float motorL, motorR;

    float accelerationAngle = 0;
    bool wheelForwardInteracting = false, wheelBackwardInteracting = false;
    public float _ACCELMINI = 0, _ACCELMAXI = 0, _DURATIONSHORT = 0.2f, _DURATIONLONG = 0.8f;
    float t = 0, minimum = 0, maximum = 4, duration = 0.2f;
    bool decreased = false;

    private Rigidbody rb;

    void Start()
    {
        minimum = _ACCELMINI;
        maximum = _ACCELMAXI;
        duration = _DURATIONSHORT;
        rb = GetComponent<Rigidbody>();
    }

	// finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
         
        Transform visualWheel = collider.transform.GetChild(0);
         
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
         
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }


    void updateAccelerationCoeff()
    {
        t += Time.deltaTime / duration;
        accelerationAngle = Mathf.Lerp(minimum, maximum, t);
        if (accelerationAngle >= _ACCELMAXI)
        {//Reach Max Acceleration -> reduce acceleration || simulates wheel interaction
            t = 0;
            minimum = _ACCELMAXI;
            maximum = _ACCELMINI;
            duration = _DURATIONLONG;
            decreased = true;
            //rb.AddForce(0f, 0f, 5000f);
           // Debug.Log("lol");
        }
        if (accelerationAngle <= _ACCELMINI && decreased)
        {
            resetAcceleration();
        }
    }
    private void resetAcceleration()
    {//Reset Acceleration from begining
        rb.drag = 0.5f;
        t = 0;
        decreased = false;
        minimum = _ACCELMINI;
        maximum = _ACCELMAXI;
        duration = _DURATIONSHORT;
        accelerationAngle = 0;
        wheelForwardInteracting = wheelBackwardInteracting = false;
    }
    public void FixedUpdate()
    {
        //motor = maxMotorTorque * -Input.GetAxis("Vertical");
        //steering = maxSteeringAngle * -Input.GetAxis("Horizontal");
        motor = steering = 0;
        rb.drag = 3;
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.E))
        {
            if (!wheelForwardInteracting)
            {
                t = 0; minimum = _ACCELMINI; maximum = _ACCELMAXI; duration = _DURATIONSHORT;
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.motor)
                    {
                      //  axleInfo.leftWheel.motorTorque = 0;
                      //  axleInfo.rightWheel.motorTorque = 0;
                      axleInfo.leftWheel.steerAngle = 0;
                      axleInfo.rightWheel.steerAngle = 0;
                    }
                }
                decreased = false;
                wheelForwardInteracting = true;
                wheelBackwardInteracting = false;
            }
          //  motor = maxMotorTorque * -rotationAngle; //Avance
            steering = maxSteeringAngle * 0;
        }
        else if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.D))
        {
            if (!wheelBackwardInteracting)
            {
                t = 0; minimum = _ACCELMINI; maximum = _ACCELMAXI; duration = _DURATIONSHORT;
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.motor)
                    {
                        axleInfo.leftWheel.motorTorque = 0;
                        axleInfo.rightWheel.motorTorque = 0;
                    }
                }
                decreased = false;
                wheelBackwardInteracting = true;
                wheelForwardInteracting = false;
            }
           // motor = maxMotorTorque * rotationAngle; //Recule
            steering = maxSteeringAngle * 0;
        }
       /* else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            gameObject.transform.eulerAngles += new Vector3(0, 2f, 0);
        }
        else if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Q))
        {
            gameObject.transform.eulerAngles -= new Vector3(0, 2f, 0);
        }*/
        else if (Input.GetKey(KeyCode.A))
        {
            steering = maxSteeringAngle * 1;
            motor = maxMotorTorque * -.75f; //Tourne Gauche
            resetAcceleration();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            steering = maxSteeringAngle * -1; //Tourne Droite
            motor = maxMotorTorque * -.75f;
            resetAcceleration();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            steering = maxSteeringAngle * 1;
            motor = maxMotorTorque * .75f; //Tourne Gauche
            resetAcceleration();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steering = maxSteeringAngle * -1; //Tourne Droite
            motor = maxMotorTorque * .75f;
            resetAcceleration();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            motor = 0;
            steering = 0;
            rb.drag = 1;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.drag = 0.1f;
        }

        if(wheelForwardInteracting || wheelBackwardInteracting) {
            updateAccelerationCoeff();
        }
        if (wheelForwardInteracting)
        {
            rb.drag = 0.1f;
            motor = maxMotorTorque * -1;// accelerationAngle; //Avance
        }
        if (wheelBackwardInteracting)
        {
            rb.drag = 0.1f;
            motor = maxMotorTorque * 1;// accelerationAngle; //Recule
        }

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
                Debug.Log((int)axleInfo.leftWheel.steerAngle + " - " + (int)axleInfo.rightWheel.steerAngle + " | " + (int)axleInfo.leftWheel.motorTorque + " - " + (int)axleInfo.rightWheel.motorTorque);
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

       // Debug.Log(rb.velocity);
    }
}
