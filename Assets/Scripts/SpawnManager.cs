using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager singleton { get; internal set;}

    private NetworkManager m_networkManager => NetworkManager.singleton;
    private SubSceneManager m_subSceneManager => SubSceneManager.singleton;
    private UIManager m_uiManager => UIManager.singleton;

    public readonly SyncDictionary<string, List<uint>> spawnedObjects = new SyncDictionary<string, List<uint>>();
    public float spawnInterval;

    private string m_defaultSceneName = "OnlineScene";    
    private float m_spawnTime = -1;

    void Awake()
    {
        singleton = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isServer)
        {
            m_spawnTime = 0;
        }

        foreach (KeyValuePair<string, List<uint>> kvp in spawnedObjects)
        {
            if (m_subSceneManager.subSceneNames.Contains(kvp.Key) && !m_subSceneManager.GetLoadedSubScenes().Contains(kvp.Key)) // First test to ensure we don't deactivate the default scene's objects
            {
                SetSceneObjectsVisibility(kvp.Key, false);
            }            
        }
    }

    void Update()
    {
        if (isServer && m_spawnTime >= 0)
        {
            m_spawnTime += Time.deltaTime;

            if (m_spawnTime > spawnInterval)
            {
                SpawnItem();
                m_spawnTime = 0;
            }
        }   
    }

    private void SpawnItem()
    {
        ComputeRandomPosition(out Vector3 pos, out string sceneName);
        int index = Random.Range(0, m_networkManager.spawnPrefabs.Count);
        GameObject go = Instantiate(m_networkManager.spawnPrefabs[index], pos, Quaternion.identity, transform);
        NetworkServer.Spawn(go);
        RegisterObject(sceneName, go.GetComponent<NetworkIdentity>().netId);
        m_uiManager.notificationManager.SendNotif($"A new {m_networkManager.spawnPrefabs[index].name} has been spawned", false);
    }

    private void ComputeRandomPosition(out Vector3 pos, out string sceneName)
    {
        sceneName = m_defaultSceneName;

        if(m_subSceneManager.HasSubScenesLoaded(out List<string> sceneNames, out int count))
        {
            sceneNames.Add(m_defaultSceneName);
            sceneName = sceneNames[Random.Range(0, count+1)];
        }        

        GameObject floor = GameObject.FindGameObjectWithTag(sceneName);
        Bounds bounds = floor.GetComponent<Collider>().bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        pos = new Vector3(x, 1, z);
    }

    [Server]
    private void RegisterObject(string sceneName, uint netId)
    {
        if (spawnedObjects.ContainsKey(sceneName))
        {
            spawnedObjects[sceneName].Add(netId);
            spawnedObjects[sceneName] = new List<uint>(spawnedObjects[sceneName]); // The list is replaced to be sure it is synchronised and the callback is called if needed (which not always done when we change just an element of the list)
        }
        else
        {
            List<uint> netIds = new List<uint>() { netId };
            spawnedObjects.Add(sceneName, netIds);
        }
    }

    [Server]
    public void DestructObject(string sceneName, uint netId, GameObject go)
    {
        if (spawnedObjects.ContainsKey(sceneName) && spawnedObjects[sceneName].Contains(netId))
        {

            spawnedObjects[sceneName].Remove(netId);
            spawnedObjects[sceneName] = new List<uint>(spawnedObjects[sceneName]);
            Destroy(go);
        }
    }

    public void SetSceneObjectsVisibility(string sceneName, bool visibility)
    {
        if (spawnedObjects.ContainsKey(sceneName))
        {
            foreach (uint netId in spawnedObjects[sceneName])
            {
                if (NetworkClient.spawned.ContainsKey(netId))
                {
                    GameObject go = NetworkClient.spawned[netId].gameObject;
                    go.GetComponent<Collider>().enabled = visibility;
                    go.GetComponent<Renderer>().enabled = visibility;
                    go.GetComponent<Collectible>().enabled = visibility;                   
                }
            }
        }
    }
}