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
/// Die Klasse bestimmt das Verhalten von Stampfern.
/// </summary>
public class Stomper : MonoBehaviour
{
    #region Variablen
    // Variable für das Anpassen der Fallgeschwindigkeit
    public float downSpeed = 3f;
    // Variable für das Anpassen der Steiggeschwindigkeit
    public float upSpeed = 3f;
    
    // Liste für das Speichern der Wegpunkte
    public List<Vector3> wayPointPosition;
    // Variable für Speicherung des aktuellen Wegpunktlistenindexes
    int currentWaypoint = 0;
    // Variable für Speicherung der Differenz zwischen Wegpunkt und aktueller Position
    Vector3 targetPositionDelta;
    // Variable für die Bewegung des Stampfers
    Vector3 movDirection = Vector3.zero;
    #endregion

    // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Startwegpunkt ermitteln
        Vector3 start = transform.position;
        start.y--;
        wayPointPosition.Add(start);
        // Endwegpunkt ermitteln
        Vector3 end = transform.position;
        end.y += 4f;
        wayPointPosition.Add(end);
    }

    // Update aktualisiert das Verhalten und wird jedes Einzelbild aufgerufen
    void Update()
    {
        // Aufruf der Hilfsmethode zur Wegpunktermittlung
        WaypointWalk();
        // Aufruf der Hilfsmethode für die Bewegung des Stampfers
        Move();
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
            if (currentWaypoint >= wayPointPosition.Count)
            {
                currentWaypoint = 0;
            }
        }
    }

    // Methode ermöglicht die Bewegung des Stampfers anhand der Wegpunkte
    void Move()
    {
        // ist der Wegpunktindex 0, bewegt sich der Stampfer nach unten
        if (currentWaypoint == 0)
        {
            movDirection = targetPositionDelta.normalized * downSpeed;
            transform.Translate(movDirection * Time.deltaTime, Space.World);
        }
        // ist der Wegpunkt ungleich 0, bewegt sich der Stampfer nach oben
        else
        {
            movDirection = targetPositionDelta.normalized * upSpeed;
            transform.Translate(movDirection * Time.deltaTime, Space.World);
        }
    }
}