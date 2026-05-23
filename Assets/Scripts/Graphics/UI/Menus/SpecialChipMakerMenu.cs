using Seb.Vis.UI;
using Seb.Vis;
using UnityEngine;
using static DLS.Graphics.DrawSettings;
using System;
using System.Collections.Generic;
using DLS.Game;
using DLS.Description;
using System.Linq;



namespace DLS.Graphics
{
    public static class SpecialChipMakerMenu
    {
        public static List<int> PinBitCountsMade = new();
        public static List<KeyValuePair<int, int>> MergeSplitsMade = new();

        public static List<int> PinBitCountsAwaitingSave = new();
        public static List<KeyValuePair<int, int>> MergeSplitsAwaitingSave = new();

        public static bool saved;

        const float textSpacing = 0.25f;
        const float entrySpacing = 0.5f;
        const float menuWidth = 55;
        const float verticalOffset = 22;

        static readonly Vector2 entrySize = new(menuWidth, DrawSettings.SelectorWheelHeight);
        public static readonly Vector2 settingFieldSize = new(entrySize.x / 3, entrySize.y);

        static int previousValue;
        public static bool changeToBeAdded;
        public static bool displayDone;

        static readonly string[] SpecialChipTypes =
        {
            "Pins",
            "Merge/Split",
            "ROM",
            "RAM"
        };
        const int OPTION_PIN = 0;
        const int OPTION_MERGE_SPLIT = 1;
        const int OPTION_ROM = 2;
        const int OPTION_RAM = 3;


        static readonly UIHandle ID_SpecialChipTypes = new("SPEC_SpecialChipTypes");
        static readonly UIHandle ID_PinSize = new("SPEC_PinSize");

        static readonly UIHandle ID_FirstMergeSplit = new("SPEC_MergeSplitA");
        static readonly UIHandle ID_SecondMergeSplit = new("SPEC_MergeSplitB");

        static readonly UIHandle ID_RomAddrBits = new("SPEC_RomAddrBits");
        static readonly UIHandle ID_RomDataBits = new("SPEC_RomDataBits");



        static readonly Func<string, bool> pinSizeInputValidator = ValidatePinSizeInput;

