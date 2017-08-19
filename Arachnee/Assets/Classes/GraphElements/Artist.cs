namespace Assets.Classes.GraphElements
{
    public class Artist : Entry
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
