using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders.Tailors;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_FriendlyViewsScene : MonoBehaviour
    {
        public EntryView entryViewPrefab;

        void Start()
        {
            var builder = new ModelViewBuilder();
            builder.SetPrefab<Movie>(entryViewPrefab);
            builder.SetPrefab<Artist>(entryViewPrefab);
            builder.SetTailor(new TextTailor());

            var manager = new ModelViewManager(new OnlineDatabase(), builder);

            var movie = manager.GetEntryView("Movie-218");
            var artist = manager.GetEntryView("Artist-1100");

            movie.transform.position = Random.onUnitSphere * 4;
            artist.transform.position = Random.onUnitSphere * 4;
        }
    }
}