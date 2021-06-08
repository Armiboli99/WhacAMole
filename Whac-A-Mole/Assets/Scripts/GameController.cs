using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject mainMenu, inGameUI,endScreen,recordPanel;

    public Transform molesParent;
    private MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration = 60f;
    public float timePlayed;

    int points = 0;
    int clicks = 0;
    int failedClicks = 0;
    int recordPoints = 0;

    public TMP_InputField nameField;
    string playerName;

    public TextMeshProUGUI infoGame, etiquetaPuntos, recordScoreLayer, etiquetaTiempo;



    void Awake()
    {
        if (GameController.instance == null)
        {
            ConfigureInstance();
        }
        else
        {
            Destroy(this);
        }

    }

    void ConfigureInstance()
    {
        //Configura acceso a moles
        moles = new MoleBehaviour[molesParent.childCount];
        for (int i = 0; i < molesParent.childCount; i++)
        {
            moles[i] = molesParent.GetChild(i).GetComponent<MoleBehaviour>();
        }

        //Inicia los puntos
        points = 0;
        clicks = 0;
        failedClicks = 0;


        //referenciar texto

        

        //Activa la UI inicial
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playing == true)
        {
            timePlayed += Time.deltaTime;
            etiquetaTiempo.text = "Tiempo: " + Mathf.Floor(timePlayed) + "s";
            if (timePlayed >= gameDuration)
            {

                ShowEndScreen();
                playing = false;
                for (int i = 0; i < moles.Length; i++)
                {
                    moles[i].StopMole();
                }

                
            }
            else
            {
                CheckClicks();
            }
            etiquetaPuntos.text = points.ToString(); 
        }
        recordScoreLayer.text = PlayerPrefs.GetString("NombreJugador", playerName) + "  " + PlayerPrefs.GetInt("PuntuacionMaxima", recordPoints).ToString() + " puntos";
       
    }

    
    void ShowEndScreen()
    {
        endScreen.SetActive(true);
        infoGame.text = " Total points : " + points + "\n Record actual : " + PlayerPrefs.GetInt("PuntuacionMaxima", recordPoints).ToString() + " por " + PlayerPrefs.GetString("NombreJugador", playerName) + "\n 10" + "% good shots \n" + "999" + " bad shots";

        bool isRecord = false;

        if (points > PlayerPrefs.GetInt("PuntuacionMaxima", recordPoints))
        {
            PlayerPrefs.SetString("NombreRecord", playerName);
            isRecord = true;
        }
        //si hay nuevo record mostrar el panel recordPanel
        recordPanel.SetActive(isRecord);


    }

    /// <summary>
    /// Function called from End Screen when players hits Retry button
    /// </summary>
    public void Retry()
    {
        //Guardar record si es necesario

        //Acceso al texto escrito
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);
        //Guarda Puntuacion maxima  
        GuardarRecord();
        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(true);
        mainMenu.SetActive(false);
        endScreen.SetActive(false);
        //Activa juego
        playing = true;

        //Reinicia moles
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole();
        }
    }

    /// <summary>
    /// Restarts all info game
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].StopMole();
        }

        timePlayed = 0.0f;
        points = 0;
        clicks = 0;
        failedClicks = 0;
    }

    public void EnterMainScreen()
    {
        playerName = "ArmicheBack";
        //Guarda puntuacion maxima
        GuardarRecord();
        //Reinicia información del juego
        ResetGame();
        
        //Cambia las pantallas
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);

    }

    /// <summary>
    /// Used to check if players hits or not the moles/powerups
    /// </summary>
    public void CheckClicks()
    {
        if ((Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
          
            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                pos = Input.GetTouch(0).position;
            }

            Ray rayo = Camera.main.ScreenPointToRay(pos);
            RaycastHit hitInfo;
            if (Physics.Raycast(rayo, out hitInfo))
            {
                if (hitInfo.collider.tag.Equals("Mole"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    points += 100;
                    Debug.Log(points);
                    if (mole != null)
                    {
                        mole.OnHitMole();
                        
                    }
                }
            }
        }
    }

    public void OnGameStart()
    {
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
        points = 0;
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole(moles[i].initTimeMin, moles[i].initTimeMax);
        }
        playing = true;
    }

    /// <summary>
    /// Funcion para entrar en pausa, pone playing en false y muestra la pantalla de pausa.
    /// </summary>
    public void EnterOnPause()
    { 
    
    
    }

    public void GuardarRecord()
    {
        if (points > PlayerPrefs.GetInt("PuntuacionMaxima", recordPoints))
        {
            
            PlayerPrefs.SetInt("PuntuacionMaxima", points);
            PlayerPrefs.SetString("NombreJugador", playerName);
            PlayerPrefs.Save();
            recordScoreLayer.text = recordPoints.ToString();
        }
        
    }
    public void GuardarNombreRecord()
    {
        if (points > PlayerPrefs.GetInt("PuntuacionMaxima", recordPoints))
        {
            PlayerPrefs.SetString("NombreRecord", playerName);
       
        }
         
    }
}
