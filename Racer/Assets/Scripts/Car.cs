using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    public float topSpeed = 800;
    public float maxReverseSpeed = -150;
    public float maxTurnAngle = 50;
    public float maxTorque = 10000;
    public float decelerationTorque = 10000;
    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    public float spoilerRatio = 0.1f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformBL;
    public Transform wheelTransformBR;
    private Rigidbody body;

    //Break Lights
    public GameObject breakLights;
    //Head Lights
    public GameObject leftHeadLight;
    public GameObject rightHeadLight;

    public float maxBreakTorque = 200;
    private bool b_applyHandBreak = false;

    public float handBreakForwardSlip = 0.04f;
    public float handBreakSideWaysSlip = 0.08f;

    void Start()
    {
        //lower center of mass for roll-over resistance
        body = GetComponent<Rigidbody>();
        body.centerOfMass += centerOfMassAdjustment;
    }


    void SetSlipValue(float forward, float sideways)
    {
        // change the stiffness values of wheel frictions and apply it
        WheelFrictionCurve tempStruct = wheelBR.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBR.forwardFriction = tempStruct;

        tempStruct = wheelBR.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBR.sidewaysFriction = tempStruct;

        tempStruct = wheelBL.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBL.forwardFriction = tempStruct;

        tempStruct = wheelBL.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBL.sidewaysFriction = tempStruct;
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            leftHeadLight.SetActive(!leftHeadLight.activeInHierarchy);
            rightHeadLight.SetActive(!rightHeadLight.activeInHierarchy);
        }


        //Break Lights
        if ((Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow)) || (Input.GetButton("Jump")))
        {
            breakLights.SetActive(true);
        }
        else
        {
            breakLights.SetActive(false);
        }


        //Apply hand break if we press space key
        if (Input.GetButton("Jump"))
        {
            b_applyHandBreak = true;
            wheelFL.brakeTorque = maxBreakTorque;
            wheelFR.brakeTorque = maxBreakTorque;
            if (GetComponent<Rigidbody>().velocity.magnitude > 1)
            {
                SetSlipValue(handBreakForwardSlip, handBreakSideWaysSlip);
            }
            else
            {

                SetSlipValue(1f, 1f);
            }

        }
        else
        {
            b_applyHandBreak = false;
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
            SetSlipValue(1f, 1f);
        }

		//calculate max speed in KM/H (optimized calc)
		float currentSpeed = wheelBL.radius*wheelBL.rpm*Mathf.PI*0.12f;
		if(currentSpeed < topSpeed && currentSpeed > maxReverseSpeed)
		{
			//rear wheel drive.
			wheelBL.motorTorque = Input.GetAxis("Vertical") * maxTorque;
			wheelBR.motorTorque = Input.GetAxis("Vertical") * maxTorque;
		}
		else
		{
			//can't go faster, already at top speed that engine produces.
			wheelBL.motorTorque = 0;
			wheelBR.motorTorque = 0;
		}
		
		//Spoilers add down pressure based on the car’s speed. (Upside-down lift)
		Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
		body.AddForce(-transform.up * (localVelocity.z * spoilerRatio),ForceMode.Impulse);
		
		//front wheel steering
		wheelFL.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;
		wheelFR.steerAngle = Input.GetAxis("Horizontal")* maxTurnAngle;
		
		//apply deceleration when not pressing the gas or when breaking in either direction.
		if(!b_applyHandBreak && ((Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0)||(Input.GetAxis("Vertical") >= 0.5f && localVelocity.z < 0)))
		{
			wheelBL.brakeTorque = decelerationTorque + maxTorque;
			wheelBR.brakeTorque = decelerationTorque + maxTorque;
		}
		else if(!b_applyHandBreak && Input.GetAxis("Vertical") == 0)
		{
			wheelBL.brakeTorque = decelerationTorque;
			wheelBR.brakeTorque = decelerationTorque;
		}
		else
		{
			wheelBL.brakeTorque = 0;
			wheelBR.brakeTorque = 0;
		}
	}
	
	void Update()
	{
		//rotate the wheels based on RPM
		float rotationThisFrame = 360*Time.deltaTime;
		wheelTransformFL.Rotate(wheelFL.rpm / rotationThisFrame,0, 0);
		wheelTransformFR.Rotate(wheelFR.rpm / rotationThisFrame,0, 0);
		wheelTransformBL.Rotate(wheelBL.rpm / rotationThisFrame,0, 0);
		wheelTransformBR.Rotate(wheelBR.rpm / rotationThisFrame,0,0);
		
		//turn the wheels according to steering. But make sure you take into account the rotation being applied above.
		wheelTransformFL.localEulerAngles = new Vector3(wheelTransformFL.localEulerAngles.x, wheelFL.steerAngle  - wheelTransformFL.localEulerAngles.z, wheelTransformFL.localEulerAngles.z);
		wheelTransformFR.localEulerAngles = new Vector3(wheelTransformFR.localEulerAngles.x, wheelFR.steerAngle  - wheelTransformFR.localEulerAngles.z, wheelTransformFR.localEulerAngles.z);
	
		UpdateWheelPositions();
	}
	
	void UpdateWheelPositions()
	{
		//move wheels based on their suspension.
		WheelHit contact = new WheelHit();
		if(wheelFL.GetGroundHit(out contact))
		{
			Vector3 temp = wheelFL.transform.position;
			temp.y = (contact.point + (wheelFL.transform.up  * wheelFL.radius)).y;
			wheelTransformFL.position = temp;
		}
		if(wheelFR.GetGroundHit(out contact))
		{
			Vector3 temp = wheelFR.transform.position;
			temp.y = (contact.point + (wheelFR.transform.up * wheelFR.radius)).y;
            wheelTransformFR.position = temp;
		}
		if(wheelBL.GetGroundHit(out contact))
		{
			Vector3 temp = wheelBL.transform.position;
			temp.y = (contact.point + (wheelBL.transform.up * wheelBL.radius)).y;
            wheelTransformBL.position = temp;
		}
		if(wheelBR.GetGroundHit(out contact))
		{
			Vector3 temp = wheelBR.transform.position;
			temp.y = (contact.point + (wheelBR.transform.up  * wheelBR.radius)).y;
            wheelTransformBR.position = temp;
		}
	}
}
