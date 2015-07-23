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
/// Die Klasse bestimmt das Verhalten von Münzen.
/// </summary>
public class Coin : MonoBehaviour
{
    #region Variablen
    // Variable für das Anpassen der Rotationsgeschwindigkeit
    public float rotSpeed;
    #endregion

    // Update aktualisiert das Verhalten und wird jedes Einzelbild aufgerufen
    void Update()
    {
        // Münze wird gedreht, so dass eine Rotation entsteht
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
    }

    // Methode wird aufgerufen, wenn ein anderer Collider in den Trigger eintritt
    void OnTriggerEnter(Collider other) 
    {
        // Prüfung, ob zugehöriges Gameobject des Colliders der Spielfigur gehört
        if (other.gameObject.CompareTag("Player")) 
        {
            // Anzahl der gesammelten Münzen wird erhöht
            GameManager.coinCount++;
            // Münzen-Gameobject wird zerstört, da es eingesammelt wurde
            Destroy(gameObject);
        }
    }
}