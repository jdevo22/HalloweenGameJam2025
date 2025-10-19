using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class BeginnerTutorialManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject GreenCircle;
    public bool GreenCircleBlinking;

    private void Awake()
    {
        GreenCircle = this.gameObject;
        GreenCircleBlinking=false;
    }
    void Start()
    {
        
        StartBlinking();
    }

    // Update is called once per frame
    void Update()
    {
        if (GreenCircleBlinking)
        {
            this.transform.Rotate(Vector3.forward * 30f * Time.deltaTime);
        }
        else {
            GetComponent<Renderer>().enabled = false;
        }
    }

    public void StartBlinking() {
        if (!GreenCircleBlinking)
        {
            GreenCircleBlinking = true;
            StartCoroutine(GreenCircleBlink());
        }
    }
    IEnumerator GreenCircleBlink() {
        while (GreenCircleBlinking)
        {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
            yield return new WaitForSeconds(0.5f);
        }
        GetComponent<Renderer>().enabled = false;
    }

}
