using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class PickUp : Script
    {
        public enum Type
        {
            Health,
            Ammo
        }
        public int Amount;
        public Type type;

        public PickUp(Type pType, int amount)
        {
            type = pType;
            Amount = amount;
        }
    }
}
