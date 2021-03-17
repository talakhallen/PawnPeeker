using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    class PawnPeeker : GameComponent
    {
        private static bool _hoveringNow = false;
        private static bool _hoveringPreviously = false;

        private static bool _peekingNow = false;
        private static bool _peekingPreviously = false;

        private static bool _handledClick = false;

        public PawnPeeker()
        {
        }

        public PawnPeeker(Game game)
        {
        }

        public override void GameComponentOnGUI()
        {
            _hoveringNow = false;

            if (Input.GetMouseButton(0) ||
                Input.GetMouseButton(1) ||
                Input.GetMouseButton(2))
            {
                _handledClick = true;

                return;
            }

            _handledClick = false;

            if (Find.ColonistBar.ColonistOrCorpseAt(UI.MousePositionOnUIInverted) is Pawn colonist)
            {
                if (Settings.Peek.PawnsAnywhere ||
                    // Only peek a pawn when on the same map.
                    (!colonist.IsWorldPawn() && !WorldRendererUtility.WorldRenderedNow && colonist.Map == Find.CurrentMap) ||
                    // Only peek a world pawn when the world is rendered.
                    (colonist.IsWorldPawn() && WorldRendererUtility.WorldRenderedNow))
                {
                    _hoveringNow = true;

                    if (!_peekingPreviously)
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
            if (_handledClick)
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
            if (_hoveringNow)
            {
                if (Hover.TryStart())
                {
                    Debug.Log("Start hovering!");
                }

                if (!float.IsNaN(Hover.StopTime))
                {
                    Hover.StopTime = float.NaN;
                }

                if ((!_peekingNow ||
                     (Peek.PreviousColonist != Peek.NowColonist)) &&
                    Hover.IsStarted())
                {
                    Debug.Log("Started hovering!");

                    _peekingNow = true;
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

                if (_peekingNow && Peek.IsDonePeekingWhileLingering())
                {
                    Debug.Log("Done peeking while lingering!");

                    isDonePeekingWhileLingering = true;
                }

                if (_peekingNow && Peek.IsDoneLingering())
                {
                    Debug.Log("Done lingering!");

                    _peekingNow = false;
                }

                if (Hover.DidStartWaitTimeout())
                {
                    Debug.Log("Start wait timed out!");

                    Hover.StopTime = float.NaN;

                    Hover.StartTime = float.NaN;
                }
            }

            // Peek
            if (_peekingNow)
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
                if (_peekingPreviously)
                {
                    Debug.Log("Done peeking!");

                    if (!Settings.Peek.AndLinger)
                    {
                        Peek.TryStop();
                    }
                }
            }

            _hoveringPreviously = _hoveringNow;

            _peekingPreviously = _peekingNow;
        }
    }
}
