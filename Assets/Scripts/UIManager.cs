using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton { get; internal set; }

    public ConnexionScreen connexionScreen;
    public InformationPanel informationPanel;
    public NotificationManager notificationManager;
    public ChatManager chatManager;

    void Awake()
    {
        singleton = this;
    }

    void OnToggleInformationPanel(InputValue value)
    {
        informationPanel.ToggleVisibility();
    }

    void OnToggleChat(InputValue value)
    {
        chatManager.ToggleVisibility();
    }    
}