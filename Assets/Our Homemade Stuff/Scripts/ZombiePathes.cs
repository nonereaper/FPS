using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePathes : MonoBehaviour
{   
    [SerializeField] List<GameObject> connectedPathes;
    private static int allID;
    private int id;
    // Start is called before the first frame update
    void Start()
    {
        id = allID;
        allID++;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < connectedPathes.Count; i++) {
            Debug.DrawLine(transform.position,connectedPathes[i].transform.position,Color.magenta);
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
    }

    public string search(GameObject adj, int idToFind) {
        ZombiePathes zp = adj.GetComponent<ZombiePathes>();
        if (zp.getID() == idToFind) {
            // found path

        } 
        for (int i = 0; i < zp.getConnectedPathes().Count; i++) {
            search(zp.getConnectedPathes()[i],idToFind);
        }
        return null;
    }
    public int getID() {
        return id;
    }
    public List<GameObject> getConnectedPathes() {
        return connectedPathes;
    }
}
