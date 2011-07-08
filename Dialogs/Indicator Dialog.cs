// Indicator_Dialog Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    //    ###################################################################################
    //    # +-----------+ |--------------------------- Slot Type -------------------(!)(i)| #
    //    # |           |                                                                   #
    //    # |           | |------------------------- lblIndicator ------------------------| #
    //    # |           |                                                                   #
    //    # |           | |----------------- aLblList[0] ----------------------| | Group  | #
    //    # |           | |----------------- aCbxList[0] ----------------------| |cbxGroup| #
    //    # |           |                                                                   #
    //    # |           | |--------- aLblList[1] --------| |--------- aLblList[2] --------| #
    //    # |           | |--------- aCbxList[1] --------| |--------- aCbxList[2] --------| #
    //    # |           |                                                                   #
    //    # |           | |--------- aLblList[3] --------| |--------- aLblList[4] --------| #
    //    # |           | |--------- aCbxList[3] --------| |--------- aCbxList[4] --------| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[0]||aNudNumeric[0]| |aLblNumeric[1]||aNudNumeric[1]| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[2]||aNudNumeric[2]| |aLblNumeric[3]||aNudNumeric[3]| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[4]||aNudNumeric[4]| |aLblNumeric[5]||aNudNumeric[5]| #
    //    # |           |                                                                   #
    //    # |           | |v|------- aChbCheck[0] -------| |v|------- aChbCheck[1] -------| #
    //    # |           |                                                                   #
    //    # |           |                                                                   #
    //    # +-----------+ [   Accept   ]    [  Default  ]     [   Help   ]   [  Cancel  ]   #
    //    #                                                                                 #
    //    ###################################################################################

    /// <summary>
    /// Form dialog contains controls for adjusting the indicator's parameters.
    /// </summary>
    public class Indicator_Dialog : Form
    {

        Fancy_Panel pnlTreeViewBase;
        TreeView    trvIndicators;
        Fancy_Panel pnlParameters;

        Label       lblIndicatorInfo;
        Label       lblIndicatorWarning;
        Label       lblIndicator;
        Label       lblGroup;
        ComboBox    cbxGroup;
        Label[]     aLblList;
        ComboBox[]  aCbxList;
        Label[]     aLblNumeric;
        NUD[]       aNudNumeric;
        CheckBox[]  aChbCheck;
        Button      btnAccept;
        Button      btnDefault;
        Button      btnHelp;
        Button      btnCancel;
        ToolTip     toolTip = new ToolTip();

        int         slot;
        SlotTypes   slotType;
        string      slotTitle;
        bool        isSlotDefined;
        string      indicatorName;

        string description;
        string warningMessage = "";

        OppositeDirSignalAction oppSignalBehaviour;
        bool oppSignalSet = false;
        List<IndicatorSlot> closingConditions = new List<IndicatorSlot>();
        bool closingSlotsRemoved = false;

        bool isPaintAllowed = false;
        int  border = 2;

        /// <summary>
        /// Gets or sets the caption of a ComboBox control.
        /// </summary>
        Label[] ListLabel { get { return aLblList; } set { aLblList = value; } }

        /// <summary>
        /// Gets or sets the parameters of a ComboBox control.
        /// </summary>
        ComboBox[] ListParam { get { return aCbxList; } set { aCbxList = value; } }

        /// <summary>
        /// Gets or sets the caption of a NumericUpDown control.
        /// </summary>
        Label[] NumLabel { get { return aLblNumeric; } set { aLblNumeric = value; } }
 
        /// <summary>
        /// Gets or sets the parameters of a NumericUpDown control.
        /// </summary>
        NUD[] NumParam { get { return aNudNumeric; } set { aNudNumeric = value; } }
 
        /// <summary>
        /// Gets or sets the parameters of a CheckBox control.
        /// </summary>
        CheckBox[] CheckParam { get { return aChbCheck; } set { aChbCheck = value; } }

// ---------------------------------------------------------------------------

        /// <summary>
        /// Constructor
        /// </summary>
        public Indicator_Dialog(int slotNumb, SlotTypes slotType, bool isSlotDefined)
        {
            this.slot     = slotNumb;
            this.slotType = slotType;
            this.isSlotDefined = isSlotDefined;

            if (slotType == SlotTypes.Open)
            {
                slotTitle       = Language.T("Opening Point of the Position");
                pnlParameters   = new Fancy_Panel(slotTitle, LayoutColors.ColorSlotCaptionBackOpen, LayoutColors.ColorSlotCaptionText);
                pnlTreeViewBase = new Fancy_Panel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackOpen, LayoutColors.ColorSlotCaptionText);
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                slotTitle       = Language.T("Opening Logic Condition");
                pnlParameters   = new Fancy_Panel(slotTitle, LayoutColors.ColorSlotCaptionBackOpenFilter, LayoutColors.ColorSlotCaptionText);
                pnlTreeViewBase = new Fancy_Panel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackOpenFilter, LayoutColors.ColorSlotCaptionText);
            }
            else if (slotType == SlotTypes.Close)
            {
                slotTitle       = Language.T("Closing Point of the Position");
                pnlParameters   = new Fancy_Panel(slotTitle, LayoutColors.ColorSlotCaptionBackClose, LayoutColors.ColorSlotCaptionText);
                pnlTreeViewBase = new Fancy_Panel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackClose, LayoutColors.ColorSlotCaptionText);
            }
            else
            {
                slotTitle       = Language.T("Closing Logic Condition");
                pnlParameters   = new Fancy_Panel(slotTitle, LayoutColors.ColorSlotCaptionBackCloseFilter, LayoutColors.ColorSlotCaptionText);
                pnlTreeViewBase = new Fancy_Panel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackCloseFilter, LayoutColors.ColorSlotCaptionText);
            }

            trvIndicators = new TreeView();
            btnAccept     = new Button();
            btnHelp       = new Button();
            btnDefault    = new Button();
            btnCancel     = new Button();

            lblIndicatorInfo    = new Label();
            lblIndicatorWarning = new Label();
            lblIndicator        = new Label();
            lblGroup            = new Label();
            cbxGroup            = new ComboBox();
            aLblList            = new Label[5];
            aCbxList            = new ComboBox[5];
            aLblNumeric         = new Label[6];
            aNudNumeric         = new NUD[6];
            aChbCheck           = new CheckBox[2];

            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon            = Data.Icon;
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            AcceptButton    = btnAccept;
            CancelButton    = btnCancel;
            Text = Language.T("Logic and Parameters of the Indicators");

            // Panel TreeViewBase
            pnlTreeViewBase.Parent  = this;
            pnlTreeViewBase.Padding = new Padding(border, (int)pnlTreeViewBase.CaptionHeight, border, border);

            // TreeView trvIndicators
            trvIndicators.Parent      = pnlTreeViewBase;
            trvIndicators.Dock        = DockStyle.Fill;
            trvIndicators.BackColor   = LayoutColors.ColorControlBack;
            trvIndicators.ForeColor   = LayoutColors.ColorControlText;
            trvIndicators.BorderStyle = BorderStyle.None;
            trvIndicators.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(TrvIndicators_NodeMouseDoubleClick);
            trvIndicators.KeyPress   += new KeyPressEventHandler(TrvIndicators_KeyPress);

            pnlParameters.Parent = this;

            // lblIndicatorInfo
            lblIndicatorInfo.Parent          = pnlParameters;
            lblIndicatorInfo.Size            = new Size(16, 16);
            lblIndicatorInfo.BackColor       = Color.Transparent;
            lblIndicatorInfo.BackgroundImage = Properties.Resources.information;
            lblIndicatorInfo.Click          += new EventHandler(LblIndicatorInfo_Click);
            lblIndicatorInfo.MouseEnter     += new EventHandler(Label_MouseEnter);
            lblIndicatorInfo.MouseLeave     += new EventHandler(Label_MouseLeave);

            // LAbel Indicator Warning
            lblIndicatorWarning.Parent          = pnlParameters;
            lblIndicatorWarning.Size            = new Size(16, 16);
            lblIndicatorWarning.BackColor       = Color.Transparent;
            lblIndicatorWarning.BackgroundImage = Properties.Resources.warning;
            lblIndicatorWarning.Visible         = false;
            lblIndicatorWarning.Click          += new EventHandler(LblIndicatorWarning_Click);
            lblIndicatorWarning.MouseEnter     += new EventHandler(Label_MouseEnter);
            lblIndicatorWarning.MouseLeave     += new EventHandler(Label_MouseLeave);

            // Label Indicator
            lblIndicator.Parent    = pnlParameters;
            lblIndicator.TextAlign = ContentAlignment.MiddleCenter;
            lblIndicator.Font      = new Font(Font.FontFamily, 14, FontStyle.Bold);
            lblIndicator.ForeColor = LayoutColors.ColorSlotIndicatorText;
            lblIndicator.BackColor = Color.Transparent;

            // Label aLblList[0]
            aLblList[0] = new Label();
            aLblList[0].Parent    = pnlParameters;
            aLblList[0].TextAlign = ContentAlignment.BottomCenter;
            aLblList[0].ForeColor = LayoutColors.ColorControlText;
            aLblList[0].BackColor = Color.Transparent;

            // ComboBox aCbxList[0]
            aCbxList[0] = new ComboBox();
            aCbxList[0].Parent        = pnlParameters;
            aCbxList[0].DropDownStyle = ComboBoxStyle.DropDownList;
            aCbxList[0].SelectedIndexChanged  += new EventHandler(Param_Changed);

            // Logical Group
            lblGroup = new Label();
            lblGroup.Parent    = pnlParameters;
            lblGroup.TextAlign = ContentAlignment.BottomCenter;
            lblGroup.ForeColor = LayoutColors.ColorControlText;
            lblGroup.BackColor = Color.Transparent;
            lblGroup.Text      = Language.T("Group");

            cbxGroup = new ComboBox();
            cbxGroup.Parent  = pnlParameters;
            if(slotType == SlotTypes.OpenFilter)
                cbxGroup.Items.AddRange(new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "All"});
            if(slotType == SlotTypes.CloseFilter)
                cbxGroup.Items.AddRange(new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "all"});
            cbxGroup.SelectedIndexChanged += new EventHandler(Group_Changed);
            cbxGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            toolTip.SetToolTip(cbxGroup, Language.T("The logical group of the slot."));

            // ListParams
            for (int i = 1; i < 5; i++)
            {
                aLblList[i] = new Label();
                aLblList[i].Parent    = pnlParameters;
                aLblList[i].TextAlign = ContentAlignment.BottomCenter;
                aLblList[i].ForeColor = LayoutColors.ColorControlText;
                aLblList[i].BackColor = Color.Transparent;

                aCbxList[i] = new ComboBox();
                aCbxList[i].Parent  = pnlParameters;
                aCbxList[i].Enabled = false;
                aCbxList[i].SelectedIndexChanged += new EventHandler(Param_Changed);
                aCbxList[i].DropDownStyle = ComboBoxStyle.DropDownList;
            }

            // NumParams
            for (int i = 0; i < 6; i++)
            {
                aLblNumeric[i] = new Label();
                aLblNumeric[i].Parent    = pnlParameters;
                aLblNumeric[i].TextAlign = ContentAlignment.MiddleRight;
                aLblNumeric[i].ForeColor = LayoutColors.ColorControlText;
                aLblNumeric[i].BackColor = Color.Transparent;

                aNudNumeric[i] = new NUD();
                aNudNumeric[i].Parent        = pnlParameters;
                aNudNumeric[i].TextAlign     = HorizontalAlignment.Center;
                aNudNumeric[i].Enabled       = false;
                aNudNumeric[i].ValueChanged += new EventHandler(Param_Changed);
            }

            // CheckParams
            for (int i = 0; i < 2; i++)
            {
                aChbCheck[i] = new CheckBox();
                aChbCheck[i].Parent          = pnlParameters;
                aChbCheck[i].CheckAlign      = ContentAlignment.MiddleLeft;
                aChbCheck[i].TextAlign       = ContentAlignment.MiddleLeft;
                aChbCheck[i].CheckedChanged += new EventHandler(Param_Changed);
                aChbCheck[i].ForeColor       = LayoutColors.ColorControlText;
                aChbCheck[i].BackColor       = Color.Transparent;
                aChbCheck[i].Enabled         = false;
            }

            // Button Accept
            btnAccept.Parent       = this;
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.Click       += new EventHandler(BtnOk_Click);
            btnAccept.UseVisualStyleBackColor = true;

            // Button Default
            btnDefault.Parent      = this;
            btnDefault.Text        = Language.T("Default");
            btnDefault.Click      += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

            // Button Help
            btnHelp.Parent         = this;
            btnHelp.Text           = Language.T("Help");
            btnHelp.Click         += new EventHandler(BtnHelp_Click);
            btnHelp.UseVisualStyleBackColor = true;

            // Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            SetTreeViewIndicators();

            // ComboBoxindicator index selection.
            if (isSlotDefined)
            {
                TreeNode [] atrn = trvIndicators.Nodes.Find(Data.Strategy.Slot[slot].IndParam.IndicatorName, true);
                trvIndicators.SelectedNode = atrn[0];
                UpdateFromIndicatorParam(Data.Strategy.Slot[slot].IndParam);
                SetLogicalGroup();
                CalculateIndicator();
            }
            else
            {
                string sDefaultIndicator;
                if (slotType == SlotTypes.Open)
                    sDefaultIndicator = "Bar Opening";
                else if (slotType == SlotTypes.OpenFilter)
                    sDefaultIndicator = "Accelerator Oscillator";
                else if (slotType == SlotTypes.Close)
                    sDefaultIndicator = "Bar Closing";
                else
                    sDefaultIndicator = "Accelerator Oscillator";

                TreeNode[] atrn = trvIndicators.Nodes.Find(sDefaultIndicator, true);
                trvIndicators.SelectedNode = atrn[0];
                TrvIndicatorsLoadIndicator(atrn[0]);
            }

            oppSignalBehaviour = Data.Strategy.OppSignalAction;

            if (slotType == SlotTypes.Close && Data.Strategy.CloseFilters > 0)
                for (int iSlot = Data.Strategy.CloseSlot + 1; iSlot < Data.Strategy.Slots; iSlot++)
                    closingConditions.Add(Data.Strategy.Slot[iSlot].Clone());

            return;
        }
                
        /// <summary>
        /// Paints the base panel
        /// </summary>
        void PanelBase_Paint(object sender, PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ((Panel)sender).ClientRectangle, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width  = 670;
            Height = 570;

            MinimumSize = new Size(Width, Height);

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight  = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth   = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace  = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace   = (int)(Data.HorizontalDLU * 3);
            int space         = btnHrzSpace;
            int textHeight    = Font.Height;
            int controlHeight = Font.Height + 4;

            int rightColumnWight = 4 * buttonWidth + 3 * btnHrzSpace;
            int pnlTreeViewWidth = ClientSize.Width - rightColumnWight - 3 * space;
            
            // Panel pnlTreeViewBase
            pnlTreeViewBase.Size     = new Size(pnlTreeViewWidth, ClientSize.Height - 2 * space);
            pnlTreeViewBase.Location = new Point(space, space);

            int rightColumnLeft = pnlTreeViewWidth + 2 * space;
            int nudWidth  = 65;

            // pnlParameterBase
            pnlParameters.Width    = rightColumnWight;
            pnlParameters.Location = new Point(rightColumnLeft, space);

            //Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(rightColumnLeft, ClientSize.Height - btnVertSpace - buttonHeight);

            //Button Default
            btnDefault.Size     = btnAccept.Size;
            btnDefault.Location = new Point(btnAccept.Right + btnHrzSpace, btnAccept.Top);

            //Button Help
            btnHelp.Size     = btnAccept.Size;
            btnHelp.Location = new Point(btnDefault.Right + btnHrzSpace, btnAccept.Top);

            //Button Cancel
            btnCancel.Size     = btnAccept.Size;
            btnCancel.Location = new Point(btnHelp.Right + btnHrzSpace, btnAccept.Top);

            // lblIndicatorInfo
            lblIndicatorInfo.Location = new Point(pnlParameters.Width - lblIndicatorInfo.Width - 1, 1);

            // lblIndicatorWarning
            lblIndicatorWarning.Location = new Point(lblIndicatorInfo.Left - lblIndicatorWarning.Width - 1, 1);

            // lblIndicator
            lblIndicator.Size     = new Size(pnlParameters.ClientSize.Width - 2 * border - 2 * space, 3 * lblIndicator.Font.Height / 2);
            lblIndicator.Location = new Point(border + space, (int)pnlParameters.CaptionHeight + space);

            // Logical Group
            Graphics g = CreateGraphics();
            lblGroup.Width = (int)g.MeasureString(Language.T("Group"), Font).Width + 10;
            g.Dispose();
            lblGroup.Height   = textHeight;
            lblGroup.Location = new Point(pnlParameters.ClientSize.Width - border - space - lblGroup.Width, lblIndicator.Bottom + space);

            // ComboBox Groups
            cbxGroup.Size     = new Size(lblGroup.Width, controlHeight);
            cbxGroup.Location = new Point(lblGroup.Left, aLblList[0].Bottom + space);

            int rightShift = Configs.UseLogicalGroups && (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter) ? (lblGroup.Width + space) : 0;

            // Label Logic
            aLblList[0].Size     = new Size(pnlParameters.ClientSize.Width - 2 * border - 2 * space - rightShift, textHeight);
            aLblList[0].Location = new Point(border + space, lblIndicator.Bottom + space);

            // ComboBox Logic
            aCbxList[0].Size     = new Size(pnlParameters.ClientSize.Width - 2 * border - 2 * space - rightShift, controlHeight);
            aCbxList[0].Location = new Point(border + space, aLblList[0].Bottom + space);

            // ListParams
            for (int m = 0; m < 2; m++)
            for (int n = 0; n < 2; n++)
            {
                int i = 2 * m + n + 1;
                int x = (aCbxList[1].Width + space) * n + space + border;
                int y = (textHeight + controlHeight + 3 * space) * m + aCbxList[0].Bottom + 2 * space;

                aLblList[i].Size = new Size((pnlParameters.ClientSize.Width - 3 * space - 2 * border) / 2, textHeight);
                aLblList[i].Location = new Point(x, y);

                aCbxList[i].Size = new Size((pnlParameters.ClientSize.Width - 3 * space - 2 * border) / 2, controlHeight);
                aCbxList[i].Location = new Point(x, y + textHeight + space);
            }

            // NumParams
            for (int m = 0; m < 3; m++)
            for (int n = 0; n < 2; n++)
            {
                int i = 2 * m + n;
                int lblWidth = (pnlParameters.ClientSize.Width - 5 * space - 2 * nudWidth - 2 * border) / 2;
                aLblNumeric[i].Size     = new Size(lblWidth, controlHeight);
                aLblNumeric[i].Location = new Point((lblWidth + nudWidth + 2 * space) * n + space + border, (controlHeight + 2 * space) * m + 2 * space + aCbxList[3].Bottom);

                aNudNumeric[i].Size     = new Size(nudWidth, controlHeight);
                aNudNumeric[i].Location = new Point(aLblNumeric[i].Right + space, aLblNumeric[i].Top);
            }

            // CheckParams
            for (int i = 0; i < 2; i++)
            {
                int chbWidth = (pnlParameters.ClientSize.Width - 3 * space - 2 * border) / 2;
                aChbCheck[i].Size     = new Size(chbWidth, controlHeight);
                aChbCheck[i].Location = new Point((space + chbWidth) * i + space + border, aNudNumeric[4].Bottom + space);
            }

            pnlParameters.ClientSize = new Size(pnlParameters.ClientSize.Width, aChbCheck[0].Bottom + space + border);

            this.ClientSize = new Size(ClientSize.Width, pnlParameters.Bottom + 2 * btnVertSpace + buttonHeight);

            return;
        }

        /// <summary>
        /// Sets the controls' parameters.
        /// </summary>
        void UpdateFromIndicatorParam(IndicatorParam ip)
        {
            indicatorName = ip.IndicatorName;
            lblIndicator.Text = indicatorName;

            isPaintAllowed = false;

            // List params
            for (int i = 0; i < 5; i++)
            {
                ListParam[i].Items.Clear();
                ListParam[i].Items.AddRange (ip.ListParam[i].ItemList);
                ListLabel[i].Text          = ip.ListParam[i].Caption;
                ListParam[i].SelectedIndex = ip.ListParam[i].Index;
                ListParam[i].Enabled       = ip.ListParam[i].Enabled;
                toolTip.SetToolTip(ListParam[i], ip.ListParam[i].ToolTip);
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                NumParam[i].BeginInit();
                NumLabel[i].Text          = ip.NumParam[i].Caption;
                NumParam[i].Minimum       = (decimal)ip.NumParam[i].Min;
                NumParam[i].Maximum       = (decimal)ip.NumParam[i].Max;
                NumParam[i].Value         = (decimal)ip.NumParam[i].Value;
                NumParam[i].DecimalPlaces = ip.NumParam[i].Point;
                NumParam[i].Increment     = (decimal)Math.Pow(10, -ip.NumParam[i].Point);
                NumParam[i].Enabled       = ip.NumParam[i].Enabled;
                NumParam[i].EndInit();
                toolTip.SetToolTip(NumParam[i], ip.NumParam[i].ToolTip + Environment.NewLine + "Minimum: " + NumParam[i].Minimum + " Maximum: " + NumParam[i].Maximum);
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                CheckParam[i].Text    = ip.CheckParam[i].Caption;
                CheckParam[i].Checked = ip.CheckParam[i].Checked;
                toolTip.SetToolTip(CheckParam[i], ip.CheckParam[i].ToolTip);

                if (Data.AutoUsePrvBarValue && ip.CheckParam[i].Caption == "Use previous bar value")
                    CheckParam[i].Enabled = false;
                else
                    CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            isPaintAllowed = true;

            return;
        }

        /// <summary>
        /// Sets the logical group of the slot.
        /// </summary>
        void SetLogicalGroup()
        {
            if (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter)
            {
                string group = Data.Strategy.Slot[slot].LogicalGroup;
                if (string.IsNullOrEmpty(group))
                    SetDefaultGroup();
                else
                {
                    if (group.ToLower() == "all")
                        cbxGroup.SelectedIndex = cbxGroup.Items.Count - 1;
                    else
                        cbxGroup.SelectedIndex = char.ConvertToUtf32(group.ToLower(), 0) - char.ConvertToUtf32("a", 0);
                }
            }

            return;
        }

        /// <summary>
        /// Sets the default logical group of the slot.
        /// </summary>
        void SetDefaultGroup()
        {
            if (slotType == SlotTypes.OpenFilter)
            {
                if (indicatorName == "Data Bars Filter" ||
                    indicatorName == "Date Filter"      ||
                    indicatorName == "Day of Month"     ||
                    indicatorName == "Enter Once"       ||
                    indicatorName == "Entry Time"       ||
                    indicatorName == "Long or Short"    ||
                    indicatorName == "Lot Limiter"      ||
                    indicatorName == "Random Filter")
                    cbxGroup.SelectedIndex = cbxGroup.Items.Count - 1; // "All" group.
                else
                    cbxGroup.SelectedIndex = 0;
            }

            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slot - Data.Strategy.CloseSlot - 1;
                cbxGroup.SelectedIndex = index;
            }

            return;
        }

        /// <summary>
        /// Sets the trvIndicators nodes
        /// </summary>
        void SetTreeViewIndicators()
        {
            TreeNode trnAll = new TreeNode();
            trnAll.Name = "trnAll";
            trnAll.Text = Language.T("All");
            trnAll.Tag  = false;

            TreeNode trnIndicators = new TreeNode();
            trnIndicators.Name = "trnIndicators";
            trnIndicators.Text = Language.T("Indicators");
            trnIndicators.Tag  = false;

            TreeNode trnAdditional = new TreeNode();
            trnAdditional.Name = "trnAdditional";
            trnAdditional.Text = Language.T("Additional");
            trnAdditional.Tag  = false;

            TreeNode trnOscillatorOfIndicators = new TreeNode();
            trnOscillatorOfIndicators.Name = "trnOscillatorOfIndicators";
            trnOscillatorOfIndicators.Text = Language.T("Oscillator of Indicators");
            trnOscillatorOfIndicators.Tag  = false;

            TreeNode trnIndicatorsMAOscillator = new TreeNode();
            trnIndicatorsMAOscillator.Name = "trnIndicatorMA";
            trnIndicatorsMAOscillator.Text = Language.T("Indicator's MA Oscillator");
            trnIndicatorsMAOscillator.Tag  = false;

            TreeNode trnDateTime = new TreeNode();
            trnDateTime.Name = "trnDateTime";
            trnDateTime.Text = Language.T("Date/Time Functions");
            trnDateTime.Tag  = false;

            TreeNode trnCustomIndicators = new TreeNode();
            trnCustomIndicators.Name = "trnCustomIndicators";
            trnCustomIndicators.Text = Language.T("Custom Indicators");
            trnCustomIndicators.Tag  = false;

            trvIndicators.Nodes.AddRange(new TreeNode[]
            { 
                trnAll, trnIndicators, trnAdditional, trnOscillatorOfIndicators,
                trnIndicatorsMAOscillator, trnDateTime, trnCustomIndicators
            });

            foreach (string indicatorName in Indicator_Store.GetIndicatorNames(slotType))
            {
                // Checks the indicator name in the list of forbidden indicators.
                bool toContinue = false;
                foreach (string forbiden in Data.IndicatorsForBacktestOnly)
                    if (indicatorName == forbiden)
                    {
                        toContinue = true;
                        break;
                    }
                if (toContinue)
                    continue;

                TreeNode trn = new TreeNode();
                trn.Tag  = true;
                trn.Name = indicatorName;
                trn.Text = indicatorName;
                trnAll.Nodes.Add(trn);

                Indicator indicator = Indicator_Store.ConstructIndicator(indicatorName, slotType);
                TypeOfIndicator type = indicator.IndParam.IndicatorType;

                if (indicator.CustomIndicator)
                {
                    TreeNode trnCustom = new TreeNode();
                    trnCustom.Tag  = true;
                    trnCustom.Name = indicatorName;
                    trnCustom.Text = indicatorName;
                    trnCustomIndicators.Nodes.Add(trnCustom);
                } 

                TreeNode trnGroups = new TreeNode();
                trnGroups.Tag  = true;
                trnGroups.Name = indicatorName;
                trnGroups.Text = indicatorName;
                
                if (type == TypeOfIndicator.Indicator)
                {
                    trnIndicators.Nodes.Add(trnGroups);
                } 
                else if (type == TypeOfIndicator.Additional)
                {
                    trnAdditional.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.OscillatorOfIndicators)
                {
                    trnOscillatorOfIndicators.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.IndicatorsMA)
                {
                    trnIndicatorsMAOscillator.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.DateTime)
                {
                    trnDateTime.Nodes.Add(trnGroups);
                }
            }

            return;
        }

        /// <summary>
        /// Loads the default parameters for the chosen indicator.
        /// </summary>
        void TrvIndicators_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!(bool)trvIndicators.SelectedNode.Tag)
                return;

            TrvIndicatorsLoadIndicator(e.Node);
            
            return;
        }

        /// <summary>
        /// Loads the default parameters for the chosen indicator.
        /// </summary>
        void TrvIndicators_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != ' ')
                return;

            if (!(bool)trvIndicators.SelectedNode.Tag)
                return;

            TrvIndicatorsLoadIndicator(trvIndicators.SelectedNode);

            return;
        }

        /// <summary>
        /// Loads an Indicator
        /// </summary>
        /// <param name="treeNode">The node</param>
        void TrvIndicatorsLoadIndicator(TreeNode treeNode)
        {
            Indicator indicator = Indicator_Store.ConstructIndicator(trvIndicators.SelectedNode.Text, slotType);
            UpdateFromIndicatorParam(indicator.IndParam);
            SetDefaultGroup();
            CalculateIndicator();
            
            return;
        }

        /// <summary>
        /// Loads the defaults parameters for the selected indicator.
        /// </summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            Indicator indicator = Indicator_Store.ConstructIndicator(indicatorName, slotType);
            UpdateFromIndicatorParam(indicator.IndParam);
            SetDefaultGroup();
            CalculateIndicator();

            return;
        }

        /// <summary>
        /// Shows help for the selected indicator.
        /// </summary>
        void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/indicators/start");
            }
            catch { }
       
            return;
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        void BtnOk_Click(object sender, EventArgs e)
        {
            return;
        }

        /// <summary>
        /// Sets the slot group.
        /// </summary>
        void Group_Changed(object sender, EventArgs e)
        {
            if (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter)
                Data.Strategy.Slot[slot].LogicalGroup = cbxGroup.Text;

            Param_Changed(sender, e);

            return;
        }

        /// <summary>
        /// Calls Preview()
        /// </summary>
        void Param_Changed(object sender, EventArgs e)
        {
            CalculateIndicator();

            return;
        }

        /// <summary>
        /// Calculates the selected indicator chart.
        /// </summary>
        void CalculateIndicator()
        {
            if (!Data.IsData || !isPaintAllowed) return;

            SetOppositeSignalBehaviour();
            SetClosingLogicConditions();

            Indicator indicator = Indicator_Store.ConstructIndicator(indicatorName, slotType);

            // List params
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index   = ListParam[i].SelectedIndex;
                indicator.IndParam.ListParam[i].Text    = ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ListParam[i].Enabled;
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value   = (double)NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = NumParam[i].Enabled;
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = CheckParam[i].Enabled;
                if (CheckParam[i].Text == "Use previous bar value")
                    indicator.IndParam.CheckParam[i].Enabled = true;
                else
                    indicator.IndParam.CheckParam[i].Enabled = CheckParam[i].Enabled;
            }

            indicator.Calculate(slotType);

            //Sets the Data.Strategy.
            Data.Strategy.Slot[slot].IndicatorName  = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam       = indicator.IndParam;
            Data.Strategy.Slot[slot].Component      = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue      = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined      = true;

            // Searches the indicators' components to determine the Data.FirstBar. 
            Data.FirstBar = Data.Strategy.SetFirstBar();

            // Checks "Use previous bar value".
            if (Data.Strategy.AdjustUsePreviousBarValue())
            {
                for (int i = 0; i < 2; i++)
                    if (indicator.IndParam.CheckParam[i].Caption == "Use previous bar value")
                        aChbCheck[i].Checked = Data.Strategy.Slot[slot].IndParam.CheckParam[i].Checked;
            }

            SetIndicatorNotification(indicator);

            return;
        }

        /// <summary>
        /// Sets the indicator overview and warning message.
        /// </summary>
        void SetIndicatorNotification(Indicator indicator)
        {
            // Warning message.
            warningMessage = indicator.WarningMessage;
            lblIndicatorWarning.Visible = !string.IsNullOrEmpty(warningMessage);

            // Set description.
            indicator.SetDescription(slotType);
            description = "Long position:" + Environment.NewLine;
            if (slotType == SlotTypes.Open)
            {
                description += "   Open a long position " + indicator.EntryPointLongDescription + "." + Environment.NewLine + Environment.NewLine;
                description += "Short position:" + Environment.NewLine;
                description += "   Open a short position " + indicator.EntryPointShortDescription + ".";
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                description += "   Open a long position when " + indicator.EntryFilterLongDescription + "." + Environment.NewLine + Environment.NewLine;
                description += "Short position:" + Environment.NewLine;
                description += "   Open a short position when " + indicator.EntryFilterShortDescription + ".";
            }
            else if (slotType == SlotTypes.Close)
            {
                description += "   Close a long position " + indicator.ExitPointLongDescription + "." + Environment.NewLine + Environment.NewLine;
                description += "Short position:" + Environment.NewLine;
                description += "   Close a short position " + indicator.ExitPointShortDescription + ".";
            }
            else
            {
                description += "   Close a long position when " + indicator.ExitFilterLongDescription + "." + Environment.NewLine + Environment.NewLine;
                description += "Short position:" + Environment.NewLine;
                description += "   Close a short position when " + indicator.ExitFilterShortDescription + ".";
            }

            for (int i = 0; i < 2; i++)
                if (indicator.IndParam.CheckParam[i].Caption == "Use previous bar value")
                    description += Environment.NewLine + "-------------" + Environment.NewLine + "* Use the value of " + indicator.IndicatorName + " from the previous bar.";

            toolTip.SetToolTip(lblIndicatorInfo, description);
        }

        /// <summary>
        /// Sets or restores the closing logic conditions.
        /// </summary>
        void SetClosingLogicConditions()
        {
            bool isClosingFiltersAllowed = Indicator_Store.ClosingIndicatorsWithClosingFilters.Contains(indicatorName);

            // Removes or recovers closing logic slots.
            if (slotType == SlotTypes.Close && !isClosingFiltersAllowed && Data.Strategy.CloseFilters > 0)
            {   // Removes all the closing logic conditions.
                Data.Strategy.RemoveAllCloseFilters();
                closingSlotsRemoved = true;
            }
            else if (slotType == SlotTypes.Close && isClosingFiltersAllowed && closingSlotsRemoved)
            {
                foreach (IndicatorSlot inslot in closingConditions)
                {   // Recovers all the closing logic conditions.
                    Data.Strategy.AddCloseFilter();
                    Data.Strategy.Slot[Data.Strategy.Slots - 1] = inslot.Clone();
                }
                closingSlotsRemoved = false;
            }

            return;
        }

        /// <summary>
        /// Sets or restores the opposite signal behaviour.
        /// </summary>
        void SetOppositeSignalBehaviour()
        {
            // Changes opposite signal behaviour.
            if (slotType == SlotTypes.Close && indicatorName == "Close and Reverse" && oppSignalBehaviour != OppositeDirSignalAction.Reverse)
            {   // Sets the strategy opposite signal to Reverse.
                Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
                oppSignalSet = true;
            }
            else if (slotType == SlotTypes.Close && indicatorName != "Close and Reverse" && oppSignalSet)
            {   // Recovers the original opposite signal.
                Data.Strategy.OppSignalAction = oppSignalBehaviour;
                oppSignalSet = false;
            }

            return;
        }

        /// <summary>
        /// Shows the indicator description
        /// </summary>
        void LblIndicatorInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(description, slotTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return;
        }

        /// <summary>
        /// Shows the indicator warning
        /// </summary>
        void LblIndicatorWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show(warningMessage, indicatorName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return;
        }

        /// <summary>
        /// Changes the background color of a label when the mouse leaves.
        /// </summary>
        void Label_MouseLeave(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            lbl.BackColor = Color.Transparent;

            return;
        }
 
        /// <summary>
        /// Changes the background color of a label when the mouse enters.
        /// </summary>
        void Label_MouseEnter(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            lbl.BackColor = Color.Orange;

            return;
        }
    }
}
