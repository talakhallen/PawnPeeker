﻿using RimWorld.Planet;
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
        public static Pawn Colonist;

        // From
        public static bool FromWorld;
        public static Vector3 FromWorldPosition;

        public static IntVec3 FromMapPosition;
        public static Map FromMap;

        // Settings
        public static float LingerTimeSeconds = 0.5f;
        public static bool PawnsAnywhere = false;

        public static bool IsDoneLingering()
        {
            return !float.IsNaN(Hover.StopTime) &&
                   ((Time.realtimeSinceStartup - Hover.StopTime) >= Peek.LingerTimeSeconds);
        }

        public static void TryJump(Pawn colonist, bool worldRenderedNow)
        {
            Patch.HandleClicks.ShouldHandleDoubleClick = false;

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

        public static void StartOrContinue(Pawn colonist, bool worldRenderedNow)
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
                }
            }
            else
            {
                if (FromWorld || FromMapPosition != Find.CameraDriver.MapPosition || FromMap != Find.CurrentMap)
                {
                    FromWorld = false;

                    FromMapPosition = Find.CameraDriver.MapPosition;
                    FromMap = Find.CurrentMap;
                }
            }

            Peek.Colonist = colonist;
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
                Stay = false;
            }
        }
    }
}