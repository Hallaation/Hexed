using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
public class CameraShake : MonoBehaviour
{

    // Transform of the camera to shake. Grabs the gameObject's transform, a camera' transform reference.
    // if null.
    static CameraShake minstance;
    public Transform ref_camTransform;

    // How long the object should shake for.
    public float m_fShakeDuration = 1.0f;
    private float m_fTimer;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float m_fShakeAmount = 10.0f;
    public float m_fDecreaseFactor = 1.0f;

    public bool m_bShakeCamera;
    Vector3 m_vOriginalPos;

    public static CameraShake Instance
    {
        get
        {
            if (!minstance)
            {
                minstance = (CameraShake)FindObjectOfType(typeof(CameraShake));
                if (!minstance)
                {
                    minstance = (new GameObject("CameraShake")).AddComponent<CameraShake>();
                }
            }
            return minstance;
        }
    }

    public void ShakeCamera()
    {
        m_fTimer = 0;
        m_bShakeCamera = true;
    }
    void Awake()
    {
        ref_camTransform = Camera.main.transform;
        m_vOriginalPos = ref_camTransform.localPosition;
        m_fTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            ShakeCamera();

        if (m_bShakeCamera)
        {
           //? for (int i = 0; i < 4; i++)
           //?  {
           //?      GamePad.SetVibration(PlayerIndex.One + i, m_fShakeAmount * 0.01f, m_fShakeAmount * 0.01f);
           //?  }
            m_fTimer += Time.deltaTime;
            if (m_fTimer < m_fShakeDuration)
            {
                ref_camTransform.localPosition += Random.insideUnitSphere * m_fShakeAmount * Time.deltaTime;
            }
            else
            {
                m_bShakeCamera = false;
                //ref_camTransform.localPosition = m_vOriginalPos;
                m_fTimer = 0;
            }
        }
        else
        {
            m_vOriginalPos = ref_camTransform.position;
        }

    }

}
