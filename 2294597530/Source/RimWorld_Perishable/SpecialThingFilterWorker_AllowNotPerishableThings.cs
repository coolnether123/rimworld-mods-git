using RimWorld;
using Verse;
using System;

namespace Perishable
{
	public class SpecialThingFilterWorker_AllowNotPerishableThings : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            CompRottable compRottable = t.TryGetComp<CompRottable>();
            return compRottable == null;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            CompProperties_Rottable compProperties = def.GetCompProperties<CompProperties_Rottable>();
            return compProperties == null;
        }
    }
	
}


