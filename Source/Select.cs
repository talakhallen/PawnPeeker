using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld.Planet;
using Verse;

namespace PawnPeeker
{
    class Select
    {
        public static bool TrySelect(Pawn pawn)
        {
            if (!pawn.IsWorldPawn())
            {
                if (!Find.Selector.IsSelected(pawn))
                {
                    Debug.Log(string.Format("Select {0}!", pawn.Name));

                    // TODO: Restore the previous selection after peeking.
                    Find.Selector.ClearSelection();

                    Find.Selector.Select(pawn);
                }
            }
            else
            {
                if (!Find.WorldSelector.IsSelected(pawn.GetCaravan()))
                {
                    Debug.Log(string.Format("Select {0}!", pawn.Name));

                    // TODO: Restore the previous selection after peeking.
                    Find.WorldSelector.ClearSelection();

                    Find.WorldSelector.Select(pawn.GetCaravan());
                }
            }

            return true;
        }

        public static Thing GetSelected()
        {
            if (Find.Selector.FirstSelectedObject is Thing thing)
            {
                return thing;
            }

            return null;
        }
    }
}
