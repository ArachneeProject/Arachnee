using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.Layouts
{
    public class VerticalLayout : LayoutBase
    {
        public RectTransform start;
        public RectTransform end;

        public float spacing = 100;

        private RectTransform _area;
        private readonly Stack<RectTransform> _stack = new Stack<RectTransform>();

        public override void Start()
        {
            _area = this.GetComponent<RectTransform>();
            if (_area == null)
            {
                Logger.LogError($"No {nameof(RectTransform)} component found in {nameof(VerticalLayout)} GameObject.");
                return;
            }
        }

        public override void Clear()
        {
            _stack.Clear();
        }

        public override bool Add(RectTransform transformToAdd)
        {
            if (transformToAdd == null)
            {
                Logger.LogError($"Unable to add null item to {nameof(VerticalLayout)}.");
                return false;
            }

            transformToAdd.SetParent(_area.transform);
            transformToAdd.position = start.position;

            transformToAdd.Translate(Vector3.down * (transformToAdd.sizeDelta.y / 2f + _stack.Count * (transformToAdd.sizeDelta.y + spacing)));
            
            if (transformToAdd.position.y - transformToAdd.sizeDelta.y / 2f < end.position.y)
            {
                return false;
            }

            _stack.Push(transformToAdd);
            return true;
        }
    }
}