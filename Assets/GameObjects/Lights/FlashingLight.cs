using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    private Light m_light;

    private float m_frames;

    [SerializeField]
    private float m_litFrames = 120;
    
    [SerializeField]
    private float m_unlitFrames = 120;

    private void Awake()
    {
        m_light = GetComponent<Light>();
        m_frames = 0;
    }

    private void FixedUpdate()
    {
        m_frames++;

        if (m_light.enabled)
        {
            if (m_frames > m_litFrames)
            {
                m_frames = 0;
                m_light.enabled = false;
            }
        }
        else
        {
            if (m_frames > m_unlitFrames)
            {
                m_frames = 0;
                m_light.enabled = true;
            }
        }

    }
}
