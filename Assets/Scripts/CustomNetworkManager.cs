using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    private Dictionary<NetworkConnectionToClient, GameObject> m_playersDict = new Dictionary<NetworkConnectionToClient, GameObject>();
    private bool m_isHost = false;
    private string m_localPlayerName;
    private uint m_localPlayerNetId;

    public void SetLocalPlayerInfos(bool isHost, string playerName) 
    {
        m_isHost = isHost;
        m_localPlayerName = playerName;
    }

    public string GetLocalPlayerName()
    {
        return m_localPlayerName;
    }

    public void SetLocalPlayerNetId(uint netId)
    {
        m_localPlayerNetId = netId;
    }

    public uint GetLocalPlayerNetId()
    {
        return m_localPlayerNetId;
    }

    public void RequestDisconnection()
    {
        if(m_isHost)
        {
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    public override void RegisterPlayer(NetworkConnectionToClient conn, GameObject go) 
    {
        m_playersDict.Add(conn, go);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        SubSceneManager.singleton.UpdateSyncDict(m_playersDict[conn].GetComponent<PlayerCollision>().CurrentScene, -1);
        if (m_playersDict.ContainsKey(conn))
        {
            m_playersDict.Remove(conn);
        }       

        base.OnServerDisconnect(conn);
    }
}