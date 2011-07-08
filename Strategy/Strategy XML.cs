// Strategy_XML Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Xml;

namespace Forex_Strategy_Trader
{
    public class Strategy_XML
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Strategy_XML() { }

        /// <summary>
        /// Represents the Strategy as a XmlDocument.
        /// </summary>
        /// <returns>StrategyXmlDoc</returns>
        public XmlDocument CreateStrategyXmlDoc(Strategy strategy)
        {
            // Create the XmlDocument.
            XmlDocument xmlDocStrategy = new XmlDocument();
            xmlDocStrategy.LoadXml("<strategy></strategy>");

            //Create the XML declaration. 
            XmlDeclaration xmldecl;
            xmldecl = xmlDocStrategy.CreateXmlDeclaration("1.0", null, null);

            //Add new node to the document.
            XmlElement root = xmlDocStrategy.DocumentElement;
            xmlDocStrategy.InsertBefore(xmldecl, root);

            // Add the program name.
            XmlElement newElem = xmlDocStrategy.CreateElement("programName");
            newElem.InnerText = Data.ProgramName;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the program version.
            newElem = xmlDocStrategy.CreateElement("programVersion");
            newElem.InnerText = Data.ProgramVersion.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the strategy name.
            newElem = xmlDocStrategy.CreateElement("strategyName");
            newElem.InnerText = strategy.StrategyName;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Symbol.
            newElem = xmlDocStrategy.CreateElement("instrumentSymbol");
            newElem.InnerText = strategy.Symbol;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Period.
            newElem = xmlDocStrategy.CreateElement("instrumentPeriod");
            newElem.InnerText = strategy.DataPeriod.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the same direction signal action.
            newElem = xmlDocStrategy.CreateElement("sameDirSignalAction");
            newElem.InnerText = strategy.SameSignalAction.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the opposite direction signal action.
            newElem = xmlDocStrategy.CreateElement("oppDirSignalAction");
            newElem.InnerText = strategy.OppSignalAction.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Permanent Stop Loss
            newElem = xmlDocStrategy.CreateElement("permanentStopLoss");
            newElem.InnerText = strategy.PermanentSL.ToString();
            newElem.SetAttribute("usePermanentSL", strategy.UsePermanentSL.ToString());
            newElem.SetAttribute("permanentSLType", strategy.PermanentSLType.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Permanent Take Profit
            newElem = xmlDocStrategy.CreateElement("permanentTakeProfit");
            newElem.InnerText = strategy.PermanentTP.ToString();
            newElem.SetAttribute("usePermanentTP", strategy.UsePermanentTP.ToString());
            newElem.SetAttribute("permanentTPType", strategy.PermanentTPType.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Break Even
            newElem = xmlDocStrategy.CreateElement("breakEven");
            newElem.InnerText = strategy.BreakEven.ToString();
            newElem.SetAttribute("useBreakEven", strategy.UseBreakEven.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the max open lots
            newElem = xmlDocStrategy.CreateElement("maxOpenLots");
            newElem.InnerText = strategy.MaxOpenLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add Use Account Percent Entry
            newElem = xmlDocStrategy.CreateElement("useAccountPercentEntry");
            newElem.InnerText = strategy.UseAccountPercentEntry.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Entry lots
            newElem = xmlDocStrategy.CreateElement("entryLots");
            newElem.InnerText = strategy.EntryLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Adding lots
            newElem = xmlDocStrategy.CreateElement("addingLots");
            newElem.InnerText = strategy.AddingLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Reducing lots
            newElem = xmlDocStrategy.CreateElement("reducingLots");
            newElem.InnerText = strategy.ReducingLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Description
            newElem = xmlDocStrategy.CreateElement("description");
            newElem.InnerText = strategy.Description;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the number of open filters.
            newElem = xmlDocStrategy.CreateElement("openFilters");
            newElem.InnerText = strategy.OpenFilters.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the number of close filters.
            newElem = xmlDocStrategy.CreateElement("closeFilters");
            newElem.InnerText = strategy.CloseFilters.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the slots.
            for (int iSlot = 0; iSlot < strategy.Slots; iSlot++)
            {
                SlotTypes slType = strategy.Slot[iSlot].SlotType;

                // Add a slot element.
                XmlElement newSlot = xmlDocStrategy.CreateElement("slot");
                newSlot.SetAttribute("slotNumber", iSlot.ToString());
                newSlot.SetAttribute("slotType", slType.ToString());

                if (slType == SlotTypes.OpenFilter || slType == SlotTypes.CloseFilter)
                    newSlot.SetAttribute("logicalGroup", strategy.Slot[iSlot].LogicalGroup);

                // Add an element.
                newElem = xmlDocStrategy.CreateElement("indicatorName");
                newElem.InnerText = strategy.Slot[iSlot].IndicatorName;
                newSlot.AppendChild(newElem);

                // Add the list params.
                for (int iParam = 0; iParam < strategy.Slot[iSlot].IndParam.ListParam.Length; iParam++)
                {
                    if (strategy.Slot[iSlot].IndParam.ListParam[iParam].Enabled)
                    {
                        // Add an element.
                        XmlElement newListElem = xmlDocStrategy.CreateElement("listParam");
                        newListElem.SetAttribute("paramNumber", iParam.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.ListParam[iParam].Caption;
                        newListElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("index");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.ListParam[iParam].Index.ToString();
                        newListElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.ListParam[iParam].Text;
                        newListElem.AppendChild(newElem);

                        newSlot.AppendChild(newListElem);
                    }
                }

                // Add the num params.
                for (int iParam = 0; iParam < strategy.Slot[iSlot].IndParam.NumParam.Length; iParam++)
                {
                    if (strategy.Slot[iSlot].IndParam.NumParam[iParam].Enabled)
                    {
                        // Add an element.
                        XmlElement newNumElem = xmlDocStrategy.CreateElement("numParam");
                        newNumElem.SetAttribute("paramNumber", iParam.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.NumParam[iParam].Caption;
                        newNumElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.NumParam[iParam].ValueToString;
                        newNumElem.AppendChild(newElem);

                        newSlot.AppendChild(newNumElem);
                    }
                }

                // Add the check params.
                for (int iParam = 0; iParam < strategy.Slot[iSlot].IndParam.CheckParam.Length; iParam++)
                {
                    if (strategy.Slot[iSlot].IndParam.CheckParam[iParam].Enabled)
                    {
                        // Add an element.
                        XmlElement newCheckElem = xmlDocStrategy.CreateElement("checkParam");
                        newCheckElem.SetAttribute("paramNumber", iParam.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.CheckParam[iParam].Caption;
                        newCheckElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[iSlot].IndParam.CheckParam[iParam].Checked.ToString();
                        newCheckElem.AppendChild(newElem);

                        newSlot.AppendChild(newCheckElem);
                    }
                }

                xmlDocStrategy.DocumentElement.AppendChild(newSlot);
            }

            return xmlDocStrategy;
        }

        /// <summary>
        /// Pareses a strategy from a xml document.
        /// </summary>
        public Strategy ParseXmlStrategy(XmlDocument xmlDocStrategy)
        {
            // Read the number of slots
            int openFilters  = int.Parse(xmlDocStrategy.GetElementsByTagName("openFilters" )[0].InnerText);
            int closeFilters = int.Parse(xmlDocStrategy.GetElementsByTagName("closeFilters")[0].InnerText);

            // Create the strategy.
            Strategy tempStrategy = new Strategy(openFilters, closeFilters);

            // Same and Opposite direction Actions
            tempStrategy.SameSignalAction = (SameDirSignalAction    )Enum.Parse(typeof(SameDirSignalAction    ), xmlDocStrategy.GetElementsByTagName("sameDirSignalAction")[0].InnerText);
            tempStrategy.OppSignalAction  = (OppositeDirSignalAction)Enum.Parse(typeof(OppositeDirSignalAction), xmlDocStrategy.GetElementsByTagName("oppDirSignalAction" )[0].InnerText);

            // Market
            tempStrategy.Symbol     = xmlDocStrategy.GetElementsByTagName("instrumentSymbol")[0].InnerText;
            tempStrategy.DataPeriod = (DataPeriods)Enum.Parse(typeof(DataPeriods), xmlDocStrategy.GetElementsByTagName("instrumentPeriod")[0].InnerText);

            // Permanent Stop Loss
            tempStrategy.PermanentSL    = Math.Abs(int.Parse(xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].InnerText)); // Math.Abs() removes the negative sign from previous versions.
            tempStrategy.UsePermanentSL = bool.Parse(xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].Attributes["usePermanentSL"].InnerText);
            try
            {
                tempStrategy.PermanentSLType = (PermanentProtectionType)Enum.Parse(typeof(PermanentProtectionType), xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].Attributes["permanentSLType"].InnerText);
            }
            catch
            {
                tempStrategy.PermanentSLType = PermanentProtectionType.Relative;
            }

            // Permanent Take Profit
            tempStrategy.PermanentTP    = int.Parse(xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].InnerText);
            tempStrategy.UsePermanentTP = bool.Parse(xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].Attributes["usePermanentTP"].InnerText);
            try
            {
                tempStrategy.PermanentTPType = (PermanentProtectionType)Enum.Parse(typeof(PermanentProtectionType), xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].Attributes["permanentTPType"].InnerText);
            }
            catch
            {
                tempStrategy.PermanentTPType = PermanentProtectionType.Relative;
            }

            // Break Even
            try {
                tempStrategy.BreakEven    = int.Parse(xmlDocStrategy.GetElementsByTagName("breakEven")[0].InnerText);
                tempStrategy.UseBreakEven = bool.Parse(xmlDocStrategy.GetElementsByTagName("breakEven")[0].Attributes["useBreakEven"].InnerText);
            } catch { }

            // Money Management
            try
            {
                tempStrategy.UseAccountPercentEntry = bool.Parse(xmlDocStrategy.GetElementsByTagName("useAccountPercentEntry")[0].InnerText);
            }
            catch
            {
                tempStrategy.UseAccountPercentEntry = bool.Parse(xmlDocStrategy.GetElementsByTagName("useAcountPercentEntry")[0].InnerText);
            }
            tempStrategy.MaxOpenLots  = StringToDouble(xmlDocStrategy.GetElementsByTagName("maxOpenLots")[0].InnerText);
            tempStrategy.EntryLots    = StringToDouble(xmlDocStrategy.GetElementsByTagName("entryLots")[0].InnerText);
            tempStrategy.AddingLots   = StringToDouble(xmlDocStrategy.GetElementsByTagName("addingLots")[0].InnerText);
            tempStrategy.ReducingLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("reducingLots")[0].InnerText);

