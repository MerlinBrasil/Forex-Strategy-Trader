// Strategy Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Xml;

namespace Forex_Strategy_Trader
{
    public enum PermanentProtectionType { Relative, Absolute };
    public enum StrategySlotStatus { Open, Locked, Linked };

    /// <summary>
    /// Strategy Class.
    /// </summary>
    public partial class Strategy
    {
        string strategyName;
        string description = string.Empty;

        int openFilters;
        int closeFilters;
        IndicatorSlot[] indicatorSlot;

        string symbol = "EURUSD";
        DataPeriods period = DataPeriods.day;
        
        SameDirSignalAction     sameDirSignlAct = SameDirSignalAction.Nothing;
        OppositeDirSignalAction oppDirSignlAct  = OppositeDirSignalAction.Nothing;

        bool   useAccountPercentEntry = false;
        double maxOpenLots  = 20;
        double entryLots    = 1;
        double addingLots   = 1;
        double reducingLots = 1;

        bool usePermanentSL = false;
        PermanentProtectionType permanentSLType = PermanentProtectionType.Relative;
        int permanentSL = 1000;

        bool usePermanentTP = false;
        PermanentProtectionType permanentTPType = PermanentProtectionType.Relative;
        int permanentTP = 1000;

        bool useBreakEven = false;
        int breakEven = 1000;

        int firstBar;

        /// <summary>
        /// Gets the max count of Open Filters.
        /// </summary>
        public static int MaxOpenFilters { get { return Configs.MAX_ENTRY_FILTERS; } }

        /// <summary>
        /// Gets the max count of Close Filters.
        /// </summary>
        public static int MaxCloseFilters { get { return Configs.MAX_EXIT_FILTERS; } }

        /// <summary>
        /// Gets the max count of Strategy slots
        /// </summary>
        public static int MaxSlots { get { return MaxOpenFilters + MaxCloseFilters + 2; } }

        /// <summary>
        /// Gets or sets the strategy name.
        /// </summary>
        public string StrategyName { get { return strategyName; } set { strategyName = value; } }

        /// <summary>
        /// Gets or sets the count of Open Filters.
        /// </summary>
        public int OpenFilters { get { return openFilters; } set { openFilters = value; } }

        /// <summary>
        /// Gets or sets the count of Close Filters.
        /// </summary>
        public int CloseFilters { get { return closeFilters; } set { closeFilters = value; } }

        /// <summary>
        /// Gets or sets the Data Period.
        /// </summary>
        public DataPeriods DataPeriod { get { return period; } set { period = value; } }

        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        public string Symbol { get { return symbol; } set { symbol = value; } }

        /// <summary>
        /// Gets or sets the UsePermanentSL
        /// </summary>
        public bool UsePermanentSL { get { return usePermanentSL; } set { usePermanentSL = value; } }

        /// <summary>
        /// Gets or sets the type of Permanent SL
        /// </summary>
        public PermanentProtectionType PermanentSLType { get { return permanentSLType; } set { permanentSLType = value; } }

        /// <summary>
        /// Gets or sets the PermanentSL
        /// </summary>
        public int PermanentSL { get { return permanentSL; } set { permanentSL = value; } }

        /// <summary>
        /// Gets or sets the UsePermanentTP
        /// </summary>
        public bool UsePermanentTP { get { return usePermanentTP; } set { usePermanentTP = value; } }

        /// <summary>
        /// Gets or sets the type of Permanent TP
        /// </summary>
        public PermanentProtectionType PermanentTPType { get { return permanentTPType; } set { permanentTPType = value; } }

        /// <summary>
        /// Gets or sets the PermanentTP
        /// </summary>
        public int PermanentTP { get { return permanentTP; } set { permanentTP = value; } }

        /// <summary>
        /// Gets or sets the UseBreakEven
        /// </summary>
        public bool UseBreakEven { get { return useBreakEven; } set { useBreakEven = value; } }

        /// <summary>
        /// Gets or sets the BreakEven
        /// </summary>
        public int BreakEven { get { return breakEven; } set { breakEven = value; } }

        /// <summary>
        /// Gets or sets the UseAccountPercentEntry
        /// </summary>
        public bool UseAccountPercentEntry { get { return useAccountPercentEntry; } set { useAccountPercentEntry = value; } }

        /// <summary>
        /// Gets or sets the Number of lots to enter the market
        /// </summary>
        public double EntryLots { get { return entryLots; } set { entryLots = value; } }

