/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Die Klasse erzeugt einen Rhythmus auf Basis von drei Parametern.
/// </summary>
class Rhythm
{
    #region Variablen
    // Instanzvaribale für die Speicherung des Rhythmuslängenwertes
    float rhythmLength;
    // Instanzvariable für die Speicherung des Rhythmusdichtewertes
    float rhythmDensity;
    // Dictonary zum Speichern der Bewegungsmanöver in Form eines Rhythmuses
    Dictionary<float, float> moveActionQueue;
    #endregion

    #region Konstruktor und Get-Methoden
    // Konstruktor der Klasse, erhält die Rhythmuseigenschaftsparamater 
    public Rhythm(float l, string d, string t){
        // abhängig zum Typ werden Variablen initialisiert
        // Initialisierung im Fall eines regular-Rhythmus
        if (t.Equals("regular")){
            rhythmLength = l;
            rhythmDensity = SetDensity(rhythmLength, d, t);
            moveActionQueue = new Dictionary<float, float>();
            // Generierung eines Rhythmus vom Typ regular
            RegularGenerate();
        }
        // Initialisierung im Fall eines swing-Rhythmus
        if (t.Equals("swing")){
            rhythmLength = l;
            rhythmDensity = SetDensity(rhythmLength, d, t);
            moveActionQueue = new Dictionary<float, float>();
            // Generierung eines Rhythmus vom Typ regular
            SwingGenerate(d);
        }        
    }

    // Get-Methode zur Rückgabe des generiereten Rhythmus
    public Dictionary<float, float> GetMoveActionQueue() 
    {
        return moveActionQueue;
    }

    // Get-Methode zur Rückgabe der Rhythmuslänge
    public int GetRhythmLength() 
    {
        return Mathf.RoundToInt(rhythmLength);
    }
    #endregion
    
    // Methode zur Bestimmung der Dichte für spätere Berechnungen
    float SetDensity(float length, string density, string type) 
    {
        // Standardwert für die Dichte
        float den = 1f;
        // Unterscheidung für späterere Zuordnung anhand von type
        // Zuordnung der Dichte beim Typ regular
        if (type.Equals("regular"))
        {
          switch (density) { 
            case "low":
                  if (length == 5f)  { den = 3f; }
                  if (length == 10f) { den = 4f; }
                  if (length == 15f) { den = 5f; }
                  if (length == 20f) { den = 6f; }
                break;
            case "medium":
                if (length == 5f)  { den = 2f; }
                if (length == 10f) { den = 3f; }
                if (length == 15f) { den = 3f; }
                if (length == 20f) { den = 4f; }
                break;
            case "high":
                if (length == 5f)  { den = 1f; }
                if (length == 10f) { den = 2f; }
                if (length == 15f) { den = 2f; }
                if (length == 20f) { den = 2f; }
                break;
            default:
                break;
          }
        }

        // Zuordnung der Dichte beim Typ swing
        if (type.Equals("swing")) 
        {
            switch (density)
            {
                case "low":
                    if (length == 5f)  { den = 2f; }
                    if (length == 10f) { den = 5f; }
                    if (length == 15f) { den = 4f; }
                    if (length == 20f) { den = 6f; }
                    break;
                case "medium":
                    if (length == 5f)  { den = 1f; }
                    if (length == 10f) { den = 3f; }
                    if (length == 15f) { den = 4f; }
                    if (length == 20f) { den = 3f; }
                    break;
                case "high":
                    if (length == 5f)  { den = 0f; }
                    if (length == 10f) { den = 1f; }
                    if (length == 15f) { den = 1f; }
                    if (length == 20f) { den = 1f; }
                    break;
                default:
                    break;
            }
        }
        return den;    
    }
    
    // Methode berechnet einen Rhythmus vom Typ regular
    void RegularGenerate() 
    {
        // Rhythmus wird anhand der Länge berechnet
        // Stellen der Sprungtakte werden durch Dichte bestimmt
        int d = Mathf.RoundToInt(rhythmDensity);
        // Erzeugung von Elementen anhand der Rhythmuslänge für die moveActionQueue, welches den Rhythmus darstellt
        for (int i = 0; i < rhythmLength; i++)
        {
            // Prüfung, ob aktuelles Element eine Sprungstelle ist
            if (i == d)
            {
                // Sprungstellen werden mit 0 in der moveActionQueue markiert
                moveActionQueue.Add(i, 0);
                // Berechnung der nächsten Sprungstelle
                d += Mathf.RoundToInt(rhythmDensity);
            }
            else
            {
                // normale Bewegungsstellen werden mit 1 in der moveActionQueue markiert
                moveActionQueue.Add(i, 1);
            }
        }
    }

    // Methode berechnet einen Rhythmus vom Typ swing
    void SwingGenerate(string den) 
    {
        // Standardwerte für die Swing-Variablen werden festgelegt
        // Varibale für die erste Sprungstelle
        int s1 = Mathf.RoundToInt(rhythmDensity);
        // Varibale für die zweite Sprungstelle
        int s2 = s1 + 1;
        // Abstand zwischen den einzelnen Swing-Sprüngen
        int dist = 1;
        // zusätzlicher Elemente in der Liste, damit Swing auch beendet wird
        int count_add = 1;

        // Festlegung der Swingvariablen anhand der Länge
        switch (Mathf.RoundToInt(rhythmLength)) 
        { 
            case 5:
                switch (den)
                {
                    case "low":     dist = 4; count_add = 1; break;
                    case "medium":  dist = 3; count_add = 2; break;
                    case "high":    dist = 3; count_add = 4; break;
                    default: break;
                }
                break;
            case 10:
                switch (den)
                {
                    case "low":     dist = 5; count_add = 0; break;
                    case "medium":  dist = 4; count_add = 1; break;
                    case "high":    dist = 3; count_add = 3; break;
                    default: break;
                }
                break;
            case 15:
                switch (den)
                {
                    case "low":     dist = 7; count_add = 2; break;
                    case "medium":  dist = 5; count_add = 2; break;
                    case "high":    dist = 3; count_add = 1; break;
                    default: break;
                }
                break;
            case 20:
                switch (den)
                {
                    case "low":     dist = 7; count_add = 3; break;
                    case "medium":  dist = 4; count_add = 2; break;
                    case "high":    dist = 3; count_add = 2; break;
                    default: break;
                }
                break;
            default:
                break;
        }

        // Erzeugung von Elementen anhand der Rhythmuslänge für die moveActionQueue, welches den Rhythmus darstellt        
        for (int i = 0; i < rhythmLength + count_add; i++)
        {
            // Prüfung, ob aktuelles Element erste Sprungstelle ist
            if (i == s1)
            {
                // Sprungstellen werden mit 0 in der moveActionQueue markiert
                moveActionQueue.Add(i, 0);
                // nächste erste Sprungstelle wird berechnet
                s1 += dist;
            }
            // Prüfung, ob aktuelles Element zweite Sprungstelle ist
            else if (i == s2) 
            {
                // Sprungstellen werden mit 0 in der moveActionQueue markiert
                moveActionQueue.Add(i, 0);
                // nächste zweite Sprungstelle wird berechnet
                s2 += dist;
            }
            else
            {
                // normale Bewegungsstellen werden mit 1 in der moveActionQueue markiert
                moveActionQueue.Add(i, 1);
            }
        }        
    }
}
