using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class ConnexionScreen : MonoBehaviour
{
    private CustomNetworkManager m_networkManager => (CustomNetworkManager)NetworkManager.singleton;

    public TMP_InputField pseudoInputField;
    public TMP_InputField hostAddressInputField;
    public Button hostButton;
    public Button joinButton;    

    void Start()
    {
        pseudoInputField.onValueChanged.AddListener(CheckButtonsInteractability);
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            pseudoInputField.text = savedName;
            SetButtonsInteractability(true);
        }
        else
        {
            SetButtonsInteractability(false);
        }
    }

    private void CheckButtonsInteractability(string newName)
    {
        if (string.IsNullOrEmpty(newName.Trim()))
        {
            SetButtonsInteractability(false);
        }
        else
        {
            SetButtonsInteractability(true);
        }
    }

    private void SetButtonsInteractability(bool interactability)
    {
        hostButton.interactable = interactability;
        joinButton.interactable = interactability;
    }

    private void StartHost()
    {
        m_networkManager.StartHost();

        SavePlayerName();
    }

    private void StartClient()
    {
        m_networkManager.networkAddress = hostAddressInputField.text;
        m_networkManager.StartClient();

        SavePlayerName();
    }

    private void SavePlayerName()
    {
        m_networkManager.SetLocalPlayerInfos(true, pseudoInputField.text);
        PlayerPrefs.SetString("PlayerName", pseudoInputField.text);
    }
}