        /// <summary>
        /// Gets or sets the max number of open lots to enter the market
        /// </summary>
        public double MaxOpenLots { get { return maxOpenLots; } set { maxOpenLots = value; } }

        /// <summary>
        /// Gets or sets the Number of lots to add to the position
        /// </summary>
        public double AddingLots { get { return addingLots; } set { addingLots = value; } }

        /// <summary>
        /// Gets or sets the Number of lots to reduce the position
        /// </summary>
        public double ReducingLots { get { return reducingLots; } set { reducingLots = value; } }

        /// <summary>
        /// Gets or sets the Strategy description
        /// </summary>
        public string Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Gets the count of slots.
        /// </summary>
        public int Slots { get { return openFilters + closeFilters + 2; } }

        /// <summary>
        /// Gets the number of Open Slot.
        /// </summary>
        public int OpenSlot { get { return 0; } }

        /// <summary>
        /// Gets the number of Close Slot.
        /// </summary>
        public int CloseSlot { get { return openFilters + 1; } }

        /// <summary>
        /// Gets or sets the indicators build up the strategy.
        /// </summary>
        public IndicatorSlot[] Slot { get { return indicatorSlot; } set { indicatorSlot = value; } }

        /// <summary>
        /// Gets or sets a value representing how the new opposite signal reflects the position.
        /// </summary>
        public OppositeDirSignalAction OppSignalAction { get { return oppDirSignlAct; } set { oppDirSignlAct = value; } }

        /// <summary>
        /// Gets or sets a value representing how the new same dir signal reflects the position.
        /// </summary>
        public SameDirSignalAction SameSignalAction { get { return sameDirSignlAct; } set { sameDirSignlAct = value; } }

        /// <summary>
        /// The time when the position entry occurs
        /// </summary>
        public ExecutionTime EntryExecutionTime { get { return Slot[0].IndParam.ExecutionTime; } }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public Strategy()
        {
            CreateStrategy(0, 0);
        }

        /// <summary>
        /// Sets a new strategy.
        /// </summary>
        public Strategy(int openFilters, int closeFilters)
        {
            CreateStrategy(openFilters, closeFilters);
        }

        /// <summary>
        /// Creates a strategy
        /// </summary>
        void CreateStrategy(int openFilters, int closeFilters)
        {
            strategyName = "Unnamed";
            this.openFilters  = openFilters;
            this.closeFilters = closeFilters;
            indicatorSlot = new IndicatorSlot[Slots];

            for (int slot = 0; slot < Slots; slot++)
            {
                indicatorSlot[slot] = new IndicatorSlot();
                indicatorSlot[slot].SlotNumber = slot;
                indicatorSlot[slot].SlotType   = GetSlotType(slot);
            }
        }

        /// <summary>
        /// Generates a new strategy.
        /// </summary>
        public static void GenerateNew()
        {
            Data.Strategy = new Strategy(0, 0);

            int iOpenSlotNum  = Data.Strategy.OpenSlot;
            int iCloseSlotNum = Data.Strategy.CloseSlot;

            Data.Strategy.StrategyName = "New";

            Bar_Opening barOpening = new Bar_Opening(SlotTypes.Open);
            barOpening.Calculate(SlotTypes.Open);
            Data.Strategy.Slot[iOpenSlotNum].IndParam       = barOpening.IndParam;
            Data.Strategy.Slot[iOpenSlotNum].IndicatorName  = barOpening.IndicatorName;
            Data.Strategy.Slot[iOpenSlotNum].Component      = barOpening.Component;
            Data.Strategy.Slot[iOpenSlotNum].SeparatedChart = barOpening.SeparatedChart;
            Data.Strategy.Slot[iOpenSlotNum].SpecValue      = barOpening.SpecialValues;
            Data.Strategy.Slot[iOpenSlotNum].MaxValue       = barOpening.SeparatedChartMaxValue;
            Data.Strategy.Slot[iOpenSlotNum].MinValue       = barOpening.SeparatedChartMinValue;
            Data.Strategy.Slot[iOpenSlotNum].IsDefined      = true;

            Bar_Closing barClosing = new Bar_Closing(SlotTypes.Close);
            barClosing.Calculate(SlotTypes.Close);
            Data.Strategy.Slot[iCloseSlotNum].IndParam       = barClosing.IndParam;
            Data.Strategy.Slot[iCloseSlotNum].IndicatorName  = barClosing.IndicatorName;
            Data.Strategy.Slot[iCloseSlotNum].Component      = barClosing.Component;
            Data.Strategy.Slot[iCloseSlotNum].SeparatedChart = barClosing.SeparatedChart;
            Data.Strategy.Slot[iCloseSlotNum].SpecValue      = barClosing.SpecialValues;
            Data.Strategy.Slot[iCloseSlotNum].MaxValue       = barClosing.SeparatedChartMaxValue;
            Data.Strategy.Slot[iCloseSlotNum].MinValue       = barClosing.SeparatedChartMinValue;
            Data.Strategy.Slot[iCloseSlotNum].IsDefined      = true;

            return;
        }

