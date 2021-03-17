using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    static class Peek
    {
        private static bool _stay = false;

        // To
        public static Pawn NowColonist;
        public static Pawn PreviousColonist;

        // From
        private static bool _fromWorld;
        private static Vector3 _fromWorldPosition;

        private static IntVec3 _fromMapPosition;
        private static Map _fromMap;

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
        }

        public static void SavePosition(bool worldRenderedNow)
        {
            // Save the current world position or map position. Always jump to
            // the original world position or map position even if multiple
            // pawns are peeked.
            if (worldRenderedNow)
            {
                if (!_fromWorld || _fromWorldPosition != Find.WorldCameraDriver.CurrentlyLookingAtPointOnSphere)
                {
                    _fromWorld = true;

                    _fromWorldPosition = Find.WorldCameraDriver.CurrentlyLookingAtPointOnSphere;

                    Debug.Log(string.Format("Peek pawn from world from {0}!", _fromWorldPosition));
                }
            }
            else
            {
                if (_fromWorld || _fromMapPosition != Find.CameraDriver.MapPosition || _fromMap != Find.CurrentMap)
                {
                    _fromWorld = false;

                    _fromMapPosition = Find.CameraDriver.MapPosition;
                    _fromMap = Find.CurrentMap;

                    Debug.Log(string.Format("Peek pawn from {0} at {1}!", _fromMap, _fromMapPosition));
                }
            }
        }

        public static void TryStop()
        {
            if (!_stay)
            {
                if (_fromWorld)
                {
                    if (CameraJumper.TryShowWorld())
                    {
                        Find.WorldCameraDriver.JumpTo(_fromWorldPosition);
                    }
                }
                else
                {
                    CameraJumper.TryJump(_fromMapPosition, _fromMap);
                }
            }
            else
            {
                Debug.Log("Stay at pawn!");

                _stay = false;
            }
        }
    }
}
