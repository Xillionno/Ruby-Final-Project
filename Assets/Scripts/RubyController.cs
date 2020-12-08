using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public static int level;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip throwSound;
    public GameObject healthdown;
    public GameObject healthup;

    public GameObject BackgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;

    private int scoreValue = 0;
    public Text score;
    public Text endscreen;

    private bool gameOver;

    private int cogValue = 4;
    public Text cogText;
    

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        endscreen.text = " ";
        cogText.text = "Ammo : " + cogValue.ToString() + "/99";
        gameOver = false;
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogValue >= 1)
            {
                Launch();
                cogValue -= 1;
                cogText.text = "Ammo : " + cogValue.ToString() + "/99";
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();

                    if (scoreValue >= 4)
                    {
                        level += 1;
                        scoreValue = 0;
                        SceneManager.LoadScene("Scene2");
                        
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }
        if (level >= 1)
        { if (scoreValue >= 4)
            {
                gameOver = true;
                endscreen.text = "You win! Press r to restart!";
                speed = 0;
                Object.Destroy(BackgroundMusic);
                PlaySound(winSound);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                

            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            GameObject damage = Instantiate(healthdown, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            GameObject projectileObject = Instantiate(healthup, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            gameOver = true;
            speed = 0;
            endscreen.text = "You lose! Press r to restart!";
            Object.Destroy(BackgroundMusic);
            PlaySound(loseSound);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void ChangeScore(int amount)
    {

        if (amount > 0)
        {
            scoreValue += 1;
            score.text = "Robots Fixed : " + scoreValue.ToString();
        }

        if (scoreValue >= 4)
        {
            endscreen.text = "Talk to Jambi for Stage 2!";
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "CogPickup")
        {
            cogValue += 3;
            cogText.text = "Ammo : " + cogValue.ToString() + "/99";
            Destroy(collision.collider.gameObject);
        }
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(throwSound);
    }
}