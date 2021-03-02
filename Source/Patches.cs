using System.Collections.Generic;
using System.Reflection.Emit;
using System;

using HarmonyLib;
using RimWorld.Planet;
using RimWorld;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    namespace Patch
    {
        [HarmonyPatch(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI))]
        static class HandleColonistBarOnGUI
        {
            static bool Prefix()
            {
                Hover.Handled = false;

                Hover.Now = false;

                return true;
            }

            // HandleClicks.Prefix runs between these two

            static void Postfix()
            {
                if (!Hover.Handled)
                {
                    return;
                }

                // Hover
                if (Hover.Now)
                {
                    if (!Hover.Previously)
                    {
                        Hover.TryStart();

                        if (!float.IsNaN(Hover.StopTime))
                        {
                            Hover.StopTime = float.NaN;
                        }
                    }

                    if (!Peek.Now && Hover.IsStarted())
                    {
                        Peek.Now = true;
                    }
                }
                else
                {
                    if (float.IsNaN(Hover.StopTime) && Hover.IsStarted())
                    {
                        Hover.TryStop();
                    }

                    if (Peek.Now && Peek.IsDoneLingering())
                    {
                        Peek.Now = false;
                    }

                    if (Hover.DidStartWaitTimeout())
                    {
                        Hover.StopTime = float.NaN;

                        Hover.StartTime = float.NaN;
                    }
                }

                // Peek
                if (Peek.Now)
                {
                    Peek.TryJump(Peek.Colonist, WorldRendererUtility.WorldRenderedNow);
                }
                else
                {
                    if (Peek.Previously)
                    {
                        Peek.TryStop();
                    }
                }

                Hover.Previously = Hover.Now;

                Peek.Previously = Peek.Now;
            }
        }

        [HarmonyPatch(typeof(ColonistBarColonistDrawer), nameof(ColonistBarColonistDrawer.HandleClicks))]
        static class HandleClicks
        {
            public static bool ShouldHandleDoubleClick = true;

            static bool Prefix(Rect rect, Pawn colonist)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return true;
                }

                if (Input.GetMouseButton(0) ||
                    Input.GetMouseButton(1) ||
                    Input.GetMouseButton(2))
                {
                    return true;
                }

                Hover.Handled = true;

                if (!Mouse.IsOver(rect))
                {
                    return true;
                }

                if (Settings.Peek.PawnsAnywhere ||
                    // Only peek a pawn when on the same map.
                    (!colonist.IsWorldPawn() && !WorldRendererUtility.WorldRenderedNow && colonist.Map == Find.CurrentMap) ||
                    // Only peek a world pawn when the world is rendered.
                    (colonist.IsWorldPawn() && WorldRendererUtility.WorldRenderedNow))
                {
                    Hover.Now = true;

                    if (!Peek.Previously)
                    {
                        Peek.StartOrContinue(colonist, WorldRendererUtility.WorldRenderedNow);
                    }
                }

                return true;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool found = false;

                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.Calls(AccessTools.Method(typeof(CameraJumper),
                                          nameof(CameraJumper.TryJump),
                                          new Type[] { typeof(GlobalTargetInfo) })))
                    {
                        if (!found)
                        {
                            found = true;

                            yield return new CodeInstruction(OpCodes.Call,
                                                             AccessTools.Method(typeof(HandleClicks),
                                                             nameof(HandleClicks.DidHandleDoubleClick)));
                        }
                    }

                    yield return instruction;
                }
            }

            static void DidHandleDoubleClick()
            {
                if (ShouldHandleDoubleClick)
                {
                    if (!Peek.Stay)
                    {
                        Peek.Stay = true;
                    }
                }
            }
        }
    }
}
