using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField] private List<Mole> moles;

  [Header("UI objects")]
  [SerializeField] private GameObject playButton;
  [SerializeField] private GameObject gameUI; 
  [SerializeField] private GameObject outOfTimeText;
  [SerializeField] private GameObject bombText;
  [SerializeField] private TMPro.TextMeshProUGUI timeText;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText;
  [SerializeField] private TMPro.TextMeshProUGUI recordText;

    // Hardcoded variables you may want to tune.
    private float startingTime = 30f;

  // Global variables
  private float timeRemaining;
  private HashSet<Mole> currentMoles = new HashSet<Mole>();
  private int score;
  private int record;
  private bool playing = false;

  // This is public so the play button can see it.
  public void StartGame() {
    // Hide/show the UI elements we don't/do want to see.
    playButton.SetActive(false);
    outOfTimeText.SetActive(false);
    bombText.SetActive(false);
    gameUI.SetActive(true);
    // Hide all the visible moles.
    for (int i = 0; i < moles.Count; i++) {
      moles[i].Hide();
      moles[i].SetIndex(i);
    }
    // Remove any old game state.
    currentMoles.Clear();
    // Start with 30 seconds.
    timeRemaining = startingTime;
    score = 0;
    scoreText.text = "0";
    playing = true;
    recordText.text = "" + PlayerPrefs.GetInt("record", 0);
  }

  public void GameOver(int type)
  {
    // Show the message.
    if (type == 0)
    {
      outOfTimeText.SetActive(true);
    }
    else
    {
      bombText.SetActive(true);
    }
    //Ocultar todos los lunares.
    foreach (Mole mole in moles)
    {
      mole.StopGame();
    }
    // Detenga el juego y muestre la interfaz de usuario de inicio.
    playing = false;
    playButton.SetActive(true);
  }

  // Update is called once per frame
  void Update()
  {
    if (playing)
    {
       // Tiempo de actualizacion.
       timeRemaining -= Time.deltaTime;
      if (timeRemaining <= 0)
      {
        timeRemaining = 0;
        GameOver(0);
      }
      timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
       // Comprueba si necesitamos empezar más moñacos.
      if (currentMoles.Count <= (score / 10))
      {
          // Elige un moñaco al azar
         int index = Random.Range(0, moles.Count);
         // No importa si ya está haciendo algo, lo intentaremos de nuevo en el siguiente cuadro.
         if (!currentMoles.Contains(moles[index]))
         {
           currentMoles.Add(moles[index]);
           moles[index].Activate(score / 10);
         }
      }
      if (score > PlayerPrefs.GetInt("record", 0))
      {
          PlayerPrefs.SetInt("record", score);
      }

    }

  }

  public void AddScore(int moleIndex)
  {
     //Añadir y actualizar puntuación
    score += 1;
    scoreText.text = $"{score}";
    // Aumente el tiempo un poco.
    timeRemaining += 1;
    // Eliminar de lunares activos.
    currentMoles.Remove(moles[moleIndex]);
  }

  public void Missed(int moleIndex, bool isMole) {
    if (isMole)
    {
     // Disminuye el tiempo un poco..
      timeRemaining -= 2;
    }
        // Eliminar de lunares activos.
    currentMoles.Remove(moles[moleIndex]);
  }

    public void Salir()
    {
        Application.Quit();
    }
}
