using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTool : MonoBehaviour
{
    public UnityEngine.UI.Text allWeaponInfo;
    [SerializeField] Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        allWeaponInfo.text = player.getHighlightedUse();
    }
}
