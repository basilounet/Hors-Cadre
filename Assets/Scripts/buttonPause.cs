using UnityEngine;

public class buttonPause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Pause() 
    { 
        pauseMenu.SetActive(true);
    }
}
