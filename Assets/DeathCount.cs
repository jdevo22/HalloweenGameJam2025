using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEditor.Build.Content;

public class DeathCount : MonoBehaviour
{
    public static DeathCount Instance;
    public int deathCount = 0;
    public TextMeshProUGUI deathText;
    private const string DeathCountKey = "TotalDeaths";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void YouDiedSucker()
    {
        int deathCount = PlayerPrefs.GetInt(DeathCountKey, 0); //makes it work across scenes
        deathCount++;
        Debug.Log("Deaths: " + deathCount);
    }
    public static int GetDeathCount()
    {
        return PlayerPrefs.GetInt(DeathCountKey, 0); //returns # if not 0
    }

    // Update is called once per frame
    void Update()
    {
        if (DeathCount.Instance != null)
        {
            deathText.text = "Deaths: " + DeathCount.Instance.deathCount.ToString();

        }
        else
        {
            deathText.text = "Deaths: 0";
        }
    }
}
