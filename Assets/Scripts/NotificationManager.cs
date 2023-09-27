using UnityEngine.UI;
using UnityEngine.UIElements;
using Mirror;
using UnityEngine;

public class NotificationManager : NetworkBehaviour
{
    private CustomNetworkManager m_networkManager => (CustomNetworkManager)NetworkManager.singleton;

    public ScrollView scrollView;
    public Scrollbar scrollbar;
    public Text notifContent;
    public CanvasGroup canvasGroup;
    public float fadeDuration;
    public float notifDisplayDuration;

    private float m_fadeTime = -1;
    private float m_startPoint;
    private float m_endPoint;
    private bool m_isVisible = false;

    private float m_notifDisplayTime = 0;

    void Update()
    {
        if (m_isVisible)
        {
            if (m_notifDisplayTime > notifDisplayDuration)
            {
                ToggleVisibility();                
            }
            else
            {
                m_notifDisplayTime += Time.deltaTime;
            }            
        }

        if (m_fadeTime >= 0)
        {
            float t = m_fadeTime / fadeDuration;
            canvasGroup.alpha = Mathf.LerpUnclamped(m_startPoint, m_endPoint, t);
            m_fadeTime += Time.deltaTime;
            if (m_fadeTime > fadeDuration)
            {
                canvasGroup.alpha = m_endPoint; // to prevent stopping a bit above 0 when hiding
                m_fadeTime = -1;

                if (!m_isVisible)
                {
                    notifContent.text = string.Empty;
                }
            }
        }        
    }

    public void SendNotif(string notif, bool addPlayerName = true)
    {
        if (!string.IsNullOrWhiteSpace(notif))
        {
            string finalNotif = string.Empty;
            if (addPlayerName) finalNotif = m_networkManager.GetLocalPlayerName();
            finalNotif += notif;
            CmdSendNotif(finalNotif);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSendNotif(string notif)
    {
        RpcReceiveNotif(notif);
    }

    [ClientRpc]
    void RpcReceiveNotif(string notif)
    {
        AddNotifToFeed(notif);
    }

    public void AddNotifToFeed(string notif)
    {
        if(!m_isVisible)
        {
            ToggleVisibility();
        }

        notifContent.text += notif + "\n";
        scrollbar.value = 0;
        m_notifDisplayTime = 0; 
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
        m_notifDisplayTime = 0;
    }
}