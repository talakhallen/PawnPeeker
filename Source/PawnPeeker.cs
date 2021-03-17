using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    class PawnPeeker : GameComponent
    {
        public PawnPeeker()
        {
        }

        public PawnPeeker(Game game)
        {
        }

        public override void GameComponentOnGUI()
        {
            Hover.Now = false;

            if (Input.GetMouseButton(0) ||
                Input.GetMouseButton(1) ||
                Input.GetMouseButton(2))
            {
                Did = true;

                return;
            }

            Did = false;

            if (Find.ColonistBar.ColonistOrCorpseAt(UI.MousePositionOnUIInverted) is Pawn colonist)
            {
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
            }
        }

        public override void GameComponentTick()
        {
            // Reset the start time if there was a click and hovering has
            // not started.
            if (HandleClicks.Did)
            {
                if (!float.IsNaN(Hover.StartTime) && !Hover.IsStarted())
                {
                    Debug.Log("Reset start time after click!");

                    Hover.StartTime = float.NaN;

                    Hover.TryStart();
                }
            }

            bool isDonePeekingWhileLingering = false;

            // Hover
            if (Hover.Now)
            {
                if (Hover.TryStart())
                {
                    Debug.Log("Start hovering!");
                }

                if (!float.IsNaN(Hover.StopTime))
                {
                    Hover.StopTime = float.NaN;
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
                // Reset the start time if hovering never started.
                if (!float.IsNaN(Hover.StartTime) && !Hover.IsStarted())
                {
                    Debug.Log("Reset start time!");

                    Hover.StartTime = float.NaN;
                }

                if (Hover.IsStarted())
                {
                    if (Hover.TryStop())
                    {
                        Debug.Log("Stop hovering!");
                    }
                }

                if (Peek.Now && Peek.IsDonePeekingWhileLingering())
                {
                    Debug.Log("Done peeking while lingering!");

                    isDonePeekingWhileLingering = true;
                }

                if (Peek.Now && Peek.IsDoneLingering())
                {
                    Debug.Log("Done lingering!");

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
                if (!isDonePeekingWhileLingering)
                {
                    Peek.TryJump(Peek.NowColonist, WorldRendererUtility.WorldRenderedNow);

                    if (Peek.PreviousColonist != Peek.NowColonist)
                    {
                        Debug.Log(string.Format("Peek {0}!", Peek.NowColonist.Name));

                        Peek.PreviousColonist = Peek.NowColonist;
                    }
                }
            }
            else
            {
                if (Peek.Previously)
                {
                    Debug.Log("Done peeking!");

                    if (!Settings.Peek.AndLinger)
                    {
                        Peek.TryStop();
                    }
                }
            }

            Hover.Previously = Hover.Now;

            Peek.Previously = Peek.Now;
        }
    }
}