        static bool canAddChip;
        static int currentlyAddingPinBitOfSize;
static KeyValuePair<int, int> currentlyAddingMergeSplit;
static KeyValuePair<int, int> currentlyAddingRom;
static KeyValuePair<int, int> currentlyAddingRam;

public static List<KeyValuePair<int, int>> RomsAwaitingSave = new();
public static List<string> RomsMade = new();

public static List<KeyValuePair<int, int>> RamsAwaitingSave = new();
public static List<string> RamsMade = new();

public static void DrawMenu()
{
    UI.DrawFullscreenPanel(ActiveUITheme.MenuBackgroundOverlayCol);

    UIThemeDLS theme = ActiveUITheme;
    InputFieldTheme inputTheme = ActiveUITheme.ChipNameInputField;
    Draw.ID panelID = UI.ReservePanel();
    
    const float headerSpacing = 1.5f;
    Vector2 topLeft = UI.Centre + new Vector2(-menuWidth / 2, verticalOffset);
    Vector2 labelPosCurr = topLeft;
    Color labelCol = Color.white;
    Color headerCol = new(0.46f, 1, 0.54f);
    Color errorCol = new(1, 0.4f, 0.45f);
    Color doneCol = new(128, 128, 0);

    using (UI.BeginBoundsScope(true))
    {
        DrawHeader("SPECIAL CHIPS:");
        int mainPinNamesMode = DrawNextWheel("Special chip type:", SpecialChipTypes, ID_SpecialChipTypes);

        if (mainPinNamesMode == OPTION_PIN)
        {
            DrawSpecialPinMenu();
        }
        else if (mainPinNamesMode == OPTION_MERGE_SPLIT)
        {
            DrawSpecialMergeSplitMenu();
        }
        else if (mainPinNamesMode == OPTION_ROM)
        {
            DrawSpecialRomMenu();
        }
        else if (mainPinNamesMode == OPTION_RAM)
        {
            DrawSpecialRamMenu();
            }

            void DrawSpecialRamMenu()
            {
            DrawHeader("NEW RAM CHIP:");
            InputFieldState addrBitsInput = MenuHelper.LabeledInputField("Address bits:", labelCol, labelPosCurr, entrySize, ID_RomAddrBits, pinSizeInputValidator, settingFieldSize.x);
            AddSpacing();
            InputFieldState dataBitsInput = MenuHelper.LabeledInputField("Data bits:", labelCol, labelPosCurr, entrySize, ID_RomDataBits, pinSizeInputValidator, settingFieldSize.x);
            int addrBitsAttempt = int.TryParse(addrBitsInput.text, out int a) ? a : -1;
            int dataBitsAttempt = int.TryParse(dataBitsInput.text, out int b) ? b : -1;
            (bool valid, string reason) confirmation = RealRamConfirmation(addrBitsAttempt, dataBitsAttempt);

            if (addrBitsAttempt != -1 && dataBitsAttempt != -1 && !confirmation.valid && !displayDone)
            {
                AddSpacing();
                DrawErrorSection(confirmation.reason);
                canAddChip = false;
            }
            else if (addrBitsAttempt != -1 && dataBitsAttempt != -1 && confirmation.valid)
            {
                canAddChip = true;
                currentlyAddingRam = new(addrBitsAttempt, dataBitsAttempt);
                displayDone = DisplayDone(false);
                return;
            }
            displayDone = DisplayDone(addrBitsAttempt == -1 && dataBitsAttempt == -1);
            canAddChip = false;
            }

                AddSpacing();
                DrawDoneSection(displayDone);

                Vector2 buttonTopLeft = new(labelPosCurr.x, UI.PrevBounds.Bottom - 2f);
                int addOrClose = UI.VerticalButtonGroup(new[] { "Add special chip", "Save", "Close" }, new[] {canAddChip && !displayDone, !saved, true },
                ActiveUITheme.ButtonTheme, buttonTopLeft + (menuWidth / 2) * Vector2.right, entrySize, false, false, entrySpacing);

                if(mainPinNamesMode == OPTION_PIN && canAddChip && addOrClose == 0)
                {
                    AddNewBitSize(currentlyAddingPinBitOfSize);
                    changeToBeAdded = false;
                }
                if(mainPinNamesMode == OPTION_MERGE_SPLIT && canAddChip && addOrClose == 0)
                {
                    AddNewMergeSplit(currentlyAddingMergeSplit.Key, currentlyAddingMergeSplit.Value);
                    changeToBeAdded = false;
                }
                if(mainPinNamesMode == OPTION_ROM && canAddChip && addOrClose == 0)
                {
                    AddNewRom(currentlyAddingRom.Key, currentlyAddingRom.Value);
                    changeToBeAdded = false;
                }
                if(mainPinNamesMode == OPTION_RAM && canAddChip && addOrClose == 0)
                {
                    AddNewRam(currentlyAddingRam.Key, currentlyAddingRam.Value);
                    changeToBeAdded = false;
                }


                if (addOrClose == 1)
                {
                    SaveChanges();
                    Main.ActiveProject.SaveCurrentProjectDescription() ;
                    saved = true;
                }

                if (addOrClose == 2)
                {
                    UIDrawer.SetActiveMenu(UIDrawer.MenuType.None);
                }

                MenuHelper.DrawReservedMenuPanel(panelID, UI.GetCurrentBoundsScope());
            }
            
            if(KeyboardShortcuts.CancelShortcutTriggered())
            {
                UIDrawer.SetActiveMenu(UIDrawer.MenuType.None) ;
            }

            void DrawSpecialRomMenu()
            {
                DrawHeader("NEW ROM CHIP:");
                InputFieldState addrBitsInput = MenuHelper.LabeledInputField("Address bits:", labelCol, labelPosCurr, entrySize, ID_RomAddrBits, pinSizeInputValidator, settingFieldSize.x);
                AddSpacing();
                InputFieldState dataBitsInput = MenuHelper.LabeledInputField("Data bits:", labelCol, labelPosCurr, entrySize, ID_RomDataBits, pinSizeInputValidator, settingFieldSize.x);
                int addrBitsAttempt = int.TryParse(addrBitsInput.text, out int a) ? a : -1;
                int dataBitsAttempt = int.TryParse(dataBitsInput.text, out int b) ? b : -1;
                (bool valid, string reason) confirmation = RealRomConfirmation(addrBitsAttempt, dataBitsAttempt);

                if (addrBitsAttempt != -1 && dataBitsAttempt != -1 && !confirmation.valid && !displayDone)
                {
                    AddSpacing();
                    DrawErrorSection(confirmation.reason);
                    canAddChip = false;
                }
                else if (addrBitsAttempt != -1 && dataBitsAttempt != -1 && confirmation.valid)
                {
                    canAddChip = true;
                    currentlyAddingRom = new(addrBitsAttempt, dataBitsAttempt);
                    displayDone = DisplayDone(false);
                    return;
                }
                displayDone = DisplayDone(addrBitsAttempt == -1 && dataBitsAttempt == -1);
                canAddChip = false;
            }

            void DrawSpecialMergeSplitMenu()
            {
                DrawHeader("NEW MERGE/SPLIT CHIP:");
                InputFieldState firstPinSize = MenuHelper.LabeledInputField("First pin size:", labelCol, labelPosCurr, entrySize, ID_FirstMergeSplit, pinSizeInputValidator, settingFieldSize.x);
                AddSpacing();
                InputFieldState secondPinSize = MenuHelper.LabeledInputField("Second pin size:", labelCol, labelPosCurr, entrySize, ID_SecondMergeSplit, pinSizeInputValidator, settingFieldSize.x);
                int firstPinSizeAttempt = int.TryParse(firstPinSize.text, out int a) ? a : -1;
                int secondPinSizeAttempt = int.TryParse(secondPinSize.text, out int b) ? b : -1;
                (bool valid, string reason) confirmation = RealMergeSplitConfirmation(firstPinSizeAttempt, secondPinSizeAttempt);
                


                if (firstPinSizeAttempt != -1 && secondPinSizeAttempt != -1 && !confirmation.valid && !displayDone)
                {
                    AddSpacing();
                    DrawErrorSection(confirmation.reason);
                    canAddChip = false;
                }

                else if (firstPinSizeAttempt != -1 && secondPinSizeAttempt != -1 && confirmation.valid)
                {
                    canAddChip = true;
                    currentlyAddingMergeSplit = new (Math.Max(firstPinSizeAttempt, secondPinSizeAttempt), Math.Min(firstPinSizeAttempt, secondPinSizeAttempt));
                    displayDone = DisplayDone(false);
                    return;
                }
                displayDone = DisplayDone(firstPinSizeAttempt == -1 && secondPinSizeAttempt == -1);
                canAddChip = false;
            }

            void DrawSpecialPinMenu()
            {
                DrawHeader("NEW PIN:");
                InputFieldState pinSizeInput = MenuHelper.LabeledInputField("Size of new pin:", labelCol, labelPosCurr,entrySize,ID_PinSize, pinSizeInputValidator, settingFieldSize.x);
                int pinSizeAttempt = int.TryParse(pinSizeInput.text, out int a) ? a : -1;
                (bool valid, string reason) confirmation = RealSizeConfirmation(pinSizeAttempt);
                if (!confirmation.valid && pinSizeAttempt != -1 && !displayDone)
                {
                    AddSpacing();
                    DrawErrorSection(confirmation.reason);
                    canAddChip = false;
                }
                else if (confirmation.valid && pinSizeAttempt != -1) {
                    canAddChip = true;
                    currentlyAddingPinBitOfSize = pinSizeAttempt;
                    displayDone = DisplayDone(false);
                    return;
                }
                displayDone = DisplayDone(pinSizeAttempt == -1);
                canAddChip = false;
            }

            void DrawDoneSection(bool done) {
                if(!done) { return; }
                AddHeaderSpacing();
                UI.DrawText("DONE !", theme.FontBold, theme.FontSizeRegular, labelPosCurr, Anchor.TextCentreLeft, doneCol);
                AddHeaderSpacing();

            }

            void DrawErrorSection(string reason)
            {
                AddHeaderSpacing();
                UI.DrawText("ERROR", theme.FontBold, theme.FontSizeRegular, labelPosCurr, Anchor.TextCentreLeft, errorCol);
                AddHeaderSpacing();

                AddTextSpacing();
                UI.DrawText("   "+reason, theme.FontRegular, theme.FontSizeRegular, labelPosCurr, Anchor.TextCentreLeft, errorCol);
                AddTextSpacing();


            }

            int DrawNextWheel(string label, string[] options, UIHandle id)
            {
                int index = MenuHelper.LabeledOptionsWheel(label, labelCol, labelPosCurr, entrySize, id, options, settingFieldSize.x, true);
                AddSpacing();
                return index;
            }

            void DrawHeader(string text)
            {
                AddHeaderSpacing();
                UI.DrawText(text, theme.FontBold, theme.FontSizeRegular, labelPosCurr, Anchor.TextCentreLeft, headerCol);
                AddHeaderSpacing();
            }

            void AddSpacing()
            {
                labelPosCurr.y -= entrySize.y + entrySpacing;
            }

            void AddHeaderSpacing()
            {
                labelPosCurr.y -= headerSpacing;
            }

            void AddTextSpacing()
            {
                labelPosCurr.y -= textSpacing;
            }

        }

