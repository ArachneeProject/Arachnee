using System;
using Assets.Classes.CoreVisualization.ModelViews;
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
        
        private EntryView _selectedEntry;

        public event Action<EntryView> OnExpandRequested;
        public event Action<EntryView> OnFoldUpRequested;
        public event Action<EntryView> OnHideRequested;

        public void Start()
        {
            expandButton.onClick.RemoveListener(Expand);
            expandButton.onClick.AddListener(Expand);

            foldUpButton.onClick.RemoveListener(FoldUp);
            foldUpButton.onClick.AddListener(FoldUp);

            hideButton.onClick.RemoveListener(Hide);
            hideButton.onClick.AddListener(Hide);
            
            ClosePanel();
        }

        public void OpenPanel(EntryView selectedEntry)
        {
            _selectedEntry = selectedEntry;
            entryText.text = selectedEntry.Entry.ToString();

            this.gameObject.SetActive(true);
        }

        public void ClosePanel()
        {
            this.gameObject.SetActive(false);
            _selectedEntry = null;
        }
        
        private void Expand()
        {
            if (_selectedEntry != null)
            {
                OnExpandRequested?.Invoke(_selectedEntry);
            }
        }
        
        private void FoldUp()
        {
            if (_selectedEntry != null)
            {
                OnFoldUpRequested?.Invoke(_selectedEntry);
            }
        }

        private void Hide()
        {
            if (_selectedEntry != null)
            {
                OnHideRequested?.Invoke(_selectedEntry);
            }
        }
    }
}