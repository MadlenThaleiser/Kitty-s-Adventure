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
/// Die Klasse verwaltet die Generierung von Level, welche basierend auf der
/// Arbeit von Smith et al. erfolgt.
/// </summary>
public class GeneratorManager : MonoBehaviour
{
    #region Variablen
    // Variable für die Speicherung des PreFabs der Spielfigur
    public GameObject playerPrefab;
    // Variable für die Speicherung aller PreFabs von Levelkomponenten
    public List<GameObject> tileSet;
    // Variablen zur Speicherung der Rhythmusparametereigenschaften
    public string rhythmType;
    public string rhythmDensity;
    public float rhythmLength;
    // Koordinaten für den Beginn der Generierung
    public float startX;
    public float startY;
    // Vektor für das Positionieren der Komponenten
    Vector3 spawnPosition;
    // Variable für die Erzeugung eines Levels, das auf Kriterien geprüft wurde
    Critics levelWithCritics;
    // vorhergenede Komponente für Prüfung innerhalb des simplen Multiweges
    float previousTile = -1f;
    // Liste mit Start- und Endvektoren von komplexen Multiwegen
    List<Vector3> sndWayStartEnds = new List<Vector3>();
    // rnd-Parameter für die Auswahlmethoden
    System.Random rnd = new System.Random();  
    #endregion

    // Awake wird für die Initialiserung verwendet, ehe ein Spiel gestartet ist
    void Awake()
    {  
        // Initialisieurng des Positionsvektors
        spawnPosition = new Vector3(startX, startY, 0f);
        // Erzeugung der Startplattform mit der Spielfigur
        CreateLeftSpace(6f, spawnPosition);
        spawnPosition = CreateStartEndBetweenTile(5, spawnPosition);
        Instantiate(playerPrefab, new Vector3(-2f, 1f, 0f), Quaternion.identity);
     }

    // Start wird für die Initialisierung verwendet
    void Start()
    {
        // Wertzuweisung der Rhythmusparameter durch die gewählten Werte vom Generatormenü
        rhythmLength = float.Parse(PlayerPrefs.GetString("length"));
        rhythmDensity = PlayerPrefs.GetString("density");
        rhythmType = PlayerPrefs.GetString("type");
        // Erzeugung eines Rhythmus
        Rhythm rhythm = new Rhythm(rhythmLength, rhythmDensity, rhythmType);
        // Generierung eines Levels basierend auf dem Rhythmus
        levelWithCritics = new Critics(rhythm);
        levelWithCritics.CreateLevel();
        spawnPosition = CreateLevel(spawnPosition);
        CreateRightSpace(6f, spawnPosition);
        // Lücken deaktivieren, die den Zugang zum Multiweg behindern
        DeactivateGaps();
    }

    // Methode generierit ein Level, wobei eine Liste mit allen Komponenten durchgegangen wird
    Vector3 CreateLevel(Vector3 vector)
    {
        // erhalten der Levelliste
        List<int> level = levelWithCritics.GetLevelList();
        // Liste mit Start- und Endpositionen des komplexen Multiweges 
        List<int[]> multiPathsStartsEnds = levelWithCritics.GetSecWayList();
        // Zählvaribale für die aktuelle Anzahl der Komponenten
        int count = 0;
        // Startvektor für die Positionierung des Multiweges
        Vector3 multiPathStart = Vector3.zero;
        // Information auf Bildschirm über komplexen Multiweg
        if (multiPathsStartsEnds.Count > 0)
        {
            GameManager.multiWeg = "true";
        }
        // Durchlaufen aller Levelkomponenten
        foreach (int tile in level)
        {
            // Prüfung, ob komplexe Multiwege vorhanden sind
            if (multiPathsStartsEnds.Count > 0)
            {
                // Zählvariable ist xte Komponente und startet einen komplexen Multiweg
                if (count == multiPathsStartsEnds[0][0])
                {
                    // Berechnung des Vektors für die Positionierung der Multiwegkomponenten
                    multiPathStart = vector;
                    multiPathStart.y -= 10f;
                    multiPathStart.x++;
                    // Generierung eines komplexen Multiweges
                    CreateComplexPath(multiPathsStartsEnds[0], multiPathStart);
                }
                // Zählvariable ist xte Komponente und beendet einen komplexen Multiweg
                if (count == multiPathsStartsEnds[0][1])
                {
                    // Löschung verwendeter Start- und Endpositionen des Multiweges
                    multiPathStart = Vector3.zero;
                    int[] tmp = multiPathsStartsEnds[0];
                    multiPathsStartsEnds.Remove(tmp);
                }
            }
            // Zuordnung der Komponenten des Hauptweges
            vector = AssignTile(tile, vector);
            // Speicherung des vorhergehenden Komponente für Prüfung eines simplen Multiweges
            previousTile = tile;
            count++;
        }
        // Erzeugung der Zielkomponente des Levels
        Instantiate(tileSet[13], new Vector3(vector.x, vector.y + 1, 0), Quaternion.identity);
        return vector;
    }

