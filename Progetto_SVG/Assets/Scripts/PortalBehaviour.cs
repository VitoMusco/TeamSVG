using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] private bool completesLevel = true;
    [SerializeField] private int sceneToLoad = 0;
    [SerializeField] Transform gate;
    private MeshRenderer portal;
    private BoxCollider portalCollider;
    private ParticleSystem portalParticles;

    void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.tag == "Player")
        {
            collidedObj.gameObject.GetComponent<PlayerController>().stopDoingAnything();
            if (completesLevel) LevelHandler.completedLevel();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void enable()
    {
        portal = GetComponent<MeshRenderer>();
        portalCollider = GetComponent<BoxCollider>();
        portalParticles = GetComponentInChildren<ParticleSystem>();
        portal.enabled = true;
        portalCollider.enabled = true;
        portalParticles.Play();
    }

    public void enableWithGate() {
        enable();
        gate.localPosition = new Vector3(0f, 0.25f, -0.25f);
    }
}
