using System;
using Asteroids;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDViewMB : MonoBehaviour
{
    public TMP_Text WinText; 
    public TMP_Text LoseText; 
    public TMP_Text ScoreText;
    public GameObject HeartsContainer;
    public GameObject HeartsPrototype;
    public Button NewGameButton;
    public Slider ShieldSlider;

    private IMatchState _matchState;
    // Start is called before the first frame update
    public void Init(IMatchState matchState)
    {
        _matchState = matchState;
        Refresh();
        _matchState.OnUpdated += Refresh;
        NewGameButton.onClick.AddListener(() => SceneManager.LoadScene(0));//reboot
    }

    private void OnDestroy()
    {
        if (_matchState != null)
        {
            _matchState.OnUpdated -= Refresh;
        }
    }

    // Update is called once per frame
    void Refresh()
    {
        ScoreText.text = $"SCORE: {_matchState.Score}";
        WinText.gameObject.SetActive(_matchState.State == IMatchState.Status.Win);
        LoseText.gameObject.SetActive(_matchState.State == IMatchState.Status.Lose);
        NewGameButton.gameObject.SetActive(_matchState.State != IMatchState.Status.InProgress);
        while (_matchState.Hearts > HeartsContainer.transform.childCount)//instantiate missing
        {
            Instantiate(HeartsPrototype, HeartsContainer.transform);
        }

        for (int i = 0; i < HeartsContainer.transform.childCount -_matchState.Hearts; i++)//delete excess;
        {
            Destroy(HeartsContainer.transform.GetChild(HeartsContainer.transform.childCount - 1).gameObject);
        }
    }

    private void Update()
    {
        ShieldSlider.gameObject.SetActive(_matchState.ShieldNormalizedDurationLeft > 0.0001f);
        if (_matchState.ShieldNormalizedDurationLeft > 0.0001f)
        {
            ShieldSlider.value = _matchState.ShieldNormalizedDurationLeft;
        }
    }
}
