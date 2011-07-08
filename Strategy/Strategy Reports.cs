// Strategy Reports
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Text;

namespace Forex_Strategy_Trader
{
    public partial class Strategy
    {
        /// <summary>
        /// Saves the strategy in BBCode format.
        /// </summary>
        public string GenerateBBCode()
        {
            string strBBCode = "";
            string nl  = Environment.NewLine;
            string nl2 = Environment.NewLine + Environment.NewLine;

            strBBCode += "[b]" + Data.ProgramName + " v" + Data.ProgramVersion + (Data.IsProgramBeta ? " Beta" : "") + "[/b]" + nl;
            strBBCode += "Strategy name: [b]" + strategyName + "[/b]" + nl;
            strBBCode += "Exported on: " + DateTime.Now + nl;
            strBBCode += nl;

            // Description
            strBBCode += "[b]Description[/b]" + nl;

            if (Description != "")
            {
                if (!Data.IsStrDescriptionRelevant())
                    strBBCode += "(This description might be outdated!)" + nl2;

                strBBCode += Description + nl2;
            }
            else
                strBBCode += "   None." + nl2;

            strBBCode += UseAccountPercentEntry ? "Use account % for margin" + nl : "";
            string sTradingUnit = UseAccountPercentEntry ? "% of the account for margin" : "";
            strBBCode += "Maximum open lots: " + MaxOpenLots + nl;
            strBBCode += "Entry lots: "    + EntryLots    + sTradingUnit + nl;
            strBBCode += "Adding lots: "   + AddingLots   + sTradingUnit + nl;
            strBBCode += "Reducing lots: " + ReducingLots + sTradingUnit + nl;
            strBBCode += nl;

            if (SameSignalAction == SameDirSignalAction.Add)
                strBBCode += "[b]A same direction signal[/b] - [i]Adds to the position[/i]" + nl;
            else if (SameSignalAction == SameDirSignalAction.Winner)
                strBBCode += "[b]A same direction signal[/b] - [i]Adds to a wining position[/i]" + nl;
            else if (SameSignalAction == SameDirSignalAction.Nothing)
                strBBCode += "[b]A same direction signal[/b] - [i]Does nothing[/i]" + nl;

            if (OppSignalAction == OppositeDirSignalAction.Close)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Closes the position[/i]" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Reduces the position[/i]" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Reverses the position[/i]" + nl;
            else
                strBBCode += "[b]An opposite direction signal[/b] - [i]Does nothing[/i]" + nl;

            strBBCode += "[b]Permanent Stop Loss[/b] - [i]"   + (Data.Strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentSL.ToString() : "None") + "[/i]" + nl;
            strBBCode += "[b]Permanent Take Profit[/b] - [i]" + (Data.Strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentTP.ToString() : "None") + "[/i]" + nl;
            strBBCode += "[b]Break Even[/b] - [i]" + (Data.Strategy.UseBreakEven ? Data.Strategy.BreakEven.ToString() : "None") + "[/i]" + nl2;

            // Add the slots.
            foreach(IndicatorSlot indSlot in this.indicatorSlot)
            {
                string sSlotType;
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        sSlotType = "Opening Point of the Position";
                        break;
                    case SlotTypes.OpenFilter:
                        sSlotType = "Opening Logic Condition";
                        break;
                    case SlotTypes.Close:
                        sSlotType = "Closing Point of the Position";
                        break;
                    case SlotTypes.CloseFilter:
                        sSlotType = "Closing Logic Condition";
                        break;
                    default:
                        sSlotType = "";
                        break;
                }

                strBBCode += "[b][" + sSlotType + "][/b]" + nl;
                strBBCode += "[b][color=blue]" + indSlot.IndicatorName + "[/color][/b]" + nl;

                // Add the list params.
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled)
                    {
                        if (listParam.Caption == "Logic")
                            strBBCode += "     [b][color=teal]" +
                                (Configs.UseLogicalGroups && (indSlot.SlotType == SlotTypes.OpenFilter || indSlot.SlotType == SlotTypes.CloseFilter) ?
                                "[" + (indSlot.LogicalGroup.Length == 1 ? " " + indSlot.LogicalGroup + " " : indSlot.LogicalGroup) + "]   " : "") + listParam.Text + "[/color][/b]" + nl;
                        else
                            strBBCode += "     [b]" + listParam.Caption + "[/b]  -  [i]" + listParam.Text + "[/i]" + nl;
                    }

