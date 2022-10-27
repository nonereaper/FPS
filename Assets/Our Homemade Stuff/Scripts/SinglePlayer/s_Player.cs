using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class s_Player : MonoBehaviour
{
    PlayerInner player;
    public void Start() {
        player = GetComponent<PlayerInner>();
    }
    public void FixedUpdate () {
        player.changeStateOfCharacter(Input.GetButton("Sprint"),Input.GetButtonDown("Crouch"),Input.GetButtonUp("Crouch"));
        player.move(Input.GetAxis("Vertical"),Input.GetAxis("Vertical"),Input.GetButtonDown("Jump"));
    }
    public void Update() {
        player.rotatePlayer(Input.GetAxis("Mouse X") * Time.deltaTime * player.getMouseSen());
        player.rotateCamera(Input.GetAxis("Mouse Y") * Time.deltaTime * player.getMouseSen());
        player.setUseTool(Input.GetButton("Use"));
        if (Input.GetButtonDown("Drop")) {
            player.drop();
        }
        if (Input.GetButtonDown("Reload")) {
            player.reload();
        }
        if (Input.GetButtonDown("LockMouse")) {
            player.switchView();
        }
        if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) {
            player.useWeapon(Input.GetButton("Fire1"));
        }
        if (Input.GetButtonDown("Fire2")) {
            player.switchWeaponSights(true);
        }
        if (Input.GetButtonUp("Fire2")) {
            player.switchWeaponSights(false);
        }
        for (int i = 0; i < player.getWeaponBarSize(); i++) {
            if (Input.GetKeyDown(""+(i+1))) {
                player.swapWeaponTo(i);
            }
        }
    }
}
