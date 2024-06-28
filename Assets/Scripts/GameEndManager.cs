using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class GameEndManager : MonoBehaviour
{
    public Text timerText;          // Texto que muestra el tiempo
    public GameObject endGamePanel; // Panel que se muestra al final del juego
    public Text endGameTimeText;    // Texto en el panel final que muestra el tiempo
    public InputField playerNameInput; // Campo de entrada para el nombre del jugador
    public Button submitButton;     // Botón para enviar los datos

    private float timeElapsed;      // Tiempo transcurrido
    private bool gameRunning;       // Estado del juego (corriendo o no)

    void Start()
    {
        timeElapsed = 0;
        gameRunning = true;
        endGamePanel.SetActive(false);
        submitButton.onClick.AddListener(SubmitScore);
    }

    void Update()
    {
        if (gameRunning)
        {
            timeElapsed += Time.deltaTime;
            timerText.text = "Time: " + timeElapsed.ToString("F2");
        }
    }

    public void EndGame()
    {
        gameRunning = false;
        endGamePanel.SetActive(true);
        endGameTimeText.text = "Your Time: " + timeElapsed.ToString("F2");
    }

    public void SubmitScore()
    {
        string playerName = playerNameInput.text;
        StartCoroutine(SendScoreToDatabase(playerName, timeElapsed));
    }

    private IEnumerator SendScoreToDatabase(string playerName, float time)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", playerName);
        form.AddField("time", time.ToString("F2"));

        UnityWebRequest www = UnityWebRequest.Post("http://tuservidor.com/api/submit_score.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score submitted successfully!");
        }
        else
        {
            Debug.Log("Error submitting score: " + www.error);
        }
    }
}