        public static void OnMenuOpened()
        {
            PinBitCountsAwaitingSave = new();
            MergeSplitsAwaitingSave = new();
            RomsAwaitingSave = new();
            RamsAwaitingSave = new();

            RefreshPinBitCounts();
            RefreshMergeSplits();
            RefreshRoms();
            RefreshRams();
            saved = true;
            changeToBeAdded = true;
            displayDone = false;
            previousValue = -1;
        }

        public static void RefreshPinBitCounts()
        {
            PinBitCountsMade = new();
            foreach(var pinBit in Main.ActiveProject.description.pinBitCounts)
            {
                PinBitCountsMade.Add(pinBit.BitCount);
            }
            foreach(var pinBit in PinBitCountsAwaitingSave)
            {
                PinBitCountsMade.Add(pinBit);
            }
        }

        public static void RefreshMergeSplits()
        {
            MergeSplitsMade = new();
            foreach (var PinBitPair in Main.ActiveProject.description.SplitMergePairs)
            {
                MergeSplitsMade.Add(new(PinBitPair.Key, PinBitPair.Value));
            }

            foreach(var pair in MergeSplitsAwaitingSave)
            {
                MergeSplitsMade.Add(pair);
            }
        }

        public static void RefreshRoms()
        {
            RomsMade = new();
            foreach (var name in Main.ActiveProject.description.RomNames())
            {
                RomsMade.Add(name);
            }

            foreach (var pair in RomsAwaitingSave)
            {
                RomsMade.Add($"ROM {(long)Math.Pow(2, pair.Key)}x{pair.Value}");
            }
        }
        
