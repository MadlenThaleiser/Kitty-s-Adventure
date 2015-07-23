/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

/*
 * Die bewegenden Plattformen wurden nicht in das Spiel intregiert. Dies liegt daran, dass die 
 * Spielfigur zwar mit den beweglichen fahren kann, jedoch nach dem Verlassen der beweglichen
 * Plattformen durch die stationären hindurchfällt.
 * Eine Lösung dieses Problemes ist bislang nicht gefunden worden, weshalb diese Komponenten bei
 * der Erzeugung von Leveln nicht berücksichtigt wurden.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Die Klasse bestimmt das Verhalten von bewegenden Plattformen.
/// </summary>
public class MovingPlatforms : MonoBehaviour
{
    #region Variablen
    // Variable zur Kontrolle der Bewegungsgeschwindigkeit
    public float moveSpeed;
    // Variablen zur Steuerung der Richtung, in der sich die Plattform bewegen soll
    public bool up;
    public bool right;
    
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
        // Plattform bewegt sich horizontal
        if (right)
        {
            // Startwegpunkt ermitteln
            Vector3 start = transform.position;
            start.x--;
            wayPointPosition.Add(start);
            // Endwegpunkt ermitteln
            Vector3 end = transform.position;
            end.x += 5f;
            wayPointPosition.Add(end);
        }
        else 
        {
            // Plattform bewegt sich vertikal aufwärts
            if (up)
            {
                // Startwegpunkt ermitteln
                Vector3 start = transform.position;
                start.y--;
                wayPointPosition.Add(start);
                // Endwegpunkt ermitteln
                Vector3 end = transform.position;
                end.y += 7f;
                wayPointPosition.Add(end);
            }
            // Plattform bewegt sich vertikal abwärts
            else 
            {
                // Startwegpunkt ermitteln
                Vector3 start = transform.position;
                start.y++;
                wayPointPosition.Add(start);
                // Endwegpunkt ermitteln
                Vector3 end = transform.position;
                end.y -= 7f;
                wayPointPosition.Add(end);
            }
        }
    }

    // Methode wird aufgerufen, wenn ein anderer Collider in den Trigger eintritt
    void OnTriggerEnter(Collider other) 
    {
        // Spielfigur betritt die Plattform
        if (other.gameObject.tag.Equals("Player"))
        {
            // Spielfigur wird zum Kindobject der Plattform
            other.transform.parent = transform;
        }
    }
    
    // Methode wird aufgerufen, wenn ein anderer Collider den Trigger verlässt    
    void OnTriggerExit(Collider other)
    {
        // Spielfigur verlässt die Plattform
        if (other.gameObject.tag.Equals("Player"))
        {
            // Spielfigur ist nicht mehr Kindobject der Plattform
            other.transform.parent = null;
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
            if (currentWaypoint >= wayPointPosition.Count)
            {
                currentWaypoint = 0;
            }
        }
    }

    // Methode ermöglicht die Bewegung der Plattformen anhand der Wegpunkte
    void Move()
    {
        // ist der Wegpunktindex 0, bewegt sich die Plattform zum Startpunkt
        if (currentWaypoint == 0)
        {
            movDirection = targetPositionDelta.normalized * moveSpeed;
            transform.Translate(movDirection * Time.deltaTime, Space.World);
        }
        // ist der Wegpunktindex 0, bewegt sich die Plattform zum Endpunkt
        else
        {
            movDirection = targetPositionDelta.normalized * moveSpeed;
            transform.Translate(movDirection * Time.deltaTime, Space.World);
        }
    }
}