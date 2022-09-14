using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private List<GameObject> projectiles;
    private List<GameObject> decay;
    // Start is called before the first frame update
    void Start()
    {
        projectiles = new List<GameObject>();
        decay = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addNewProjectile(GameObject go) {
        projectiles.Add(go);
    } 
    public void addNewDecay(GameObject go) {
        decay.Add(go);
    }
}
