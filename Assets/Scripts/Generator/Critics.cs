/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Die Klasse erzeugt eine Levelliste mit Komponenten, die den Kriterien am Besten gerecht wird.
/// </summary>
class Critics
{
    #region Variablen
    // Instanzvariablen
    // Instanzvariable für die Speicherung des Rhythmus
    Rhythm rhythm;
    // Liste mit allen Komponenten der Moving-Gruppe
    List<int[]> moveTile;
    // Liste mit allen Komponenten der Jumping-Gruppe
    List<int[]> jumpTile;
    // aktuelle Distanz, wird zur Berechnung der Gesamtnullliniendistanz verwendet
    int currentDistance;
    //  Instanzvariable für die Speicherung des erzeugten Levels mit seinen Komponenten
    List<int> levelList;
    // Instanzvariable für die Speicherung der Start- und Endpositionen von komplexer Multiwege
    List<int[]> secLevel;
    // Instanzvariable für die Speicherung der Nullliniendistanzen aller Levelkomponenten
    List<int> distanceList;

    // Dictonary mit allen vorkommenden Sprüngen und deren Position
    Dictionary<int, int> jumps;
    // Dictonary mit allen Komponenten der Jumping- und Movinggruppe und deren Nulllininendistanz
    Dictionary<int, int> tilesWithDistance = new Dictionary<int,int>();
    // rnd-Parameter u. a. fürs Auswählen
    System.Random rnd = new System.Random();
    // Liste für die Sprünge, welche Zugang/Ausgang von komplexen Multiwegen sind
    int[] goodJumps = { 200, 100, 101, 102, 103, 104, 105, 114, 115 };
    #endregion
   
    #region Konstruktor und Get-Methoden
    // Konstruktor der Klasse, erhält eine Instanz der Klasse Rhythm als Parameter
    public Critics(Rhythm r) 
    {
        // Initialisierung der Instanzvariablen
        moveTile = new List<int[]>();
        jumpTile = new List<int[]>();
        rhythm = r;
        currentDistance = 50;
        levelList = new List<int>();
        secLevel = new List<int[]>();
        distanceList = new List<int>();
        // Füllung der Listen moveTile und jumpTile
        FillMoveList();
        FillJumpList();
    }

    // Methode gibt die Levelliste mit den kodierten Komponenten zurück
    public List<int> GetLevelList() 
    {
        return levelList;
    }

    // Methode gibt die Liste mit den Start- und Endpostionen komplexer Multiwege zurück
    public List<int[]> GetSecWayList()
    {
        return secLevel;
    }

    // Methode gibt eine Liste mit allen Nullliniendistanzen und deren Komponenten der
    // Gruppe Jumping und Moving zurück
    public Dictionary<int, int> GetTileDistance() 
    {
        return tilesWithDistance;
    }
    #endregion
    
