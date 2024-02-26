using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManagerTester : MonoBehaviour
{
    [SerializeField] private QuizManager QuizManager;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        //Selecionar Quiz (Q)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuizManager.SelectQuiz();
        }
        //Carregar questões fácil, média e difícil (W, E, R)
        if (Input.GetKeyDown(KeyCode.W))
        {
            QuizManager.SelectQuestion(0);
        }
        else
        if (Input.GetKeyDown(KeyCode.E))
        {
            QuizManager.SelectQuestion(1);
        }
        else
        if (Input.GetKeyDown(KeyCode.R))
        {
            QuizManager.SelectQuestion(2);
        }
    }
}
