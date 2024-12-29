using System;
using RimWorld;
using Verse;

namespace Perishable
{

	public class SpecialThingFilterWorker_AllowPerishableThings : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            CompRottable compRottable = t.TryGetComp<CompRottable>();
            return compRottable != null && compRottable.PropsRot.rotDestroys;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            CompProperties_Rottable compProperties = def.GetCompProperties<CompProperties_Rottable>();
            return compProperties != null && compProperties.rotDestroys;
        }
    }
}


