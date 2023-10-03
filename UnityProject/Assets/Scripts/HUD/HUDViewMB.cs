using Asteroids;
using TMPro;
using UnityEngine;

public class HUDViewMB : MonoBehaviour
{
    public TMP_Text WinText; 
    public TMP_Text LoseText; 
    public TMP_Text ScoreText;
    public GameObject HeartsContainer;
    public GameObject HeartsPrototype;

    private IMatchState _matchState;
    // Start is called before the first frame update
    public void Init(IMatchState matchState)
    {
        _matchState = matchState;
        Refresh();
        _matchState.OnUpdated += Refresh;
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
        while (_matchState.Hearts > HeartsContainer.transform.childCount)//instantiate missing
        {
            Instantiate(HeartsPrototype, HeartsContainer.transform);
        }

        for (int i = 0; i < HeartsContainer.transform.childCount -_matchState.Hearts; i++)//delete excess;
        {
            Destroy(HeartsContainer.transform.GetChild(HeartsContainer.transform.childCount - 1).gameObject);
        }
    }
}
