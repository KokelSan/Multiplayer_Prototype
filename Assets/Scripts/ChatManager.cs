using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ChatManager : NetworkBehaviour
{
    private CustomNetworkManager m_networkManager => (CustomNetworkManager)NetworkManager.singleton;
    private UIManager m_uiManager => UIManager.singleton;

    [Header("UI Elements")]
    public RectTransform panelTransform;
    public Text chatContent;
    public Scrollbar scrollbar;
    public InputField inputField;

    [Header("Show/hide animation parameters")]
    public AnimationCurve animationCurve;
    public float finalVisiblePoint; // around -20
    public float finalHiddenPoint; // around 210
    public float animationDuration;

    private float m_animationTime = - 1;
    private float m_startPoint;
    private float m_endPoint;
    
    private RectTransform m_rectTransform;
    private bool m_isVisible = false;

    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();

        inputField.onSubmit.AddListener(OnSubmitMessage);
    }

    void Update()
    {
        if (m_animationTime >= 0)
        {
            float t = animationCurve.Evaluate(m_animationTime / animationDuration);
            float newPos = Mathf.LerpUnclamped(m_startPoint, m_endPoint, t);
            panelTransform.anchoredPosition3D = new Vector3(newPos, 0f, 0f);
            m_animationTime += Time.deltaTime;

            if (m_animationTime > animationDuration)
            {
                m_animationTime = -1;                
            }
        }
    }

    public void ToggleVisibility()
    {
        // Max/min to take in count the current position if we toggle when the animation is running
        if (m_isVisible)
        {
            m_startPoint = Mathf.Max(panelTransform.anchoredPosition3D.x, finalVisiblePoint);
            m_endPoint = finalHiddenPoint;
        }
        else
        {
            m_startPoint = Mathf.Min(panelTransform.anchoredPosition3D.x, finalHiddenPoint);
            m_endPoint = finalVisiblePoint;
        }
        m_isVisible = !m_isVisible;
        m_animationTime = 0;
        GameManager.SetPauseState(m_isVisible);
    }     

    // Called by the InputField when Enter is pressed
    public void OnSubmitMessage(string input)
    {
        SendMessage();
    }

    public void SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(inputField.text))
        {            
            CmdSendMessage(m_networkManager.GetLocalPlayerNetId(), m_networkManager.GetLocalPlayerName(), inputField.text.Trim());
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSendMessage(uint playerNetId, string playerName, string message)
    {
        RpcReceiveMessage(playerNetId, playerName, message);
    }

    [ClientRpc]
    void RpcReceiveMessage(uint playerNetId, string playerName, string message)
    {
        DisplayNewMessage(playerNetId, playerName, message);
    }
    
    void DisplayNewMessage(uint playerNetId, string playerName, string message)
    {
        string finalMess = playerName;

        if (playerNetId == m_networkManager.GetLocalPlayerNetId()) 
        {
            finalMess = "You";
        }

        finalMess += " : " + message;
        chatContent.text += finalMess + "\n";
        scrollbar.value = 0; // to focus the view on the last message

        if (!m_isVisible)
        {
            m_uiManager.notificationManager.AddNotifToFeed("New message !");
        }
    }
}