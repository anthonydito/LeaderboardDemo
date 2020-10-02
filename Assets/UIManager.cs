using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject LoadingText;
    [SerializeField] Slider AddValueSlider;
    [SerializeField] Text AddRecordText;
    [SerializeField] ResultsList Items;

    private Leaderboards.Leaderboard CurrLeaderboard = new Leaderboards.LowerIsBetterLeaderboard();

    // Start is called before the first frame update
    void Start()
    {
        StopLoading();
        OnSliderChanged();
    }

    public void MyTopRecordPressed()
    {
        StartCoroutine(PerformMyTopRecord());
    }

    private IEnumerator PerformMyTopRecord()
    {
        // Show our loading indicator.
        StartLoading();

        // Get the enumerator that fetches the top record for this user.
        var enumerator = CurrLeaderboard.MyTopRecord();

        // We will be just waiting until the enumerator is done. This means that
        // we are in the loading state.
        while (enumerator.MoveNext())
        {
            yield return null;
        }

        // We are finished, stop the loading.
        StopLoading();

        // Fetches the response.
        var value = (Leaderboards.MyTopRecordResponsePayload)enumerator.Current;

        if (value.IsError)
        {
            // An error occurred. We need to handle that.
            Items.DisplayItems(new List<string> { "An error occurred. Please try again." });
            yield break;
        }

        if (value.HasFinished)
        {
            Items.DisplayItems(new List<string> { string.Format("You've finished this level in place={0} value={1}", value.Record.Place, value.Record.Value) });
        } else
        {
            Items.DisplayItems(new List<string> { string.Format("You have not finished this level") });
        }
    }

    public void TopRecordsPressed()
    {
        StartCoroutine(PerformTopRecords());
    }

    private IEnumerator PerformTopRecords()
    {
        // Show our loading indicator.
        StartLoading();

        // Get the enumerator that fetches the top records. 10 is the number of
        // records that we want to fetch
        var enumerator = CurrLeaderboard.TopRecords(10); 

        // We will be just waiting until the enumerator is done. This means that
        // we are in the loading state.
        while (enumerator.MoveNext())
        {
            yield return null;
        }

        // We are finished, stop the loading.
        StopLoading();

        // Fetches the top records.
        var value = (Leaderboards.TopRecordsResponsePayload)enumerator.Current;

        if (value.IsError)
        {
            // An error occurred. We need to handle that.
            Items.DisplayItems(new List<string> { "An error occurred. Please try again." });
            yield break;
        }

        // Assemble the data that we are going to display on the list.
        var items = new List<string>();
        foreach (var item in value.Records)
        {
            items.Add(string.Format("place={0} value={1}", item.Place, item.Value));
        }
        Items.DisplayItems(items);
    }

    public void AddRecordPressed()
    {
        StartCoroutine(PerformAddRecord());
    }

    private IEnumerator PerformAddRecord()
    {
        // Show our loading indicator
        StartLoading();

        // Get the enumerator that adds a record and then fetches the values
        // around our record.
        var submittingValue = AddValueSlider.value;
        var enumerator = CurrLeaderboard.AddRecord(submittingValue, 10);

        // We will be just waiting until the enumerator is done. This means that
        // we are in the loading state.
        while (enumerator.MoveNext())
        {
            yield return null;
        }

        // We are finished, stop the loading.
        StopLoading();

        // Fetches the add record response from the enumerator.
        var value = (Leaderboards.AddRecordResponsePayload)enumerator.Current;

        if (value.IsError)
        {
            // An error occurred. We need to handle that.
            Items.DisplayItems(new List<string> { "An error occurred. Please try again." });
            yield break;
        }

        // Assemble the data that we are going to display on the list.
        var items = new List<string>();
        if (value.IsBestScore)
        {
            items.Add("This was your best score!");
        } else
        {
            items.Add("This was not your best score.");
        }
        // This operation returns the user's best record for you to use if you
        // want to.
        var topRecord = value.TopRecord;
        items.Add(string.Format("This User's Top Record place={0} value={1}", topRecord.Place, topRecord.Value));
        foreach (var better in value.Better) {
            items.Add(string.Format("Better Score place={0} value={1}, isCurrentUser={2}", better.Place, better.Value, better.IsCurrentUser));
        }
        items.Add(string.Format("Your value is: {0}", submittingValue));
        foreach (var worse in value.Worse)
        {
            items.Add(string.Format("Worse Score place={0} value={1}, isCurrentUser={2}", worse.Place, worse.Value, worse.IsCurrentUser));
        }
        Items.DisplayItems(items);
    }

    public void LeaderboardSelectorChanged(int index)
    {
        if (index == 0)
        {
            CurrLeaderboard = new Leaderboards.LowerIsBetterLeaderboard();
        } else
        {
            CurrLeaderboard = new Leaderboards.HigherIsBetterLeaderboard();
        }
        ClearData();
    }

    public void OnSliderChanged()
    {
        var value = AddValueSlider.value;
        AddRecordText.text = string.Format("Add Record: {0}", value);
    }

    private void ClearData()
    {
        Items.Clear();
    }

    private void StartLoading()
    {
        ClearData();
        LoadingText.SetActive(true);
    }

    private void StopLoading()
    {
        LoadingText.SetActive(false);
    }
}
