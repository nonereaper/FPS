using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePathes : MonoBehaviour
{   
    [SerializeField] List<GameObject> connectedPathes;
    private static int allID;
    private int id;
    private int[] pathToTake;
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
    public void setup() {
        pathToTake = new int[connectedPathes.Count];
        for (int i = 0; i < pathToTake.Length; i++) {
            if (i != id) {

            }
        }

    }
    public string run(ZombiePathes zp, int targetID, string pathSoFar) {
        if (targetID == zp.getID()) return pathSoFar;
        List<string> foundPath = new List<string>();
        List<GameObject> zps = zp.getConnectedPathes();
        string[] allPathSoFar = pathSoFar.Split(",");
        string temppsf = "";
        for (int i = 0; i < allPathSoFar.Length; i++) {
            temppsf += allPathSoFar[i]+",";
        }
        return null;
    }
}
