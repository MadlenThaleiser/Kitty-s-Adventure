/*
 * Diese Scripte wurden im Rahmen der Bachelorarbeit
 * "Implementierung und Erweiterung eines 
 *  rhythmusbasierten Levelgenerators für
 *  2D-Plattformer" 
 * von Madlen Thaleiser erstellt.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Die Klasse verwaltet die Anzahl der Leben sowie die Anzahl an gesammelten Münzen,
/// sowie den Spielverlauf.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variablen
    // Klassenvariable zum Verwalten der Münzenanzahl
    public static int coinCount;
    // Klassenvariable zum Verwalten der Lebensanzahl
    public static int lifeCount;
    // Klassenvariable zum Anzeigen von komplexen Multiwegen
    public static string multiWeg;
    // Variable für den Zugriff auf Eigenschaften des CoinText-Gameobjektes
    public GameObject coinText;
    // Variable für den Zugriff auf Eigenschaften des HeartText-Gameobjektes
    public GameObject heartText;
    // Variable für den Zugriff auf Eigenschaften des CMText-Gameobjektes
    public GameObject cmText;
    // Variable für den Zugriff auf Eigenschaften des Pausen-GUI-Gameobjektes
    public GameObject pause;
    // Variable zur Speicherung des Pausierungsstandes
    static bool isPaused = false;
    #endregion

    // Awake wird für die Initialiserung verwendet, ehe ein Spiel gestartet ist
    void Awake() 
    {
        // Initialisierung der Klassenvariablen
        coinCount = 0;
        lifeCount = 3;
        multiWeg = "false";
        // Initialisierung der Textvariablen
        coinText.GetComponent<Text>().text = coinCount.ToString();
        heartText.GetComponent<Text>().text = lifeCount.ToString();
        cmText.GetComponent<Text>().text = multiWeg;

    }

    // Update aktualisiert das Verhalten und wird jedes Einzelbild aufgerufen
    void Update()
    {
        // Rückkehr zum Hauptmenü, wenn keine Leben mehr vorhanden sind
        if (lifeCount <= 0) 
        {
            Application.LoadLevel("Menu");
        }
        // Hinzufügung eines Lebens, wenn 30 Münzen gesammelt wurden
        if (coinCount == 30) 
        {
            lifeCount++;
            coinCount = 0;
        }
        // Aktivierung/Beendigung des Pausenmenüs
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.SetActive(!pause.activeSelf);
            ToggleTimeScale();
        }
        // Update GUI
        UpdateGUI();
    }

    // Methode aktualisiert Spielinterface
    void UpdateGUI() 
    {
        coinText.GetComponent<Text>().text = coinCount.ToString();
        heartText.GetComponent<Text>().text = lifeCount.ToString();
        cmText.GetComponent<Text>().text = multiWeg;
    }
    
    // Methode regelt die Zeit während und nach Pausierung des Spieles
    public static void ToggleTimeScale()
    {
        // Pausierung der Zeit
        if (!isPaused)
        {
            Time.timeScale = 0;
        }
        // Aufhebung der Pausierung
        else
        {
            Time.timeScale = 1;
        }
        isPaused = !isPaused;
    }
}