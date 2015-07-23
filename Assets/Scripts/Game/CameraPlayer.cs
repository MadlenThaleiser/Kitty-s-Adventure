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
/// Die Klasse legt das Verhalten der Kamera fest.
/// </summary>
public class CameraPlayer : MonoBehaviour
{
    #region Variablen
    // Variable für den Zugriff auf die Eigenschaften der Spielfigur 
    public GameObject player;
    // Variable für die Kontrolle Versatz der Kamera 
    public int zOffset;
    // Variable für die Bewegung der Kamera
    Vector3 position = Vector3.zero;
    #endregion

    // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Finden des Player-Gameobjectes in der Szene
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // LateUpdate wird jedes Einzelbild nach den Update-Methoden aufgerufen
    void LateUpdate()
    {
        // Prüfung, ob Gameobject der Spielfigur vorhanden ist
        if (player != null)
        {
            // Bewegung der Kamera anhand er Position der Spielfigur ermitteln
            position = player.transform.position;
            position.z -= zOffset;
            transform.position = position;
        }
        else 
        {
            // Finden des Player-Gameobjectes in der Szene
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}