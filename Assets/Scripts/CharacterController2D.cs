using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

    public float speed = 5f;
    public float jumpForce = 250f;

    public bool moveBetweenBlackholes = false;
    public bool playerCanMove = true;

    private bool _faceright = true; 
    private bool _isGrounded = false;
    private bool _isRunning  = false;
    private bool _isInverted = false;
    private bool _isCounting = false;
    private bool lightIsOn = false;

    private float _xVelocity = 0.0f;
    private float _yVelocity = 0.0f;
    private float Timer = 0.0f;
    private float holdTime = 1.0f;
    private float energyDrainRate = 8.0f;

    private int counter = 60; 

    // 

    public LayerMask ground;
    public Transform groundCheck;

    public AudioClip lifeSFX;
    public AudioClip powerUpSFX;
    public AudioClip BatterySFX;
    public AudioClip fallSFX;
    public AudioClip jumpSFX;
    public AudioClip victorySFX;
    public AudioClip turnLightSFX;
    public AudioClip noEnoughEnergySFX;

    private Transform _transform;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private Camera maincamera;
    private AudioSource _audio;
    private Color32 bkColor;

    private void Awake()
    {
        _rb2d = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _transform = gameObject.GetComponent<Transform>();
        _audio = gameObject.GetComponent<AudioSource>();
    }

    private void Start ()
    {
        maincamera = Camera.main;
        bkColor = maincamera.backgroundColor;
	}
	
	private void Update ()
    {
        if (!moveBetweenBlackholes)
            counter--;

        if (counter <= 0)
        {
            counter = 60; 
            moveBetweenBlackholes = true;
        }

        if (!playerCanMove || Time.timeScale == 0f)
            return;

        _xVelocity = Input.GetAxisRaw("Horizontal");
        
        if (_xVelocity != 0)
            _isRunning = true;
        else
            _isRunning = false;

        _animator.SetBool("Running", _isRunning);

        _yVelocity = _rb2d.velocity.y;

        _isGrounded = Physics2D.Linecast(_transform.position, groundCheck.position, ground);
        _animator.SetBool("Grounded", _isGrounded);

        if (_isGrounded && Input.GetButtonDown("Jump"))
            Jump();

        if (Input.GetButtonUp("Jump") && _yVelocity > 0f)
            _yVelocity = 0f;

        _rb2d.velocity = new Vector2(_xVelocity * speed, _yVelocity);

        if (Input.GetKeyDown(KeyCode.E))
            _isCounting = true;

        if (Input.GetKey(KeyCode.E))
        {
            if (_isCounting)
            {
                Timer += Time.deltaTime;
                if (Timer >= holdTime)
                    InvertLight();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Timer = 0.0f;
            _isCounting = false;
        }

        if (lightIsOn)
        {
            GameManager.gm.energy -= Time.deltaTime * energyDrainRate;
            if (GameManager.gm.energy <= 0)
                InvertLight();
        }

    }


    private void LateUpdate()
    {
        Vector3 localScale = _transform.localScale;
        if (_xVelocity > 0)
            _faceright = true;
        else if(_xVelocity < 0)
            _faceright = false;

        if (_faceright && localScale.x < 0  || !_faceright && localScale.x > 0)
            localScale.x *= -1;

        _transform.localScale = localScale; 
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Heart"))
        {
            CollectLife();
            Destroy(other.gameObject);
        }

        else if (other.gameObject.CompareTag("Potion"))
        {
            CollectPowerUp();
            Destroy(other.gameObject);
        }

        else if (other.gameObject.CompareTag("Battery"))
        {
            CollectBattery();
            Destroy(other.gameObject);
        }

    }

    private void Jump()
    {
        PlaySound(jumpSFX);
        _yVelocity = 0.0f;
        _rb2d.AddForce(new Vector2(0, jumpForce));
    }

    public void Transition(Vector3 pos)
    {
        _transform.localPosition = pos; 
    }

    private void InvertLight()
    {
        moveBetweenBlackholes = false; 

        if (_isInverted)
        {
            bkColor = new Color32(0, 0, 0, 255);
            lightIsOn = false;
            _isInverted = !_isInverted;
            PlaySound(turnLightSFX);
        }
        else
        {
            if (GameManager.gm.energy >= 10.0f)
            {
                bkColor = new Color32(238, 235, 233, 255);
                lightIsOn = true;
                _isInverted = !_isInverted;
                PlaySound(turnLightSFX);
            }

            else
                PlaySound(noEnoughEnergySFX);

        }

        Timer = 0.0f;
        _isCounting = false;
        maincamera.backgroundColor = bkColor;
    }

    void FreezeMotion()
    {
        playerCanMove = false;
        _rb2d.isKinematic = true;
    }

    void UnFreezeMotion()
    {
        playerCanMove = true;
        _rb2d.isKinematic = false;
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    public void FallDeath()
    {
        if (playerCanMove)
        {
            PlaySound(fallSFX);
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator KillPlayer()
    {
        if (playerCanMove)
        {
            FreezeMotion();

            yield return new WaitForSeconds(0.1f);

            if (GameManager.gm) 
                GameManager.gm.ResetGame();
            else 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void CollectBattery()
    {
        PlaySound(BatterySFX);

        if (GameManager.gm) 
            GameManager.gm.AddEnergy();
    }

    public void CollectLife()
    {
        PlaySound(lifeSFX);

        if (GameManager.gm)
            GameManager.gm.AddLife();
    }

    public void CollectPowerUp()
    {
        PlaySound(powerUpSFX);

        if (GameManager.gm)
        {
            GameManager.gm.AddSpeed();
            GameManager.gm.makeVulnerable();
        }
    }

    public void Victory()
    {
        PlaySound(victorySFX);
        StartCoroutine(playerWin());

    }

    IEnumerator playerWin()
    {
        FreezeMotion();

        //_animator.SetTrigger("Victory");

        yield return new WaitForSeconds(0.2f);
        
        if (GameManager.gm)
            GameManager.gm.LevelCompete();
    }

    public void Respawn(Vector3 spawnloc)
    {
        UnFreezeMotion();
        _transform.parent = null;
        _transform.position = spawnloc;
        _animator.SetTrigger("Respawn");
    }
}
