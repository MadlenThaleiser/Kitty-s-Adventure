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
/// Die Klasse lässt Hintergründe mit der Spielfigur sich mitbewegen.
/// </summary>
public class LayerParallax : MonoBehaviour
{
    #region Variablen
    //  Variable für den Zugriff auf Eigenschaften des CharacterControllers der Spielfigur
    public CharacterController cplayer;
    //  Variable für den Zugriff auf Eigenschaften des Gameobjectes der Spielfigur
    public GameObject player;
    // Variable für das Anpassen der Bewegungsgeschwindigkeit
    public float speedFactor = 1;
    // Variable für die Startposition
    Vector3 startpostion;
    // Variable für die Bewegungsrichtung
    Vector3 direction;
    #endregion

     // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Initialisierung der Gameobjecte und der Startpostion
        player = GameObject.FindGameObjectWithTag("Player");
        cplayer = player.GetComponent<CharacterController>();
        startpostion = transform.position;
    }
    
    // LateUpdate wird jedes Einzelbild nach den Update-Methoden aufgerufen
     void LateUpdate()
    {
         // Prüfung, ob Gameobject der Spielfigur vorhanden ist
         if (player != null)
         {
             // Bewegung des Hintergrundes anhand er Position der Spielfigur ermitteln
             direction.x = cplayer.velocity.x * speedFactor * (-1);
             transform.Translate(direction * Time.deltaTime);
         }
         else 
         {
             // Initialisierung der Startpostion und Ermittlung des Gameobjectes der Spielfigur
             transform.position = startpostion;
             player = GameObject.FindGameObjectWithTag("Player");
             cplayer = player.GetComponent<CharacterController>();
         }
     }
}