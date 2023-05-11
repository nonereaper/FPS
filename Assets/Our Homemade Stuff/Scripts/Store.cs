using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private bool refreshAmmo;
    [SerializeField] private GameObject itemToSell;
    [SerializeField] private string perk;
    [SerializeField] private int price;
    [SerializeField] private GameObject spotToSpawn;
    [SerializeField] private GameObject usePoint;

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
    public GameObject getUsePoint() {
        return usePoint;
    }
    public bool canBuy(int p) {
        return p>=price;
    }
    public void buyItem(PlayerInner playerInner) {
        if (refreshAmmo) {
            playerInner.maxAmmo();
        } else if (itemToSell != null && itemToSell.GetComponent<Weapon>() != null) {
            GameObject o = Instantiate(itemToSell, spotToSpawn.transform.position, spotToSpawn.transform.rotation,controller.getWeaponTf());
            controller.addWeapon(o);
            playerInner.addWeaponToPlayer(controller.getLastWeapon());
        } else {
            playerInner.addPerk(perk);
        }
    }
    public string getItemName() {
        if (refreshAmmo) {
            return "Ammo";
        }else if (itemToSell == null) {
        return perk;
        }
        return itemToSell.name;
        
    }
    public bool hasPerk() {
        return itemToSell == null && !perk.Equals("");
    }
    public string getPerk() {
        return perk;
    }
    public int getPrice() {
        return price;
    }
}
