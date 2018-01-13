using UnityEngine;

namespace Assets.Classes.CoreVisualization
{
    public class LoadingFeedback : MonoBehaviour
    {
        private int loadingsInProgress = 0;

        public void Start()
        {
            if (loadingsInProgress == 0)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void StartLoading()
        {
            loadingsInProgress++;
            this.gameObject.SetActive(true);
        }

        public void StopLoading()
        {
            loadingsInProgress--;
            if (loadingsInProgress < 1)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}