    // Methode dekodiert Komponenten und ordnet dieser entsprechende Auswahlmethode zu
    Vector3 AssignTile(int tile, Vector3 vector)
    {
        // Deklaration von Variablen für Auswahl
        int jumpVar = 0;
        // Prüfung, ob Komponente zur Moving-Gruppe gehört
        if (tile < 10)
        {
            // Komponente stellt Zwischenstück dar
            if (tile == 9)
            {
                vector = CreateStartEndBetweenTile(3, vector);
            }
            else
            {   // Komponente stellt einen Stampfer da
                if (tile == 5)
                {
                    vector = CreateStomper(vector);
                }
                // Komponente stellt eine Bewegungskomponente da
                else
                {
                    vector = ChooseMove(tile, vector);
                }
            }
        }
        // Komponente gehört zur Jumping-Gruppe
        else
        {   // Komponente ist ein einfacher Sprung
            if (tile >= 200)
            {
                jumpVar = tile - 200;
                vector = ChooseJump(jumpVar, vector);
            }
            // Komponente ist ein zusammengesetzter Sprung
            else
            {
                // erster Teil hat eine Lücke
                jumpVar = tile - 100;
                if (jumpVar < 10)
                {
                    vector = ChooseJumpCombi(0, jumpVar, vector);
                }
                // erster Teil hat keine Lücke 
                else
                {
                    jumpVar -= 10;
                    vector = ChooseJumpCombi(1, jumpVar, vector);
                }
            }
        }
        return vector;
    }

