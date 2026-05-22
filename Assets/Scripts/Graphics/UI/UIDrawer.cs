using System.Diagnostics;
using DLS.Game;
using DLS.Simulation;
using UnityEngine;
using Seb.Vis;
using Seb.Vis.UI;

namespace DLS.Graphics
{
	public static class UIDrawer
	{
		public enum MenuType
		{
			None,
			ChipSave,
			ChipLibrary,
			BottomBarMenuPopup,
			ChipCustomization,
			Preferences,
			PinRename,
			MainMenu,
			RebindKeyChip,
			RomEdit,
			ChipStats,
			CollectionStats,
			ProjectStats,
			PulseEdit,
            ConstantEdit,
            UnsavedChanges,
			Search,
			ChipLabelPopup,
			SpecialChipMaker,
			ProjectTools
		}

		static MenuType activeMenuOld;

		public static MenuType ActiveMenu { get; private set; }

		public static void Draw()
		{
			NotifyIfActiveMenuChanged();

			using (UI.CreateFixedAspectUIScope(drawLetterbox: true))
			{
				if (ActiveMenu is MenuType.MainMenu)
				{
					DrawAppMenus();
				}
				else
				{
					DrawProjectMenus(Project.ActiveProject);
					DrawPerformanceOverlay(Project.ActiveProject);
				}
			}

			InteractionState.MouseIsOverUI = UI.IsMouseOverUIThisFrame;
		}

		static void DrawPerformanceOverlay(Project project)
		{
			if (project.description.Prefs_ShowPerformanceOverlay)
			{
				float fps = 1.0f / Time.unscaledDeltaTime;
				int sps = (int)project.simAvgTicksPerSec;
				string text = $"FPS: {Mathf.RoundToInt(fps)}\nSPS: {sps} ({project.description.Prefs_SimTargetStepsPerSecond})";
				
				UI.DrawText(text, DrawSettings.ActiveUITheme.FontBold, UIThemeLibrary.FontSizeMedium, UI.TopLeft + new Vector2(1, -1), Anchor.TopLeft, new Color(1, 1, 1, 0.5f));
			}
		}

		static void DrawAppMenus()
		{
			MainMenu.Draw();
		}

		static void DrawProjectMenus(Project project)
		{
			MenuType menuToDraw = ActiveMenu; // cache state in case it changes while drawing/updating the menus

			if (menuToDraw != MenuType.ChipCustomization) BottomBarUI.DrawUI(project);

			bool aMenuIsOpen = true;
			if (menuToDraw == MenuType.ChipSave) ChipSaveMenu.DrawMenu();
			else if (menuToDraw == MenuType.ChipLibrary) ChipLibraryMenu.DrawMenu();
			else if (menuToDraw == MenuType.ChipCustomization) ChipCustomizationMenu.DrawMenu();
			else if (menuToDraw == MenuType.Preferences) PreferencesMenu.DrawMenu(project);
			else if (menuToDraw == MenuType.PinRename) PinEditMenu.DrawMenu();
			else if (menuToDraw == MenuType.RebindKeyChip) RebindKeyChipMenu.DrawMenu();
			else if (menuToDraw == MenuType.RomEdit) RomEditMenu.DrawMenu();
			else if (menuToDraw == MenuType.ChipStats) ChipStatsMenu.DrawMenu(); 
			else if (menuToDraw == MenuType.CollectionStats) CollectionStatsMenu.DrawMenu();
			else if (menuToDraw == MenuType.ProjectStats) ProjectStatsMenu.DrawMenu();
			else if (menuToDraw == MenuType.UnsavedChanges) UnsavedChangesPopup.DrawMenu();
			else if (menuToDraw == MenuType.Search) SearchPopup.DrawMenu();
			else if (menuToDraw == MenuType.ChipLabelPopup) ChipLabelMenu.DrawMenu();
			else if (menuToDraw == MenuType.PulseEdit) PulseEditMenu.DrawMenu();
			else if (menuToDraw == MenuType.ConstantEdit)  ConstantEditMenu.DrawMenu();
			else if (menuToDraw == MenuType.SpecialChipMaker) SpecialChipMakerMenu.DrawMenu();
			else if (menuToDraw == MenuType.ProjectTools) ProjectToolsMenu.DrawMenu(project);
			else
			{
				bool showSimPausedBanner = project.simPaused;
				if (showSimPausedBanner) SimPausedUI.DrawPausedBanner();
				if (project.chipViewStack.Count > 1) ViewedChipsBar.DrawViewedChipsBanner(project, showSimPausedBanner);
				if (SimChip.isCreatingACache) CreateCacheUI.DrawCreatingCacheInfo();
				aMenuIsOpen = false;
			}
			// Cancel current caching process when a menu gets opened
			if(aMenuIsOpen)
				SimChip.AbortCache();

			ContextMenu.Update();
		}

		public static bool InInputBlockingMenu() => !(ActiveMenu is MenuType.None or MenuType.BottomBarMenuPopup or MenuType.ChipCustomization);

		static void NotifyIfActiveMenuChanged()
		{
			// UI Changed -- notify opened
			if (ActiveMenu != activeMenuOld)
			{
				if (activeMenuOld == MenuType.ChipCustomization) CustomizationSceneDrawer.OnCustomizationMenuClosed();

				if (ActiveMenu == MenuType.ChipSave) ChipSaveMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.ChipLibrary) ChipLibraryMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.ChipCustomization) ChipCustomizationMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.PinRename) PinEditMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.Preferences) PreferencesMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.MainMenu) MainMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.RebindKeyChip) RebindKeyChipMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.RomEdit) RomEditMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.Search) SearchPopup.OnMenuOpened();
				else if (ActiveMenu == MenuType.ChipLabelPopup) ChipLabelMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.PulseEdit) PulseEditMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.ProjectStats) ProjectStatsMenu.OnMenuOpened();
                else if (ActiveMenu == MenuType.ConstantEdit) ConstantEditMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.SpecialChipMaker) SpecialChipMakerMenu.OnMenuOpened();
				else if (ActiveMenu == MenuType.ProjectTools) ProjectToolsMenu.OnMenuOpened();


				if (InInputBlockingMenu() && Project.ActiveProject != null && Project.ActiveProject.controller != null)
				{
					Project.ActiveProject.controller.CancelEverything();
				}

				activeMenuOld = ActiveMenu;
			}
		}

		public static void ToggleBottomPopupMenu()
		{
			SetActiveMenu(ActiveMenu is MenuType.None ? MenuType.BottomBarMenuPopup : MenuType.None);
		}

		public static void SetActiveMenu(MenuType type)
		{
			ActiveMenu = type;
		}

		public static void Reset()
		{
			SetActiveMenu(MenuType.None);
			activeMenuOld = MenuType.None;
			ContextMenu.Reset();
			UI.ResetAllStates();
			BottomBarUI.Reset();
			ChipSaveMenu.Reset();
			RomEditMenu.Reset();
			ChipLibraryMenu.Reset();
			SearchPopup.Reset();
		}
	}
}