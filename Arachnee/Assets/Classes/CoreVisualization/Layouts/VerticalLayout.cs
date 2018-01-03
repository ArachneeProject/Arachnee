using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.Layouts
{
    public class VerticalLayout : LayoutBase
    {
        private VerticalLayoutGroup _verticalLayoutGroup;

        public override void Start()
        {
            _verticalLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
            if (_verticalLayoutGroup == null)
            {
                Logger.LogError($"No {nameof(VerticalLayoutGroup)} component found on children of {nameof(VerticalLayout)} GameObject.");
                return;
            }
        }

        public override void Add(Transform transformToAdd)
        {
            transformToAdd.SetParent(_verticalLayoutGroup.transform);
        }
    }
}