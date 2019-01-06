using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public int startLives = 3;
    public int lives = 0;

    public float energy = 100; 
    public float powerUpTime = 10f;

    public bool isInvulnerable = false;

    private float timePassed = 0.0f;

    public Slider energyIndicator; 
    public GameObject[] UIExtraLives;
    public GameObject UIGamePaused;
    public GameObject UIWin;
    public GameObject UILose; 

    public AudioSource backgroundMusic;

    private GameObject _player;
    private Vector3 _spawnLocation;
    private Scene _scene;

 

    void Awake()
    {
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        //Camera.main.backgroundColor = Color.black; 

        lives = startLives; 

        setupDefaults();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
            {
                UIGamePaused.SetActive(true); 
                Time.timeScale = 0f; 
            }
            else
            {
                Time.timeScale = 1f; 
                UIGamePaused.SetActive(false); 
            }


        }

        energy += Time.deltaTime;
        energyIndicator.value = energy;
    }

    
    void setupDefaults()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");

            if (_player == null)
                Debug.LogWarning("No player in scene");
            else
                _spawnLocation = _player.transform.position;
        }

        _scene = SceneManager.GetActiveScene();


        refreshGUI();
    }

    void refreshGUI()
    {
        for (int i = 0; i < UIExtraLives.Length; i++)
        {
            if (i < lives)
            {
                UIExtraLives[i].SetActive(true);
            }
            else
            {
                UIExtraLives[i].SetActive(false);
            }
        }
    }

  
    public void ResetGame()
    {
        lives--;
        refreshGUI();

        if (lives <= 0)
        {
            UILose.SetActive(true); 
        }
        else
        { 
            _player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
        }
    }

    public void LevelCompete()
    {
        UIWin.SetActive(true); 
    }

    public void AddLife()
    {
        lives += 1;

        for (int i = 0; i < UIExtraLives.Length; i++)
        {
            if (i < lives)
            {
                UIExtraLives[i].SetActive(true);
            }
            else
            {
                UIExtraLives[i].SetActive(false);
            }
        }

        if (lives >= 5)
            lives = 5;
    }


    public void AddSpeed()
    {
        timePassed = 0;
        StartCoroutine(SpeedUp());
    }

    public void makeVulnerable()
    {
        timePassed = 0;
        StartCoroutine(Vulnerable());
    }

    public void AddEnergy()
    {
        energy += 25.0f;
        if (energy >= 100.0f)
            energy = 100.0f;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(_scene.name);
    }


    IEnumerator SpeedUp()
    {

        _player.GetComponent<CharacterController2D>().speed += 5.0f;

        if (backgroundMusic)
            backgroundMusic.pitch = 1.25f;

        while (timePassed < powerUpTime)
        {

            timePassed += Time.deltaTime;

            yield return null;
        }

        _player.GetComponent<CharacterController2D>().speed -= 5.0f;

        if (backgroundMusic)
            backgroundMusic.pitch = 1f;
    }

    IEnumerator Vulnerable()
    {
        if (!isInvulnerable)
            isInvulnerable = true;

        while (timePassed < powerUpTime)
        {
            timePassed += Time.deltaTime;

            yield return null;
        }

        isInvulnerable = false;

    }


}