                // Add the num params.
                foreach(NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        strBBCode += "     [b]" + numParam.Caption + "[/b]  -  [i]" + numParam.ValueToString + "[/i]" + nl;

                // Add the check params.
                foreach(CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        strBBCode += "     [b]" + checkParam.Caption + "[/b]  -  [i]" + (checkParam.Checked ? "Yes" : "No") + "[/i]" + nl;

                strBBCode += nl;
            }

            return strBBCode;
        }

        /// <summary>
        /// Generate Overview in HTML code
        /// </summary>
        /// <returns>the HTML code</returns>
        public string GenerateHTMLOverview()
        {
            StringBuilder sb = new StringBuilder();
            string bgcolor   = String.Format("#{0:X2}{1:X2}{2:X2}", LayoutColors.ColorControlBack.R, LayoutColors.ColorControlBack.G, LayoutColors.ColorControlBack.B);
            string forecolor = String.Format("#{0:X2}{1:X2}{2:X2}", LayoutColors.ColorControlText.R, LayoutColors.ColorControlText.G, LayoutColors.ColorControlText.B);

            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + strategyName + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {margin: 5px; padding: 0 10px 10px 10px; background-color: " + bgcolor + "; color: " + forecolor + "; font-size: 16px}");
            sb.AppendLine(".content h1 {font-size: 1.4em;}");
            sb.AppendLine(".content h2 {font-size: 1.2em;}");
            sb.AppendLine(".content h3 {font-size: 1em;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"fsb_header\">");

            // Description
            sb.AppendLine("<h2>" + Language.T("Author's Description") + "</h2>");
            if (Description != String.Empty)
            {
                string strStrategyDescription = Description;
                strStrategyDescription = Description.Replace("\r\n", "<br />");
                strStrategyDescription = strStrategyDescription.Replace("&", "&amp;");
                strStrategyDescription = strStrategyDescription.Replace("\"", "&quot;");

                sb.AppendLine("<p>" + strStrategyDescription + "</p>");

            }
            else
                sb.AppendLine("<p>" + Language.T("There isn't a description.") + "</p>");

            // Strategy Logic
            sb.AppendLine();
            sb.AppendLine("<h2>" + Language.T("Generated Description") + "</h2>");

            // Opening
            sb.AppendLine("<h3>" + Language.T("Opening (Entry Signal)") + "</h3>");
            sb.AppendLine(OpeningLogicHTMLReport().ToString());

            // Closing
            sb.AppendLine("<h3>" + Language.T("Closing (Exit Signal)") + "</h3>");
            sb.AppendLine(ClosingLogicHTMLReport().ToString());

            // Averaging
            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "</h3>");
            sb.AppendLine(AveragingHTMLReport().ToString());

            // Trading Sizes
            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            sb.AppendLine(TradingSizeHTMLReport().ToString());

