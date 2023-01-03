using System.Collections;
using System.Collections.Generic;
using System;
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
    public static int getAllID() {
        return allID;
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
                string temp = run(this,i,id+",0.0");
                if (temp == null) Debug.Log("error in thing");
                else {
                    string[] temp2 = temp.Split(",");
                    pathToTake[i] = int.Parse(temp2[1]);
                }
            }
        }

    }
    public string run(ZombiePathes zp, int targetID, string pathSoFar) {
        if (targetID == zp.getID()) return pathSoFar;
        List<string> foundPath = new List<string>();
        List<GameObject> zps = zp.getConnectedPathes();
        string[] allPathSoFar = pathSoFar.Split(",");
        string temppsf = "";
        for (int i = 0; i < allPathSoFar.Length-1; i++) {
            temppsf += allPathSoFar[i]+",";
        }
        for (int i = 0; i < zps.Count; i++) {
            bool alreadyDid = false;
            for (int q = 0; q < allPathSoFar.Length-1; q++) {
                //Debug.Log(allPathSoFar[q]+":"+zps[i].GetComponent<ZombiePathes>().getID() + "  /" + allPathSoFar[q].Equals(""+zps[i].GetComponent<ZombiePathes>().getID()));
                if (int.Parse(allPathSoFar[q]) == zps[i].GetComponent<ZombiePathes>().getID()) {
                    alreadyDid = true;
                }
            }
            if (!alreadyDid) {
                string psf = temppsf+zps[i].GetComponent<ZombiePathes>().getID()+","+(Double.Parse(allPathSoFar[allPathSoFar.Length-1])+distance(zps[i]));
                foundPath.Add(run(zps[i].GetComponent<ZombiePathes>(),targetID, pathSoFar));
            }
        }
            int index = -1;
            double dist = Double.MaxValue;
            for (int i = 0; i < foundPath.Count; i++) {
                if (foundPath[i] != null) {
                    string[] temp = foundPath[i].Split(",");
                    if (Double.Parse(temp[temp.Length-1]) < dist) {
                        dist = Double.Parse(temp[temp.Length-1]);
                        index = i;
                    }
                }
            }
            if (index == -1)
        return null;
        else return foundPath[index];
    }
    public double distance(GameObject zp) {
        return Vector3.Distance(transform.position, zp.gameObject.transform.position);
    }
}
