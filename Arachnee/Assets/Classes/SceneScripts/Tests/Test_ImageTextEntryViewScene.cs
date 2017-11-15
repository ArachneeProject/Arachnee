using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_ImageTextEntryViewScene : MonoBehaviour
    {
        public ImageTextEntryView imageTextEntryViewPrefab;

        void Start()
        {
            var builder = new ModelViewBuilder();
            builder.SetPrefab<Movie>(imageTextEntryViewPrefab);
            builder.SetPrefab<Artist>(imageTextEntryViewPrefab);

            var provider = new ModelViewProvider(new OnlineDatabase(), builder);

            var movie = provider.GetEntryView("Movie-218");
            var artist = provider.GetEntryView("Artist-1100");

            movie.transform.position = Random.onUnitSphere * 4;
            artist.transform.position = Random.onUnitSphere * 4;
        }
    }
}