    // Generierung eines komplexen Multiweges durch Erstellung eines Teilstückes des Hauptweges
    void CreateComplexPath(int[] way, Vector3 vector) 
    {
        // Levelliste mit allen Komponenten des Levels
        List<int> level = levelWithCritics.GetLevelList();
        //  Nullliniendistanzliste aller Komponenten des Levels
        Dictionary<int, int> distanceList = levelWithCritics.GetTileDistance();
        // Berechnung der Komponentenanzahl des Multiweges
        int tileCount = way[1] - way[0];
        // Teilliste des Hauptweges
        List<int> subLevel = level.GetRange(way[0], tileCount+1);
        // Hinzufügung des Startvektors zur Liste aller Start/Endvektoren des Multiwegess
        sndWayStartEnds.Add(vector);
        // Deklaration des Startvektors als Startpunkt für die spätere Generierung einer großen Lücke
        Vector3 startLargeGap = vector;
        // Festlegung der aktuellen kleinsten y-Koordinate für die spätere Generierung einer großen Lücke
        float min = startLargeGap.y;
        // Erzeugung der Startkomponente des Multiweges
        vector = CreateMultiStart(vector);
        // Standardwerte für einige Variablen festlegen
        // Anzahl Plattformen für die Endkomponente
        int numberPlatforms = 2;
        // Anzahl von Komponenten, die nicht erzeugt werden
        int notConsidered = 1;
        
        // Liste mit Sprungkomponenten, die schwierig zu handhaben sind
        List<int>jumpDown = new List<int>{101, 102, 103};
        // Prüfung, ob letztes Element in der Liste mit den schwierigen Sprungkomponenten vorkommt
        // und Anpassung der Variablen daran
        if (jumpDown.Contains(subLevel[subLevel.Count-1]))
        {
            notConsidered = 2;
            numberPlatforms = 4;
        }
        // Prüfung, ob letzte Komponente eine Spring- oder eine short_jump_down-Komponente ist
        // und Anpassung der Variablen daran
        if (subLevel[subLevel.Count - 1] == 114 || subLevel[subLevel.Count - 1] == 101)
        {
            numberPlatforms = 5;
        }
        
        // Durchgehen aller Komponenten des Levelteilstückes
        for (int i = 0; i < subLevel.Count-notConsidered; i++)
        {
            // aktuelle Komponente
            int tile = subLevel[i];            
            // Nullliniendistanz der Komponente ermitteln
            int distanceOfTile = distanceList[tile];
            // Ermittlung sämtlicher Komponenten mit gleicher Nullliniendistan
            List<int> possTile = GetAllTiles(distanceOfTile, distanceList);
            // eine Zufallsnummer generiren für die Auswahl einer Komponente
            int index = rnd.Next(0, possTile.Count);
            vector = AssignTile(possTile[index], vector);
            // Prüfung, ob Minium noch aktuell ist und ggf. Aktualisierung des Minimums
            if (vector.y < min) 
            {
                min = vector.y;
            }
        }
        // Hinzufügung des Endvektors zur Liste aller Start/Endvektoren des Multiweges
        sndWayStartEnds.Add(new Vector3(vector.x+numberPlatforms,vector.y,vector.z));
        // Erzeugung der Endkomponente des Multiweges
        vector = CreateMultiEnd(numberPlatforms, vector);        
        // Berechnung der große Lücken
        // Deklaration des Endvektors für die Berechnung
        Vector3 end = vector;
        // Größe der Lücke berechnen
        float size = end.x - startLargeGap.x + 10;
        // Berechnung der x- und y-Koordinate der großen Lücke
        float x = end.x ;
        if (end.y < min) 
        {
            min = end.y;
        }
        // sErzeugung der großen Lücke
        CreateLeftSpace(size, new Vector3(x, min-2f, 0));
    }
 
    #region Auswahlmethoden
    // Methode wählt Create-Methode  der Moving-Gruppe entsprechend dem Wert m aus
    // m stellt den dekodierten Wert der Komponente dar
    Vector3 ChooseMove(int m, Vector3 vector)
    {
        switch (m)
        {
            case 0:
                vector = CreateFlatPlatform(5, vector); break;
            case 1:
                // Abfrage für Generierung eines simplen Multiweges
                if (previousTile == 2)
                {
                    // simpler Multiweg wird erzeugt
                    CreateSimplePath(5, 3, 5, vector);
                }
                vector = CreateSteepUp(vector); break;
            case 2:
                vector = CreateSteepDown(vector); break;
            case 3:
                // Abfrage für Generierung eines simplen Multiweges
                if (previousTile == 2)
                {
                    // simpler Multiweg wird erzeugt
                    CreateSimplePath(5, 3, 5, vector);
                }
                vector = CreateGradualUp(vector); break;
            case 4:
                vector = CreateGradualDown(vector); break;
            default:
                break;
        }
        return vector;
    }

    // Methode wählt Create-Methode der Jumping-Gruppe für einfache Sprungkomponenten
    // entsprechend dem Wert j aus
    // j stellt den dekodierten Wert der Komponenten dar
    Vector3 ChooseJump(int j, Vector3 vector)
    {
        switch (j)
        {
            case 0:
                vector = CreateFlatGap(vector); break;
            case 2:
                // Abfrage für Generierung eines simplen Multiweges
                if (previousTile == 0)
                {
                    // Anzahl an Blöcken wird bestimmt
                    int c = rnd.Next(0, 4);
                    // simpler Multiweg wird erzeugt
                    CreateSimplePath(c, 2, 3, vector);
                }
                vector = CreateEnemyKill(vector);
                break;
            case 3:
                // Abfrage für Generierung eines simplen Multiweges
                if (previousTile == 0)
                {
                    // Anzahl an Blöcken wird bestimmt
                    int c = rnd.Next(0, 4);
                    // simpler Multiweg wird erzeugt
                    CreateSimplePath(c, 1, 3, vector);
                }
                vector = CreateEnemyAvoid(vector);
                break;
            default:
                break;
        }
        return vector;
    }

