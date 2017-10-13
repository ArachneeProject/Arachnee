using System.Collections.Generic;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public abstract class GraphEngine : MonoBehaviour
    {
        protected HashSet<EntryView> Vertices = new HashSet<EntryView>();
        protected HashSet<ConnectionView> Edges = new HashSet<ConnectionView>();

        public abstract void Add(EntryView entryView);

        public abstract void Add(ConnectionView connectionView);

        public abstract void Remove(EntryView entryView);

        public abstract void Remove(ConnectionView connectionView);
    }
}