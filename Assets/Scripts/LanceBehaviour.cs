using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class LanceBehaviour : CharBehaviour
{

    public override void Special()
    {
        Debug.Log("Used lance guy's special move, heal");

        hp += 10;

        
    }

}
