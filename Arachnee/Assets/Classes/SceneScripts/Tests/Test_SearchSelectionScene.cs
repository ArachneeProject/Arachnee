using System.Collections.Generic;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;
using UnityEngine.UI;

public class Test_SearchSelectionScene : MonoBehaviour
{
    public Vertex vertexPrefab;

    public InputField input;
    public Text clickedVertexName;
    public Button validateButton;

    private readonly GameObjectProvider _provider = new GameObjectProvider();

    private Vertex _selectedVertex;
    private readonly List<Vertex> _searchResults = new List<Vertex>();

    void Start () 
    {
        _provider.BiggerProvider = new OnlineDatabase();
        _provider.VertexPrefabs.Add(typeof(Movie), vertexPrefab);
        _provider.VertexPrefabs.Add(typeof(Artist), vertexPrefab);

        clickedVertexName.gameObject.SetActive(false);
        validateButton.gameObject.SetActive(false);
    }

    public void RunSearch()
    {
        // clear previous search
        clickedVertexName.gameObject.SetActive(false);
        validateButton.gameObject.SetActive(false);

        _selectedVertex = null;
        foreach (var searchResult in _searchResults)
        {
            searchResult.OnClicked -= UpdateClickedVertex;
            DestroyImmediate(searchResult.gameObject); // I know what I'm doing here
        }
        _searchResults.Clear();

        // run search
        var results = _provider.GetVerticesResults<Entry>(input.text);
        Debug.Log(results.Count + " results for " + input.text);

        // update search results
        while (results.Count > 0)
        {
            var result = results.Dequeue();
            result.OnClicked += UpdateClickedVertex;
            result.transform.position = Random.onUnitSphere*2;
            _searchResults.Add(result);
        }
    }

    private void UpdateClickedVertex(Vertex vertex)
    {
        _selectedVertex = vertex;

        clickedVertexName.gameObject.SetActive(true);
        clickedVertexName.text = vertex.ToString();

        validateButton.gameObject.SetActive(true);
    }
    
    public void Validate()
    {
        var particle = _selectedVertex.gameObject.GetComponent<ParticleSystem>();
        particle.Emit(10);

        validateButton.gameObject.SetActive(false);
        clickedVertexName.gameObject.SetActive(false);
    }
}
