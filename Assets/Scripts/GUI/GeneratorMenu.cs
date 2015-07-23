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

/// <summary>
/// Die Klasse verwaltet die Szenenauswahl des Spieles sowie die Option
/// zum Beenden.
/// </summary>
public class GeneratorMenu : MonoBehaviour 
{
    #region Variables
    // Variable für den Zugriff auf die Eigenschaften des Dichte-Schiebereglers 
    public GameObject densitySlider;
    // Variable für den Zugriff auf die Eigenschaften des Dichte-Textes
    public GameObject densityText;

    // Variable für den Zugriff auf die Eigenschaften des Längen-Schiebereglers
    public GameObject lengthSlider;
    // Variable für den Zugriff auf die Eigenschaften des Längen-Textes
    public GameObject lengthText;

    // Standardwert für die Rhythmusdichte 
    private string rhythmDensity = "low";
    // Standardwert für den Rhythmustyp
    private string rhythmType = "regular";
    // Standardwert für die Rhythmuslänge
    private string rhythmLength = "5";
    #endregion

    // Optionsfeldmethode zum Setzen des Typs auf regular
    public void SetRegular() 
    {
        rhythmType = "regular";
    }

    // Optionsfeldmethode zum Setzen des Typs auf swing
    public void SetSwing()
    {
        rhythmType = "swing";
    }

    // Schiebereglermethode zum Setzen der Dichte auf entsprechende Auswahl
    public void SetDensity()
    {
        // auslesen des aktuellen Schiebereglerwertes
        int value = Mathf.RoundToInt(densitySlider.GetComponent<Slider>().value);
        // Zuordnung und Setzen des Dichtewertes anhand des Schiebereglerwertes
        switch (value)
        {
            case 0: rhythmDensity = "low"; densityText.GetComponent<Text>().text = rhythmDensity; break;
            case 1: rhythmDensity = "medium"; densityText.GetComponent<Text>().text = rhythmDensity; break;
            case 2: rhythmDensity = "high"; densityText.GetComponent<Text>().text = rhythmDensity; break;
            default: break;
        }
    }

    // Schiebereglermethode zum Setzen der Länge auf entsprechende Auswahl
    public void SetLength() 
    {
        // auslesen des aktuellen Schiebereglerwertes
        int value = Mathf.RoundToInt(lengthSlider.GetComponent<Slider>().value);
        // Zuordnung und Setzen des Längenwertes anhand des Schiebereglerwertes
        switch (value) 
        {
            case 0: rhythmLength = "5";  lengthText.GetComponent<Text>().text = rhythmLength; break;
            case 1: rhythmLength = "10"; lengthText.GetComponent<Text>().text = rhythmLength; break;
            case 2: rhythmLength = "15"; lengthText.GetComponent<Text>().text = rhythmLength; break;
            case 3: rhythmLength = "20"; lengthText.GetComponent<Text>().text = rhythmLength; break;
            default: break;
        }
    }

    // Schaltflächenmethode zum Laden der Levelszene
    public void GenerateLevel() 
    {
        // Speicherung der aktuellen Werte für Dichte, Type und Länge
        PlayerPrefs.SetString("density", rhythmDensity);
        PlayerPrefs.SetString("type", rhythmType);
        PlayerPrefs.SetString("length", rhythmLength);
        // Laden der Szene mit dem Namen des Levels
        Application.LoadLevel("Level");
    }

    // Schaltflächenmethode zum Laden der Hauptmenüszene
    public void Menu() 
    {
        // Laden der Szene mit dem Namen des Hauptmenüs
        Application.LoadLevel("Menu");
    }
}

