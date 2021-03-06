using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour, Reset
{
    public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float m_MinSize = 6.5f;                  // The smallest orthographic size the camera can be.

    public float yOffset = 0.5f;
    [HideInInspector]
    public List<Transform> m_Targets;                   // All the targets the camera needs to encompass.
    public List<Transform> m_TargetsCopy;

    private Camera m_Camera;                        // Used for referencing the camera.
    private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 m_DesiredPosition;              // The position the camera is moving towards.
    private int m_AlivePlayers;                     // Used to zoom in when winner is decided.

    private bool m_PlayerWinsCamera = false;
    public bool m_Debug = false; // Used when you don't want camera to zoom in too far when only 1 player.
    public static CameraControl mInstance;

    [Header("Death")]
    public float DeathWaitTime = 3;
    List<Timer> deathTimers;
    private void Awake()
    {

        mInstance = this;
        m_Targets = new List<Transform>();
        deathTimers = new List<Timer>();

        m_Camera = GetComponentInChildren<Camera>();

        //populate my target list if any players are already found (when the scene has reloaded)
        if (FindObjectOfType(typeof(PlayerStatus)))
        {
            foreach (PlayerStatus FoundPlayer in GameObject.FindObjectsOfType<PlayerStatus>())
            {
                if (!m_Targets.Contains(FoundPlayer.transform))
                {
                    AddTarget(FoundPlayer.transform);
                }
            }
        }
        m_TargetsCopy = m_Targets;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < deathTimers.Count; i++)
        {
            if (m_Targets[i].GetComponent<PlayerStatus>().IsDead)
            {
                if (deathTimers[i].Tick(Time.deltaTime))
                {
                    m_Targets.Remove(m_Targets[i]);
                    deathTimers.Remove(deathTimers[i]);
                }
            }
        }
        // Move the camera towards a desired position.
        Move();

        // Change the size of the camera based.
        Zoom();
    }


    private void Move()
    {
        // Find the average position of the targets.
        FindAveragePosition();

        // Smoothly transition to that position.
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // Go through all the targets and add their positions together.
        for (int i = 0; i < m_Targets.Count; i++)
        {
            // If the target isn't active, go on to the next one.
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // Add to the average and increment the number of targets in the average.
            averagePos += m_Targets[i].position;
            numTargets++;
        }

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
            averagePos /= numTargets;

        // Keep the same y value.
        averagePos.z = transform.position.z;
        // The desired position is the average position;
        if (m_AlivePlayers == 1)
        {
            float temp = DeathWaitTime;
            DeathWaitTime = .5f;
            m_DesiredPosition = averagePos + new Vector3(0, 3, 0);
            StartCoroutine(ResetDeathWaitTIme(temp));
        }
        else
            m_DesiredPosition = averagePos;

    }

    IEnumerator ResetDeathWaitTIme(float a_DeathTimeOriginal)
    {
        yield return new WaitForSeconds(1);
        DeathWaitTime = a_DeathTimeOriginal;
    }

    private void Zoom()
    {
        // Find the required size based on the desired position and smoothly transition to that size.
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        m_AlivePlayers = m_Targets.Count;
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        for (int i = 0; i < m_Targets.Count; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!m_Targets[i].gameObject.activeSelf)
            {
                if (!m_Debug)
                {
                    m_AlivePlayers--;
                }
                continue;
            }

            // Otherwise, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            targetLocalPos.y -= yOffset;
            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }
        if (m_AlivePlayers == 2)
        {
            for (int i = 0; i < m_Targets.Count; ++i)
            {
                deathTimers[i].mfTimeToWait = .1f;
            }
        }

        if (m_AlivePlayers == 1)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                if (m_Targets[i].gameObject.activeSelf)
                {
                    Vector3 desiredPosToTarget = m_Targets[i].position + new Vector3(0, 11, 0);
                    size = 5;
                    return size;
                }
            }


        }
        else
        {
            // Add the edge buffer to the size.
            size += m_ScreenEdgeBuffer;

            // Make sure the camera's size isn't below the minimum.
            size = Mathf.Max(size, m_MinSize);


        }
        return size;
    }


    public void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_DesiredPosition;

        // Find and set the required size of the camera.
        m_Camera.orthographicSize = FindRequiredSize();
    }

    public void AddTarget(Transform target)
    {
        m_Targets.Add(target);
        deathTimers.Add(new Timer(DeathWaitTime));
    }
    public void Reset()
    {
        //populate my target list if any players are already found (when the scene has reloaded)
        //if (FindObjectOfType(typeof(PlayerStatus)))
        //{
        //    foreach (PlayerStatus FoundPlayer in GameObject.FindObjectsOfType<PlayerStatus>())
        //    {
        //        if (!m_Targets.Contains(FoundPlayer.transform))
        //        {
        //            AddTarget(FoundPlayer.transform);
        //        }
        //    }
        //}

        m_Targets = m_TargetsCopy;

    }
}

 