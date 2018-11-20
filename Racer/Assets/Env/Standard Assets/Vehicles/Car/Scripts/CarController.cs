using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CarController : MonoBehaviour
    {
               
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }


        public WheelCollider wheelFL;
        public WheelCollider wheelFR;
        public WheelCollider wheelBL;
        public WheelCollider wheelBR;
        public float maxBreakTorque = 2000;
        private bool b_applyHandBreak = false;
        public float handBreakForwardSlip = 0.04f;
        public float handBreakSideWaysSlip = 0.08f;
        //Break Lights
        public GameObject breakLights;

        public Texture2D speedOmeter;
        public Texture2D needle;


        private Vector3 startPos;
        private Quaternion startRot;
        private Vector3 newPos;
        private Quaternion newRot;


        // AI1
        private Vector3 startPosAI1;
        private Quaternion startRotAI1;
        private Vector3 newPosAI1;
        private Quaternion newRotAI1;
        // AI2
        private Vector3 startPosAI2;
        private Quaternion startRotAI2;
        private Vector3 newPosAI2;
        private Quaternion newRotAI2;
        // AI3
        private Vector3 startPosAI3;
        private Quaternion startRotAI3;
        private Vector3 newPosAI3;
        private Quaternion newRotAI3;

        // StartCamera
        private Vector3 startCameraPos;
        private Quaternion startCameraRot;
        private Vector3 startCameraNewPos;
        private Quaternion startCameraNewRot;


        // Use this for initializations
        private void Start()
        {

            if (this.gameObject.name == "CamaroPlayer")
            {
                startPos = this.transform.position;
                startRot = this.transform.rotation;
                newPos = startPos;
                newRot = startRot;

                m_MaximumSteerAngle = 25;
                m_FullTorqueOverAllWheels = 3200f;
                m_ReverseTorque = 3000f;
            }

            if (this.gameObject.name == "CamaroAI01")
            {
                startPosAI1 = this.transform.position;
                startRotAI1 = this.transform.rotation;
                newPosAI1 = startPosAI1;
                newRotAI1 = startRotAI1;
                m_FullTorqueOverAllWheels = 3200f;
            }

            if (this.gameObject.name == "CamaroAI02")
            {

                startPosAI2 = this.transform.position;
                startRotAI2 = this.transform.rotation;
                newPosAI2 = startPosAI2;
                newRotAI2 = startRotAI2;
                m_FullTorqueOverAllWheels = 3200f;
            }

            if (this.gameObject.name == "CamaroAI03")
            {

                startPosAI3 = this.transform.position;
                startRotAI3 = this.transform.rotation;
                newPosAI3 = startPosAI3;
                newRotAI3 = startRotAI3;
                m_FullTorqueOverAllWheels = 3200f;
            }

            if (this.gameObject.name == "StartCamera")
            {

                startCameraPos = this.transform.position;
                startCameraRot = this.transform.rotation;
                startCameraNewPos = startCameraPos;
                startCameraNewRot = startCameraRot;
                
            }



            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
        }


        void OnGUI()
        {
            if (GameManager.cl_GameManager.b_GameIsPaused == false)
            {
                GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 240, 300, 300), speedOmeter);
                float speedFactor = CurrentSpeed / m_Topspeed;
                float rotationAngle = Mathf.Lerp(-45, 210, Mathf.Abs(speedFactor));
                GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 150, Screen.height - 95));
                GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 245, 300, 300), needle);
            }


        }

        //Saving New Position and Rotation for Player based on collisions with wayPoints
        public void UpdatePositionAndRotation()
        {
            newPos = this.transform.position;
            newRot = this.transform.rotation;
        }
        //Respawn player car at last position
        public void RespawnPlayer()
        {
            this.transform.position = newPos;
            this.transform.rotation = newRot;

        }

        //Update Position for CamaroAI1
        public void UpdatePosAndRotAI1()
        {
            newPosAI1 = this.transform.position;
            newRotAI1 = this.transform.rotation;
            //Debug.Log("THIS IS REAL PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance);
            //Debug.Log("THIS IS TEMP PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI1);
        }
        //Respawn CamaroAI01
        public void RespawnAI1()
        {
            this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance = this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI1;
            this.transform.position = newPosAI1;
            this.transform.rotation = newRotAI1;
            
        }

        //Update Position for CamaroAI2
        public void UpdatePosAndRotAI2()
        {
            newPosAI2 = this.transform.position;
            newRotAI2 = this.transform.rotation;
            //Debug.Log("THIS IS REAL PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance);
            //Debug.Log("THIS IS TEMP PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI2);
        }

        //Respawn CamaroAI02
        public void RespawnAI2()
        {
            this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance = this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI2;
            this.transform.position = newPosAI2;
            this.transform.rotation = newRotAI2;
            
        }

        //Update Position for CamaroAI3
        public void UpdatePosAndRotAI3()
        {
            newPosAI3 = this.transform.position;
            newRotAI3 = this.transform.rotation;
           // Debug.Log("THIS IS REAL PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance);
           // Debug.Log("THIS IS TEMP PROGRESS DISTANCE " + this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI3);
        }

        //Respawn CamaroAI03
        public void RespawnAI3()
        {
            this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance = this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceAI3;
            this.transform.position = newPosAI3;
            this.transform.rotation = newRotAI3;
            
        }


        //Update Position for StartCamera
        public void UpdatePosAndRotStartCamera()
        {
            startCameraNewPos = this.transform.position;
            startCameraNewRot = this.transform.rotation;
            Debug.Log("THIS IS REAL PROGRESS DISTANCE START CAMERA " + this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance);
            Debug.Log("THIS IS TEMP PROGRESS DISTANCE START CAMERA" + this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceStartCamera);
        }

        //Respawn StartCamera
        public void RespawnStartCamera()
        {
            Debug.Log("Function Works!!!");
            this.gameObject.GetComponent<WaypointProgressTracker>().progressDistance = this.gameObject.GetComponent<WaypointProgressTracker>().f_lastProgressDistanceStartCamera;
            this.transform.position = startCameraNewPos;
            this.transform.rotation = startCameraNewRot;

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

        private void Update()
        {


        //Checking magnitude of velocity for AI1 and respawn AI1 if it is less than 2
        if (this.gameObject.name == "CamaroAI01" && this.m_Rigidbody.velocity.magnitude < 1f)
        {
          RespawnAI1();
        }
        //Checking magnitude of velocity for AI2 and respawn AI1 if it is less than 2
        if (this.gameObject.name == "CamaroAI02" && this.m_Rigidbody.velocity.magnitude < 1f)
        {
          RespawnAI2();
        }
        //Checking magnitude of velocity for AI3 and respawn AI1 if it is less than 2
        if (this.gameObject.name == "CamaroAI03" && this.m_Rigidbody.velocity.magnitude < 1f)
        {
             RespawnAI3();
        }
        // Here we can respawn player's car
        if (Input.GetKeyDown(KeyCode.P) && this.gameObject.name == "CamaroPlayer" && this.m_Rigidbody.velocity.magnitude < 2f)
        {
           RespawnPlayer();
        }

        }


        private void FixedUpdate()
        {
            //Changing SteerAngle for player's car based on velocity
            if (this.gameObject.name == "CamaroPlayer" && m_Rigidbody.velocity.magnitude > 20f)
            {
                m_MaximumSteerAngle = 17f;
                //Debug.Log(m_MaximumSteerAngle);
                //Debug.Log(m_Rigidbody.velocity.magnitude); 
            }
            //Changing SteerAngle for player's car based on velocity, here car can turns faster
            if (this.gameObject.name == "CamaroPlayer" && m_Rigidbody.velocity.magnitude < 10f)
            {
                //Debug.Log(m_MaximumSteerAngle);
                m_MaximumSteerAngle = 25f;
            }

            
                //Break Lights
                if (Input.GetKey(KeyCode.S) && this.gameObject.name == "CamaroPlayer" || Input.GetKey(KeyCode.DownArrow) && this.gameObject.name == "CamaroPlayer" || Input.GetButton("Jump") && this.gameObject.name == "CamaroPlayer")
            {
                breakLights.SetActive(true);

                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelEffects[i].EmitTyreSmoke();
                    }

                    }
                    else
                    {
                        breakLights.SetActive(false);
                    }

            //Apply hand break if we press space key
            if (Input.GetButton("Jump") && this.gameObject.name == "CamaroPlayer")
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
        }


        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
            float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake*m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();
        }

        //Calculating car speed depending on speed type
        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }


        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;

            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].StopAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
