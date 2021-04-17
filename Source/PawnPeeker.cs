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

        private bool HandledClick()
        {
            return Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
        }

        private bool CanPeekPawn(bool peekPawnsAnywhere, Pawn pawn, bool worldRenderedNow, Map currentMap)
        {
            return peekPawnsAnywhere ||
                // Only peek a pawn when on the same map.
                (!pawn.IsWorldPawn() && !worldRenderedNow && pawn.Map == currentMap) ||
                // Only peek a world pawn when the world is rendered.
                (pawn.IsWorldPawn() && worldRenderedNow);
        }

        public override void GameComponentUpdate()
        {
            bool enabled = KeyBindings.Peek != null && KeyBindings.Peek.IsDown;

            if (!enabled)
            {
                return;
            }

            if (HandledClick())
            {
                return;
            }

            bool peekedPawn = false;

            if (Find.ColonistBar.ColonistOrCorpseAt(UI.MousePositionOnUIInverted) is Pawn pawn)
            {
                if (CanPeekPawn(Settings.Peek.PawnsAnywhere, pawn, WorldRendererUtility.WorldRenderedNow, Find.CurrentMap))
                {
                    peekedPawn = true;

                    Debug.Log(string.Format("Peek {0}!", pawn.Name));

                    if (Peek.TryJump(pawn, WorldRendererUtility.WorldRenderedNow))
                    {
                        if (Settings.Peek.AndSelect)
                        {
                            if (!Find.Selector.IsSelected(pawn))
                            {
                                // TODO: Restore the previous selection after peeking.
                                Find.Selector.ClearSelection();

                                // World pawns cannot be selected.
                                if (!pawn.IsWorldPawn())
                                {
                                    Debug.Log(string.Format("Select {0}!", pawn.Name));

                                    Find.Selector.Select(pawn);
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error(string.Format("Could not peek {0}!", pawn.Name));
                    }
                }
            }

            if (!peekedPawn)
            {
                if (Settings.Peek.Selected)
                {
                    if (Find.Selector.FirstSelectedObject is Thing thing)
                    {
                        bool canPeek = false;

                        if (thing is Pawn selectedPawn)
                        {
                            if (CanPeekPawn(Settings.Peek.PawnsAnywhere, selectedPawn, WorldRendererUtility.WorldRenderedNow, Find.CurrentMap))
                            {
                                canPeek = true;
                            }
                        }
                        else
                        {
                            canPeek = true;
                        }

                        if (canPeek)
                        {
                            if (Peek.TryJump(thing, WorldRendererUtility.WorldRenderedNow))
                            {
                                Debug.Log(string.Format("Peek selected {0}!", thing.Label));
                            }
                            else
                            {
                                Log.Error(string.Format("Could not peek selected {0}!", thing.Label));
                            }
                        }
                    }
                }
            }
        }
    }
}
