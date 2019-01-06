using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float moveSpeed = 4f;  
    public float stunnedTime = 3f;
    public float waitAtWaypointTime = 1f;

    public string stunnedLayer = "StunnedEnemy"; 
    public string playerLayer = "Player";  

    public bool isStunned = false;
    public bool loopWaypoints = true;

    private int _myWaypointIndex = 0;
    private int _enemyLayer;
    private int _stunnedLayer;

    private float _moveTime;
    private float _vx = 0f;

    private bool _moving = true;

    //

    public GameObject[] myWaypoints;
    public AudioClip stunnedSFX;
    public AudioClip attackSFX;

    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private AudioSource _audio; 


    void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();


        _moveTime = 0f;
        _moving = true;

        _enemyLayer = this.gameObject.layer;
        _stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true);
    }

    void Update()
    {
        if (!isStunned)
        {
            if (Time.time >= _moveTime)
            {
                EnemyMovement();
            }
            else
            {
                _animator.SetBool("Moving", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player") && !isStunned)
        {
            CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();

            if (!GameManager.gm.isInvulnerable)
            {
                Flip(collision.transform.position.x - _transform.position.x);
                _rigidbody.velocity = new Vector2(0, 0);

                _animator.SetTrigger("Attack");

                player.FallDeath();

                playSound(attackSFX);
            
                _moveTime = Time.time + stunnedTime;
            }

            else
            {
                Stunned(); 
            }
        }
    }

    void EnemyMovement()
    {
        if ((myWaypoints.Length != 0) && (_moving))
        {

            Flip(_vx);
            _vx = myWaypoints[_myWaypointIndex].transform.position.x - _transform.position.x;

            if (Mathf.Abs(_vx) <= 0.05f)
            {
                _rigidbody.velocity = new Vector2(0, 0);
                _myWaypointIndex++;

                if (_myWaypointIndex >= myWaypoints.Length)
                {
                    if (loopWaypoints)
                        _myWaypointIndex = 0;
                    else
                        _moving = false;
                }

                _moveTime = Time.time + waitAtWaypointTime;
            }
            else
            {
                _animator.SetBool("Moving", true);
                _rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
            }

        }
    }

    void Flip(float _vx)
    {

        Vector3 localScale = _transform.localScale;

        if ((_vx > 0f) && (localScale.x > 0f) || (_vx < 0f) && (localScale.x < 0f))
            localScale.x *= -1;

        _transform.localScale = localScale;
    }

    public void Stunned()
    {
        if (!isStunned)
        {
            isStunned = true;

            playSound(stunnedSFX);

            _animator.SetTrigger("Stunned");
            _rigidbody.velocity = new Vector2(0, 0);

            this.gameObject.layer = _stunnedLayer;

            StartCoroutine(Stand());
        }
    }

    void playSound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    IEnumerator Stand()
    {
        yield return new WaitForSeconds(stunnedTime);

        isStunned = false;

        this.gameObject.layer = _enemyLayer;

        _animator.SetTrigger("Stand");
    }
}
