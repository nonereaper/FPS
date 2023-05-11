using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
//[ExecuteInEditMode]
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
        for (int i = 0; i < connectedPathes.Count; i++) {
            if (connectedPathes[i].GetComponent<ZombiePathes>().searchAdj(id) == null) {
                connectedPathes[i].GetComponent<ZombiePathes>().addPath(gameObject);
            }
        }
    }
    public static void resetID (){
        allID = 0;
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < connectedPathes.Count; i++) {
            Debug.DrawLine(transform.position,connectedPathes[i].transform.position,Color.magenta);
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
    }
    public void addPath(GameObject go) {
        connectedPathes.Add(go);
    }
    public static int getAllID() {
        return allID;
    }
    public int search(int idToFind) {
        return pathToTake[idToFind];
    }
    public GameObject searchAdj(int idToFind) {
        for (int i = 0; i < connectedPathes.Count; i++) {
            if (connectedPathes[i].GetComponent<ZombiePathes>().getID() == idToFind)
            return connectedPathes[i];
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
        pathToTake = new int[allID];
        for (int i = 0; i < pathToTake.Length; i++) {
            if (i != id) {
                string temp = run(this,i,id+"",0.0);
                if (temp == null) Debug.Log("error in thing");
                else {
                    string[] temp2 = temp.Split(",");
                    pathToTake[i] = int.Parse(temp2[1]);
                }
            }
        }

    }
    public string run(ZombiePathes zp, int targetID, String pathSoFar, double currDist) {
        if (targetID == zp.getID()) return pathSoFar+","+currDist;
        List<String> foundPath = new List<String>();
        List<GameObject> zps = zp.getConnectedPathes();
        /*
        for (int i = 0; i < zps.Count; i++) {
            Debug.Log(zps[i].GetComponent<ZombiePathes>().getID());
        }
        Debug.Log(zp.getID() + " new " + pathSoFar);*/
        String[] allPathSoFar = pathSoFar.Split(",");

        for (int i = 0; i < zps.Count; i++) {
            bool alreadyDid = false;
            for (int q = 0; q < allPathSoFar.Length; q++) {
                if (int.Parse(allPathSoFar[q]) == zps[i].GetComponent<ZombiePathes>().getID()) {
                    //Debug.Log(pathSoFar + "same thing" + allPathSoFar[q] + ":" + zps[i].GetComponent<ZombiePathes>().getID());
                    alreadyDid = true;
                    break;
                }
            }
            if (!alreadyDid) {
                String psf = pathSoFar+","+zps[i].GetComponent<ZombiePathes>().getID();
                double distNew = currDist+distance(zps[i],zp.gameObject);
                String temp2= run(zps[i].GetComponent<ZombiePathes>(),targetID, psf,distNew);

                //Debug.Log(psf);
                //Debug.Log(distNew);

                if (temp2 != null){
                foundPath.Add(temp2);
                }
            }
        }
            int index = -1;
            double dist = Double.MaxValue;
            for (int i = 0; i < foundPath.Count; i++) {
                    String[] temp = foundPath[i].Split(",");
                    if (Double.Parse(temp[temp.Length-1]) < dist) {
                        dist = Double.Parse(temp[temp.Length-1]);
                        index = i;
                    }
            }
            if (index == -1)
        return null;
        else return foundPath[index];
    }
    public double distance(GameObject zp, GameObject zp2) {
        return Vector3.Distance(zp2.gameObject.transform.position, zp.gameObject.transform.position);
    }
}
