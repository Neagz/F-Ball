using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 4f;
    public float movement = 0f;
    public Rigidbody2D rigidBody2D;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    public Vector3 RespawnPoint;
    public LevelManager levelManager;
    private Animator playerAnimator;
    public bool inputDisable;
    public bool PlayerDied;
    public PauseScript pause;
    public CircleCollider2D circleCollider;
    public AudioSource audioSource;
    public AudioClip DeathClip;
    public AudioClip[] ThemeClip;
    public int index = 0;
    public GameObject WarningUI;
    public MovingPlank movingPlank;

    private bool _rightB = false;
    private bool _leftB = false;

    public AudioSource myBtn;
    public AudioClip clickBtn;


    // Start is called before the first frame update
    void Start()
    {
        movingPlank = FindObjectOfType<MovingPlank>();
        WarningUI.SetActive(false);
        rigidBody2D = GetComponent<Rigidbody2D>();
        RespawnPoint = transform.position;
        pause = FindObjectOfType<PauseScript>();
        levelManager = FindObjectOfType<LevelManager>();
        playerAnimator =  GetComponent<Animator>();
        inputDisable = false;
        PlayerDied = false;
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerDied)
        {
            _rightB = false;
            _leftB = false;
        }

        if (Vector3.Distance(transform.position, movingPlank.transform.position) <= 3f)
        {
            WarningUI.SetActive(true);
        }
        else
        {
            WarningUI.SetActive(false);
        }
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        //movement = Input.GetAxis("Horizontal");

        if (_rightB)
        {
            if(movement >= 0 && movement <= 1)
            {
                movement += 1 * Time.deltaTime;
            }
            if (movement > 0f)
            {
                rigidBody2D.velocity = new Vector2(movement * speed, rigidBody2D.velocity.y);
                playerAnimator.SetInteger("isMoving", 1);
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
                rigidBody2D.velocity = new Vector2(movement * speed, rigidBody2D.velocity.y);
                playerAnimator.SetInteger("isMoving", 0);
            }
        }

        if (!inputDisable)
        {
            if (movement != 0f)
            {
                rigidBody2D.velocity = new Vector2(movement * speed, rigidBody2D.velocity.y);
                playerAnimator.SetInteger("isMoving", 1);
            }
            else
            {
                playerAnimator.SetInteger("isMoving", 0);
            }
            if (Input.GetButtonDown("Jump") && (isTouchingGround))
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpSpeed);
            }
        }
        if(index == 2)
        {
            audioSource.volume = 0.4f;
        }
    }

    public void TaskOnClickJump()
    {
        if (isTouchingGround)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpSpeed);
            myBtn.PlayOneShot(clickBtn);
        }
    }

    public void TaskOnClickRightTrue()
    {
        if (!inputDisable)
        {
            _rightB = true;
        }
    }

    public void TaskOnClickRightFalse()
    {
        if (!inputDisable)
        {
            _rightB = false;
        }
        movement = 0f;
    }

    public void TaskOnClickLeftTrue()
    {
        if (!inputDisable)
        {
            _leftB = true;
        }
    }

    public void TaskOnClickLeftFalse()
    {
        if (!inputDisable)
        {
            _leftB = false;
        }
        movement = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Checkpoint")
        {
            RespawnPoint = other.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "FallDetector")
        {
            inputDisable = true;
            PlayerDied = true;
            Debug.Log("FallDetected!");
            circleCollider.enabled = false;
            StartCoroutine("WaitforAnimation");
        }
        if(other.gameObject.tag == "Enemy")
        {
            inputDisable = true;
            PlayerDied = true;
            circleCollider.enabled = false;
            StartCoroutine("WaitforAnimation");
        }
    }

    private IEnumerator WaitforAnimation()
    {
        audioSource.Stop();
        audioSource.clip = DeathClip;
        audioSource.volume = 1f;
        audioSource.Play();
        audioSource.loop = false;
        rigidBody2D.velocity = new Vector3(0, 3f);
        playerAnimator.Play("PlayerDeath");
        rigidBody2D.gravityScale = 0;
        rigidBody2D.mass = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        yield return new WaitForSeconds(audioSource.clip.length);
        levelManager.Respawn();
    }
}
