using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Jump : MonoBehaviour
{
    public float JumpSpeed = 10f;
    public bool isGround;
    private Rigidbody2D rigidBody;
    
    void Start()
    {
        isGround = true;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(isGround == true)
        {
            rigidBody.velocity = new Vector2(0, JumpSpeed);
            isGround = false;
        }
    }

    public void OnTriggerStay2D(Collider2D other) 
    {
        if (other.tag == "GroundCheck")
        {
            isGround = true;
        }
    }
}
