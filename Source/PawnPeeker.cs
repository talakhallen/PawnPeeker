using RimWorld.Planet;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    class PawnPeeker : GameComponent
    {
        private static bool _handledClick = false;

        private static Pawn _previousColonist = null;

        public PawnPeeker()
        {
        }

        public PawnPeeker(Game game)
        {
        }

        private bool HandledClick()
        {
            return Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
        }

        private bool CanPeekPawn(bool peekPawnsAnywhere, Pawn colonist, bool worldRenderedNow, Map currentMap)
        {
            return peekPawnsAnywhere ||
                // Only peek a pawn when on the same map.
                (!colonist.IsWorldPawn() && !worldRenderedNow && colonist.Map == currentMap) ||
                // Only peek a world pawn when the world is rendered.
                (colonist.IsWorldPawn() && worldRenderedNow);
        }

        public override void GameComponentUpdate()
        {
            bool enabled = KeyBindings.Enable != null && KeyBindings.Enable.IsDown;

            if (!enabled)
            {
                return;
            }

            _handledClick = HandledClick();

            if (_handledClick)
            {
                return;
            }

            if (Find.ColonistBar.ColonistOrCorpseAt(UI.MousePositionOnUIInverted) is Pawn colonist)
            {
                if (CanPeekPawn(Settings.Peek.PawnsAnywhere, colonist, WorldRendererUtility.WorldRenderedNow, Find.CurrentMap))
                {
                    if (_previousColonist != colonist)
                    {
                        Debug.Log(string.Format("Peek {0}!", colonist.Name));

                        _previousColonist = colonist;
                    }

                    if (Peek.TryJump(colonist, WorldRendererUtility.WorldRenderedNow))
                    {
                        if (Settings.Peek.AndSelect)
                        {
                            if (!Find.Selector.IsSelected(colonist))
                            {
                                Debug.Log(string.Format("Select {0}!", colonist.Name));

                                // TODO: Restore the previous selection after peeking.
                                Find.Selector.ClearSelection();
                                Find.Selector.Select(colonist);
                            }
                        }
                    }
                }
            }
        }
    }
}