            // Description
            tempStrategy.Description = xmlDocStrategy.GetElementsByTagName("description")[0].InnerText;

            // Strategy name.
            tempStrategy.StrategyName = xmlDocStrategy.GetElementsByTagName("strategyName")[0].InnerText;

            // Reading the slots
            XmlNodeList xmlSlotList = xmlDocStrategy.GetElementsByTagName("slot");
            for (int iSlot = 0; iSlot < xmlSlotList.Count; iSlot++)
            {
                XmlNodeList xmlSlotTagList = xmlSlotList[iSlot].ChildNodes;

                SlotTypes slType = (SlotTypes)Enum.Parse(typeof(SlotTypes), xmlSlotList[iSlot].Attributes["slotType"].InnerText);

                // Logical group
                if (slType == SlotTypes.OpenFilter || slType == SlotTypes.CloseFilter)
                {
                    XmlAttributeCollection attributes = xmlSlotList[iSlot].Attributes;
                    XmlNode nodeGroup = attributes.GetNamedItem("logicalGroup");
                    string defGroup = GetDefaultGroup(slType, iSlot, tempStrategy.CloseSlot);
                    if (nodeGroup != null)
                    {
                        tempStrategy.Slot[iSlot].LogicalGroup = nodeGroup.InnerText;
                        if (nodeGroup.InnerText != defGroup && !Configs.UseLogicalGroups)
                        {
                            System.Windows.Forms.MessageBox.Show(
                                Language.T("The strategy requires logical groups.") + Environment.NewLine +
                                Language.T("\"Use Logical Groups\" option was temporarily switched on."),
                                Language.T("Logical Groups"),
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                            Configs.UseLogicalGroups = true;
                        }
                    }
                    else
                        tempStrategy.Slot[iSlot].LogicalGroup = defGroup;
                } 

                // Indicator name.
                string    sIndicatorName = xmlSlotTagList[0].InnerText;
                Indicator indicator      = Indicator_Store.ConstructIndicator(sIndicatorName, slType);

                for (int iTag = 1; iTag < xmlSlotTagList.Count; iTag++)
                {
                    // List parameters
                    if (xmlSlotTagList[iTag].Name == "listParam")
                    {
                        int iListParam = int.Parse(xmlSlotTagList[iTag].Attributes["paramNumber"].InnerText);
                        XmlNode xmlListParamNode = xmlSlotTagList[iTag].FirstChild;

                        indicator.IndParam.ListParam[iListParam].Caption = xmlListParamNode.InnerText;

                        xmlListParamNode = xmlListParamNode.NextSibling;
                        int index = int.Parse(xmlListParamNode.InnerText);
                        indicator.IndParam.ListParam[iListParam].Index = index;
                        indicator.IndParam.ListParam[iListParam].Text  = indicator.IndParam.ListParam[iListParam].ItemList[index];
                    }

                    // Numeric parameters
                    if (xmlSlotTagList[iTag].Name == "numParam")
                    {
                        XmlNode xmlNumParamNode = xmlSlotTagList[iTag].FirstChild;
                        int iNumParam = int.Parse(xmlSlotTagList[iTag].Attributes["paramNumber"].InnerText);
                        indicator.IndParam.NumParam[iNumParam].Caption = xmlNumParamNode.InnerText;

                        xmlNumParamNode = xmlNumParamNode.NextSibling;
                        string sNumParamValue = xmlNumParamNode.InnerText;
                        sNumParamValue = sNumParamValue.Replace(',', Data.PointChar);
                        sNumParamValue = sNumParamValue.Replace('.', Data.PointChar);
                        float fValue = float.Parse(sNumParamValue);

                        // Removing of the Stop Loss negative sign used in previous versions.
                        string sParCaption = indicator.IndParam.NumParam[iNumParam].Caption;
                        if (sParCaption == "Trailing Stop"     ||
                            sParCaption == "Initial Stop Loss" ||
                            sParCaption == "Stop Loss")
                            fValue = Math.Abs(fValue);
                        indicator.IndParam.NumParam[iNumParam].Value = fValue;
                    }

                    // Check parameters
                    if (xmlSlotTagList[iTag].Name == "checkParam")
                    {
                        XmlNode xmlCheckParamNode = xmlSlotTagList[iTag].FirstChild;
                        int iCheckParam = int.Parse(xmlSlotTagList[iTag].Attributes["paramNumber"].InnerText);
                        indicator.IndParam.CheckParam[iCheckParam].Caption = xmlCheckParamNode.InnerText;

                        xmlCheckParamNode = xmlCheckParamNode.NextSibling;
                        indicator.IndParam.CheckParam[iCheckParam].Checked = bool.Parse(xmlCheckParamNode.InnerText);
                    }
                }

                // Calculate the indicator.
                indicator.Calculate(slType);
                tempStrategy.Slot[iSlot].IndicatorName  = indicator.IndicatorName;
                tempStrategy.Slot[iSlot].IndParam       = indicator.IndParam;
                tempStrategy.Slot[iSlot].Component      = indicator.Component;
                tempStrategy.Slot[iSlot].SeparatedChart = indicator.SeparatedChart;
                tempStrategy.Slot[iSlot].SpecValue      = indicator.SpecialValues;
                tempStrategy.Slot[iSlot].MinValue       = indicator.SeparatedChartMinValue;
                tempStrategy.Slot[iSlot].MaxValue       = indicator.SeparatedChartMaxValue;
                tempStrategy.Slot[iSlot].IsDefined      = true;
            }

            return tempStrategy;
        }

        static double StringToDouble(string input)
        {
            string sDecimalPoint = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(sDecimalPoint))
            {
                input = input.Replace(".", sDecimalPoint);
                input = input.Replace(",", sDecimalPoint);
            }

            double number;

            try
            {
                number = double.Parse(input);
            }
            catch
            {
                number = double.NaN;
            }

            return number;
        }
 
        /// <summary>
        /// Gets the default logical group of the slot.
        /// </summary>
        string GetDefaultGroup(SlotTypes slotType, int slotIndex, int closeSlotIndex)
        {
            string group = "";

            if (slotType == SlotTypes.OpenFilter)
            {
                group = "A";
            }
            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slotIndex - closeSlotIndex - 1;
                group = char.ConvertFromUtf32(char.ConvertToUtf32("a", 0) + index);
            }

            return group;
        }
   }
}
