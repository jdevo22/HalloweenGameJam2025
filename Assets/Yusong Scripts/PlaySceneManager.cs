using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }
    void Update()
    { }
        public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene("FinalLevel1");
    }
    public void CreditScene(string sceneName)
    {
        SceneManager.LoadScene("CreditsScene");
    } 
    public void BackFromCredits(string sceneName)
    {
        SceneManager.LoadScene("FinalLevel0");
    }
}

 