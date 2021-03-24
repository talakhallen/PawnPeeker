using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    static class Peek
    {
        public static bool TryJump(Pawn colonist, bool worldRenderedNow)
        {
            if (colonist.IsWorldPawn())
            {
                CameraJumper.TryJump(colonist);

                return false;
            }
            else
            {
                if (worldRenderedNow)
                {
                    CameraJumper.TryHideWorld();
                }

                if (Find.CurrentMap != colonist.Map)
                {
                    Current.Game.CurrentMap = colonist.Map;
                }

                Find.CameraDriver.JumpToCurrentMapLoc(colonist.DrawPos);

                return true;
            }
        }
    }
}
