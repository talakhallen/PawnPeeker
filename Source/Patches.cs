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
                        Debug.Log("Start hovering!");

                        Hover.TryStart();

                        if (!float.IsNaN(Hover.StopTime))
                        {
                            Hover.StopTime = float.NaN;
                        }
                    }

                    if ((!Peek.Now ||
                         (Peek.PreviousColonist != Peek.NowColonist)) &&
                        Hover.IsStarted())
                    {
                        Debug.Log("Started hovering!");

                        Peek.Now = true;
                    }
                }
                else
                {
                    if (float.IsNaN(Hover.StopTime) && Hover.IsStarted())
                    {
                        Debug.Log("Stop hovering!");

                        Hover.TryStop();
                    }

                    if (Peek.Now && Peek.IsDoneLingering())
                    {
                        Debug.Log("Stopped lingering!");

                        Peek.Now = false;
                    }

                    if (Hover.DidStartWaitTimeout())
                    {
                        Debug.Log("Start wait timed out!");

                        Hover.StopTime = float.NaN;

                        Hover.StartTime = float.NaN;
                    }
                }

                // Peek
                if (Peek.Now)
                {
                    Peek.TryJump(Peek.NowColonist, WorldRendererUtility.WorldRenderedNow);

                    if (Peek.PreviousColonist != Peek.NowColonist)
                    {
                        Debug.Log(string.Format("Peek {0}!", Peek.NowColonist.Name));

                        Peek.PreviousColonist = Peek.NowColonist;
                    }
                }
                else
                {
                    if (Peek.Previously)
                    {
                        Debug.Log("Not peeking!");

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
                        Peek.SavePosition(WorldRendererUtility.WorldRenderedNow);
                    }

                    if (Peek.PreviousColonist != colonist)
                    {
                        Peek.NowColonist = colonist;
                    }
                }

                return true;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Debug.Log("HandleClicks Transpiler!");

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

                            Debug.Log("HandleClicks Transpiler DidHandleDoubleClick!");

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
                Debug.Log("DidHandleDoubleClick!");

                if (ShouldHandleDoubleClick)
                {
                    if (!Peek.Stay)
                    {
                        Debug.Log("DidHandleDoubleClick stay!");

                        Peek.Stay = true;
                    }
                }
            }
        }
    }
}
