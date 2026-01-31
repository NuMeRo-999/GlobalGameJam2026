using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Goal Reached!");
            sceneController.LoadNextScene();
        }
    }
}