        /// <summary>
        /// Gets the type of the slot.
        /// </summary>
        public SlotTypes GetSlotType(int slot)
        {
            SlotTypes slotType = SlotTypes.NotDefined;

            if (slot == OpenSlot)
                slotType = SlotTypes.Open;
            else if (slot < CloseSlot)
                slotType = SlotTypes.OpenFilter;
            else if (slot == CloseSlot)
                slotType = SlotTypes.Close;
            else
                slotType = SlotTypes.CloseFilter;

            return slotType;
        }

        /// <summary>
        /// Gets the default logical group for the designated slot number.
        /// </summary>
        public string GetDefaultGroup(int slot)
        {
            string group = "";
            string sndicatorName = Slot[slot].IndicatorName;
            SlotTypes slotType = GetSlotType(slot);
            if (slotType == SlotTypes.OpenFilter)
            {
                if (sndicatorName == "Data Bars Filter" ||
                    sndicatorName == "Date Filter"      ||
                    sndicatorName == "Day of Month"     ||
                    sndicatorName == "Enter Once"       ||
                    sndicatorName == "Entry Time"       ||
                    sndicatorName == "Long or Short"    ||
                    sndicatorName == "Lot Limiter"      ||
                    sndicatorName == "Random Filter")
                    group = "All";
                else
                    group = "A";
            }
            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slot - CloseSlot - 1;
                group = char.ConvertFromUtf32(char.ConvertToUtf32("a", 0) + index);
            }

            return group;
        }

        /// <summary>
        /// Adds a new Open Filter to the strategy.
        /// </summary>
        /// <returns>The number of new Open Filter Slot.</returns>
        public int AddOpenFilter()
        {
            Data.Log("Adding an Open Filter");

            OpenFilters++;
            IndicatorSlot[] aIndSlotOld = (IndicatorSlot[])indicatorSlot.Clone();
            indicatorSlot = new IndicatorSlot[Slots];
            int newSlotNumb = OpenFilters; // The number of new open filter slot.

            // Copy the open slot and all old open filters.
            for (int slot = 0; slot < newSlotNumb; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot];

            // Copy the close slot and all close filters.
            for (int slot = newSlotNumb + 1; slot < Slots; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot - 1];

            // Create the new slot.
            indicatorSlot[newSlotNumb] = new IndicatorSlot();
            indicatorSlot[newSlotNumb].SlotType = SlotTypes.OpenFilter;

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                indicatorSlot[slot].SlotNumber = slot;

            return newSlotNumb;
        }

        /// <summary>
        /// Adds a new Close Filter to the strategy.
        /// </summary>
        /// <returns>The number of new Close Filter Slot.</returns>
        public int AddCloseFilter()
        {
            Data.Log("Adding a Close Filter");

            CloseFilters++;
            IndicatorSlot[] aIndSlotOld = (IndicatorSlot[])indicatorSlot.Clone();
            indicatorSlot = new IndicatorSlot[Slots];
            int newSlotNumb = Slots - 1; // The number of new close filter slot.

            // Copy all old slots.
            for (int slot = 0; slot < newSlotNumb; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot];

            // Create the new slot.
            indicatorSlot[newSlotNumb] = new IndicatorSlot();
            indicatorSlot[newSlotNumb].SlotType = SlotTypes.CloseFilter;

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                indicatorSlot[slot].SlotNumber = slot;

            return newSlotNumb;
        }

