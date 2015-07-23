/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Die Klasse bestimmt das Verhalten der Spielfigur und die Interaktion mit der Spielwelt.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Variablen
    // Variable für die Speicherung des Prefabs der Spielfigur
    public GameObject prefabPlayer;
    // Variable für das Anpassen der Bewegungsgeschwindigkeit
    public float movSpeed = 10f;
    // Variable für das Anpassen der Gravitation
    public float gravity = 0.8f;
    // Variable für das Anpassen der Sprungstärke
    public float jumpPower = 12f;
    // Variable für das Anpassen der Trampolinsprungstärke
    public float springPower = 12f;
    // Variable die Richtung
    float direction = 0f;
    // Variable zur Speicherung, ob Spielfigur springt
    bool isJumping = false;
    // Variable für die Bewegung der Spielfigur
    Vector3 movDirection = Vector3.zero;
    //  Variable für den Zugriff auf Eigenschaften des CharacterControllers der Spielfigur
    CharacterController player;
    // Zurücksetzpunkt nach Tod der Spielfigur
    Vector3 respawnPoint = new Vector3(-2, 1, 0);
    // Variable für die Drehung der Spielfigur in richtige Blickrichtung
    private bool isrotated = false;
    #endregion

    // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Initialisierung des CharacterControllers
        player = GetComponent<CharacterController>();
        // abspielen der Standardanimation
        animation.Play("StandingRight");
     }

    // Update aktualisiert das Verhalten und wird jedes Einzelbild aufgerufen
    void Update()
    {
        // Spiel wurde nicht pausiert
        if (Time.timeScale > 0)
        {
            // Aufruf der Hilfsmethode zur Ermittlung der Tasteneingaben
            InputCheck();
            // Aufruf der Hilfsfunktion für die Bewegung der Spielfigur
            Move();
        }
    }

    // Methode wird aufgerufen, wenn ein anderer Collider in den Trigger eintritt
    void OnTriggerEnter(Collider other)
    {
        // Spieler berührt tötbaren Gegner unbeabsichtigt
        if (other.gameObject.tag.Equals("killEnemy") && player.isGrounded)
        {
            Die();
        }
        // Spieler berührt nichttötbaren Gegner unbeabsichtigt
        if (other.gameObject.tag.Equals("avoidEnemy")) 
        {
            Die();
        }
        // Spieler berührt Stampfer unbeabsichtigt
        if (other.gameObject.tag.Equals("Stomper")) 
        {
            Die();
        }
        // Spieler berührt stationäres Trampolin
        if (other.gameObject.tag.Equals("Spring"))
        {
            // Anpassung der Bewgung der Spielfigur
            movDirection.y = springPower;
            player.Move(movDirection * Time.deltaTime);
        }
        // Spieler ist durch eine Lücke gefallen
        if (other.gameObject.tag.Equals("Gap"))
        {
            Die();
        }
        // Spieler hat das Ende erreicht
        if (other.gameObject.tag.Equals("End"))
        {
            Application.LoadLevel("Menu");
        }
    }

    // Methode wird aufgerufen, wenn ein anderer Collider im Trigger bleibt
    void OnTriggerStay(Collider other) 
    {
        // Spieler berührt stationäres Trampolin
        if (other.gameObject.tag.Equals("Spring"))
        {
            // Anpassung der Bewgung der Spielfigur
            movDirection.y = springPower;
            player.Move(movDirection * Time.deltaTime);
        }
    }
	
    // Methode überprüft Tastatureingaben und setzt entsprechende Variablen
    void InputCheck() 
    {
        // aktuelle Richtung ermitteln
        direction = Input.GetAxis("Horizontal") * movSpeed;

        // Spielfigur blickt in die entsprechende Richtung
        // nach rechts
        if (direction > 0) 
        {
            if (isrotated)
            {
                FlipChar();
            }
        }
        // nach links
        if (direction < 0)
        {
            if (!isrotated)
            {
                FlipChar();
            }
        }

        // Prüfung, ob Sprungtaste gedrückt wurde
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }
        else 
        {
            isJumping = false;
        }
    }

    // Methode ermöglicht die Bewegung der Spielfigur
    void Move() 
    {
        // Spielfigur steht am Boden
        if (player.isGrounded) 
        {
            movDirection.y = -1;
            // Sprungtaste wurde gedruckt
            if (isJumping) 
            {
                movDirection.y = jumpPower;
            }
        }
        else 
        { 
            movDirection.y -= gravity;
        }
        movDirection.x = direction;
        // Spielfigur wird entsprechend der berechneten Bewegungsrichtung bewegt
        player.Move(movDirection * Time.deltaTime);
    }

    // Methode zerstört das aktuelle Gameobject und erzeugt ein neues an Start des Levels
    void Die() 
    {        
        // aktuelles Gameobject wird zerstört
        Destroy(gameObject);
        // neues Gameobject wird erzeugt
        prefabPlayer = Instantiate(prefabPlayer, respawnPoint, Quaternion.identity) as GameObject;
        prefabPlayer.GetComponent<PlayerController>().enabled = true;
        prefabPlayer.GetComponent<Animation>().enabled = true;
        // Lebensanzahl wird verringert
        GameManager.lifeCount--;
        // Blickrichtungsvariable wird zurückgesetzt
        isrotated = false;
    }

    // Methode dreht die Spielfigur
    void FlipChar()
    {
        isrotated = !isrotated;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale; 
    }
}