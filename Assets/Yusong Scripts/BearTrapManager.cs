using UnityEngine;

public class BearTrapManager : MonoBehaviour
{
    private GameObject BearTraps;
    public Transform trap1;
    public Transform trap2;
    private bool FirstTrapOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        BearTraps = this.gameObject;
        trap1 = BearTraps.transform.GetChild(0);
        trap2 = BearTraps.transform.GetChild(1);
    }
    void Start()
    {
        trap1.gameObject.SetActive(true);
        trap2.gameObject.SetActive(false);
        FirstTrapOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchTraps()
    {
        if (FirstTrapOn)
        {
            trap1.gameObject.SetActive(false);
            trap2.gameObject.SetActive(true);
            FirstTrapOn = false;
        }
        else {
            trap2.gameObject.SetActive(false);
            trap1.gameObject.SetActive(true);
            FirstTrapOn = true;
        }
    }
}
