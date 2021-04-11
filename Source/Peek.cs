using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    static class Peek
    {
        public static bool TryJump(Pawn pawn, bool worldRenderedNow)
        {
            if (pawn.IsWorldPawn())
            {
                CameraJumper.TryJump(pawn);

                return false;
            }
            else
            {
                if (worldRenderedNow)
                {
                    CameraJumper.TryHideWorld();
                }

                if (Find.CurrentMap != pawn.Map)
                {
                    Current.Game.CurrentMap = pawn.Map;
                }

                Find.CameraDriver.JumpToCurrentMapLoc(pawn.DrawPos);

                return true;
            }
        }
    }
}
