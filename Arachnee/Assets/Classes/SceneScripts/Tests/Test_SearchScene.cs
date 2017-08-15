using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.Physical;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        private readonly OnlineDatabase _onlineDatabase = new OnlineDatabase();
        private readonly PhysicalProvider _physicalProvider = new PhysicalProvider();

        void Start ()
        {
            _physicalProvider.BiggerProvider = _onlineDatabase;

            

        }
    }
}
