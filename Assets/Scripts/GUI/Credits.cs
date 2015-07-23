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
/// Die Klasse verwaltet die Informationen des Creditsmenü.
/// </summary>
public class Credits : MonoBehaviour {

    #region Variables
    // Variable für den Zugriff auf die Eigenschaften der Weiter-Schaltfläche 
    public GameObject nextButton;
    // Variable für den Zugriff auf die Eigenschaften der Zurück-Schaltfläche 
    public GameObject backButton;

    // Variable für den Zugriff auf die Eigenschaften des UnityCredits-Gameobjectes 
    public GameObject unityCredits;
    // Variable für den Zugriff auf die Eigenschaften des GrafikCredits-Gameobjectes
    public GameObject graficCredits;
    // Variable für den Zugriff auf die Eigenschaften des DankeschönCredits-Gameobjectes
    public GameObject thanksCredits;
    // Variable speichert, welche Information angezeigt wird
    int count = 0;
    #endregion

    // Schaltflächenmethode zum Laden der Hauptmenüszene
    public void Menu()
    {
        // Laden der Szene mit dem Namen des Hauptmenüs
        Application.LoadLevel("Menu");
    }

    // Schaltfächenmethode zum Navigieren zu den nächsten Credits
    public void Next() 
    {
        switch (count) 
        { 
            case 0:
                backButton.SetActive(true);
                unityCredits.SetActive(false);
                graficCredits.SetActive(true);
                count = 1;
                break;
            case 1:
                nextButton.SetActive(false);
                graficCredits.SetActive(false);
                thanksCredits.SetActive(true);
                count = 2;
                break;
            default: break;
        }
    }

    // Schaltfächenmethode zum Navigieren zurück zu vorherigen Credits
    public void Back()
    {
        switch (count)
        {
            case 1:
                backButton.SetActive(false);
                unityCredits.SetActive(true);
                graficCredits.SetActive(false);
                count = 0;
                break;
            case 2:
                nextButton.SetActive(true);
                graficCredits.SetActive(true);
                thanksCredits.SetActive(false);
                count = 1;
                break;
            default: break;
        }
    }
}
