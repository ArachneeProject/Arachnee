using Assets.Classes.Core.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.CoreVisualization
{
    public class SidePanel : MonoBehaviour
    {
        public InputField mainInputField;

        public Text entryText;

        public Button expandButton;
        public Button foldUpButton;
        public Button hideButton;
        public Button connectedToButton;

        private Entry _selectedEntry;

        public void Start()
        {
            expandButton.onClick.AddListener(Expand);
            foldUpButton.onClick.AddListener(FoldUp);
            hideButton.onClick.AddListener(Hide);
            connectedToButton.onClick.AddListener(ConnectTo);

            ClosePanel();
        }

        public void OpenPanel(Entry selectedEntry)
        {
            _selectedEntry = selectedEntry;

            entryText.text = selectedEntry.ToString();

            this.gameObject.SetActive(true);
        }

        public void ClosePanel()
        {
            this.gameObject.SetActive(false);
        }

        private void ConnectTo()
        {
            mainInputField.ActivateInputField();
            ClosePanel();
        }

        private void Hide()
        {
            throw new System.NotImplementedException();
        }

        private void FoldUp()
        {
            throw new System.NotImplementedException();
        }

        private void Expand()
        {
            throw new System.NotImplementedException();
        }
    }
}