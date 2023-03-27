using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float timeToExplode;
    private float currentTimeLeft;
    private float savedTime;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int damage;
    [SerializeField] private GameObject explosion;
    private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
      currentTimeLeft = timeToExplode; 
    controller = GameObject.Find("Controller").GetComponent<Controller>();

      savedTime =  UnityEngine.Time.time;
    }
    public void setup(float tte, float er, int dam) {
        timeToExplode = tte;
        explosionRadius =er;
        damage = dam;
    }
    // Update is called once per frame
    void Update()
    {
        float differenceInTime = UnityEngine.Time.time-savedTime;
        savedTime = UnityEngine.Time.time;
        currentTimeLeft -= differenceInTime;
        if (currentTimeLeft < 0) currentTimeLeft = 0;
        
        if (currentTimeLeft == 0) {
            List<GameObject> zombies = controller.getAllZombies();
            for (int i = 0; i < zombies.Count; i++) {
                if (Vector3.Distance(zombies[i].transform.position,transform.position) <= explosionRadius) {
                    zombies[i].GetComponent<Zombie>().reduceHealth(damage);
                }
            }
            explosion.transform.localScale = new Vector3(explosionRadius,explosionRadius,explosionRadius);
            GameObject o2 = Instantiate(explosion, transform.position,transform.rotation,controller.getDecayTf());
            controller.removeProjectile(gameObject);
            Destroy(gameObject);
        }
    }
}
