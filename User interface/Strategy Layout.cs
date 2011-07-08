// Strategy Layout
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Represents the strategies slots into a readable form
    /// </summary>
    public class Strategy_Layout : Panel
    {
        VScrollBar      VScrollBarStrategy; // Vertical scrollbar
        FlowLayoutPanel flowLayoutStrategy; // Contains the strategy slots
        public Panel[]  apnlSlot;           // Indicator's parameters
        public Panel    pnlProperties;      // Strategy properties panel
        public Button[] abtnRemoveSlot;     // Removes the indicator slot
        public Button   btnAddOpenFilter;   // Add an Open Filter indicator slot
        public Button   btnAddCloseFilter;  // Add an Close Filter indicator slot
        Button btnClosingFilterHelp;

        ToolTip  toolTip;
        Strategy strategy;
        int space  = 4;
        int border = 2;
        int slots;
        SlotSizeMinMidMax slotMinMidMax;
        bool   bShowAddSlotButtons    = true;
        bool   bShowRemoveSlotButtons = true;
        string slotToolTipText       = Language.T("Long position logic.");
        string slotPropertiesTipText = Language.T("Averaging, Trading size, Protection.");

        /// <summary>
        /// Sets the size of the strategy's slots
        /// </summary>
        public SlotSizeMinMidMax SlotMinMidMax { get { return slotMinMidMax; } set { slotMinMidMax = value; } }

        /// <summary>
        /// Show Add Slot Buttons
        /// </summary>
        public bool ShowAddSlotButtons { set { bShowAddSlotButtons = value; } }

        /// <summary>
        /// Show Remove Slot Buttons
        /// </summary>
        public bool ShowRemoveSlotButtons { set { bShowRemoveSlotButtons = value; } }

        /// <summary>
        /// Sets the tool tip text
        /// </summary>
        public string SlotToolTipText { set { slotToolTipText = value; } }

        /// <summary>
        /// Sets the tool tip text
        /// </summary>
        public string SlotPropertiesTipText { set { slotPropertiesTipText = value; } }

        /// <summary>
        /// Initializes the strategy field
        /// </summary>
        public Strategy_Layout(Strategy strategy)
        {
            this.strategy      = strategy;
            slots              = strategy.Slots;
            slotMinMidMax      = SlotSizeMinMidMax.mid;
            toolTip            = new ToolTip();
            flowLayoutStrategy = new FlowLayoutPanel();
            VScrollBarStrategy = new VScrollBar();
            apnlSlot           = new Panel[slots];
            pnlProperties      = new Panel();

            for (int iSlot = 0; iSlot < slots; iSlot++)
                apnlSlot[iSlot] = new Panel();

            if (bShowRemoveSlotButtons)
            {
                abtnRemoveSlot = new Button[slots];
                for (int iSlot = 0; iSlot < slots; iSlot++)
                    abtnRemoveSlot[iSlot] = new Button();
            }

            // FlowLayoutStrategy
            flowLayoutStrategy.Parent     = this;
            flowLayoutStrategy.AutoScroll = false;

            //VScrollBarStrategy
            VScrollBarStrategy.Parent  = this;
            VScrollBarStrategy.TabStop = true;
            VScrollBarStrategy.Scroll += new ScrollEventHandler(VScrollBarStrategy_Scroll);

            if (bShowAddSlotButtons)
            {
                // btnAddOpenFilter
                btnAddOpenFilter = new Button();
                btnAddOpenFilter.Tag       = strategy.OpenSlot;
                btnAddOpenFilter.Text      = Language.T("Add an Opening Logic Condition");
                btnAddOpenFilter.Margin    = new Padding(30, 0, 0, space);
                btnAddOpenFilter.UseVisualStyleBackColor = true;
                toolTip.SetToolTip(btnAddOpenFilter, Language.T("Add a new entry logic slot to the strategy."));

                // btnAddCloseFilter
                btnAddCloseFilter = new Button();
                btnAddCloseFilter.Tag       = strategy.CloseSlot;
                btnAddCloseFilter.Text      = Language.T("Add a Closing Logic Condition");
                btnAddCloseFilter.Margin    = new Padding(30, 0, 0, space);
                btnAddCloseFilter.UseVisualStyleBackColor = true;
                toolTip.SetToolTip(btnAddCloseFilter, Language.T("Add a new exit logic slot to the strategy."));

                // btnClosingFilterHelp
                btnClosingFilterHelp = new Button();
                btnClosingFilterHelp.Image   = Properties.Resources.info;
                btnClosingFilterHelp.Margin  = new Padding(2, 2, 0, space);
                btnClosingFilterHelp.TabStop = false;
                btnClosingFilterHelp.Click  += new EventHandler(BtnClosingFilterHelp_Click);
                btnClosingFilterHelp.UseVisualStyleBackColor = true;
            }

            return;
        }

        /// <summary>
        /// Initializes the strategy slots
        /// </summary>
        public void InitializeStrategySlots()
        {
            apnlSlot       = new Panel[slots];
            abtnRemoveSlot = new Button[slots];

            // Strategy properties panel
            pnlProperties = new Panel();
            pnlProperties.Cursor  = Cursors.Hand;
            pnlProperties.Tag     = 100;
            pnlProperties.Margin  = new Padding(0, 0, 0, space);
            pnlProperties.Paint  += new PaintEventHandler(PnlProperties_Paint);
            pnlProperties.Resize += new EventHandler(PnlSlot_Resize);
            toolTip.SetToolTip(pnlProperties, slotPropertiesTipText);
   
            // Slot panels settings
            for (int iSlot = 0; iSlot < slots; iSlot++)
            {
                apnlSlot[iSlot] = new Panel();
                apnlSlot[iSlot].Cursor  = Cursors.Hand;
                apnlSlot[iSlot].Tag     = iSlot;
                apnlSlot[iSlot].Margin  = new Padding(0, 0, 0, space);
                apnlSlot[iSlot].Paint  += new PaintEventHandler(PnlSlot_Paint);
                apnlSlot[iSlot].Resize += new EventHandler(PnlSlot_Resize);
                toolTip.SetToolTip(apnlSlot[iSlot], slotToolTipText);

                if (bShowRemoveSlotButtons && iSlot != strategy.OpenSlot && iSlot != strategy.CloseSlot)
                {   // RemoveSlot buttons
                    abtnRemoveSlot[iSlot] = new Button();
                    abtnRemoveSlot[iSlot].Parent = apnlSlot[iSlot];
                    abtnRemoveSlot[iSlot].Tag    = iSlot;
                    abtnRemoveSlot[iSlot].Cursor = Cursors.Arrow;
                    abtnRemoveSlot[iSlot].BackgroundImage = Properties.Resources.close_button;
                    abtnRemoveSlot[iSlot].UseVisualStyleBackColor = true;
                    toolTip.SetToolTip(abtnRemoveSlot[iSlot], Language.T("Discard the logic condition."));
                }
            }

            // Adds the controls to the flow layout
            flowLayoutStrategy.Controls.Add(pnlProperties);
            for (int iSlot = 0; iSlot < slots; iSlot++)
            {
                if (bShowAddSlotButtons && iSlot == strategy.CloseSlot)
                    flowLayoutStrategy.Controls.Add(btnAddOpenFilter);
                flowLayoutStrategy.Controls.Add(apnlSlot[iSlot]);
            }
            if (bShowAddSlotButtons)
            {
                flowLayoutStrategy.Controls.Add(btnAddCloseFilter);
                flowLayoutStrategy.Controls.Add(btnClosingFilterHelp);
            }

            return;
        }

        /// <summary>
        /// Calculates the position of the controls
        /// </summary>
        public void ArrangeStrategyControls()
        {
            int iWidth  = ClientSize.Width;
            int iHeight = ClientSize.Height;
            int iTotalHeight = PnlSlotCalculateTotalHeight(iWidth);
            if (iTotalHeight < iHeight)
            {
                VScrollBarStrategy.Enabled = false;
                VScrollBarStrategy.Visible = false;
            }
            else
            {
                iWidth       = ClientSize.Width - VScrollBarStrategy.Width;
                iTotalHeight = PnlSlotCalculateTotalHeight(iWidth);
                VScrollBarStrategy.Enabled     = true;
                VScrollBarStrategy.Visible     = true;
                VScrollBarStrategy.Value       = 0;
                VScrollBarStrategy.SmallChange = 100;
                VScrollBarStrategy.LargeChange = 200;
                VScrollBarStrategy.Maximum     = Math.Max(iTotalHeight - iHeight + 220, 0);
                VScrollBarStrategy.Location    = new Point(iWidth, 0);
                VScrollBarStrategy.Height      = iHeight;
            }

            flowLayoutStrategy.Location = Point.Empty;
            flowLayoutStrategy.Width    = iWidth;
            flowLayoutStrategy.Height   = iTotalHeight;

            // Strategy properties panel size
            int iPnlPropertiesWidth  = flowLayoutStrategy.ClientSize.Width;
            int iPnlPropertiesHeight = PnlPropertiesCalculateHeight(iPnlPropertiesWidth);
            pnlProperties.Size = new Size(iPnlPropertiesWidth, iPnlPropertiesHeight);

            // Sets the strategy slots size
            for (int iSlot = 0; iSlot < slots; iSlot++)
            {
                int iStrWidth  = flowLayoutStrategy.ClientSize.Width;
                int iStrHeight = PnlSlotCalculateHeight(iSlot, iStrWidth);
                apnlSlot[iSlot].Size = new Size(iStrWidth, iStrHeight);
            }

            if (bShowAddSlotButtons)
            {
                int iButtonWidth  = flowLayoutStrategy.ClientSize.Width - 60;
                int iButtonHeight = (int)(Font.Height * 1.7);
                btnAddOpenFilter.Size     = new Size(iButtonWidth, iButtonHeight);
                btnAddCloseFilter.Size    = new Size(iButtonWidth, iButtonHeight);
                btnClosingFilterHelp.Size = new Size(iButtonHeight - 4, iButtonHeight - 4);
            }

            return;
        }

        /// <summary>
        /// Sets add new slot buttons
        /// </summary>
        protected void SetAddSlotButtons()
        {
            // Shows or not btnAddOpenFilter
            btnAddOpenFilter.Enabled = strategy.OpenFilters < Strategy.MaxOpenFilters;

            bool isClosingFiltersAllowed = Indicator_Store.ClosingIndicatorsWithClosingFilters.Contains(strategy.Slot[strategy.CloseSlot].IndicatorName);

            // Shows or not btnAddCloseFilter
            btnAddCloseFilter.Enabled = (strategy.CloseFilters < Strategy.MaxCloseFilters && isClosingFiltersAllowed);

            // Shows or not btnClosingFilterHelp
            btnClosingFilterHelp.Visible = !isClosingFiltersAllowed;

            return;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// <summary>
        void VScrollBarStrategy_Scroll(object sender, ScrollEventArgs e)
        {
            VScrollBar vscroll = (VScrollBar)sender;
            flowLayoutStrategy.Location = new Point(0, -vscroll.Value);
        }

        /// <summary>
        /// Rebuilds all the controls in panel Strategy
        /// </summary>
        public void RebuildStrategyControls(Strategy strategy)
        {
            this.strategy = strategy;
            slots = strategy.Slots;
            flowLayoutStrategy.SuspendLayout();
            flowLayoutStrategy.Controls.Clear();
            InitializeStrategySlots();
            ArrangeStrategyControls();
            if (bShowAddSlotButtons) SetAddSlotButtons();
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Rearrange all controls in panel Strategy.
        /// </summary>
        public void RearangeStrategyControls()
        {
            flowLayoutStrategy.SuspendLayout();
            ArrangeStrategyControls();
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Repaints the strategy slots
        /// </summary>
        /// <param name="strategy">The strategy</param>
        public void RepaintStrategyControls(Strategy strategy)
        {
            this.strategy = strategy;
            slots = strategy.Slots;
            foreach (Panel pnl in apnlSlot)
                pnl.Invalidate();
            pnlProperties.Invalidate();
        }

        /// <summary>
        /// Panel Strategy Resize
        /// </summary>
        void PnlSlot_Resize(object sender, EventArgs e)
        {
            Panel pnl = (Panel)sender;
            pnl.Invalidate();
        }

        /// <summary>
        /// Calculates the height of the Panel Slot
        /// </summary>
        int PnlSlotCalculateHeight(int slot, int width)
        {
            Font fontCaption = new Font(Font.FontFamily, 9f);
            int  iVPosition  = (int)Math.Max(fontCaption.Height, 18) + 3;

            Font fontIndicator = new Font(Font.FontFamily, 11f);
            iVPosition += fontIndicator.Height;

            if (slotMinMidMax == SlotSizeMinMidMax.min)
                return iVPosition + 5;

            // Calculate the height of Logic string
            if (strategy.Slot[slot].IndParam.ListParam[0].Enabled)
            {
                Graphics g = CreateGraphics();
                float fPadding = space;

                StringFormat stringFormatLogic  = new StringFormat();
                stringFormatLogic.Alignment     = StringAlignment.Center;
                stringFormatLogic.LineAlignment = StringAlignment.Center;
                stringFormatLogic.Trimming      = StringTrimming.EllipsisCharacter;
                stringFormatLogic.FormatFlags   = StringFormatFlags.NoClip;

                string sValue = strategy.Slot[slot].IndParam.ListParam[0].Text;
                Font  fontLogic = new Font(Font.FontFamily, 10.5f, FontStyle.Regular);
                SizeF sizeValue = g.MeasureString(sValue, fontLogic, (int)(width - 2 * fPadding), stringFormatLogic);
                iVPosition += (int)sizeValue.Height;
                g.Dispose();
            }

            if (slotMinMidMax == SlotSizeMinMidMax.mid)
                return iVPosition + 6;

            Font fontParam = new Font(Font.FontFamily, 9f, FontStyle.Regular);

            // List Params
            for (int i = 1; i < 5; i++)
                iVPosition += strategy.Slot[slot].IndParam.ListParam[i].Enabled ? fontParam.Height : 0;

            // NumericParam
            foreach (NumericParam nump in strategy.Slot[slot].IndParam.NumParam)
                iVPosition += nump.Enabled ? fontParam.Height : 0;

            // CheckParam
            foreach (CheckParam checkp in strategy.Slot[slot].IndParam.CheckParam)
                iVPosition += checkp.Enabled ? fontParam.Height : 0;

            iVPosition += 10;

            return iVPosition;
        }

        /// <summary>
        /// Calculates the height of the Averaging Panel
        /// </summary>
        int PnlPropertiesCalculateHeight(int width)
        {
            width -= 2; // the width of the border

            Font fontCaption = new Font(Font.FontFamily, 9f);
            int  iVPosition  = (int)Math.Max(fontCaption.Height, 18) + 3;

            Font fontAveraging = new Font(Font.FontFamily, 9f);

            if (slotMinMidMax == SlotSizeMinMidMax.min)
                iVPosition += fontAveraging.Height;
            else
                iVPosition += 5 * fontAveraging.Height + 5;

            return iVPosition + 8;
        }

        /// <summary>
        /// Calculates the total height of the Panel Slot
        /// </summary>
        int PnlSlotCalculateTotalHeight(int width)
        {
            int iTotalHeight = 0;

            for (int iSlot = 0; iSlot < slots; iSlot++)
                iTotalHeight += space + PnlSlotCalculateHeight(iSlot, width);

            if (bShowAddSlotButtons)
                iTotalHeight += 2 * btnAddCloseFilter.Height + space;

            iTotalHeight += space + PnlPropertiesCalculateHeight(width);

            return iTotalHeight;
        }

        /// <summary>
        /// Panel Slot Paint
        /// </summary>
        void PnlSlot_Paint(object sender, PaintEventArgs e)
        {
            Panel     pnl      = (Panel)sender;
            Graphics  g        = e.Graphics;
            int       iSlot    = (int)pnl.Tag;
            int       iWidth   = pnl.ClientSize.Width;
            SlotTypes slotType = strategy.GetSlotType(iSlot);

            Color colorBackground             = LayoutColors.ColorSlotBackground;
            Color colorCaptionText            = LayoutColors.ColorSlotCaptionText;
            Color colorCaptionBackOpen        = LayoutColors.ColorSlotCaptionBackOpen;
            Color colorCaptionBackOpenFilter  = LayoutColors.ColorSlotCaptionBackOpenFilter;
            Color colorCaptionBackClose       = LayoutColors.ColorSlotCaptionBackClose;
            Color colorCaptionBackCloseFilter = LayoutColors.ColorSlotCaptionBackCloseFilter;
            Color colorIndicatorNameText      = LayoutColors.ColorSlotIndicatorText;
            Color colorLogicText              = LayoutColors.ColorSlotLogicText;
            Color colorParamText              = LayoutColors.ColorSlotParamText;
            Color colorValueText              = LayoutColors.ColorSlotValueText;
            Color colorDash                   = LayoutColors.ColorSlotDash;

            // Caption
            string stringCaptionText = string.Empty;
            Color  colorCaptionBack  = LayoutColors.ColorSignalRed;

            switch (slotType)
            {
                case SlotTypes.Open:
                    stringCaptionText = Language.T("Opening Point of the Position");
                    colorCaptionBack = colorCaptionBackOpen;
                    break;
                case SlotTypes.OpenFilter:
                    stringCaptionText = Language.T("Opening Logic Condition");
                    colorCaptionBack = colorCaptionBackOpenFilter;
                    break;
                case SlotTypes.Close:
                    stringCaptionText = Language.T("Closing Point of the Position");
                    colorCaptionBack = colorCaptionBackClose;
                    break;
                case SlotTypes.CloseFilter:
                    stringCaptionText = Language.T("Closing Logic Condition");
                    colorCaptionBack = colorCaptionBackCloseFilter;
                    break;
                default:
                    break;
            }

            Pen penBorder = new Pen(Data.ColorChanage(colorCaptionBack, -LayoutColors.DepthCaption), border);

            Font  fontCaptionText  = new Font(Font.FontFamily, 9);
            float fCaptionHeight   = (float)Math.Max(fontCaptionText.Height, 18);
            float fCaptionWidth    = iWidth;
            Brush brushCaptionText = new SolidBrush(colorCaptionText);
            StringFormat stringFormatCaption  = new StringFormat();
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            stringFormatCaption.Alignment     = StringAlignment.Center;

            RectangleF rectfCaption = new RectangleF(0, 0, fCaptionWidth, fCaptionHeight);
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);

            if (bShowRemoveSlotButtons && iSlot != strategy.OpenSlot && iSlot != strategy.CloseSlot)
            {
                int iButtonDimentions = (int)fCaptionHeight - 2;
                int iButtonX = iWidth - iButtonDimentions - 1;
                abtnRemoveSlot[iSlot].Size     = new Size(iButtonDimentions, iButtonDimentions);
                abtnRemoveSlot[iSlot].Location = new Point(iButtonX, 1);

                float  fCaptionTextWidth = g.MeasureString(stringCaptionText, fontCaptionText).Width;
                float  fCaptionTextX     = (float)Math.Max((fCaptionWidth - fCaptionTextWidth) / 2f, 0);
                PointF pfCaptionText     = new PointF(fCaptionTextX, 0);
                SizeF  sfCaptionText     = new SizeF(iButtonX - fCaptionTextX, fCaptionHeight);
                rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);
                stringFormatCaption.Alignment = StringAlignment.Near;
            }
            g.DrawString(stringCaptionText, fontCaptionText, brushCaptionText, rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.Height);
            g.DrawLine(penBorder, pnl.Width - border + 1, fCaptionHeight, pnl.Width - border + 1, pnl.Height);
            g.DrawLine(penBorder, 0, pnl.Height - border + 1, pnl.Width, pnl.Height - border + 1);

            // Paints the panel
            RectangleF rectfPanel = new RectangleF(border, fCaptionHeight, pnl.Width - 2 * border, pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectfPanel, colorBackground, LayoutColors.DepthControl);

            int iVPosition = (int)fCaptionHeight + 3;

            // Indicator name
            StringFormat stringFormatIndicatorName  = new StringFormat();
            stringFormatIndicatorName.Alignment     = StringAlignment.Center;
            stringFormatIndicatorName.LineAlignment = StringAlignment.Center;
            stringFormatIndicatorName.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatIndicatorName.FormatFlags   = StringFormatFlags.NoWrap;
            string     sIndicatorName = strategy.Slot[iSlot].IndicatorName;
            Font       fontIndicator  = new Font(Font.FontFamily, 11f, FontStyle.Regular);
            Brush      brushIndName   = new SolidBrush(colorIndicatorNameText);
            float      fIndNameHeight = fontIndicator.Height;
            float  fGroupWidth    = 0;
            if (Configs.UseLogicalGroups && (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter))
            {
                string sLogicalGroup = "[" + strategy.Slot[iSlot].LogicalGroup + "]";
                fGroupWidth = g.MeasureString(sLogicalGroup, fontIndicator).Width;
                RectangleF rectGroup = new RectangleF(0, iVPosition, fGroupWidth, fIndNameHeight);
                g.DrawString(sLogicalGroup, fontIndicator, brushIndName, rectGroup, stringFormatIndicatorName);
            }
            stringFormatIndicatorName.Trimming = StringTrimming.EllipsisCharacter;
            float  fIndicatorWidth = g.MeasureString(sIndicatorName, fontIndicator).Width;

            RectangleF rectIndName;
            if (iWidth >= 2 * fGroupWidth + fIndicatorWidth)
                rectIndName = new RectangleF(0, iVPosition, iWidth, fIndNameHeight);
            else
                rectIndName = new RectangleF(fGroupWidth, iVPosition, iWidth - fGroupWidth, fIndNameHeight);

            g.DrawString(sIndicatorName, fontIndicator, brushIndName, rectIndName, stringFormatIndicatorName);
            iVPosition += (int)fIndNameHeight;

            if (slotMinMidMax == SlotSizeMinMidMax.min)
                return;

            // Logic
            StringFormat stringFormatLogic  = new StringFormat();
            stringFormatLogic.Alignment     = StringAlignment.Center;
            stringFormatLogic.LineAlignment = StringAlignment.Center;
            stringFormatLogic.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatLogic.FormatFlags   = StringFormatFlags.NoClip;

            float fPadding = space;

            if (strategy.Slot[iSlot].IndParam.ListParam[0].Enabled)
            {
                string     sValue     = strategy.Slot[iSlot].IndParam.ListParam[0].Text;
                Font       fontLogic  = new Font(Font.FontFamily, 10.5f, FontStyle.Regular);
                SizeF      sizeValue  = g.MeasureString(sValue, fontLogic, (int)(iWidth - 2 * fPadding), stringFormatLogic);
                RectangleF rectValue  = new RectangleF(fPadding, iVPosition, iWidth - 2 * fPadding, sizeValue.Height);
                Brush      brushLogic = new SolidBrush(colorLogicText);

                g.DrawString(sValue, fontLogic, brushLogic, rectValue, stringFormatLogic);
                iVPosition += (int)sizeValue.Height;
            }

            if (slotMinMidMax == SlotSizeMinMidMax.mid)
                return;

            // Parameters
            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming     = StringTrimming.EllipsisCharacter;
            stringFormat.FormatFlags  = StringFormatFlags.NoWrap;

            Font  fontParam  = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Font  fontValue  = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Brush brushParam = new SolidBrush(colorParamText);
            Brush brushValue = new SolidBrush(colorValueText);
            Pen   penDash    = new Pen(colorDash);

            // Find Maximum width of the strings
            float fMaxParamWidth = 0;
            float fMaxValueWidth = 0;

            for (int i = 1; i < 5; i++)
            {
                if (!strategy.Slot[iSlot].IndParam.ListParam[i].Enabled)
                    continue;

                string sParam = strategy.Slot[iSlot].IndParam.ListParam[i].Caption;
                string sValue = strategy.Slot[iSlot].IndParam.ListParam[i].Text;
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            foreach (NumericParam numericParam in strategy.Slot[iSlot].IndParam.NumParam)
            {
                if (!numericParam.Enabled) continue;

                string sParam = numericParam.Caption;
                string sValue = numericParam.ValueToString;
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            foreach (CheckParam checkParam in strategy.Slot[iSlot].IndParam.CheckParam)
            {
                if (!checkParam.Enabled) continue;

                string sParam = checkParam.Caption;
                string sValue = checkParam.Checked ? "Yes" : "No";
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            // Padding Param Padding Dash Padding Value Padding 
            float fDashWidth = 5;
            float fNecessaryWidth = 4 * fPadding + fMaxParamWidth + fMaxValueWidth + fDashWidth;

            if (iWidth > fNecessaryWidth)
            {   // 2*Padding Param Padding Dash Padding Value 2*Padding 
                fPadding = (float)Math.Max((pnl.ClientSize.Width - fMaxParamWidth - fMaxValueWidth - fDashWidth) / 6, fPadding);
            }
            else
            {
                fPadding = 2;
            }

            float fTabParam = 2 * fPadding;
            float fTabDash  = fTabParam + fMaxParamWidth + fPadding;
            float fTabValue = fTabDash + fDashWidth + fPadding;

            // List Params
            for (int i = 1; i < 5; i++)
            {
                if (!strategy.Slot[iSlot].IndParam.ListParam[i].Enabled)
                    continue;

                string sParam = strategy.Slot[iSlot].IndParam.ListParam[i].Caption;
                string sValue = strategy.Slot[iSlot].IndParam.ListParam[i].Text;
                PointF pointParam = new PointF(fTabParam, iVPosition);
                PointF pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                PointF pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                PointF pointValue = new PointF(fTabValue, iVPosition);
                SizeF  sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                RectangleF rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }

            // Num Params
            foreach (NumericParam numericParam in strategy.Slot[iSlot].IndParam.NumParam)
            {
                if (!numericParam.Enabled) continue;

                string sParam = numericParam.Caption;
                string sValue = numericParam.ValueToString;
                PointF pointParam = new PointF(fTabParam, iVPosition);
                PointF pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                PointF pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                PointF pointValue = new PointF(fTabValue, iVPosition);
                SizeF  sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                RectangleF rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }

            // Check Params
            foreach (CheckParam checkParam in strategy.Slot[iSlot].IndParam.CheckParam)
            {
                if (!checkParam.Enabled) continue;

                string sParam = checkParam.Caption;
                string sValue = checkParam.Checked ? "Yes" : "No";
                PointF pointParam = new PointF(fTabParam, iVPosition);
                PointF pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                PointF pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                PointF pointValue = new PointF(fTabValue, iVPosition);
                SizeF  sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                RectangleF rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }

            return;
        }

        /// <summary>
        /// Panel properties Paint
        /// </summary>
        void PnlProperties_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            int iWidth = pnl.ClientSize.Width;

            Color colorCaptionBack = LayoutColors.ColorSlotCaptionBackAveraging;
            Color colorCaptionText = LayoutColors.ColorSlotCaptionText;
            Color colorBackground  = LayoutColors.ColorSlotBackground;
            Color colorLogicText   = LayoutColors.ColorSlotLogicText;
            Color colorDash        = LayoutColors.ColorSlotDash;

            // Caption
            string stringCaptionText = Language.T("Strategy Properties");
            Font   fontCaptionText   = new Font(Font.FontFamily, 9);
            float  fCaptionHeight    = (float)Math.Max(fontCaptionText.Height, 18);
            float  fCaptionWidth     = iWidth;
            Brush  brushCaptionText  = new SolidBrush(colorCaptionText);
            StringFormat stringFormatCaption  = new StringFormat();
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            stringFormatCaption.Alignment     = StringAlignment.Center;

            RectangleF rectfCaption = new RectangleF(0, 0, fCaptionWidth, fCaptionHeight);
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(stringCaptionText, fontCaptionText, brushCaptionText, rectfCaption, stringFormatCaption);

            // Border
            Pen penBorder = new Pen(Data.ColorChanage(colorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.Height);
            g.DrawLine(penBorder, pnl.Width - border + 1, fCaptionHeight, pnl.Width - border + 1, pnl.Height);
            g.DrawLine(penBorder, 0, pnl.Height - border + 1, pnl.Width, pnl.Height - border + 1);

            // Paint the panel's background
            RectangleF rectfPanel = new RectangleF(border, fCaptionHeight, pnl.Width - 2 * border, pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectfPanel, colorBackground, LayoutColors.DepthControl);
            
            int iVPosition = (int)fCaptionHeight + 2;

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming     = StringTrimming.EllipsisCharacter;
            stringFormat.FormatFlags  = StringFormatFlags.NoWrap;

            Font  fontParam  = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Font  fontValue  = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Brush brushParam = new SolidBrush(colorLogicText);
            Brush brushValue = new SolidBrush(colorLogicText);
            Pen   penDash    = new Pen(colorDash);

            string strPermaSL = strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + strategy.PermanentSL.ToString() : Language.T("None");
            string strPermaTP = strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + strategy.PermanentTP.ToString() : Language.T("None");
            string strBreakEven = strategy.UseBreakEven   ? strategy.BreakEven.ToString()   : Language.T("None");

            if (slotMinMidMax == SlotSizeMinMidMax.min)
            {
                string sParam = Language.T(strategy.SameSignalAction.ToString()) + "; " +
                                Language.T(strategy.OppSignalAction.ToString()) + "; " +
                                "SL-" + strPermaSL + "; " +
                                "TP-" + strPermaTP + "; " +
                                "BE-" + strBreakEven;

                SizeF sizeParam      = g.MeasureString(sParam, fontParam);
                float fMaxParamWidth = sizeParam.Width;

                // Padding Param Padding Dash Padding Value Padding 
                float fPadding        = space;
                float fNecessaryWidth = 2 * fPadding + fMaxParamWidth;

                if (iWidth > fNecessaryWidth)
                {   // Padding Param Padding
                    fPadding = (float)Math.Max((pnl.ClientSize.Width - fMaxParamWidth) / 2, fPadding);
                }
                else
                {
                    fPadding = 2;
                }

                float fTabParam = fPadding;

                PointF pointParam = new PointF(fTabParam, iVPosition);
                g.DrawString(sParam, fontParam, brushParam, pointParam);

            }
            else
            {
                // Find Maximum width of the strings
                string [] asParams = new string [5]
                {
                    Language.T("Same direction signal"),
                    Language.T("Opposite direction signal"),
                    Language.T("Permanent Stop Loss"),
                    Language.T("Permanent Take Profit"),
                    Language.T("Break Even")
                };

                string [] asValues = new string [5] {
                    Language.T(strategy.SameSignalAction.ToString()),
                    Language.T(strategy.OppSignalAction.ToString()),
                    strPermaSL,
                    strPermaTP,
                    strBreakEven
                };

                float fMaxParamWidth = 0;
                foreach (string param in asParams)
                {
                    if (g.MeasureString(param, fontParam).Width > fMaxParamWidth)
                        fMaxParamWidth = g.MeasureString(param, fontParam).Width;
                }

                float fMaxValueWidth = 0;
                foreach (string value in asValues)
                {
                    if (g.MeasureString(value, fontParam).Width > fMaxValueWidth)
                        fMaxValueWidth = g.MeasureString(value, fontParam).Width;
                }

                // Padding Param Padding Dash Padding Value Padding 
                float fPadding   = space;
                float fDashWidth = 5;
                float fNecessaryWidth = 4 * fPadding + fMaxParamWidth + fMaxValueWidth + fDashWidth;

                if (iWidth > fNecessaryWidth)
                {   // 2*Padding Param Padding Dash Padding Value 2*Padding 
                    fPadding = (float)Math.Max((pnl.ClientSize.Width - fMaxParamWidth - fMaxValueWidth - fDashWidth) / 6, fPadding);
                }
                else
                {
                    fPadding = 2;
                }

                float fTabParam = 2 * fPadding;
                float fTabDash  = fTabParam + fMaxParamWidth + fPadding;
                float fTabValue = fTabDash  + fDashWidth     + fPadding;

                // Same direction
                string sParam = Language.T("Same direction signal");
                string sValue = Language.T(strategy.SameSignalAction.ToString());
                PointF pointParam = new PointF(fTabParam, iVPosition);
                PointF pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                PointF pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                PointF pointValue = new PointF(fTabValue, iVPosition);
                SizeF  sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                RectangleF rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Opposite direction
                sParam = Language.T("Opposite direction signal");
                sValue = Language.T(strategy.OppSignalAction.ToString());
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Permanent Stop Loss
                sParam = Language.T("Permanent Stop Loss");
                sValue = strPermaSL;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Permanent Take Profit
                sParam = Language.T("Permanent Take Profit");
                sValue = strPermaTP;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;

                // Break Even
                sParam = Language.T("Break Even");
                sValue = strBreakEven;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height / 2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height / 2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;
            }

            return;
        }

        /// <summary>
        /// Shows Closing Filter Help.
        /// </summary>
        void BtnClosingFilterHelp_Click(object sender, EventArgs e)
        {
            string text = "You can use Closing Logic Conditions only if the Closing Point of the Position slot contains one of the following indicators:";
            string inicators = Environment.NewLine;
            foreach (string indicator in Indicator_Store.ClosingIndicatorsWithClosingFilters)
                inicators += " - " + indicator + Environment.NewLine;
            System.Windows.Forms.MessageBox.Show(Language.T(text) + inicators, Language.T("Closing Logic Condition"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            flowLayoutStrategy.SuspendLayout();
            ArrangeStrategyControls();
            flowLayoutStrategy.ResumeLayout();
        } 
    }
}