        public static bool ValidatePinSizeInput(string s)
        {
            if (string.IsNullOrEmpty(s)){ changeToBeAdded = false; return true; }
            if (s.Contains(" ")) return false;
            if (int.TryParse(s, out int a))
            {
                if(a < 0) return false;
                if(a > 65536) return false;
                changeToBeAdded = previousValue != a;
                previousValue = a;
                return true;
            }
            return false;
        }

        public static (bool valid, string reason) RealSizeConfirmation(int a)
        {
            if (a < 1) { return (false, "Pin size must be at least 1 bit."); }
            if (a > 64 && a % 8 != 0 && a<= 512) { return (false, "Pin size > 64 and not a multiple of 8."); }
            if(a > 512 && a % 64 != 0 && a <= 4096) { return (false, "Pin size > 512 and not a multiple of 64."); }
            if (a > 4096 && a % 512 != 0) { return (false, "Pin size > 4096 and not a multiple of 512."); }
            if (PinBitCountsMade.Contains(a)) { return (false, "Pins with this count already exist."); }


            return (true, "");
        }

        public static (bool valid, string reason) RealMergeSplitConfirmation(int a, int b)
        {
            if (MergeSplitsMade.Any(k => (k.Key == a && k.Value == b) || (k.Value == a && k.Key == b))) { return (false, "These Merge/Split chips already exist."); }
            if (!PinBitCountsMade.Contains(a) && !PinBitCountsMade.Contains(b) ) { return (false, $"No pins with pinsize {a} and {b} exist. Create them first."); }
            if (!PinBitCountsMade.Contains(a) ) { return (false, $"No pin with pinsize {a} exist. Create it first, if valid."); }
            if (!PinBitCountsMade.Contains(b) ) { return (false, $"No pin with pinsize {b} exist. Create it first, if valid."); }
            if (a == b) { return (false, "This seems useless..."); }
            int bigger = Math.Max(a, b);
            int smaller = Math.Min(a, b);
            if(bigger%smaller != 0) { return (false, $"{bigger} / {smaller} isn't an integer."); }


            return (true, "");
        }

