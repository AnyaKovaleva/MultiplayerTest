using ConnectionManagement;
using UnityEngine;
using Utils;
using VContainer;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkChooseSide))]

    public class ServerChooseSideState : GameStateBehaviour
    {
        public override GameState ActiveState => GameState.ChooseSide;

        [SerializeField] private NetcodeHooks _netcodeHooks;
        [SerializeField] private NetworkChooseSide _networkChooseSide;

        [Inject] private ConnectionManager _connectionManager;
    }
}