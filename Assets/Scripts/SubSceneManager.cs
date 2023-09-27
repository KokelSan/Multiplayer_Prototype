using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class SubSceneManager : NetworkBehaviour
{
    public static SubSceneManager singleton { get; internal set; }
    private SpawnManager m_spawnManager => SpawnManager.singleton;

    public readonly SyncDictionary<string, int> playersNbPerScene_SyncDict = new SyncDictionary<string, int>();
    public List<string> subSceneNames;    

    private List<string> m_locallyLoadedScene = new List<string>();    

    void Awake()
    {
        singleton = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        for (int i = 0; i < subSceneNames.Count; i++)
        {
            playersNbPerScene_SyncDict.Add(subSceneNames[i], 0);
        }
    }

    public override void OnStartClient()
    {
        playersNbPerScene_SyncDict.Callback += OnDictUpdated;

        foreach (KeyValuePair<string, int> kvp in playersNbPerScene_SyncDict)
        {
            OnDictUpdated(SyncDictionary<string, int>.Operation.OP_ADD, kvp.Key, kvp.Value);
        }

        //DebugSceneDict();
    }

    private void DebugSceneDict()
    {
        string debug = "Scenes states : ";
        for (int i = 0;i < playersNbPerScene_SyncDict.Count; i++)
        {
            debug += $" {subSceneNames[i]} : {playersNbPerScene_SyncDict[subSceneNames[i]]} ||";
        }
        Debug.Log(debug);
    }

    private void OnDictUpdated(SyncDictionary<string, int>.Operation op, string key, int nb)
    {
        if (op == SyncIDictionary<string, int>.Operation.OP_ADD || op == SyncIDictionary<string, int>.Operation.OP_SET)
        {
            CheckSceneLoading(key);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdNotifyProximityCheck(string sceneName, int passingDirection)
    {
        UpdateSyncDict(sceneName, passingDirection);        
    }

    [Server]
    public void UpdateSyncDict(string sceneName, int passingDirection)
    {
        if (playersNbPerScene_SyncDict.ContainsKey(sceneName))
        {
            playersNbPerScene_SyncDict[sceneName] = Mathf.Max(playersNbPerScene_SyncDict[sceneName] + passingDirection, 0);
        }        
    }

    void CheckSceneLoading(string sceneName)
    {
        int count = playersNbPerScene_SyncDict[sceneName];

        if (count == 1 && !m_locallyLoadedScene.Contains(sceneName)) // a scene is loaded as soon as one player is in it
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            m_locallyLoadedScene.Add(sceneName);
            m_spawnManager.SetSceneObjectsVisibility(sceneName, true);
        }
        else if (count == 0 && m_locallyLoadedScene.Contains(sceneName)) // a scene is unloaded as soon as the last player left
        {
            SceneManager.UnloadSceneAsync(sceneName);
            m_locallyLoadedScene.Remove(sceneName);
            m_spawnManager.SetSceneObjectsVisibility(sceneName, false);
        }

        //DebugSceneDict();
    }

    public bool HasSubScenesLoaded(out List<string> scenes, out int count)
    {
        count = m_locallyLoadedScene.Count;

        if (count > 0)
        {
            scenes = new List<string>(m_locallyLoadedScene);
            return true;
        }

        scenes = null;
        return false;
    }

    public List<string> GetLoadedSubScenes()
    {
        return m_locallyLoadedScene;
    }
}