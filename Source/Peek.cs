using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    static class Peek
    {
        public static bool Now = false;
        public static bool Previously = false;

        public static bool Stay = false;

        // To
        public static Pawn NowColonist;
        public static Pawn PreviousColonist;

        // From
        public static bool FromWorld;
        public static Vector3 FromWorldPosition;

        public static IntVec3 FromMapPosition;
        public static Map FromMap;

        public static bool IsDonePeekingWhileLingering()
        {
            return !float.IsNaN(Hover.StopTime) &&
                   ((Time.realtimeSinceStartup - Hover.StopTime) >= Settings.Peek.WhileLingerTimeSeconds);
        }

        public static bool IsDoneLingering()
        {
            return !float.IsNaN(Hover.StopTime) &&
                   ((Time.realtimeSinceStartup - Hover.StopTime) >= Settings.Peek.LingerTimeSeconds);
        }

        public static void TryJump(Pawn colonist, bool worldRenderedNow)
        {
            Patch.HandleClicks.ShouldHandleDoubleClick = false;

            if (Settings.Peek.AndSelect)
            {
                if (!Find.Selector.IsSelected(colonist))
                {
                    Debug.Log(string.Format("Selected {0}!", colonist.Name));

                    // TODO: Restore the previous selection after peeking.
                    Find.Selector.ClearSelection();
                    Find.Selector.Select(colonist);
                }
            }

            if (colonist.IsWorldPawn())
            {
                CameraJumper.TryJump(colonist);
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
            }

            Patch.HandleClicks.ShouldHandleDoubleClick = true;
        }

        public static void SavePosition(bool worldRenderedNow)
        {
            // Save the current world position or map position. Always jump to
            // the original world position or map position even if multiple
            // pawns are peeked.
            if (worldRenderedNow)
            {
                if (!FromWorld || FromWorldPosition != Find.WorldCameraDriver.CurrentlyLookingAtPointOnSphere)
                {
                    FromWorld = true;

                    FromWorldPosition = Find.WorldCameraDriver.CurrentlyLookingAtPointOnSphere;

                    Debug.Log(string.Format("Peek pawn from world from {0}!", FromWorldPosition));
                }
            }
            else
            {
                if (FromWorld || FromMapPosition != Find.CameraDriver.MapPosition || FromMap != Find.CurrentMap)
                {
                    FromWorld = false;

                    FromMapPosition = Find.CameraDriver.MapPosition;
                    FromMap = Find.CurrentMap;

                    Debug.Log(string.Format("Peek pawn from {0} at {1}!", FromMap, FromMapPosition));
                }
            }
        }

        public static void TryStop()
        {
            if (!Stay)
            {
                Patch.HandleClicks.ShouldHandleDoubleClick = false;

                if (FromWorld)
                {
                    if (CameraJumper.TryShowWorld())
                    {
                        Find.WorldCameraDriver.JumpTo(FromWorldPosition);
                    }
                }
                else
                {
                    CameraJumper.TryJump(FromMapPosition, FromMap);
                }

                Patch.HandleClicks.ShouldHandleDoubleClick = true;
            }
            else
            {
                Debug.Log("Stay at pawn!");

                Stay = false;
            }
        }
    }
}
