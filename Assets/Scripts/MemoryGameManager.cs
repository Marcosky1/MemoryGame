using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MemoryGameManager : MonoBehaviour
{
    private string url = "http://localhost/update_memory_game_score.php";

    public void SendCompletionTime(string username, float completionTime)
    {
        StartCoroutine(SendCompletionTimeCoroutine(username, completionTime));
    }

    private IEnumerator SendCompletionTimeCoroutine(string username, float completionTime)
    {
        MemoryGameData memoryGameData = new MemoryGameData
        {
            username = username,
            completionTime = completionTime
        };

        string jsonString = JsonUtility.ToJson(memoryGameData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
}

[System.Serializable]
public class MemoryGameData
{
    public string username;
    public float completionTime;
}
