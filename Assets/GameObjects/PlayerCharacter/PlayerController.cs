using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region INPUTS
    struct Inputs
    {
        public bool RollLeft;
        public bool RollRight;
        public bool YawLeft;
        public bool YawRight;
        public bool PitchUp;
        public bool PitchDown;
        public bool MoveForward;
        public bool MoveBackward;
        public bool MoveLeft;
        public bool MoveRight;
        public bool MoveUp;
        public bool MoveDown;
    }

    private Inputs m_inputs;

    private void ReadInput()
    {
        if (Input.GetKey(KeyCode.U))
        {
            m_inputs.RollLeft = true;
        }

        if (Input.GetKey(KeyCode.O))
        {
            m_inputs.RollRight = true;
        }

        if (Input.GetKey(KeyCode.I))
        {
            m_inputs.PitchDown = true;
        }

        if (Input.GetKey(KeyCode.K))
        {
            m_inputs.PitchUp = true;
        }

        if (Input.GetKey(KeyCode.J))
        {
            m_inputs.YawLeft = true;
        }

        if (Input.GetKey(KeyCode.L))
        {
            m_inputs.YawRight = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            m_inputs.MoveForward = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            m_inputs.MoveBackward = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            m_inputs.MoveLeft = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            m_inputs.MoveRight = true;
        }

        if (Input.GetKey(KeyCode.E))
        {
            m_inputs.MoveUp = true;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            m_inputs.MoveDown = true;
        }
    }
    #endregion

    #region SOUNDS

    [SerializeField]
    private AudioClip m_impactClip;

    [SerializeField]
    [Range(0,1)]
    private float m_impactClipVolumeMin;

    [SerializeField]
    [Range(0,1)]
    private float m_impactClipVolumeMax;

    [SerializeField]
    [Range(0,10)]
    private float m_impactNoiseMult;

    [SerializeField]
    private AudioSource m_propulsionAudioSource;

    #endregion

    private Rigidbody m_rigidbody;

    private Vector3 m_deltaVelocity = Vector3.zero;
    private float m_deltaRollSpeed = 0;
    private float m_deltaPitchSpeed = 0;
    private float m_deltaYawSpeed = 0;

    [SerializeField]
    private float m_moveAccelerationMultiplier = 1.0f;

    [SerializeField]
    private float m_forwardAccelerationMultiplier = 5.0f;

    [SerializeField]
    private float m_rotateAccelerationMultiplier = 1.0f;

    [SerializeField]
    private float m_maxSpeed = 10.0f;

    [SerializeField]
    private float m_minSpeed = 0.05f;

    [SerializeField]
    private float m_minRotSpeed = 0.05f;

    [SerializeField]
    private float m_maxRollSpeed = 2.0f;

    [SerializeField]
    private float m_maxYawSpeed = 2.0f;

    [SerializeField]
    private float m_maxPitchSpeed = 2.0f;

    private void Awake()
    {
        m_inputs = new Inputs();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        Move();

        // Clear Input
        m_inputs = new Inputs();
    }

    private void LateUpdate()
    {
        // Update propulsion noise volume
        m_propulsionAudioSource.volume = Mathf.Lerp(0.2f, 1.0f, m_rigidbody.velocity.sqrMagnitude * 0.25f);
    }

    private void Move()
    {
        // Rotation velocity changes
        if (m_inputs.RollLeft)
        {
            m_deltaRollSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.RollRight)
        {
            m_deltaRollSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.PitchUp)
        {
            m_deltaPitchSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.PitchDown)
        {
            m_deltaPitchSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.YawLeft)
        {
            m_deltaYawSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.YawRight)
        {
            m_deltaYawSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
        }

        // Movement velocity changes
        if (m_inputs.MoveForward)
        {
            m_deltaVelocity += transform.forward * m_moveAccelerationMultiplier * m_forwardAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.MoveBackward)
        {
            m_deltaVelocity += -transform.forward * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.MoveLeft)
        {
            m_deltaVelocity += -transform.right * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.MoveRight)
        {
            m_deltaVelocity += transform.right * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.MoveUp)
        {
            m_deltaVelocity += transform.up * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
        }

        if (m_inputs.MoveDown)
        {
            m_deltaVelocity += -transform.up * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
        }

        // Clamping velocities
        m_deltaVelocity = Vector3.ClampMagnitude(m_deltaVelocity, m_maxSpeed);
        m_deltaRollSpeed = Mathf.Clamp(m_deltaRollSpeed, -m_maxRollSpeed, m_maxRollSpeed);
        m_deltaYawSpeed = Mathf.Clamp(m_deltaYawSpeed, -m_maxYawSpeed, m_maxYawSpeed);
        m_deltaPitchSpeed = Mathf.Clamp(m_deltaPitchSpeed, -m_maxPitchSpeed, m_maxPitchSpeed);

        // Move
        m_rigidbody.AddForce(m_deltaVelocity, ForceMode.Acceleration);

        // Roll
        m_rigidbody.AddTorque(transform.forward * m_deltaRollSpeed, ForceMode.Acceleration);

        // Pitch
        m_rigidbody.AddTorque(transform.right * m_deltaPitchSpeed, ForceMode.Acceleration);

        // Yaw
        m_rigidbody.AddTorque(transform.up * m_deltaYawSpeed, ForceMode.Acceleration);

        // Reset so we don't keep adding velocity to rigidbody
        m_deltaVelocity = Vector3.zero;
        m_deltaRollSpeed = 0;
        m_deltaYawSpeed = 0;
        m_deltaPitchSpeed = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play impact sound
        float volume = Random.Range(m_impactClipVolumeMin, m_impactClipVolumeMax) * collision.relativeVelocity.magnitude * m_impactNoiseMult;
        AudioSource.PlayClipAtPoint(m_impactClip, collision.GetContact(0).point, volume);
    }
}
