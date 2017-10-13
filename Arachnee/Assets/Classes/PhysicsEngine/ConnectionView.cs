using System;
using System.Linq;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class ConnectionView : MonoBehaviour
    {
        public EntryView Left { get; set; }

        public EntryView Right { get; set; }
    }
}