        /// <summary>
        /// Removes a filter from the strategy.
        /// </summary>
        public void RemoveFilter(int slotToRemove)
        {
            if (Slot[slotToRemove].SlotType != SlotTypes.OpenFilter &&
                Slot[slotToRemove].SlotType != SlotTypes.CloseFilter)
                return;

            Data.Log("Remove a Filter");

            if (slotToRemove < CloseSlot)
                OpenFilters--;
            else
                CloseFilters--;
            IndicatorSlot[] aIndSlotOld = (IndicatorSlot[])indicatorSlot.Clone();
            indicatorSlot = new IndicatorSlot[Slots];

            // Copy all filters before this that has to be removed.
            for (int slot = 0; slot < slotToRemove; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot];

            // Copy all filters after this that has to be removed.
            for (int slot = slotToRemove; slot < Slots; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot + 1];

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                indicatorSlot[slot].SlotNumber = slot;

            return;
        }

        /// <summary>
        /// Removes all close filters from the strategy.
        /// </summary>
        public void RemoveAllCloseFilters()
        {
            Data.Log("Removing All Closed Filters");

            CloseFilters = 0;
            IndicatorSlot[] aIndSlotOld = (IndicatorSlot[])indicatorSlot.Clone();
            indicatorSlot = new IndicatorSlot[Slots];

            // Copy all slots except the close filters.
            for (int slot = 0; slot < Slots; slot++)
                indicatorSlot[slot] = aIndSlotOld[slot];
        }

        /// <summary>
        /// Moves a filter upwards.
        /// </summary>
        public void MoveFilterUpwards(int slotToMove)
        {
            Data.Log("Move a Filter Upwards");

            if (slotToMove > 1 && Slot[slotToMove].SlotType == Slot[slotToMove - 1].SlotType)
            {

                IndicatorSlot tempSlot = Slot[slotToMove - 1].Clone();
                Slot[slotToMove - 1] = Slot[slotToMove].Clone();
                Slot[slotToMove] = tempSlot.Clone();

                // Sets the slot numbers.
                for (int slot = 0; slot < Slots; slot++)
                    indicatorSlot[slot].SlotNumber = slot;
            }
        }

        /// <summary>
        /// Moves a filter downwards.
        /// </summary>
        public void MoveFilterDownwards(int slotToMove)
        {
            Data.Log("Move a Filter Downwards");

            if (slotToMove < Slots - 1 && Slot[slotToMove].SlotType == Slot[slotToMove + 1].SlotType)
            {

                IndicatorSlot tempSlot = Slot[slotToMove + 1].Clone();
                Slot[slotToMove + 1] = Slot[slotToMove].Clone();
                Slot[slotToMove] = tempSlot.Clone();

                // Sets the slot numbers.
                for (int slot = 0; slot < Slots; slot++)
                    indicatorSlot[slot].SlotNumber = slot;
            }
        }

        /// <summary>
        /// Duplicates a filter.
        /// </summary>
        public void DuplicateFilter(int slotToDuplicate)
        {
            Data.Log("Duplicate a Filter");

            if (Slot[slotToDuplicate].SlotType == SlotTypes.OpenFilter  && OpenFilters  < MaxOpenFilters ||
                Slot[slotToDuplicate].SlotType == SlotTypes.CloseFilter && CloseFilters < MaxCloseFilters)
            {

                IndicatorSlot tempSlot = Slot[slotToDuplicate].Clone();

                if (Slot[slotToDuplicate].SlotType == SlotTypes.OpenFilter)
                {
                    int addedSlot = AddOpenFilter();
                    Slot[addedSlot] = tempSlot.Clone();
                }

                if (Slot[slotToDuplicate].SlotType == SlotTypes.CloseFilter)
                {
                    int addedSlot = AddCloseFilter();
                    Slot[addedSlot] = tempSlot.Clone();
                }

                // Sets the slot numbers.
                for (int slot = 0; slot < Slots; slot++)
                    indicatorSlot[slot].SlotNumber = slot;
            }
        }

        /// <summary>
        /// Sets the strategy First Bar. It depends on the indicators periods.
        /// </summary>
        public int SetFirstBar()
        {
            // Searches the indicators' components to determine the first bar.
            firstBar = 0;
            foreach (IndicatorSlot slot in indicatorSlot)
                foreach (IndicatorComp comp in slot.Component)
                    if (comp.FirstBar > firstBar)
                        firstBar = comp.FirstBar;

            return firstBar;
        }

