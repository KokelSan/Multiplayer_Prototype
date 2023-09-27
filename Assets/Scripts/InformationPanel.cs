using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InformationPanel : MonoBehaviour
{
    private CustomNetworkManager m_networkManager => (CustomNetworkManager)NetworkManager.singleton;

    public CanvasGroup canvasGroup;
    public Button disconnectButton;
    public Button closeButton;
    public float fadeDuration;

    private float m_fadeTime = -1;
    private float m_startPoint;
    private float m_endPoint;    
    private bool m_isVisible = true;

    void Start()
    {
        disconnectButton.onClick.AddListener(OnDisconnect);
        closeButton.onClick.AddListener(ToggleVisibility);
        GameManager.SetPauseState(true);
    }

    void Update()
    {
        if (m_fadeTime >= 0)
        {
            float t = m_fadeTime / fadeDuration;
            canvasGroup.alpha = Mathf.LerpUnclamped(m_startPoint, m_endPoint, t);
            m_fadeTime += Time.deltaTime;
            if (m_fadeTime > fadeDuration)
            {
                canvasGroup.alpha = m_endPoint; // to prevent stopping a bit above 0 when hiding
                m_fadeTime = -1;                
            }
        }
    }

    public void ToggleVisibility()
    {
        if (m_isVisible)
        {
            m_startPoint = Mathf.Min(canvasGroup.alpha, 1);
            m_endPoint = 0;
}
        else
        {
            m_startPoint = Mathf.Max(canvasGroup.alpha, 0);
            m_endPoint = 1;
        }
        canvasGroup.interactable = !canvasGroup.interactable;
        m_isVisible = !m_isVisible;
        m_fadeTime = 0;
        GameManager.SetPauseState(m_isVisible);
    }

    private void OnDisconnect()
    {
        m_networkManager.RequestDisconnection();
    }
}