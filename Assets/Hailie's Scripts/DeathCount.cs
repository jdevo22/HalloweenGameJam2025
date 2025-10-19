using UnityEngine;

using TMPro;

public class DeathCount : MonoBehaviour
{
    public static DeathCount Instance;
    public int deathCount = 0;
    public TextMeshProUGUI deathText;
    private const string DeathCountKey = "DeathCount";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathCount = GetDeathCount();
    }
    public void YouDiedSucker()
    {
        int deathCount = PlayerPrefs.GetInt(DeathCountKey); //makes it work across scenes


      
        Debug.Log("Deaths: " + deathCount);
    }
    public static int GetDeathCount()
    {
        return PlayerPrefs.GetInt(DeathCountKey); //returns # if not 0
    }

    // Update is called once per frame
    void Update()
    {
        deathCount = GetDeathCount();
        deathText.text = "Deaths: " + deathCount.ToString();
    }
}
