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

            return true;
        }
    }
}