        public static (bool valid, string reason) RealRomConfirmation(int addrBits, int dataBits)
        {
            if (addrBits < 1) return (false, "Address bits must be at least 1.");
            if (addrBits > 16) return (false, "Address bits cannot exceed 16.");
            if (dataBits < 1) return (false, "Data bits must be at least 1.");
            if (dataBits > 32) return (false, "Data bits cannot exceed 32.");

            string name = $"ROM {(long)Math.Pow(2, addrBits)}x{dataBits}";
            if (RomsMade.Contains(name)) return (false, "This ROM already exists.");

            return (true, "");
        }

        public static void AddNewBitSize(int a)
        {
            PinBitCountsAwaitingSave.Add(a);
            RefreshPinBitCounts();
            saved = false;

        }

        public static void AddNewMergeSplit(int a, int b)
        {
            MergeSplitsAwaitingSave.Add(new(a,b));
            RefreshMergeSplits();
            saved = false;
        }

        public static void AddNewRom(int addrBits, int dataBits)
        {
            RomsAwaitingSave.Add(new(addrBits, dataBits));
            RefreshRoms();
            saved = false;
        }

        public static void AddNewRam(int addrBits, int dataBits)
        {
            RamsAwaitingSave.Add(new(addrBits, dataBits));
            RefreshRams();
            saved = false;
        }

        public static void RefreshRams()
        {
            RamsMade = new();
            if (Main.ActiveProject != null && Main.ActiveProject.description.CustomRamSizes != null)
            {
                foreach (var pair in Main.ActiveProject.description.CustomRamSizes)
                {
                    RamsMade.Add($"RAM {(long)Math.Pow(2, pair.Key)}x{pair.Value}");
                }
            }

            foreach (var pair in RamsAwaitingSave)
            {
                RamsMade.Add($"RAM {(long)Math.Pow(2, pair.Key)}x{pair.Value}");
            }
        }

        public static (bool valid, string reason) RealRamConfirmation(int addrBits, int dataBits)
        {
            if (addrBits < 1) return (false, "Address bits must be at least 1.");
            if (addrBits > 16) return (false, "Address bits cannot exceed 16.");
            if (dataBits < 1) return (false, "Data bits must be at least 1.");
            if (dataBits > 32) return (false, "Data bits cannot exceed 32.");

            string name = $"RAM {(long)Math.Pow(2, addrBits)}x{dataBits}";
            if (RamsMade.Contains(name)) return (false, "This RAM already exists.");

            return (true, "");
        }

        public static bool DisplayDone(bool b)
        {
            return b;
        }

        public static void SaveChanges()
        {
            if (!saved)
            {
                foreach (int a in PinBitCountsAwaitingSave)
                {
                    Main.ActiveProject.AddNewPinSize(a);
                }
                foreach (var pair in MergeSplitsAwaitingSave)
                {
                    Main.ActiveProject.AddNewMergeSplit(pair.Key, pair.Value);
                }
                foreach (var pair in RomsAwaitingSave)
                {
                    Main.ActiveProject.AddNewRom(pair.Key, pair.Value);
                }
                foreach (var pair in RamsAwaitingSave)
                {
                    Main.ActiveProject.AddNewRam(pair.Key, pair.Value);
                }
                saved = true;
                PinBitCountsAwaitingSave = new();
                MergeSplitsAwaitingSave = new();
                RomsAwaitingSave = new();
                RamsAwaitingSave = new();
            }
        }


    }
}