using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private List<UIPanel> _uiPanels;
        public static UIManager Instance { get; private set; }

        private UIPanel _currentPanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _uiPanels = FindObjectsOfType<UIPanel>().ToList();
            foreach (var panel in _uiPanels)
            {
                panel.Close();
            }
        }

        public void Open(Type type)
        {
            foreach (var uiPanel in _uiPanels)
            {
                if (uiPanel.GetType() == type)
                {
                    if (_currentPanel != null)
                    {
                        if (_currentPanel is not GamePanel)
                        {
                            _currentPanel.Close();
                        }
                    }

                    _currentPanel = uiPanel;
                    _currentPanel.Open();
                    return;
                }
            }
        }

        public void SetWinMessage(string message)
        {
            foreach (var uiPanel in _uiPanels)
            {
                if (uiPanel is WinPanelUI winPanelUI)
                {
                    winPanelUI.SetWinText(message);
                    return;
                }
            }
        }

        public void OpenMessage(string message)
        {
            foreach (var uiPanel in _uiPanels)
            {
                if (uiPanel is UIMessage messagePanel)
                {
                    messagePanel.SetMessage(message);
                    Open(typeof(UIMessage));
                    return;
                }
            }
        }
    }
}