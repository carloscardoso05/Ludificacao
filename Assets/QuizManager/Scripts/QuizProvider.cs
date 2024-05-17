using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

class QuizProvider : MonoBehaviour
{
    public event EventHandler<Dictionary<string, Quiz>> OnGetQuizzes;

    public static QuizProvider Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await GetQuizzes();
    }

    public async Task<Dictionary<string, Quiz>> GetQuizzes()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://quizvaultapp-ea5fb-default-rtdb.firebaseio.com/quizzes.json");
        var requestTask = www.SendWebRequest();

        while (!requestTask.isDone)
        {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            string jsonText = www.downloadHandler.text;
            var quizzesDict = JsonConvert.DeserializeObject<Dictionary<string, Quiz>>(jsonText);
            OnGetQuizzes?.Invoke(this, quizzesDict);
            return quizzesDict;
        }
    }
}