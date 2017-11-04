using System;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.SceneScripts;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.Tailors
{
    public class TextTailor : ITailor
    {
        public void DressUp(EntryView entryView)
        {
            var text = entryView.gameObject.GetComponentInChildren<FloatingText>();
            if (text == null)
            {
                return;
            }

            // ensure the text is initialized
            text.Start();

            if (entryView.Entry is Movie)
            {
                var movie = (Movie) entryView.Entry;
                DateTime date;
                text.SetText(DateTime.TryParse(movie.ReleaseDate, out date)
                    ? $"{movie.Title} ({date.Year})"
                    : $"{movie.Title} (unknown date)");
            }
            else if (entryView.Entry is Artist)
            {
                text.SetText(((Artist)entryView.Entry).Name);
            }
            else
            {
                text.SetText($"{entryView.Entry.GetType().Name} {entryView.Entry.Id}");
            }
        }

        public void DressUp(ConnectionView connectionView)
        {
            
        }

        public void DressUp(SearchResultView searchResultView)
        {
            
        }
    }
}