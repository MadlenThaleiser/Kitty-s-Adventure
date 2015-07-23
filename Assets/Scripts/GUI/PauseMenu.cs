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
/// Die Klasse pausiert das Spiel und ermöglicht entweder das Spiel fortzusetzen
/// oder zum Hauptmenü zurück zu kehren.
/// </summary>
public class PauseMenu : MonoBehaviour 
{
    #region Variablen
    // Variable für den Zugriff auf Eigenschaften des Pausen-GUI-Gameobjektes
    public GameObject pause;
    #endregion

    // Schaltflächenmethode hebt Pausierung des Spiels auf
    public void BackToGame()
    {
        // Anhalten der Zeit wird beendet
        GameManager.ToggleTimeScale();
        // Pausen-Gui wird ausgeblendet
        pause.SetActive(!pause.activeSelf);
    }

    // Schaltflächenmethode wechselt von der Levelszene zur Hauptmenüszene
    public void Menu()
    {
        // Anhalten der Zeit wird beendet
        // wichtig, da sonst beim Start eines neuen Levels Zeit angehalten bleibt
        GameManager.ToggleTimeScale();
        // Laden der Szene mit dem Namen des Hauptmenü
        Application.LoadLevel("Menu");
    }
}