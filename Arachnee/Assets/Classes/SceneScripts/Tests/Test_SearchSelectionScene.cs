using System.Collections.Generic;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.VisibleEntries;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;
using UnityEngine.UI;

public class Test_SearchSelectionScene : MonoBehaviour
{
    public EntryView entryViewPrefab;

    public InputField input;
    public Text clickedEntryViewName;
    public Button validateButton;

    private readonly EntryViewProvider _provider = new EntryViewProvider();

    private EntryView _selectedEntryView;
    private readonly List<EntryView> _searchResults = new List<EntryView>();

    void Start () 
    {
        _provider.BiggerProvider = new OnlineDatabase();
        _provider.EntryViewPrefabs.Add(typeof(Movie), entryViewPrefab);
        _provider.EntryViewPrefabs.Add(typeof(Artist), entryViewPrefab);

        clickedEntryViewName.gameObject.SetActive(false);
        validateButton.gameObject.SetActive(false);
    }

    public void RunSearch()
    {
        // clear previous search
        clickedEntryViewName.gameObject.SetActive(false);
        validateButton.gameObject.SetActive(false);

        _selectedEntryView = null;
        foreach (var searchResult in _searchResults)
        {
            searchResult.OnClicked -= UpdateClickedEntryView;
            DestroyImmediate(searchResult.gameObject); // I know what I'm doing here
        }
        _searchResults.Clear();

        // run search
        var results = _provider.GetEntryViewResults<Entry>(input.text);
        Debug.Log(results.Count + " results for " + input.text);

        // update search results
        while (results.Count > 0)
        {
            var result = results.Dequeue();
            result.OnClicked += UpdateClickedEntryView;
            result.transform.position = Random.onUnitSphere*2;
            _searchResults.Add(result);
        }
    }

    private void UpdateClickedEntryView(EntryView entryView)
    {
        _selectedEntryView = entryView;

        clickedEntryViewName.gameObject.SetActive(true);
        clickedEntryViewName.text = entryView.ToString();

        validateButton.gameObject.SetActive(true);
    }
    
    public void Validate()
    {
        var particle = _selectedEntryView.gameObject.GetComponent<ParticleSystem>();
        particle.Emit(10);

        validateButton.gameObject.SetActive(false);
        clickedEntryViewName.gameObject.SetActive(false);
    }
}
