using UnityEngine;
using Mirror;

public class PlayerCollision : NetworkBehaviour
{
    private SubSceneManager m_subSceneManager => SubSceneManager.singleton;
    private SpawnManager m_spawnManager => SpawnManager.singleton;
    private UIManager m_uiManager => UIManager.singleton; 

    private ProximityChecker m_lastLoadedScene;
    private ProximityChecker m_pendingSceneToUnload;
    private CapsuleCollider m_capsuleCollider;

    [SyncVar] private string m_currentScene;
    public string CurrentScene => m_currentScene;

    private bool m_isGrounded;
    public bool IsGrounded => m_isGrounded;

    void Start()
    {
        m_capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        ComputeClosestGround();        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer && collision.gameObject.TryGetComponent<Collectible>(out Collectible collectible))
        {
            Debug.Log("Collision with collectible");
            CmdRequestDestruction(collectible.gameObject);
            m_uiManager.notificationManager.SendNotif(collectible.onCollectedNotif);            
        }        
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestDestruction(GameObject go)
    {
        m_spawnManager.DestructObject(m_currentScene, go.GetComponent<NetworkIdentity>().netId, go);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer  && other.gameObject.TryGetComponent<ProximityChecker>(out ProximityChecker proximityChecker))
        {
            if (proximityChecker.passingDirection == 1) // we load the scene at the moment we enter the proximity checker
            {
                m_subSceneManager.CmdNotifyProximityCheck(proximityChecker.sceneToLoad, proximityChecker.passingDirection);
                proximityChecker.ToggleDirection();
                m_lastLoadedScene = proximityChecker;
            }
            else // we don't unload the scene when entering the trigger to prevent unwanted results
            {
                m_pendingSceneToUnload = proximityChecker;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLocalPlayer && other.gameObject.TryGetComponent<ProximityChecker>(out ProximityChecker proximityChecker))
        {
            if (m_lastLoadedScene)
            {
                if (m_lastLoadedScene.sceneToLoad != m_currentScene) // we unload the new loaded scene if the player don't go in it
                {
                    m_pendingSceneToUnload = m_lastLoadedScene;
                }
                m_lastLoadedScene = null;
            }            

            if (m_pendingSceneToUnload != null && m_pendingSceneToUnload.sceneToLoad != m_currentScene) // we unload the scene at the moment the player exits the proximity checker and is on another scene's floor
            {
                m_subSceneManager.CmdNotifyProximityCheck(m_pendingSceneToUnload.sceneToLoad, m_pendingSceneToUnload.passingDirection);
                m_pendingSceneToUnload.ToggleDirection();
                m_pendingSceneToUnload = null;
            }
        }
    }

    private void ComputeClosestGround()
    {
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            if(hit.transform.tag != m_currentScene)
            {
                m_currentScene = hit.transform.tag;
            }
            m_isGrounded = hit.distance <= m_capsuleCollider.height/2;
        }
    }
}