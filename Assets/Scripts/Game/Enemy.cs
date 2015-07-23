/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Die Klasse legt das Verhalten eines tötbaren Gegners fest.
/// </summary>
public class Enemy : MonoBehaviour
{
    #region Variablen
    // Variable für das Anpassen der Bewegungsgeschwindigkeit
    public float movSpeed = 3f;
    // Variable für die Anpassung der Rollgeschwindigkeit
    public float rotSpeed;
    // Liste für das Speichern der Wegpunkte
    public List<Vector3> wayPointPosition;
    // Variable für Speicherung des aktuellen Wegpunktlistenindexes
    int currentWaypoint = 0;
    // Variable für Speicherung der Differenz zwischen Wegpunkt und aktueller Position
    Vector3 targetPositionDelta;
    // Variable für die Bewegung des Stampfers
    Vector3 movDirection = Vector3.zero;
    // Variable zur Speicherung der aktuellen Geschwindigkeit
    float currentSpeed = 0;
    // Variable zur Speicherung der negierten Rollgeschwindigkeit
    float rotSpeedNeg;
    #endregion

    // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Startwegpunkt ermitteln
        Vector3 start = transform.position;
        start.x ++;
        wayPointPosition.Add(start);
        // Endwegpunkt ermitteln
        Vector3 end = transform.position;
        end.x -= 5f;        
        wayPointPosition.Add(end);
        // Rollgeschwindigkeiten ermitteln
        rotSpeedNeg -= rotSpeed;
        currentSpeed = rotSpeedNeg;
    }

    // Update aktualisiert das Verhalten und wird jedes Einzelbild aufgerufen
    void Update()
    {
        // Aufruf der Hilfsmethode zur Wegpunktermittlung
        WaypointWalk();
        // Aufruf der Hilfsmethode für die Bewegung des Gegners
        Move();
    }

    // Methode wird aufgerufen, wenn ein anderer Collider in den Trigger eintritt
    void OnTriggerEnter(Collider other) 
    {
        // Prüfung, ob zugehöriges Gameobject des Colliders der Spielfigur gehört
        // und ob die Spielfigur springt
        if (other.gameObject.tag.Equals("Player") && !other.gameObject.GetComponent<CharacterController>().isGrounded)
        {
            // Zerstörung des Gegner-Gameobjects
            Destroy(gameObject);
        }
    }

    // Methode ermittelt aktuellen Wegpunktindex
    void WaypointWalk() 
    {
        // Zielwegpunkt deklarieren
        Vector3 targetPosition = wayPointPosition[currentWaypoint];
        // aktuelle Differenz ermitteln
        targetPositionDelta = targetPosition - transform.position;
        // Überprüfung der Annäherung an Zielwegpunkt mittels Differenz
        if (targetPositionDelta.sqrMagnitude <= 1)
        {
            // erhöhen des Zielpunktindexes
            currentWaypoint++;
            // zurücksetzen des Wegpunktindexes, wenn dieser größer als  die Wegpunktanzahl ist
            // bzw. gleich der Wegpunktanzahl ist
            // und aktualisieren der aktuelle Rollgeschwindigkeit
            if (currentWaypoint >= wayPointPosition.Count)
            {
                currentWaypoint = 0;
                currentSpeed = rotSpeed;
            }
            else 
            {
                currentSpeed = rotSpeedNeg;
            }
        }
    }

    // Methode ermöglicht die Bewegung des Gegners anhand der Wegpunkte
    void Move() 
    {
        // Berechnung der Bewegungsrichtung
        movDirection = targetPositionDelta.normalized * movSpeed;
        // Bewegungsrichtung auf Gameobject anwenden
        transform.Translate(movDirection * Time.deltaTime, Space.World);
        // Gameobject entsprechend rollen lassen
        transform.Rotate(Vector3.back * currentSpeed * Time.deltaTime, Space.World);
    }
}