    // Methode erzeugt eine Levelliste mit Komponenten, die auf verschiedene Kriterien geprüft wurden
    public void CreateLevel() 
    {
        // Bestimmung der Wiederholungsanzahl
        int iter = GetIteration();
        // Aktionsliste des Rhythmus 
        Dictionary<float, float> actions = rhythm.GetMoveActionQueue();

        // Hilfsvariablen
        // Initialisierung einer Levelliste für den Generierungsprozess
        List<int> currentLevel = new List<int>();
        // Initalisierung einer Sprungliste für den Generierungsprozess
        Dictionary<int, int> tempJumps = new Dictionary<int, int>();
        
        // Festlegung des Minimums der Nullliniendistanz
        int min = rhythm.GetRhythmLength() - 5;
        // Generierungsprozess
        for (int j = 0; j < 1000; j++)
        {
            // Prüfung, ob Nullliniendistanz größer als die Bedingungen ist
            if (Math.Abs(currentDistance) > rhythm.GetRhythmLength() || Math.Abs(currentDistance) > min)
            {
                // zurücksetzen der Distanz
                currentDistance = 0;
                // Löschen der Elemente der Listen
                currentLevel.Clear();
                tempJumps.Clear();
                distanceList.Clear();
                // Hilfszählvariable für Sprungpositionen
                int count = 0;
                // Aneinanderreihung von Rhythmusgruppen
                for (int i = 0; i < iter; i++)
                {
                    // Durchlaufen der Aktionsliste des Rhythmus
                    foreach (KeyValuePair<float, float> pair in actions)
                    {
                        float currentValue = pair.Value;
                        // aktuelle Aktion gehört zur Moving-Gruppe
                        if (currentValue == 1)
                        {
                            // Bestimmung einer Komponente mit Distanz
                            int[] currentMove = GetNextMove();
                            // Hinzufügung nur der Komponente zur Levelliste
                            currentLevel.Add(currentMove[0]);
                            // Aktualisierung der Gesamtdistanz
                            currentDistance += currentMove[1];
                            // Hinzufügung der Distanz der Komponente zur Distanzliste des Levels
                            distanceList.Add(currentMove[1]);
                        }
                        // aktuelle Aktion gehört zur Jumping-Gruppe
                        else
                        {
                            // Bestimmung einer Komponente mit Distanz
                            int[] currentJump = GetNextJump();
                            // Hinzufügung nur der Komponente zur Levelliste
                            currentLevel.Add(currentJump[0]);
                            // Aktualisierung der Gesamtdistanz
                            currentDistance += currentJump[1];
                            // Hinzufügung der Distanz der Komponente zur Distanzliste des Levels
                            distanceList.Add(currentJump[1]);

                            // Prüfung, ob Sprungkomponente zur den Sprüngen gehört, die einen 
                            // komplexen Multiweg begünstigen
                            if (goodJumps.Contains<int>(currentJump[0]))
                            {
                                tempJumps.Add(count, currentJump[0]);
                            }
                        }
                        count++;
                    }
                    // Hinzufügung einer Zwischenkomponente nach Generierung einer Rhythmusgruppe
                    currentLevel.Add(9);
                    count++;
                }
            }   
        }
        // Speicherung der gefundenen Levelliste in Instanzvariable
        levelList = currentLevel;
        // Speicherung aller gefundenen Sprünge im Level
        jumps = tempJumps;
        // Erzeugung/Findes eines komplexen Multiweges
        FindSecWays();
    }

    // Methode findet anhand der vorhandenen Sprünge ggf. Multiwege
    void FindSecWays()
    {
        // Vorgängersprung festlegen
        int previousKey = jumps.Keys.First<int>();
        // Vorgängerposition festlegen
        int previousValue = jumps.Values.First<int>();
        // Hilfsvariable zur Speicherung der gefundenen Wege
        List<int[]> possWays = new List<int[]>();
        // durchlaufen aller Sprünge im Level
        foreach (KeyValuePair<int, int> pair in jumps)
        {
            // aktuellen Sprung festlegen
            int currentKey = pair.Key;
            // Prüfung, ob Sprünge mind. 5 aber maximal 10 Komponenten auseinanderliegen
            // sowie ob der Vorgängersprung nicht ungünstige Startkomponenten hat
            if (currentKey - previousKey >= 5 && currentKey - previousKey < 11 && previousValue != 115 && previousValue != 105 && previousValue != 114)
            {
                // Hinzufügung des gefundenen Weges
                int[] currentPair = { previousKey, currentKey };
                possWays.Add(currentPair);
            }
            // Aktualisierung der Vorgängerwerte
            previousKey = currentKey;
            previousValue = pair.Value;
        }
       
        // Prüfung, ob es mehr als drei gefundene Wege gibt
        if (possWays.Count > 3)
        {
            // Auswahl der ersten drei Wege an ungerade Position
            List<int[]> ways = new List<int[]>();
            for (int i = 0; i < possWays.Count; i++)
            {
                if (i % 2 == 1)
                {
                    ways.Add(possWays[i]);
                }
            }
            // Speichrung der gefundenen Multiwege in der entsprechenden Instanzvariablen
            secLevel = ways;
        }
        else
        {
            // Speichrung der gefundenen Multiwege in der entsprechenden Instanzvariablen
            secLevel = possWays;
        }
    }

    #region Hilfsmethoden
    // Methode füllt die Liste moveTile mit Elementen aus Integerarrays,
    // diese Arrays besitzen zwei Elemente [Code, Distanz]
    void FillMoveList()
    {
        // Liste mit Distanzen entsprechend er Reihenfolge der Kodierung
        // der Moving-Komponenten
        int[] movesDistances = { 0, 5, -5, 3, -3, 0};
        // Zuordnung der Distanzen zu den Komponenten
        for (int i = 0; i < 6; i++) 
        {
            int[] temp = {i, movesDistances[i]};
            // hinzufügen zur Liste
            moveTile.Add(temp);
            // Füllung der Liste aller Komponenten mit den Werten der Moving-Gruppe
            if (tilesWithDistance.Count < 7) 
            {
                tilesWithDistance.Add(i, movesDistances[i]);
            }
        }
    }

