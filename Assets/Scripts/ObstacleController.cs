using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    List<GameObject> Obstacles; 
    Transform tf;
    private float savedDistance;

    // Start is called before the first frame update
    void Start()
    {
        Obstacles = new  List<GameObject>();
        tf = GetComponent<Transform>();
        for (int i = 0; i < tf.childCount; i++) {
            Obstacles.Add(tf.GetChild(i).gameObject);
        }
    }

	public float getSavedDistance() {
		return this.savedDistance;
	}

	public void setSavedDistance(float savedDistance) {
		this.savedDistance = savedDistance;
	}
    
    public GameObject getObstacles(Vector3 p, float rad) {
        GameObject tempObstacle = null;
        float distance = float.MaxValue;
        for (int i = 0; i < Obstacles.Count; i++) {
            float tempDistance = Vector3.Distance(Obstacles[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                tempObstacle = Obstacles[i];
                distance = tempDistance;
            }
        }
        savedDistance = distance;
        return tempObstacle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
