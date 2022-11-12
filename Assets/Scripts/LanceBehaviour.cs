using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LanceBehaviour : CharBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(hp);
        if (isMoving)
        {
            // Move our position a step closer to the target.
            var step = speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, target, step);
            transform.position = target;

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                // Put it on the right spot and stop moving
                transform.position = target;
                isMoving = false;
            }
        }
    }
}
