using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 3f;

    void Start()
    {
        targetPosition = new Vector3(transform.position.x, transform.position.y, -11);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition.z == -11 ? 
                new Vector3(transform.position.x, transform.position.y, -7) : 
                new Vector3(transform.position.x, transform.position.y, -11);
        }
    }
}
