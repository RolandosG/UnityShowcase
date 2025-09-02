using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraBodyController : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float swaySpeed = 1.0f;
    public float rotationSpeed = 1.0f;

    private int headsDestroyed = 0;
    private float targetYPosition;
    private float initialXPosition;
    private float initialZPosition;

    private float currentPathTimer;
    //private float pathDuration = 1.0f;
    private Vector3 currentTargetPosition;

    void Start()
    {
        targetYPosition = transform.position.y;
        initialXPosition = transform.position.x;
        initialZPosition = transform.position.z;
    }

    void Update()
    {
        if (headsDestroyed >= 1 && headsDestroyed < 2)
        {
            // Rise by 0.5 on the Y-axis
            if (transform.position.y < targetYPosition)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

        }
        else if (headsDestroyed >= 3 && headsDestroyed < 6)
        {

            // Sway left and right
            transform.position = new Vector3(initialXPosition + Mathf.Sin(Time.time * swaySpeed) * 1.0f, transform.position.y, transform.position.z);
        }
        else if (headsDestroyed >= 6)
        {
            // Rise by 4 on the Y-axis
            if (transform.position.y < targetYPosition + 4.0f)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, targetYPosition + 4.0f, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Sway left and right
                transform.position = new Vector3(initialXPosition + Mathf.Sin(Time.time * swaySpeed) * 8.0f, transform.position.y, transform.position.z);
            }
        }
    }
        public void HeadDestroyed()
    {
        headsDestroyed++;
        targetYPosition += 0.5f;
    }
}
