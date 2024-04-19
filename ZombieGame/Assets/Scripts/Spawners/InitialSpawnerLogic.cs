using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    public class InitialSpawnerLogic : MonoBehaviour
    {
        [SerializeField]
        protected TeamType TeamType;

        public TeamType GetTeamType()
        {
            return TeamType;
        }
    }
}