    // Methode füllt die Liste jumpTile mit Elementen aus Integerarrays,
    // diese Arrays besitzen zwei Elemente [Code, Distanz]
    void FillJumpList()
    {
        for (int i = 0; i < 4; i++)
        {
            // Kodierung der Sprungkombination
            if (i == 1)
            {
                for (int f = 0; f < 2; f++)
                {
                    // Liste der Distanzen für die Sprungkombinationen
                    int[] jumpDistance = { 1, -2, -4, -6, 7, -7};
                    for (int s = 0; s < 6; s++)
                    {
                        // Erzeugung der Kodierung der Komponenten
                        string temp = "" + i + f + s;
                        int jumpNumber = Convert.ToInt32(temp);
                        int[] tempJ = { jumpNumber, jumpDistance[s] };
                        // hinzufügen zur Liste 
                        jumpTile.Add(tempJ);
                        // Füllung der Liste aller Komponenten mit den Werten der Jumping-Gruppe
                        // hier mit den Sprungkombinationen
                        if (tilesWithDistance.Count < 22) 
                        {
                            tilesWithDistance.Add(jumpNumber, jumpDistance[s]);
                        }
                    }
                }
            }
            else
            {
                // Erzeugung der Kodierung und hinzufügen zur Liste
                int[] temp = { i + 200, 0};
                jumpTile.Add(temp);
                // Füllung der Liste aller Komponenten mit den Werten der Jumping-Gruppe
                // hier der einfachen Sprünge
                if (tilesWithDistance.Count < 21)
                {
                    tilesWithDistance.Add((i+200), 0);
                }                
            }
        }
        // Füllung der Liste aller Komponenten, hier mit der Zwischenkomponente
        if (tilesWithDistance.Count == 21) 
        { 
            tilesWithDistance.Add(9, 0);
        }        
    }

    // Methode bestimmt die Anzahl an Wiederholungen der Rhythmusgruppe anhand der Rhythmuslänge
    int GetIteration() 
    {
        // Standartwert für Wiederholung
        int iter = 1;
        // Zuordnung zu einer Konstante anhand der Rhythmuslänge
        switch (rhythm.GetRhythmLength()) 
        {
            case 5:  iter = 9; break;
            case 10: iter = 3; break;
            case 15: iter = 3; break;
            case 20: iter = 3; break;
            default: break;
        }
        return iter;
    }

    // Methode bestimmt nächste Move-Komponente mit dessen Nullliniendistanz
    int[] GetNextMove() 
    { 
        // Festlung Standardwertes
        int[] move = {0,0};

        // Prüfung, ob Liste leer ist
        if (moveTile.Count == 0)
        {
            // Auffüllung der Liste
            FillMoveList();
            // zufälligen Index erzeugen
            int index = rnd.Next(0, moveTile.Count);
            // anhand es Indexes Element auswählen und aus der Liste löschen
            move = moveTile[index];
            moveTile.Remove(move);
        }
        // Liste ist nicht leer
        else 
        {
            // zufälligen Index erzeugen
            int index = rnd.Next(0, moveTile.Count);
            // anhand es Indexes Element auswählen und aus der Liste löschen
            move = moveTile[index];
            moveTile.Remove(move);
        }
        return move;
    }

    // Methdoe bestimmt nächste Jump-Komponente mit dessen Nullliniendistanz
    int[] GetNextJump() 
    {
        // Festlung Standardwertes
        int[] jump = { 0, 0 };

        // Prüfung, ob Liste leer ist
        if (jumpTile.Count == 0)
        {
            // Auffüllung der Liste
            FillJumpList();
            // zufälligen Index erzeugen
            int index = rnd.Next(0, jumpTile.Count);
            // anhand es Indexes Element auswählen und aus der Liste löschen
            jump = jumpTile[index];
            jumpTile.Remove(jump);
        }
        // Liste ist nicht leer
        else
        {   // zufälligen Index erzeugen
            int index = rnd.Next(0, jumpTile.Count);
            // anhand es Indexes Element auswählen und aus der Liste löschen
            jump = jumpTile[index];
            jumpTile.Remove(jump);
        }
        return jump;
    }
    #endregion
}

