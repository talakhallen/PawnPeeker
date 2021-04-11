using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    static class Peek
    {
        public static bool TryJump(Thing thing, bool worldRenderedNow)
        {
            if (thing is Pawn pawn && pawn.IsWorldPawn())
            {
                CameraJumper.TryJump(thing);

                return false;
            }
            else
            {
                if (worldRenderedNow)
                {
                    CameraJumper.TryHideWorld();
                }

                if (Find.CurrentMap != thing.Map)
                {
                    Current.Game.CurrentMap = thing.Map;
                }

                Find.CameraDriver.JumpToCurrentMapLoc(thing.DrawPos);

                return true;
            }
        }
    }
}
