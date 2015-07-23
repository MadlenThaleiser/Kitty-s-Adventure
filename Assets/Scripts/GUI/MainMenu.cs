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
/// Die Klasse verwaltet die Szenenauswahl des Spieles sowie die Option
/// zum Beenden.
/// </summary>
public class MainMenu : MonoBehaviour
{
    #region Variables
    // Variable für den Zugriff auf Eigenschaften des Hauptmenü-GUI-Gameobjektes
    public GameObject main;
    // Variable für den Zugriff auf Eigenschaften des Hilfs-GUI-Gameobjektes
    public GameObject help;
    #endregion

    // Schaltflächenmethode zum Laden der Generatorszene
    public void Generator()
    {
        // Laden der Szene mit dem Namen des Generators
        Application.LoadLevel("Generator");
    }

    // Schaltflächenmethode zum Einblenden des Hilfs-GUIs
    // und zum Ausblenden des Hauptmenü-GUIs
    public void Help()
    {
        // Hilfs-GUI wird eingeblendet
        help.SetActive(true);
        // Hauptmenü-GUI wird ausgeblendet
        main.SetActive(false);
    }

    // Schaltflächenmethode zum Einblenden des Hauptmenü-GUIs
    // und zum Ausblenden des Hilfs-GUIs
    public void Menu()
    {
        // Hilfs-GUI wird ausgeblendet
        help.SetActive(false);
        // Hauptmenü-GUI wird eingeblendet
        main.SetActive(true);
    }

    // Schaltflächenmethode zum Laden der Creditsszene
    public void Credits() 
    {
        // Laden der Szene mit dem Namen der Credits
        Application.LoadLevel("Credits");
    }

    // Schaltflächemmethode zum Beenden der Anwendung
    public void Exit()
    {
        // Überprüfung der Anwendung, ob es innerhalb des Webplayers ausgeführt wird
        if (Application.isWebPlayer) 
        {
            // bei Ausführung im Webplayer wird zu einer Webseite weitergeleitet
            Application.OpenURL("http://page.mi.fu-berlin.de/mthaleiser/index.html");
        }
        else
        {
            // beenden der Anwendung, wenn keine Ausführung im Webplayer
            Application.Quit();
        }        
    } 
 }