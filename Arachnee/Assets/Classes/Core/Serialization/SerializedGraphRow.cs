namespace Assets.Classes.Core.Serialization
{
    public class SerializedGraphRow
    {
        /// <summary>
        /// Id. "Movie-218" becomes "MDA": M for Movie, DA as the hexadecimal value of 218.
        /// </summary>
        public string I { get; set; }

        /// <summary>
        /// Connected Ids. "Artist-1100::Actor::Movie-218" becomes "MDA_0":
        /// <see cref="I"/> is the key, MDA is the connected id, and 0 the type of connection (Actor).
        /// </summary>
        public string C { get; set; }
    }
}