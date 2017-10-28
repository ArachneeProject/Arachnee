namespace Assets.Classes.GraphElements
{
    public class DefaultEntry : Entry
    {
        private static DefaultEntry _singleton;

        public static DefaultEntry Instance => _singleton ?? (_singleton = new DefaultEntry());

        private DefaultEntry()
        {
        }
    }
}
