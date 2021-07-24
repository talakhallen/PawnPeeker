using RimWorld.Planet;
using Verse;

namespace PawnPeeker
{
    static class Peek
    {
        public static bool TryJump(Thing thing, bool worldRenderedNow)
        {
            if (thing.Map == null || (thing is Pawn pawn && pawn.IsWorldPawn()))
            {
                CameraJumper.TryJump(thing);

                return true;
            }
            else
            {
                if (worldRenderedNow)
                {
                    if (!CameraJumper.TryHideWorld())
                    {
                        Log.Warning("Could not hide world!");
                    }
                }

                if (Current.Game.CurrentMap != thing.Map)
                {
                    Current.Game.CurrentMap = thing.Map;
                }

                Find.CameraDriver.JumpToCurrentMapLoc(thing.DrawPos);

                return true;
            }
        }
    }
}