            // Protection
            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            if (!Data.Strategy.UsePermanentSL)
            {
                sb.AppendLine("<p>" + Language.T("The strategy doesn't use a Permanent Stop Loss.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Stop Loss limits the loss of a position to") + (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentSL);
                sb.AppendLine(Language.T("pips plus the charged spread and rollover.") + "</p>");
            }

            if (!Data.Strategy.UsePermanentTP)
            {
                sb.AppendLine("<p>" + Language.T("The strategy doesn't use a Permanent Take Profit.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Take Profit closes a position at") + (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentTP);
                sb.AppendLine(Language.T("pips profit reduced with the charged spread and rollover.") + "</p>");
            }

            if (Data.Strategy.UseBreakEven)
            {
                sb.AppendLine("<p>" + Language.T("The position's Stop Loss will be set to Break Even price when the profit reaches") + " " + Data.Strategy.BreakEven);
                sb.AppendLine(Language.T("pips") + "." + "</p>");
            }
            
            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("Use the indicator value from the previous bar for all asterisk-marked indicators!") + "<br />");
            sb.AppendLine("</p>");

            // Footer
            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a HTML report about the opening logic.
        /// </summary>
        StringBuilder OpeningLogicHTMLReport()
        {
            StringBuilder sb = new StringBuilder();
            string sIndicatorName = Data.Strategy.Slot[0].IndicatorName;

            Indicator indOpen = Indicator_Store.ConstructIndicator(sIndicatorName, SlotTypes.Open);
            indOpen.IndParam = Data.Strategy.Slot[0].IndParam;
            indOpen.SetDescription(SlotTypes.Open);

            // Logical groups of the opening conditions.
            List<string> opengroups = new List<string>();
            for (int iSlot = 1; iSlot <= Data.Strategy.OpenFilters; iSlot++)
            {
                string group = Data.Strategy.Slot[iSlot].LogicalGroup;
                if (!opengroups.Contains(group) && group != "All")
                    opengroups.Add(group); // Adds all groups except "All"
            }
            if (opengroups.Count == 0 && Data.Strategy.OpenFilters > 0)
                opengroups.Add("All"); // If all the slots are in "All" group, adds "All" to the list.

            // Long position
            string sOpenLong = "<p>";

            if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Add)
                sOpenLong = Language.T("Open a new long position or add to an existing position");
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Winner)
                sOpenLong = Language.T("Open a new long position or add to a winning position");
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Nothing)
                sOpenLong = Language.T("Open a new long position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                sOpenLong += " " + Language.T("or close a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                sOpenLong += " " + Language.T("or reduce a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                sOpenLong += " " + Language.T("or reverse a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                sOpenLong += "";

            sOpenLong += " " + indOpen.EntryPointLongDescription;

            if (Data.Strategy.OpenFilters == 0)
                sOpenLong += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                sOpenLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                sOpenLong += " " + Language.T("when") + ":</p>";
            else
                sOpenLong += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(sOpenLong);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int iSlot = 1; iSlot <= Data.Strategy.OpenFilters; iSlot++)
                        if (Data.Strategy.Slot[iSlot].LogicalGroup == group || Data.Strategy.Slot[iSlot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int iSlot = 1; iSlot <= Data.Strategy.OpenFilters; iSlot++)
                    {
                        if (Data.Strategy.Slot[iSlot].LogicalGroup != group && Data.Strategy.Slot[iSlot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[iSlot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }

                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            // Short position
            string sOpenShort = "<p>";

            if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Add)
                sOpenShort = Language.T("Open a new short position or add to an existing position");
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Winner)
                sOpenShort = Language.T("Open a new short position or add to a winning position");
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Nothing)
                sOpenShort = Language.T("Open a new short position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                sOpenShort += " " + Language.T("or close a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                sOpenShort += " " + Language.T("or reduce a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                sOpenShort += " " + Language.T("or reverse a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                sOpenShort += "";

            sOpenShort += " " + indOpen.EntryPointShortDescription;

            if (Data.Strategy.OpenFilters == 0)
                sOpenShort += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                sOpenShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                sOpenShort += " " + Language.T("when") + ":</p>";
            else
                sOpenShort += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(sOpenShort);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int iSlot = 1; iSlot <= Data.Strategy.OpenFilters; iSlot++)
                        if (Data.Strategy.Slot[iSlot].LogicalGroup == group || Data.Strategy.Slot[iSlot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int iSlot = 1; iSlot <= Data.Strategy.OpenFilters; iSlot++)
                    {
                        if (Data.Strategy.Slot[iSlot].LogicalGroup != group && Data.Strategy.Slot[iSlot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[iSlot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }
                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the closing logic.
        /// </summary>
        StringBuilder ClosingLogicHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            int iClosingSlotNmb = Data.Strategy.CloseSlot;
            string sIndicatorName = Data.Strategy.Slot[iClosingSlotNmb].IndicatorName;

            Indicator indClose = Indicator_Store.ConstructIndicator(sIndicatorName, SlotTypes.Close);
            indClose.IndParam = Data.Strategy.Slot[iClosingSlotNmb].IndParam;
            indClose.SetDescription(SlotTypes.Close);

            bool bGroups = false;
            List<string> closegroups = new List<string>();

            if (Data.Strategy.CloseFilters > 0)
                foreach (IndicatorSlot slot in Data.Strategy.Slot)
                {
                    if (slot.SlotType == SlotTypes.CloseFilter)
                    {
                        if (slot.LogicalGroup == "all" && Data.Strategy.CloseFilters > 1)
                            bGroups = true;

                        if (closegroups.Contains(slot.LogicalGroup))
                            bGroups = true;
                        else if (slot.LogicalGroup != "all")
                            closegroups.Add(slot.LogicalGroup);
                    }
                }

            if (closegroups.Count == 0 && Data.Strategy.CloseFilters > 0)
                closegroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.


            // Long position
            string sColoseLong = "<p>" + Language.T("Close an existing long position") + " " + indClose.ExitPointLongDescription;

            if (Data.Strategy.CloseFilters == 0)
                sColoseLong += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                sColoseLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (bGroups)
                sColoseLong += " " + Language.T("when") + ":</p>";
            else
                sColoseLong += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(sColoseLong);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (bGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int iSlot = iClosingSlotNmb + 1; iSlot < Data.Strategy.Slots; iSlot++)
                        if (Data.Strategy.Slot[iSlot].LogicalGroup == group || Data.Strategy.Slot[iSlot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int iSlot = iClosingSlotNmb + 1; iSlot < Data.Strategy.Slots; iSlot++)
                    {
                        if (Data.Strategy.Slot[iSlot].LogicalGroup != group && Data.Strategy.Slot[iSlot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[iSlot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (bGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        else
                        {
                            if (iSlot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (bGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            // Short position
            string sColoseShort = "<p>" + Language.T("Close an existing short position") + " " + indClose.ExitPointShortDescription;

            if (Data.Strategy.CloseFilters == 0)
                sColoseShort += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                sColoseShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (bGroups)
                sColoseShort += " " + Language.T("when") + ":</p>";
            else
                sColoseShort += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(sColoseShort);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (bGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int iSlot = iClosingSlotNmb + 1; iSlot < Data.Strategy.Slots; iSlot++)
                        if (Data.Strategy.Slot[iSlot].LogicalGroup == group || Data.Strategy.Slot[iSlot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int iSlot = iClosingSlotNmb + 1; iSlot < Data.Strategy.Slots; iSlot++)
                    {
                        if (Data.Strategy.Slot[iSlot].LogicalGroup != group && Data.Strategy.Slot[iSlot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[iSlot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (bGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        else
                        {
                            if (iSlot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (bGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the averaging logic.
        /// </summary>
        StringBuilder AveragingHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            // Same direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the direction of the present position:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No averaging is allowed. Cancel any additional orders, which are in the same direction."));
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Winner)
                sb.AppendLine(Language.T("Add to a wining position but not to a losing one. If the position is at a loss, cancel the additional entry order. Do not exceed the maximum allowed number of lots to open."));
            else if (Data.Strategy.sameDirSignlAct == SameDirSignalAction.Add)
                sb.AppendLine(Language.T("Add to the position regardless whether it is at a profit or loss. Do not exceed the maximum allowed number of lots to open."));
            sb.AppendLine("</li></ul>");

            // Opposite direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the opposite direction:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.oppDirSignlAct == OppositeDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No modification of the present position is allowed. Cancel any additional orders, which are in the opposite direction."));
            else if (Data.Strategy.oppDirSignlAct == OppositeDirSignalAction.Reduce)
                sb.AppendLine(Language.T("Reduce the present position. If its amount is lower than or equal to the specified reducing lots, close it."));
            else if (Data.Strategy.oppDirSignlAct == OppositeDirSignalAction.Close)
                sb.AppendLine(Language.T("Close the present position regardless of its amount or result. Do not open a new position until the next entry signal has been raised."));
            else if (Data.Strategy.oppDirSignlAct == OppositeDirSignalAction.Reverse)
                sb.AppendLine(Language.T("Close the existing position and open a new one in the opposite direction using the entry rules."));
            sb.AppendLine("</li></ul>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading sizes.
        /// </summary>
        StringBuilder TradingSizeHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            if (UseAccountPercentEntry)
            {
                sb.AppendLine("<p>" + Language.T("Trade an account percent truncated to an acceptable number of lots.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + EntryLots + Language.T("% of the account equity") + ".</li>");
                if (sameDirSignlAct == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (sameDirSignlAct == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (oppDirSignlAct == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + ReducingLots + Language.T("% of the account equity") + ".</li>");
                if (oppDirSignlAct == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + EntryLots + Language.T("% of the account equity") + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("Always trade a constant number of lots.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + Plural("lot", EntryLots) + ".</li>");
                if (sameDirSignlAct == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (sameDirSignlAct == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (oppDirSignlAct == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + Plural("lot", ReducingLots) + " " + Language.T("from the current position.") + "</li>");
                if (oppDirSignlAct == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + Plural("lot", EntryLots) + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Represents the strategy in a readable form.
        /// </summary>
        public override string ToString()
        {
            string str = String.Empty;
            string nl  = Environment.NewLine;
            string nl2 = Environment.NewLine + Environment.NewLine;

            str += "Strategy Name - "       + strategyName               + nl;
            str += "Symbol - "              + Symbol                     + nl;
            str += "Period - "              + DataPeriod.ToString()      + nl;
            str += "Same dir signal - "     + sameDirSignlAct.ToString() + nl;
            str += "Opposite dir signal - " + oppDirSignlAct.ToString()  + nl;
            str += "Use account % entry - " + UseAccountPercentEntry     + nl;
            str += "Max open lots - "       + MaxOpenLots                + nl;
            str += "Entry lots - "          + EntryLots                  + nl;
            str += "Adding lots - "         + AddingLots                 + nl;
            str += "Reducing lots - "       + ReducingLots               + nl;
            str += "Use Permanent S/L - "   + usePermanentSL.ToString()  + nl;
            str += "Permanent S/L - "       + permanentSLType.ToString() + " " + permanentSL.ToString() + nl;
            str += "Use Permanent T/P - "   + usePermanentTP.ToString()  + nl;
            str += "Permanent T/P - "       + permanentTPType.ToString() + " " + permanentTP.ToString() + nl;
            str += "Use Break Even - "      + useBreakEven.ToString()    + nl;
            str += "Break Even - "          + breakEven.ToString() + " " + permanentTP.ToString() + nl2;
            str += "Description"            + nl2 + nl;

            for (int iSlot = 0; iSlot < Slots; iSlot++)
            {
                str += Slot[iSlot].SlotType.ToString() + nl;
                str += indicatorSlot[iSlot].IndParam.ToString() + nl2;
            }

            return str;
        }

        /// <summary>
        /// Appends "s" when plural
        /// </summary>
        string Plural(string sUnit, double dNumber)
        {
            if (dNumber != 1 && dNumber != -1)
                sUnit += "s";

            return dNumber.ToString() + " " + Language.T(sUnit);
        }
    }
}
