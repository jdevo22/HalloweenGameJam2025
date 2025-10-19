using UnityEngine;
using UnityEngine.UI;


public class ParticleSystem : MonoBehaviour
{
    public GameObject TokenCollected;
    public GameObject ParticleTurnOn;
    public Toggle myToggle; 
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        myToggle.onValueChanged.AddListener(delegate { OnToggleExec(); });
    }
    void OnToggleExec()
    // Update is called once per frame
 
    void Update()
    {


    }
}
