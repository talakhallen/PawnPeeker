﻿using RimWorld.Planet;
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
            bool peeking = KeyBindings.Peek != null && KeyBindings.Peek.IsDown;

            if (!peeking)
            {
                return;
            }

            if (HandledClick())
            {
                return;
            }

            bool pawnPeeked = false;

            if (Find.ColonistBar.ColonistOrCorpseAt(UI.MousePositionOnUIInverted) is Pawn pawn)
            {
                // If a pawn is being carried by another pawn, peek that pawn
                // instead. This prevents the world from being peeked in a
                // glitchy way.
                if (pawn.CarriedBy != null)
                {
                    pawn = pawn.CarriedBy;
                }

                if (CanPeekPawn(Settings.Peek.PawnsAnywhere, pawn, WorldRendererUtility.WorldRenderedNow, Find.CurrentMap))
                {
                    if (Peek.TryJump(pawn, WorldRendererUtility.WorldRenderedNow))
                    {
                        Debug.Log(string.Format("Peek {0}!", pawn.Name));

                        pawnPeeked = true;

                        if (Settings.Peek.AndSelect)
                        {
                            if (!Select.TrySelect(pawn))
                            {
                                Log.Error(string.Format("Could not select {0}!", pawn.Name));
                            }
                        }
                    }
                    else
                    {
                        Log.Error(string.Format("Could not peek {0}!", pawn.Name));
                    }
                }
            }

            if (!pawnPeeked)
            {
                if (Settings.Peek.Selected)
                {
                    Thing thing = Select.GetSelected(WorldRendererUtility.WorldRenderedNow);
                    if (thing != null &&
                        (!(thing is Pawn) || CanPeekPawn(Settings.Peek.PawnsAnywhere, thing as Pawn, WorldRendererUtility.WorldRenderedNow, Find.CurrentMap)))
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