        /// <summary>
        /// Sets "Use previous bar value" parameter.
        /// </summary>
        public bool AdjustUsePreviousBarValue()
        {
            bool isSomethingChanged = false;
            if (Data.AutoUsePrvBarValue == false)
                return isSomethingChanged;

            for (int slot = 0; slot < Slots; slot++)
            {
                isSomethingChanged = SetUsePrevBarValueCheckBox(slot) ? true : isSomethingChanged;
            }

            // Recalculates the indicators.
            if (isSomethingChanged)
            {
                for (int slot = 0; slot < Slots; slot++)
                {
                    string indicatorName = Data.Strategy.Slot[slot].IndicatorName;
                    SlotTypes slotType = Data.Strategy.Slot[slot].SlotType;
                    Indicator indicator = Indicator_Store.ConstructIndicator(indicatorName, slotType);

                    indicator.IndParam = Data.Strategy.Slot[slot].IndParam;

                    indicator.Calculate(slotType);

                    Slot[slot].IndicatorName  = indicator.IndicatorName;
                    Slot[slot].IndParam       = indicator.IndParam;
                    Slot[slot].Component      = indicator.Component;
                    Slot[slot].SeparatedChart = indicator.SeparatedChart;
                    Slot[slot].SpecValue      = indicator.SpecialValues;
                    Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
                    Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
                    Slot[slot].IsDefined      = true;
                }
            }

            return isSomethingChanged;
        }

        /// <summary>
        /// Sets the "Use previous bar value" checkbox
        /// </summary>
        /// <returns>Is any Changes</returns>
        public bool SetUsePrevBarValueCheckBox(int slot)
        {
            bool isChanged = false;

            for (int param = 0; param < Slot[slot].IndParam.CheckParam.Length; param++)
            {
                if (Slot[slot].IndParam.CheckParam[param].Caption == "Use previous bar value")
                {
                    bool isOrigChecked = Slot[slot].IndParam.CheckParam[param].Checked;
                    bool isChecked = true;

                    // Close filter slot
                    if (Slot[slot].SlotType == SlotTypes.Open)
                    {
                        isChecked = true;
                    }

                    // Open filter slot
                    else if (Slot[slot].SlotType == SlotTypes.OpenFilter)
                    {
                        isChecked = EntryExecutionTime != ExecutionTime.AtBarClosing;
                    }

                    // Close slot
                    else if (Slot[slot].SlotType == SlotTypes.Close)
                    {
                        isChecked = true;
                    }

                    // Close filter slot
                    else if (Slot[slot].SlotType == SlotTypes.CloseFilter)
                    {
                        isChecked = false;
                    }

                    if (isChecked)
                    {
                        for (int iPar = 0; iPar < Slot[slot].IndParam.ListParam.Length; iPar++)
                        {
                            if (Slot[slot].IndParam.ListParam[iPar].Caption == "Base price" &&
                                Slot[slot].IndParam.ListParam[iPar].Text == "Open")
                            {
                                isChecked = false;
                            }
                        }
                    }

                    if (isChecked != isOrigChecked)
                    {
                        isChanged = true;
                        Slot[slot].IndParam.CheckParam[param].Checked = isChecked;
                    }
                }
            }

            return isChanged;
        }

        /// <summary>
        /// Prepare the checkbox
        /// </summary>
        /// <returns>IsChecked</returns>
        public bool PrepareUsePrevBarValueCheckBox(SlotTypes slotType)
        {
            bool isChecked = true;
            if (slotType == SlotTypes.OpenFilter)
            {
                if (Data.Strategy.Slot[Data.Strategy.OpenSlot].IndicatorName == "Bar Closing" ||
                    Data.Strategy.Slot[Data.Strategy.OpenSlot].IndicatorName == "Day Closing")
                    isChecked = false;
            }
            else if (slotType == SlotTypes.Close)
            {
                isChecked = true;
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                isChecked = false;
            }
            return isChecked;
        }

