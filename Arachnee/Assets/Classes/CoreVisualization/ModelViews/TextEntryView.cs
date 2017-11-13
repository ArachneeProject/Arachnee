using System;
using Assets.Classes.Core.Models;
using Assets.Classes.SceneScripts;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class TextEntryView : EntryView
    {
        protected override void Start()
        {
            base.Start();

            var text = this.gameObject.GetComponentInChildren<FloatingText>();
            if (text == null)
            {
                return;
            }

            // ensure the text is initialized
            text.Start();

            // setup entryview
            if (this.Entry is Movie)
            {
                var movie = (Movie)this.Entry;
                DateTime date;
                text.SetText(DateTime.TryParse(movie.ReleaseDate, out date)
                    ? $"{movie.Title} ({date.Year})"
                    : $"{movie.Title} (unknown date)");
            }
            else if (this.Entry is Artist)
            {
                text.SetText(((Artist)this.Entry).Name);
            }
            else
            {
                text.SetText($"{this.Entry.GetType().Name} {this.Entry.Id}");
            }
        }
    }
}