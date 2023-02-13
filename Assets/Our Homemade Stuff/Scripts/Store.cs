using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject itemToSell;
    [SerializeField] private int price;
    [SerializeField] private GameObject spotToSpawn;

    private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool canBuy(int p) {
        return p>=price;
    }
    public void buyItem() {
        GameObject o = Instantiate(itemToSell, spotToSpawn.transform.position, spotToSpawn.transform.rotation,controller.getWeaponTf());
        controller.addWeapon(o);
        
    }
    public string getItemName() {
        return itemToSell.name;
    }
    public int getPrice() {
        return price;
    }
}