        /// <summary>
        /// Calculates the strategy indicators and returns the first meaningful bar number.
        /// </summary>
        /// <returns>First bar</returns>
        public int Calculate()
        {
            int firstBar = 0;
            foreach (IndicatorSlot indSlot in Slot)
            {
                string    sndicatorName = indSlot.IndicatorName;
                SlotTypes slotType  = indSlot.SlotType;
                Indicator indicator = Indicator_Store.ConstructIndicator(sndicatorName, slotType);

                indicator.IndParam = indSlot.IndParam;
                indicator.Calculate(slotType);

                indSlot.IndicatorName  = indicator.IndicatorName;
                indSlot.IndParam       = indicator.IndParam;
                indSlot.Component      = indicator.Component;
                indSlot.SeparatedChart = indicator.SeparatedChart;
                indSlot.SpecValue      = indicator.SpecialValues;
                indSlot.MinValue       = indicator.SeparatedChartMinValue;
                indSlot.MaxValue       = indicator.SeparatedChartMaxValue;
                indSlot.IsDefined      = true;

                foreach (IndicatorComp indComp in indSlot.Component)
                    if (indComp.FirstBar > firstBar)
                        firstBar = indComp.FirstBar;
            }

            return firstBar;
        }

        /// <summary>
        /// Saves the strategy in XML format.
        /// </summary>
        /// <param name="sFileName">The full file name.</param>
        /// <returns>0 - the saving is successfully; -1 - error.</returns>
        public int Save(string fileName)
        {
            strategyName = Path.GetFileNameWithoutExtension(fileName);
            symbol = Data.Symbol;
            period = Data.Period;

            // Create the XmlDocument.
            Strategy_XML strategyXML = new Strategy_XML();
            XmlDocument xmlDocStrategy = strategyXML.CreateStrategyXmlDoc(this);

            try
            {
                xmlDocStrategy.Save(fileName); // Save the document to a file.
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Loads the strategy from a file in XML format.
        /// </summary>
        /// <param name="sFileName">The full file name.</param>
        /// <returns>o - success, -1 - error parsing XML, 1 - error loading the file.</returns>
        public static int Load(string fileName)
        {
            // Create the XmlDocument.
            XmlDocument xmlDocStrategy = new XmlDocument();
            try
            {
                xmlDocStrategy.Load(fileName);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, Language.T("Strategy Loading"));
                return 1;
            }
            Strategy_XML strategyXML = new Strategy_XML();
            Strategy temp_strategy = strategyXML.ParseXmlStrategy(xmlDocStrategy);
            Data.Strategy = RemoveBacktestingIndicators(temp_strategy);

            return 0;
        }

        /// <summary>
        /// Removes unusable indicators.
        /// </summary>
        static Strategy RemoveBacktestingIndicators(Strategy strategy)
        {
            for (int slot = 0; slot < strategy.Slots; slot++)
            {
                string indicator = strategy.Slot[slot].IndicatorName;
                foreach (string forbidden in Data.IndicatorsForBacktestOnly)
                    if (indicator == forbidden)
                    {
                        strategy.RemoveFilter(slot);
                        slot = 0;
                        break;
                    }
            }

            return strategy;
        }

        /// <summary>
        /// Returns a copy of the current strategy.
        /// </summary>
        public Strategy Clone()
        {
            // Create the strategy.
            Strategy tempStrategy = new Strategy(openFilters, closeFilters);

            // Number of slots
            tempStrategy.openFilters  = openFilters;
            tempStrategy.closeFilters = closeFilters;

            // Strategy name
            tempStrategy.strategyName = strategyName;

            // Description
            tempStrategy.description = description;

            // Market
            tempStrategy.symbol = symbol;
            tempStrategy.period = period;

            // Same and Opposite direction Actions
            tempStrategy.sameDirSignlAct = sameDirSignlAct;
            tempStrategy.oppDirSignlAct  = oppDirSignlAct;

            // Money Management
            tempStrategy.useAccountPercentEntry = useAccountPercentEntry;
            tempStrategy.maxOpenLots  = maxOpenLots;
            tempStrategy.entryLots    = entryLots;
            tempStrategy.addingLots   = addingLots;
            tempStrategy.reducingLots = reducingLots;

            // Permanent Stop Loss
            tempStrategy.permanentSL     = permanentSL;
            tempStrategy.permanentSLType = permanentSLType;
            tempStrategy.usePermanentSL  = usePermanentSL;

            // Permanent Take Profit
            tempStrategy.permanentTP     = permanentTP;
            tempStrategy.permanentTPType = permanentTPType;
            tempStrategy.usePermanentTP  = usePermanentTP;

            // Break Even
            tempStrategy.breakEven    = breakEven;
            tempStrategy.useBreakEven = useBreakEven;

            // Reading the slots
            for (int slot = 0; slot < Slots; slot++)
                tempStrategy.Slot[slot] = indicatorSlot[slot].Clone();

            return tempStrategy;
        }
    }
}
