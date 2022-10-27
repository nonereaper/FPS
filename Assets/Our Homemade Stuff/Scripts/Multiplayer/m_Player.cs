using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class m_Player : NetworkBehaviour
{
    PlayerInner player;
    public override void OnNetworkSpawn() {
        player = GetComponent<PlayerInner>();
    }
    [ServerRpc]
    private void moveServerRpc(float v, float h, bool jump) {
        player.move(v,h,jump);
    }
    [ServerRpc]
    private void changeStateOfCharacterServerRpc(bool sprint, bool enterC, bool exitC) {
        player.changeStateOfCharacter(sprint,enterC,exitC);
    }
    [ServerRpc]
    private void rotateServerRpc(float pa, float ca) {
        player.rotatePlayer(pa);
        player.rotateCamera(ca);
    }
    [ServerRpc]
    private void useServerRpc(bool u) {
        player.setUseTool(u);
        setUseTextClientRpc(player.getUseText());
    }
    [ServerRpc]
    private void dropServerRpc() {
        player.drop();
    }
    [ServerRpc]
    private void reloadServerRpc() {
        player.reload();
    }
    [ServerRpc]
    private void swapWeaponServerRpc(int i) {
        player.swapWeaponTo(i);
    }
    [ClientRpc]
    private void setUseTextClientRpc(string s) {
        if (IsOwner && IsClient) {
            player.updateUseInfo(s);
        }
    }
    public void FixedUpdate () {
        changeStateOfCharacterServerRpc(Input.GetButton("Sprint"),Input.GetButtonDown("Crouch"),Input.GetButtonUp("Crouch"));
        moveServerRpc(Input.GetAxis("Vertical"),Input.GetAxis("Vertical"),Input.GetButtonDown("Jump"));
    }
    public void Update() {
        rotateServerRpc(Input.GetAxis("Mouse X") * Time.deltaTime * player.getMouseSen(),
        Input.GetAxis("Mouse Y") * Time.deltaTime * player.getMouseSen());
        useServerRpc(Input.GetButton("Use"));
        if (Input.GetButtonDown("Drop")) {
            dropServerRpc();
        }
        if (Input.GetButtonDown("Reload")) {
            reloadServerRpc();
        }
        if (Input.GetButtonDown("Escape")) {
            player.switchView();
        }
        if (Input.GetButtonDown("Fire2")) {
            player.switchWeaponSights(true);
        }
        if (Input.GetButtonUp("Fire2")) {
            player.switchWeaponSights(false);
        }
        for (int i = 0; i < player.getWeaponBarSize(); i++) {
            if (Input.GetKeyDown(""+(i+1))) {
                swapWeaponServerRpc(i);
            }
        }
    }
}
