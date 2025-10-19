
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
private const string Death_Count_Key = "DeathCount";
public void IncrementDeathCount()
    {
        int currentDeaths = PlayerPrefs.GetInt(Death_Count_Key); //gets the death count
        currentDeaths++; // adds to it
        PlayerPrefs.SetInt(Death_Count_Key, currentDeaths); //sets new death count
        PlayerPrefs.Save(); //saves it
        Debug.Log("The player has died : "+ currentDeaths + "times"); //text to update
    }
    void Start()
    {
        int savedDeaths = PlayerPrefs.GetInt(Death_Count_Key, 0);
        Debug.Log("loaded death count is : " + savedDeaths);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
