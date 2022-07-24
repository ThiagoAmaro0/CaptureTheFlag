using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using TMPro;
using UdpKit;
using UnityEngine;

public class TeamManager : EntityBehaviour<ITeamManager>
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _scoreBlueText;
    [SerializeField] private TextMeshProUGUI _scoreRedText;
    [SerializeField] private GameObject _playerT1;
    [SerializeField] private GameObject _playerT2;
    [SerializeField] private GameObject _botT1;
    [SerializeField] private GameObject _botT2;
    [SerializeField] private GameObject _flagT1Prefab;
    [SerializeField] private GameObject _flagT2Prefab;
    [SerializeField] private GameObject _gameOverPanel;

    public List<PlayerManager> players;

    public static TeamManager instance;

    public override void Attached()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        players = new List<PlayerManager>();
        instance = this;
        base.Attached();
        state.Timer = 60;
        state.AddCallback("Timer", UpdateTimer);
        state.AddCallback("Started", StartGame);
        state.AddCallback("ScoreRed", UpdateUI);
        state.AddCallback("ScoreBlue", UpdateUI);
    }

    private void UpdateUI()
    {
        _scoreBlueText.text = "Score\n" + state.ScoreBlue;
        _scoreRedText.text = "Score\n" + state.ScoreRed;
    }

    private void StartGame()
    {
        if (state.Started)
        {
            foreach (PlayerManager player in FindObjectsOfType<PlayerManager>())
            {
                player.RespawnAction?.Invoke();
            }
            int count = 20 - FindObjectsOfType<PlayerManager>().Length;
            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 1)
                {
                    PlayerManager player = BoltNetwork.Instantiate(_botT1, new Vector3(45, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
                    player.state.IsBlue = true;
                    players.Add(player);
                }
                else
                {
                    PlayerManager player = BoltNetwork.Instantiate(_botT2, new Vector3(-45, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
                    player.state.IsBlue = false;
                    players.Add(player);
                }
            }
        }
    }

    private void UpdateTimer()
    {
        _timerText.text = state.Started ? state.Timer.ToString() : "Warm up\n" + state.Timer;
    }

    public void StartButton()
    {
        int count = FindObjectsOfType<HumanPlayer>().Length;

        if (count % 2 == 0)
        {
            PlayerManager player = BoltNetwork.Instantiate(_playerT1, new Vector3(45, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
            player.state.IsBlue = true;
            players.Add(player);
        }
        else
        {
            PlayerManager player = BoltNetwork.Instantiate(_playerT2, new Vector3(-45, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
            player.state.IsBlue = false;
            players.Add(player);
        }

        if (!state.Pregame && !state.Started)
        {
            state.Pregame = true;
            StartCoroutine(Pregame());
        }
        if (count == 20)
        {
            StopCoroutine(Pregame());
            state.Pregame = false;
            state.Started = true;
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Pregame()
    {
        while (state.Timer > 0)
        {
            yield return new WaitForSeconds(1);
            state.Timer--;
        }
        state.Pregame = false;
        state.Started = true;
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        state.Timer = 300;
        //StartGame
        BoltNetwork.Instantiate(_flagT1Prefab, new Vector3(45, 0, 0), Quaternion.identity).GetComponent<Flag>().state.IsBlue = true;
        BoltNetwork.Instantiate(_flagT2Prefab, new Vector3(-45, 0, 0), Quaternion.identity);

        while (state.Timer > 0)
        {
            yield return new WaitForSeconds(1);
            state.Timer--;
        }
        int code = 2;
        if (state.ScoreBlue > state.ScoreRed)
        {
            code = 0;
        }
        else if (state.ScoreBlue < state.ScoreRed)
        {
            code = 1;
        }
        GameOver(code);
    }

    public void Score(bool isBlue)
    {
        if (entity.IsOwner)
            if (isBlue)
                state.ScoreRed += 1;
            else
                state.ScoreBlue += 1;
    }

    public void GameOver(int code)
    {
        var evnt = Logout.Create();
        evnt.Send();
        print("Foda");
        _gameOverPanel.SetActive(true);
        _gameOverPanel.transform.GetChild(code).gameObject.SetActive(true);
    }
}
