using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

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
        string jsonData = JsonUtility.ToJson(new ScoreData(playerName, time));
        UnityWebRequest www = new UnityWebRequest("http://localhost/update_score_vj2.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score submitted successfully!");
            RestartGame();
        }
        else
        {
            Debug.Log("Error submitting score: " + www.error);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [System.Serializable]
    private class ScoreData
    {
        public string username;
        public string timeCompleted;

        public ScoreData(string username, float time)
        {
            this.username = username;
            this.timeCompleted = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
        }
    }
}
