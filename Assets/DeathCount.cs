using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEditor.Build.Content;

public class DeathCount : MonoBehaviour
{
    public static DeathCount Instance;
    public int deathCount = 0;
    public TextMeshProUGUI deathText;

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
        deathCount++;
        Debug.Log("Deaths: " + deathCount);
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
