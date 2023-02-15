using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject itemToSell;
    [SerializeField] private string perk;
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
    public void buyItem(PlayerInner playerInner) {
        if (itemToSell != null && itemToSell.GetComponent<Weapon>() != null) {
            GameObject o = Instantiate(itemToSell, spotToSpawn.transform.position, spotToSpawn.transform.rotation,controller.getWeaponTf());
            controller.addWeapon(o);
            playerInner.addWeaponToPlayer(controller.getLastWeapon());
        } else {
            playerInner.addPerk(perk);
        }
    }
    public string getItemName() {
        if (itemToSell != null)
        return itemToSell.name;
        else
        return perk;
    }
    public int getPrice() {
        return price;
    }
}
