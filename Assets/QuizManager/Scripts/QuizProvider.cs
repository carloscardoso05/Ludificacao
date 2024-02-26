using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

class QuizProvider : MonoBehaviour
{
    public event EventHandler<Dictionary<string, Quiz>> OnGetQuizzes;

    private void Start() {
        GetQuizzes();
    }
    public void GetQuizzes() {
        StartCoroutine(GetQuizzesCore());
    }

    private IEnumerator GetQuizzesCore()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://quizvaultapp-ea5fb-default-rtdb.firebaseio.com/quizzes.json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Dictionary<string, Quiz> quizzesDict = JsonConvert.DeserializeObject<Dictionary<string, Quiz>>(www.downloadHandler.text);
            OnGetQuizzes?.Invoke(this, quizzesDict);
        }
    }
}