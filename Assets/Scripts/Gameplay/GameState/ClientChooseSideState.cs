using ConnectionManagement;
using UnityEngine;
using Utils;
using VContainer;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks))]
    public class ClientChooseSideState : GameStateBehaviour
    {
        public override GameState ActiveState
        {
            get { return GameState.ChooseSide; }
        }
        
        [SerializeField]
        NetcodeHooks m_NetcodeHooks;

        [Inject] private ConnectionManager _connectionManager;
        
        
    }
}