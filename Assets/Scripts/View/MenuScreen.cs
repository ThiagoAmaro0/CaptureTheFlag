using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;

public class MenuScreen : GlobalEventListener
{
    [SerializeField] private GameObject loadingPanel;
    public void StartServer()
    {
        BoltLauncher.StartServer();
        loadingPanel.SetActive(true);
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
        loadingPanel.SetActive(true);
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();
        BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "Game");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        base.SessionListUpdated(sessionList);
        bool join = false;
        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;
            if (photonSession.Source == UdpSessionSource.Photon)
            {
                join = true;
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
        loadingPanel.SetActive(join);

    }
    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        base.ConnectFailed(endpoint, token);
        loadingPanel.SetActive(false);

    }
}
