namespace Assets.Classes.GraphElements
{
    public class Movie : Entry
    {
        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
