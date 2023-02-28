using Enums;
using Extensions;
using Gameplay.GameState;
using UnityEngine;
using UnityEngine.UIElements;

namespace Views.Components
{
    public class ChooseSideSeat
    {
        private Label _playerNumberLabel;
        private Label _playerNameLabel;
        private Button _chooseSideButton;
        
        // playerNumber of who is sitting in this seat right now. 0-based; e.g. this is 0 for Player 1, 1 for Player 2, etc. Meaningless when m_State is Inactive (and in that case it is set to -1 for clarity)
        private int _playerNumber;

        // the last SeatState we were assigned
        private NetworkChooseSide.SeatState _state;

        // once this is true, we're never clickable again!
        private bool _isDisabled;

        private GameMarkType _markType;

        public GameMarkType MarkType => _markType;
        
        public ChooseSideSeat(VisualElement rootVE, GameMarkType markType)
        {
            _markType = markType;
            _state = NetworkChooseSide.SeatState.Inactive;
            _playerNumber = -1;
            MapFieldsToUI(rootVE);

            _chooseSideButton.clicked += () => ClientChooseSideState.Instance.OnPlayerClickedSeat(_markType);
        }

        public void SetState(NetworkChooseSide.SeatState state, int playerIndex, string playerName)
        {
            if(state == _state && playerIndex == _playerNumber)
                return; //no changes

            _state = state;
            _playerNumber = playerIndex;
            _playerNumberLabel.text = "P" + _playerNumber;
            _playerNameLabel.text = playerName;
            if (_state == NetworkChooseSide.SeatState.Inactive)
                _playerNumber = -1;
            ConfigureStateGraphics();
        }

        public bool IsLocked()
        {
            return _state == NetworkChooseSide.SeatState.LockedIn;
        }

        public void SetDisableInteractions(bool disable)
        {
            _chooseSideButton.SetEnabled(!disable);
            _isDisabled = disable;
        }

        private void ConfigureStateGraphics()
        {
            _playerNameLabel.style.color = Color.white;
            _chooseSideButton.SetEnabled(_isDisabled ? false : true);
            
            if (_state == NetworkChooseSide.SeatState.Inactive)
            {
                _playerNumberLabel.text = "";
                _playerNameLabel.text = "seat is inactive";
            }
            else if(_state == NetworkChooseSide.SeatState.LockedIn)//active or locked in
            {
                _playerNameLabel.style.color = Color.green;
                _chooseSideButton.SetEnabled(false);
            }
        }

        private void MapFieldsToUI(VisualElement root)
        {
            _playerNumberLabel = root.MapFieldToUI<Label>("PlayerNumberLabel");
            _playerNameLabel = root.MapFieldToUI<Label>("PlayerNameLabel");
            _chooseSideButton = root.MapFieldToUI<Button>();
        }
    }
}