    // Methode wählt Create-Methode der Jumping-Gruppe für zusammengesetzte Sprungkomponenten 
    // entsprechend dem Werten für first und second aus
    // first und second stellen die dekodierten Werte der Komponententeile dar
    Vector3 ChooseJumpCombi(int first, int second, Vector3 vector)
    {
        // erster Teil der zusammengesetzte Sprungkomponente wird gewählt
        switch (first)
        {
            case 0:
                vector = CreateGap(vector); break;
            case 1:
                // Abfrage für Generierung eines simplen Multiweges
                if (previousTile == 0)
                {
                    // Anzahl an Blöcken wird bestimmt
                    int c = rnd.Next(0, 4);
                    // simpler Multiweg wird erzeugt
                    CreateSimplePath(c, 1, 3, vector);
                }
                vector = CreateNoGap(vector);
                break;
            default:
                break;
        }
        // zweiter Teil der zusammengesetzte Sprungkomponente wird gewählt
        switch (second)
        {
            case 0:
                vector = CreateJumpUp(vector); break;
            case 1:
                vector = CreateJumpDownShort(vector); break;
            case 2:
                vector = CreateJumpDownMedium(vector); break;
            case 3:
                vector = CreateJumpDownLong(vector); break;
            case 4:
                vector = CreateSpring(vector); break;
            case 5:
                vector = CreateFall(vector); break;
            default:
                break;
        }
        return vector;
    }
    #endregion

