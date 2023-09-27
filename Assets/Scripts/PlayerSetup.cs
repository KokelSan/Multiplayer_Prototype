using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] behavioursToDeactivateForNonLocalPlayers;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            ((CustomNetworkManager)NetworkManager.singleton).SetLocalPlayerNetId(netId);

            return;
        }

        foreach (Behaviour behaviour in behavioursToDeactivateForNonLocalPlayers)
        {
            behaviour.enabled = false;
        }        
    }    
}