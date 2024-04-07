using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.GeneralGameLogic
{
    // This should be expaned in phase3 - need to split into zombie and non-zombie modes (make this abstract in that phase)
    public class BaseGameModeLogic : MonoBehaviour
    {
        [SerializeField] public GameModeType GameMode;

        private void Start()
        {

        }
    }
}
