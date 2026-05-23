using DLS.Description;
using DLS.Game;
using Assets.Scripts.Game.Helpers;
using Seb.Types;
using Seb.Vis;
using Seb.Vis.UI;
using UnityEngine;

namespace DLS.Graphics
{
	public static class ProjectToolsMenu
	{
		const float menuWidth = 55;
		const float verticalOffset = 22;
		const float menuHeight = 50;

		static readonly Vector2 entrySize = new(menuWidth, DrawSettings.SelectorWheelHeight);
		static readonly UIHandle ID_AngleIndicator = new("TOOLS_AngleIndicator");
		static readonly UIHandle ID_PerfOverlay = new("TOOLS_PerfOverlay");
        static readonly UIHandle ID_TranslucentWires = new("TOOLS_TranslucentWires");
        static readonly UIHandle ID_ParallelWires = ParallelWireTool.ID_ParallelWires;
        static readonly UIHandle ID_ParallelWireSnapThresholdField = ParallelWireTool.ID_ParallelWireSnapThresholdField;
		static ProjectDescription originalProjectDesc;

		public static void DrawMenu(Project project)
		{
			DrawSettings.UIThemeDLS theme = DrawSettings.ActiveUITheme;
			MenuHelper.DrawBackgroundOverlay();
			Draw.ID panelID = UI.ReservePanel();

			Color headerCol = new(0.46f, 1, 0.54f);
			Color labelCol = Color.white;
			Vector2 topLeft = UI.Centre + new Vector2(-menuWidth / 2, verticalOffset);
			
			// Increase y by a bit to make room for menu selection
			topLeft.y += entrySize.y;

			Vector2 labelPosCurr = topLeft;

			using (UI.BeginBoundsScope(true))
			{
				// ---- Draw Header ----
				UI.DrawText("PROJECT TOOLS:", theme.FontBold, theme.FontSizeRegular, labelPosCurr, Anchor.TextCentreLeft, headerCol);
				labelPosCurr.y -= 5f;

				// Show Angle Indicator
				bool showAngle = MenuHelper.LabeledOptionsWheel("Show angle indicator", labelCol, labelPosCurr, entrySize, ID_AngleIndicator, new[] { "Off", "On" }, 16, true) == 1;
				project.description.Prefs_ShowAngleIndicator = showAngle;
				labelPosCurr.y -= entrySize.y + 0.5f;

				// Show Performance Overlay
				bool showPerf = MenuHelper.LabeledOptionsWheel("Show performance info", labelCol, labelPosCurr, entrySize, ID_PerfOverlay, new[] { "Off", "On" }, 16, true) == 1;
				project.description.Prefs_ShowPerformanceOverlay = showPerf;
				labelPosCurr.y -= entrySize.y + 0.5f;

                // Translucent Wires
                int translucentWiresMode = MenuHelper.LabeledOptionsWheel("Translucent wires", labelCol, labelPosCurr, entrySize, ID_TranslucentWires, new[] { "Off", "Zero", "Hover" }, 16, true);
                project.description.Prefs_TranslucentWires = translucentWiresMode;
                labelPosCurr.y -= entrySize.y + 0.5f;


                // Parallel Wires
                int parallelWiresMode = MenuHelper.LabeledOptionsWheel("Parallel wires", labelCol, labelPosCurr, entrySize, ID_ParallelWires, ParallelWireTool.ParallelWiresOptions, 16, true);
                project.description.Prefs_ParallelWires = parallelWiresMode;
                labelPosCurr.y -= entrySize.y + 0.5f;

                InputFieldState snapThresholdState = MenuHelper.LabeledInputField("Parallel snap threshold", labelCol, labelPosCurr, entrySize, ID_ParallelWireSnapThresholdField, null, 16, true);
                float.TryParse(snapThresholdState.text, out float parallelSnapThreshold);
                project.description.Prefs_ParallelWireSnapThreshold = parallelSnapThreshold;
                labelPosCurr.y -= entrySize.y + 0.5f;

				// Draw cancel/confirm/back buttons
				Vector2 buttonTopLeft = new(topLeft.x, UI.PrevBounds.Bottom);
				int buttonIndex = MenuHelper.DrawButtonTriplet("CANCEL", "CONFIRM", "BACK", buttonTopLeft, menuWidth, true);

				// Draw menu background
				Bounds2D menuBounds = UI.GetCurrentBoundsScope();
				MenuHelper.DrawReservedMenuPanel(panelID, menuBounds);

				// ---- Handle buttons ----
				if (buttonIndex == 0) // CANCEL
				{
					project.description = originalProjectDesc;
					UIDrawer.SetActiveMenu(UIDrawer.MenuType.None);
				}
				else if (buttonIndex == 1) // CONFIRM
				{
					project.UpdateAndSaveProjectDescription(project.description);
					UIDrawer.SetActiveMenu(UIDrawer.MenuType.None);
				}
				else if (buttonIndex == 2) // BACK
				{
					UIDrawer.SetActiveMenu(UIDrawer.MenuType.Preferences);
				}
			}
		}

		public static void OnMenuOpened()
		{
			originalProjectDesc = Project.ActiveProject.description;
			UI.GetWheelSelectorState(ID_AngleIndicator).index = originalProjectDesc.Prefs_ShowAngleIndicator ? 1 : 0;
			UI.GetWheelSelectorState(ID_PerfOverlay).index = originalProjectDesc.Prefs_ShowPerformanceOverlay ? 1 : 0;
            UI.GetWheelSelectorState(ID_TranslucentWires).index = originalProjectDesc.Prefs_TranslucentWires;
            UI.GetWheelSelectorState(ID_ParallelWires).index = Mathf.Clamp(originalProjectDesc.Prefs_ParallelWires, 0, ParallelWireTool.ParallelWiresOptions.Length - 1);
            UI.GetInputFieldState(ID_ParallelWireSnapThresholdField).SetText(originalProjectDesc.Prefs_ParallelWireSnapThreshold + "", false);
		}
	}
}
