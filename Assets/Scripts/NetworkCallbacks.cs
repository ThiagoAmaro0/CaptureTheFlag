using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using UdpKit;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class NetworkCallbacks : GlobalEventListener
{
    [SerializeField] private GameObject _gameManagerPrefab;
    private TeamManager _teamManager;
    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        base.SceneLoadRemoteDone(connection, token);

    }

    public void StartButton()
    {
        _teamManager = FindObjectOfType<TeamManager>();
        if (!_teamManager)
        {
            _teamManager = BoltNetwork.Instantiate(_gameManagerPrefab).GetComponent<TeamManager>();
        }
        _teamManager.StartButton();
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
    {
        base.BoltShutdownBegin(registerDoneCallback, disconnectReason);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public override void OnEvent(Logout evnt)
    {

        print("SHUTDOWN");
        Invoke("Shutdown", 4);
    }

    public void Shutdown()
    {
        foreach (var cnt in BoltNetwork.Connections)
        {
            cnt.Disconnect();

        }
        BoltNetwork.Shutdown();
    }
}
