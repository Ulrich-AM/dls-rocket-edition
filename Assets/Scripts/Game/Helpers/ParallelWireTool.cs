using System.Collections.Generic;
using UnityEngine;
using DLS.Game;
using DLS.Description;
using DLS.Graphics;
using Seb.Vis.UI;

namespace Assets.Scripts.Game.Helpers
{
    public class ParallelWireTool
    {
        public List<WireInstance> ParallelWiresToPlace = new();
        public float ParallelWiresSpacingFactor = 1f;
        public Vector2 ParallelWiresInitialDir;

        public static readonly string[] ParallelWiresOptions = { "Off", "On" };
        public static readonly UIHandle ID_ParallelWires = new("PREFS_ParallelWires");
        public static readonly UIHandle ID_ParallelWireSnapThresholdField = new("PREFS_ParallelWireSnapThreshold");

        public void Clear()
        {
            ParallelWiresToPlace.Clear();
            ParallelWiresSpacingFactor = 1f;
        }

        public void UpdatePositions(Vector2 mousePos, Vector2 startPosPrimary)
        {
            foreach (var pw in ParallelWiresToPlace)
            {
                Vector2 startPosOther = pw.GetWirePoint(0);
                Vector2 relativeStartOffset = startPosOther - startPosPrimary;
                Vector2 targetPos = mousePos + relativeStartOffset * ParallelWiresSpacingFactor;
                pw.SetLastWirePoint(targetPos);
            }
        }

        public void AddPoint(Vector2 mousePos, Vector2 startPosPrimary)
        {
            foreach (var pw in ParallelWiresToPlace)
            {
                Vector2 startPosOther = pw.GetWirePoint(0);
                Vector2 relativeStartOffset = startPosOther - startPosPrimary;
                Vector2 targetPos = mousePos + relativeStartOffset * ParallelWiresSpacingFactor;
                pw.AddWirePoint(targetPos);
            }
        }

        public void RemoveLastPoint()
        {
            foreach (var pw in ParallelWiresToPlace)
            {
                pw.RemoveLastPoint();
            }
        }
    }
}