    #region Create-Methoden der Moving-Gruppe
    // Methode erzeugt Plattformen mit der Anzahl count an Blöcken
    Vector3 CreateFlatPlatform(int count, Vector3 vector)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(tileSet[0], vector, Quaternion.identity);
            vector.x++;
        }
        return vector;
    }

    // Methode erzeugt einen sehr steilen Weg hinauf
    Vector3 CreateSteepUp(Vector3 vector)
    {
        vector.y++;
        for (int i = 0; i < 5; i++)
        {
            Instantiate(tileSet[1], vector, Quaternion.identity);
            vector.x++;
            vector.y++;
        }
        vector.y--;
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(1, new Vector3(vector.x - 1f, vector.y + 1.2f, vector.z));
        return vector;
    }

    // Methode erzeugt einen sehr steilen Weg hinab
    Vector3 CreateSteepDown(Vector3 vector)
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(tileSet[2], vector, Quaternion.Euler(270, 0, 0));
            vector.x++;
            vector.y--;
        }
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(1, new Vector3(vector.x - 1f, vector.y + 1.2f, vector.z));
        return vector;
    }

    // Methode erzeugt einen allmählich steil werdenden Weg hinauf
    Vector3 CreateGradualUp(Vector3 vector)
    {
        vector.y++;
        for (int i = 0; i < 3; i++)
        {
            Instantiate(tileSet[3], vector, Quaternion.identity);
            vector.x++;
            Instantiate(tileSet[4], vector, Quaternion.identity);
            vector.x++;
            vector.y++;
        }
        vector.y--;
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(1, new Vector3(vector.x - 1f, vector.y + 1.2f, vector.z));
        return vector;
    }

    // Methode erzeugt einen allmählich steil werdenden Weg hinab
    Vector3 CreateGradualDown(Vector3 vector)
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(tileSet[6], vector, Quaternion.identity);
            vector.x++;
            Instantiate(tileSet[5], vector, Quaternion.identity);
            vector.x++;
            vector.y--;
        }
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(1, new Vector3(vector.x - 1f, vector.y + 1.2f, vector.z));
        return vector;
    }
    
    // Methode erzeugt Stampfer auf einer Plattform
    Vector3 CreateStomper(Vector3 vector)
    {
        vector = CreateFlatPlatform(5, vector);
        vector.x -= 3f;
        vector.y += 2f;
        // Erzeugt den Stampfer aus der Liste aller Prefabs
        Instantiate(tileSet[11], vector, Quaternion.identity);
        vector.x += 3f;
        vector.y -= 2f;
        return vector;
    }
    #endregion

    #region Create-Methoden der Jumping-Gruppe
    // Methode erzeugt eine flache mittelgroße Lücke
    Vector3 CreateFlatGap(Vector3 vector)
    {
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(2, new Vector3(vector.x, vector.y + 2.2f, vector.z));
        vector.x += 3;
        // Leerraum fürs Sterben 
        CreateLeftSpace(2, vector);
        vector = CreateFlatPlatform(1, vector);
        return vector;
    }

    // Methode erzeugt eine kleine Lücke als erste Komponente einer Sprungkombination
    Vector3 CreateGap(Vector3 vector)
    {
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsLine(1, new Vector3(vector.x, vector.y + 2.2f, vector.z));
        vector.x += 2f;
        // Leerraum fürs Sterben 
        CreateLeftSpace(1, vector);
        return vector;
    }

    // Methode erzeugt keine Lücke als erste Komponente einer Sprungkombination
    // erzeugt ggf. einen simplen Multiweg
    Vector3 CreateNoGap(Vector3 vector)
    {
        vector = CreateFlatPlatform(3, vector);
        // Platzierung von Münzen
        CoinsLine(2, new Vector3(vector.x - 2f, vector.y + 1.2f, vector.z));
        // Anzahl an Blöcken wird bestimmt
        int c = rnd.Next(0, 4);
        // simpler Multiweg wird erzeugt
        CreateSimplePath(c, 1, 3, vector);
        return vector;
    }

    // Methode erzeugt einen kleinen Sprung hinauf als zweite Komponente einer Sprungkombination
    Vector3 CreateJumpUp(Vector3 vector)
    {
        vector = CreateFlatPlatform(3, vector);
        vector.x--;
        vector.y++;
        vector = CreateFlatPlatform(3, vector);
        return vector;
    }

    // Methode erzeugt einen kurzen Sprung hinab als zweite Komponente einer Sprungkombination
    Vector3 CreateJumpDownShort(Vector3 vector)
    {
        vector = CreateFlatPlatform(1, vector);
        vector = CreateWall(2, vector);
        // Platzierung von Münzen
        CoinsRow(2, true, new Vector3(vector.x + 2f, vector.y + 1.2f, vector.z));
        vector = CreateFlatPlatform(4, vector);
        return vector;
    }

    // Methode erzeugt einen mittellangen Sprung hinab als zweite Komponente einer Sprungkombination
    Vector3 CreateJumpDownMedium(Vector3 vector)
    {
        vector = CreateFlatPlatform(1, vector);
        vector = CreateWall(4, vector);
        // Platzierung von Münzen
        CoinsRow(3, true, new Vector3(vector.x + 2f, vector.y + 1.2f, vector.z));
        vector = CreateFlatPlatform(4, vector);
        return vector;
    }

    // Methode erzeugt einen langen Sprung hinab als zweite Komponente einer Sprungkombination
    Vector3 CreateJumpDownLong(Vector3 vector)
    {
        vector = CreateFlatPlatform(1, vector);
        vector = CreateWall(6, vector);
        // Platzierung von Münzen
        CoinsRow(4, true, new Vector3(vector.x + 2f, vector.y + 1.2f, vector.z));
        vector = CreateFlatPlatform(4, vector);
        return vector;
    }

    // Methode erzeugt ein stationäres Trampolin auf einer Plattform als zweite Komponente einer Sprungkombination
    Vector3 CreateSpring(Vector3 vector)
    {
        vector = CreateFlatPlatform(5, vector);
        vector.x -= 2;
        vector.y += 0.5f;
        vector.z += 0.3f;
        // Erzeugung des Trampolins aus der Lister aller Prefabs
        Instantiate(tileSet[9], vector, Quaternion.Euler(90, -180, 0));
        // Platzierung von Münzen
        CoinsRow(3, true, new Vector3(vector.x, vector.y + 2.5f, vector.z));
        vector.y -= 0.5f;
        vector.z -= 0.3f;
        // Leerraum fürs Sterben 
        CreateRightSpace(6, vector);
        vector.x += 3f;
        vector.y += 7f;
        return vector;
    }

    // Methode erzeugt eine Sturzkomponente(von einer Plattform stürzt man sich zur anderen hinab) 
    // als zweite Komponente einer Sprungkombination
    Vector3 CreateFall(Vector3 vector)
    {
        vector = CreateFlatPlatform(2, vector);
        vector.x += 3f;
        vector.y -= 8f;
        // Leerraum fürs Sterben 
        CreateLeftSpace(5, vector);
        vector = CreateFlatPlatform(1, vector);
        // Platzierung von Münzen
        CoinsRow(3, true, new Vector3(vector.x - 1f, vector.y + 2f, vector.z));
        return vector;
    }

    // Methode erzeugt einen tötbaren Gegner auf einer geraden Plattform
    Vector3 CreateEnemyKill(Vector3 vector)
    {
        vector = CreateFlatPlatform(7, vector);
        vector.x--;
        vector.y++;
        // Gegner wird aus der Liste der Prefabs erzeugt
        Instantiate(tileSet[8], vector, Quaternion.Euler(90, -180, 0));
        vector.x++;
        vector.y--;
        return vector;
    }

    // Methode erzeugt einen nichttötbaren Gegner auf einer geraden Plattform
    Vector3 CreateEnemyAvoid(Vector3 vector)
    {
        vector = CreateFlatPlatform(5, vector);
        vector.x -= 3f;
        vector.y += 1.3f;
        vector.z += 0.3f;
        // Gegner wird aus der Liste der Prefabs erzeugt
        Instantiate(tileSet[7], vector, Quaternion.Euler(0, 0, 180));
        // Platzierung von Münzen
        CoinsLine(2, new Vector3(vector.x, vector.y + 2.2f, vector.z));
        vector.x += 3f;
        vector.y -= 1.3f;
        vector.z -= 0.3f;
        return vector;
    }
    #endregion

    #region Methoden für die Münzplatzierung
    // Methode erzeugt eine Reihe von Münzen gemäß der Anzahl numberOfCoins
    void CoinsRow(int numberOfCoins, bool up, Vector3 vector)
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Münze wird aus der Liste der Prefabs erzeugt
            Instantiate(tileSet[14], vector, Quaternion.Euler(90, 0, 0));
            if (up)
            {
                vector.y++;
            }
            else
            {
                vector.y--;
            }
        }
    }

    // Methode erezugt eine Linie von Münzen gemäß der Anzahl numberOfCoins
    void CoinsLine(int numberOfCoins, Vector3 vector)
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Münze wird aus der Liste der Prefabs erzeugt
            Instantiate(tileSet[14], vector, Quaternion.Euler(90, 0, 0));
            vector.x++;
        }
    }
    #endregion

    #region Methoden für die Erstellung von Multiwegen
    // Methode erzeugt einen simplen Multiweg in Form von Blöcken, welche die Anzahl blockcount haben
    void CreateSimplePath(int blockCount, int x, int y, Vector3 vector)
    {
        vector.x -= x;
        vector.y += y;
        for (int i = 0; i < blockCount; i++)
        {
            // Block wird aus der Liste der Prefabs erzeugt
            Instantiate(tileSet[0], vector, Quaternion.identity);
            vector.x++;
        }
    }

    // Methode erzeugt Startkomponente eines komplexen Multiweges
    Vector3 CreateMultiStart(Vector3 vector)
    {
        vector = CreateFlatPlatform(3, vector);
        vector.x -= 2f;
        vector.y += 2f;
        //Platzierung von Münzen
        CoinsRow(5, true, vector);
        vector.x += 2f;
        vector.y -= 2f;
        return vector;
    }

    // Methode erzeugt Endkomponente eines komplexen Multiweges
    // es werden Blöcke in unterschiedlichen Höhe angeordnet
    Vector3 CreateMultiEnd(int count, Vector3 vector)
    {
        vector = CreateFlatPlatform(count, vector);
        // Leerraum fürs Sterben
        CreateRightSpace(3, vector);
        vector.x -= 2;
        vector.y += 3;
        for (int i = 0; i < 2; i++)
        {
            vector = CreateFlatPlatform(1, vector);
            vector.x -= 2f;
            vector.y += 2f;
        }
        vector.x--;
        vector = CreateFlatPlatform(2, vector);
        vector.x++;
        vector = CreateFlatPlatform(2, vector);
        return vector;
    }
    #endregion

    #region diverse Hilfsmethoden 
    // Methode findet alle Komponenten mit der gleichen Nullliniendistanz value
    List<int> GetAllTiles(int value, Dictionary<int, int> dict)
    {
        List<int> tileList = new List<int>();
        foreach (KeyValuePair<int, int> pair in dict)
        {
            if (pair.Value == value)
            {
                tileList.Add(pair.Key);
            }
        }
        return tileList;
    }

    // Methode erzeugt Start- und Endplattform sowie Zwischenplattformen bei zwei Rhythmusgruppen
    Vector3 CreateStartEndBetweenTile(int count, Vector3 vector)
    {
        for (int i = 0; i < count; i++)
        {
            // Block wird aus der Liste der Prefabs erzeugt
            Instantiate(tileSet[12], vector, Quaternion.identity);
            vector.x++;
        }
        return vector;
    }

    // Methode erzeugt eine vertikale Reihe von Blöcken gemäß der Höhe height
    Vector3 CreateWall(int height, Vector3 vector)
    {
        for (int i = 0; i < height; i++)
        {
            // Block wird aus der Liste der Prefabs erzeugt
            Instantiate(tileSet[0], vector, Quaternion.identity);
            vector.y--;
        }
        return vector;
    }

    // Methode erzeugt einen Leerraum rechts von einer Komponente,  der als Indikator fürs Sterben verwendet wird
    // die Größe wird durch size bestimmt
    Vector3 CreateRightSpace(float size, Vector3 vector)
    {
        vector.x = vector.x - 1f + ((size + 1) / 2f);
        vector.y -= 1.5f;
        // Leerraum wird aus der Liste der Prefabs erzeugt
        GameObject gap = Instantiate(tileSet[10], vector, Quaternion.identity) as GameObject;
        // Größe des Leerraumes wird verändert
        gap.transform.localScale += new Vector3(size, 0, 0);
        return vector;
    }

    // Methode erzeugt einen Leerraum links von einer Komponente,  der als Indikator fürs Sterben verwendet wird
    // die Größe wird durch size bestimmt
    Vector3 CreateLeftSpace(float size, Vector3 vector)
    {
        vector.x = vector.x - 0.5f - ((size + 1) / 2f);
        vector.y -= 1.5f;
        // Leerraum wird aus der Liste der Prefabs erzeugt
        GameObject gap = Instantiate(tileSet[10], vector, Quaternion.identity) as GameObject;
        // Größe des Leerraumes wird verändert
        gap.transform.localScale += new Vector3(size, 0, 0);
        return vector;
    }

    // Methode deaktiviert Lücken, wenn sie dem Zugang zum komplexen Multiweg behindern
    void DeactivateGaps()
    {
        // Liste aller Lücken wird ermittelt
        GameObject[] gaps = GameObject.FindGameObjectsWithTag("Gap");
        // Multiwege mit ihren Start- und Endvektoren werden betrachtet
        while (sndWayStartEnds.Count > 0)
        {
            Vector3 first = sndWayStartEnds[0];
            Vector3 snd = sndWayStartEnds[1];
            sndWayStartEnds.Remove(first);
            sndWayStartEnds.Remove(snd);
            // Filterung aller Lücken, die im Bereich des Multigwegs vorkommen
            List<GameObject> poss = FindPossibleGaps(first.x - 2, snd.x, gaps);
            // jede gefundene Lücke wird auf ihre Größe geprüft und sofern es nicht die 
            // große Lücke ist, wird die Lücke deaktiviert
            foreach (GameObject gap in poss)
            {
                float localscale = gap.transform.localScale.x;
                if (localscale < 20f)
                {
                    gap.SetActive(false);
                }
            }
        }
    }

    // Methode findet alle Lücken innerhalb eines bestimmten Bereiches [xStart, xEnd] aus einer Liste gapList
    List<GameObject> FindPossibleGaps(float xStart, float xEnd, GameObject[] gapList)
    {
        List<GameObject> poss = new List<GameObject>();
        // alle Lücken in der Listen werden geprüft
        foreach (GameObject gap in gapList)
        {
            // x-Koordinate der aktuellen Lücke
            float gapX = gap.transform.position.x;
            // Prüfung, ob Lücke innerhalb des Bereiches liegt
            if (gapX >= xStart && gapX < xEnd)
            {
                poss.Add(gap);
            }
        }
        return poss;
    }
    #endregion  
}
