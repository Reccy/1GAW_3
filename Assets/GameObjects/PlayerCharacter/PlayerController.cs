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
        public bool StopMovement;
        public bool StopRotation;
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_inputs.StopMovement = true;
        }

        if (Input.GetKey(KeyCode.RightShift))
        {
            m_inputs.StopRotation = true;
        }
    }
    #endregion

    private Vector3 m_velocity = Vector3.zero;
    private float m_rollSpeed = 0;
    private float m_pitchSpeed = 0;
    private float m_yawSpeed = 0;

    [SerializeField]
    private float m_moveAccelerationMultiplier = 1.0f;

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

    private void Move()
    {
        // Rotation velocity changes
        if (m_inputs.StopRotation)
        {
            m_pitchSpeed += -m_pitchSpeed * m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            
            if (Mathf.Abs(m_pitchSpeed) < m_minRotSpeed)
            {
                m_pitchSpeed = 0;
            }

            m_yawSpeed += -m_yawSpeed * m_rotateAccelerationMultiplier * Time.fixedDeltaTime;

            if (Mathf.Abs(m_yawSpeed) < m_minRotSpeed)
            {
                m_yawSpeed = 0;
            }

            m_rollSpeed += -m_rollSpeed * m_rotateAccelerationMultiplier * Time.fixedDeltaTime;

            if (Mathf.Abs(m_rollSpeed) < m_minRotSpeed)
            {
                m_rollSpeed = 0;
            }
        }
        else
        {
            if (m_inputs.RollLeft)
            {
                m_rollSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.RollRight)
            {
                m_rollSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.PitchUp)
            {
                m_pitchSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.PitchDown)
            {
                m_pitchSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.YawLeft)
            {
                m_yawSpeed -= m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.YawRight)
            {
                m_yawSpeed += m_rotateAccelerationMultiplier * Time.fixedDeltaTime;
            }
        }

        // Movement velocity changes
        if (m_inputs.StopMovement)
        {
            m_velocity += -m_velocity.normalized * m_moveAccelerationMultiplier * Time.fixedDeltaTime;

            if (m_velocity.magnitude < m_minSpeed)
            {
                m_velocity = Vector3.zero;
            }
        }
        else
        {
            if (m_inputs.MoveForward)
            {
                m_velocity += transform.forward * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.MoveBackward)
            {
                m_velocity += -transform.forward * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.MoveLeft)
            {
                m_velocity += -transform.right * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.MoveRight)
            {
                m_velocity += transform.right * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.MoveUp)
            {
                m_velocity += transform.up * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }

            if (m_inputs.MoveDown)
            {
                m_velocity += -transform.up * m_moveAccelerationMultiplier * Time.fixedDeltaTime;
            }
        }

        // Clamping velocities
        m_velocity = Vector3.ClampMagnitude(m_velocity, m_maxSpeed);
        m_rollSpeed = Mathf.Clamp(m_rollSpeed, -m_maxRollSpeed, m_maxRollSpeed);
        m_yawSpeed = Mathf.Clamp(m_yawSpeed, -m_maxYawSpeed, m_maxYawSpeed);
        m_pitchSpeed = Mathf.Clamp(m_pitchSpeed, -m_maxPitchSpeed, m_maxPitchSpeed);

        // Move
        transform.position += m_velocity;

        // Roll
        transform.Rotate(Vector3.forward * m_rollSpeed);

        // Pitch
        transform.Rotate(Vector3.right * m_pitchSpeed);

        // Yaw
        transform.Rotate(Vector3.up * m_yawSpeed);
    }
}
