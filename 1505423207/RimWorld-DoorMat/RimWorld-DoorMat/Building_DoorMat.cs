using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace LT
{
    public class Building_DoorMat : Building
    {
        //Syrchalis' code is nice.
        static FieldInfo carriedFilthList = typeof(Pawn_FilthTracker).GetField("carriedFilth", BindingFlags.NonPublic | BindingFlags.Instance);

        //Help from ignis with DropCarriedFilth()
        static MethodInfo dropCarriedFilth = typeof(Pawn_FilthTracker).GetMethod("DropCarriedFilth", BindingFlags.Instance | BindingFlags.NonPublic);

        public override void Tick()
        {
            base.Tick();


            // Execute only every 10 ticks
            // 1.3: test at 2
            if (! this.IsHashIntervalTick(10))
            {
                return;
            }
            if (Map == null)
            {
                return;
            }
            if (this.OccupiedRect() == null)
            {
                return;
            }
            foreach (var rect in this.OccupiedRect())
            {
                if (rect == null)
                {
                    continue;
                }
                
                if (rect.Impassable(Map))
                {
                    continue;
                }

                //hotfix do surrounding area
                DoSurroundArea(rect);

                //hotfix do self
                //2nd hotfix - clean self slow
                SelfClean(rect);

                //True Magic Now
                HavePawnsDropFilth(rect);

            }
        }

        private void DoSurroundArea(IntVec3 rect)
        {
            if (this.IsHashIntervalTick(2000))
            {
                var filths = Map.thingGrid.ThingsAt(rect.RandomAdjacentCell8Way())
                        .Where(s => s.GetType() == typeof(Filth))
                        .Cast<Filth>();
                if (filths != null)
                { //Fixing ticking error issue
                    foreach (var f in filths)
                    {
                        f.Destroy(DestroyMode.Vanish);
                        break;
                    }
                }
            }
        }

        private void SelfClean(IntVec3 rect)
        {
            if (this.IsHashIntervalTick(3000))
            {
                var filths = Map.thingGrid.ThingsAt(rect)
                      .Where(s => s.GetType() == typeof(Filth))
                      .Cast<Filth>();
                if (filths != null)
                { //Fixing ticking error issue

                    foreach (var f in filths)
                    {
                        f.Destroy(DestroyMode.Vanish);
                        break;
                    }
                }
            }
        }

        private void HavePawnsDropFilth(IntVec3 rect)
        {
            var pawns = Map.thingGrid.ThingsAt(rect)
                   .Where(s => s.GetType() == typeof(Pawn))
                   .Cast<Pawn>();
            if (pawns == null)
            {
                return;
            }
            foreach (var pawn in pawns)
            {

                List<Filth> carriedFilth = (List<Filth>)carriedFilthList.GetValue(pawn.filth);
                if (!carriedFilth.NullOrEmpty())
                {
                    for (int num = carriedFilth.Count - 1; num >= 0; num--)
                    {
                        if (carriedFilth[num].CanDropAt(pawn.Position, pawn.Map))
                        {
                            dropCarriedFilth.Invoke(pawn?.filth, new object[] { carriedFilth[num] });
                            //pawn?.filth.GetType().GetMethod("DropCarriedFilth", BindingFlags.Instance | BindingFlags.NonPublic)
                            //       .Invoke(pawn.filth, new object[] { carriedFilth[num] });
                        }
                    }
                    //1.3 - //        pawn?.filth.GetType().GetMethod("TryDropFilth", BindingFlags.Instance | BindingFlags.NonPublic)
                    //1.3 - //       .Invoke(pawn.filth, null);
                    //1.3 - //carriedFilth.RemoveRange(0, carriedFilth.Count / 2); //Not as powerful as Syr's code
                }
                //1.3 - //pawn.filth.Notify_EnteredNewCell();
                //1.3 - //pawn.filth.GetType().GetMethod("TryDropFilth", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pawn.filth,null);

                //FieldInfo debug = pawn.filth.GetType().GetField("carriedFilth", BindingFlags.NonPublic | BindingFlags.Instance);
                //List<Filth> debugList = (List<Filth>)debug.GetValue(pawn.filth);
                //foreach (var f in debugList)
                //{
                //    f.Destroy();
                //}
                //debugList.Clear();


            }
        }


        public override void DrawGUIOverlay()
        {
            //
        }
    }
}