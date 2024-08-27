﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{    
    public float speed=5f;
    public float jumpSpd=8f;
    public float movement=0f;
    public Rigidbody2D rigbody;
    public Transform GroundChkPoint;
    public float GroundChkRad;
    public LayerMask GroundLay; 
    private bool isOnGround;
    public Vector3 respawnP;
    public LevelManager2 gameLevelM;
    private Animator ballAnimation;
    public Vector2 ReboundVelocity;    
    public float reboundx=5f,reboundy=7f;
    public AudioSource audioSource;
    public AudioClip[] ThemeClip;
    public AudioClip DeathClip;
    public int index = 0;

    private bool _rightB = false;
    private bool _leftB = false;
    
    public AudioSource myBtn;
    public AudioClip clickBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        rigbody= GetComponent<Rigidbody2D> ();
        respawnP= transform.position;
        gameLevelM= FindObjectOfType<LevelManager2> ();
        ballAnimation=GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;  
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled)
        {
            _rightB = false;
            _leftB = false;
        }

        isOnGround=Physics2D.OverlapCircle(GroundChkPoint.position,GroundChkRad,GroundLay);  
        //movement=Input.GetAxis("Horizontal");
        if(movement > 0f)
        {
            rigbody.velocity= new Vector2 (movement*speed,rigbody.velocity.y);
        }
        else if(movement < 0f)
        {   
            rigbody.velocity= new Vector2 (movement*speed,rigbody.velocity.y);
        }

        if(Input.GetButtonDown("Jump") && isOnGround)
        {
            rigbody.velocity= new Vector2 (rigbody.velocity.x,jumpSpd);
        }
        if(index == 2)
        {
            audioSource.volume = 0.4f;
        }

        if (_rightB)
        {
            if(movement >= 0 && movement <= 1)
            {
                movement += 1 * Time.deltaTime;
            }
            if (movement > 0f)
            {
                rigbody.velocity = new Vector2(movement * speed, rigbody.velocity.y);
            }
        }

        if (_leftB)
        {
            if(movement <= 0 && movement >= -1)
            {
                movement -= 1 * Time.deltaTime;
            }
            if (movement < 0f)
            {
                rigbody.velocity = new Vector2(movement * speed, rigbody.velocity.y);
            }
        }

        ballAnimation.SetBool("IsGround",isOnGround);
        ballAnimation.SetFloat("JumpSpeed",rigbody.velocity.y);  
    }
    public void TaskOnClickJump()
    {
        if (isOnGround)
        {
            rigbody.velocity = new Vector2(rigbody.velocity.x, jumpSpd);
            myBtn.PlayOneShot(clickBtn);
        }
    }

    public void TaskOnClickRightTrue()
    {
        _rightB = true;
    }

    public void TaskOnClickRightFalse()
    {
        _rightB = false;
        movement = 0f;
    }

    public void TaskOnClickLeftTrue()
    {
        _leftB = true;
    }

    public void TaskOnClickLeftFalse()
    {
        _leftB = false;
        movement = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "FallDetect" && enabled)
        {            
            ballAnimation.SetTrigger("IsTagFall");
            enabled=false;
            rigbody.velocity= new Vector2 (0,3.75f);
            transform.rotation=Quaternion.Euler(0,0,0);
            rigbody.freezeRotation=true;            
            rigbody.gravityScale=0;                                                            
            StartCoroutine ("AnimationCoroutine");                               
        }
        
        if(other.tag == "CheckPointTag")
        {
            respawnP=other.transform.position;
        }
    }

    public IEnumerator AnimationCoroutine()
    {
        audioSource.Stop();
        audioSource.clip = DeathClip;
        audioSource.Play();
        yield return new WaitForSeconds(2f);        
        gameObject.SetActive(false);
        transform.position=respawnP;                     
        rigbody.gravityScale=1;
        rigbody.freezeRotation=false;        
        gameLevelM.UpdateLives();
        gameObject.SetActive(true);
        enabled=true;  
        audioSource.Stop();
        audioSource.clip = ThemeClip[index];
        index++;
        audioSource.Play();
        audioSource.loop = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "EnemyBox" )
        {
            ballAnimation.SetTrigger("IsTagFall");
            enabled=false;
            rigbody.velocity= new Vector2 (0,3.75f);
            transform.rotation=Quaternion.Euler(0,0,0);
            rigbody.freezeRotation=true;            
            rigbody.gravityScale=0;                                                            
            StartCoroutine ("AnimationCoroutine");              
        }
        if(other.gameObject.tag == "CheckPointTag")
        {
            respawnP=other.transform.position;
        }
    }
    public IEnumerator ReboundCoroutine()
    {
        audioSource.Stop();
        audioSource.clip = DeathClip;
        audioSource.Play();
        yield return new WaitForSeconds(2f);
        rigbody.velocity=new Vector2(0f,0f);
        rigbody.freezeRotation=true;  
        transform.rotation=Quaternion.Euler(0f,0f,0f);
        ballAnimation.Play("PlayerIdle");                
        GetComponent<CircleCollider2D> ().enabled=true;
        transform.position=respawnP; 
        enabled=true;        
        rigbody.freezeRotation=false;
        audioSource.Stop();
        audioSource.clip = ThemeClip[index];
        index++;
        audioSource.Play();
        audioSource.loop = true;        
        gameLevelM.UpdateLives();    
    } 
}
