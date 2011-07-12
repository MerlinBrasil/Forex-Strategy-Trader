// Chart : Panel
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public enum ChartButtons
    {
        Grid, Cross,
        Volume, Orders, PositionLots, PositionPrice,
        ZoomIn, ZoomOut, Refresh, TrueCharts, Shift, AutoScroll,
        DInfoUp, DInfoDwn, DynamicInfo
    }

    /// <summary>
    /// Class Indicator Chart : Form
    /// </summary>
    public class Chart : Panel
    {
        bool isDEBUG = false;

        Panel      pnlCharts;
        Panel      pnlInfo;
        Panel      pnlPrice;
        Panel[]    pnlInd;
        Splitter[] splitterInd;
        HScrollBar scroll;

        ToolStrip stripButtons;
        ToolStripButton[] chartButtons;

        string   chartTitle;
        int      indPanels;
        int      chartBars;
        int      chartWidth;
        int      firstBar;
        int      lastBar;
        double   maxPrice;
        double   minPrice;
        double   YScale;
        string[] asInfoTitle;
        string[] asInfoValue;
        int[]    aiInfoType; // 0 - text; 1 - Indicator; 
        bool[]   repeatedIndicator;

        int mouseX;
        int mouseY;
        int mouseXOld;
        int mouseYOld;
        int barOld;

        int barPixels = 9;
        int chartRightShift = 80;
        int verticalScale = 1;

        bool isMouseInPriceChart;
        bool isMouseInIndicatorChart;

        bool isInfoPanelShown;
        bool isGridShown;
        bool isCrossShown;
        bool isVolumeShown;
        bool isPositionLotsShown;
        bool isOrdersShown;
        bool isPositionPriceShown;
        bool isChartShift;
        bool isChartAutoScroll;
        bool isTrueChartsShown;
        bool isCandleChart = true;

        int  infoRows;
        int  XDynInfoCol2;
        int  dynInfoWidth;
        bool isDrawDinInfo;
        int  dynInfoScrollValue;

        int spcBottom;		// pnlPrice bottom margin
        int spcTop;			// pnlPrice top margin
        int spcLeft;		// pnlPrice left margin
        int spcRight;		// pnlPrice right margin

        int XLeft;			// pnlPrice left coordinate
        int XRight;		    // pnlPrice right coordinate
        int YTop;			// pnlPrice top coordinate
        int YBottom;		// pnlPrice bottom coordinate
        int YBottomText;	// pnlPrice bottom coordinate for date text

        int    maxVolume; // Max Volume in the chart
        double scaleYVol; // The scale for drawing the Volume

        int    countLabels; // The count of price labels on the vertical axe.
        double deltaGrid;   // The distance between two vertical label in price.

        Chart_Data chartData;

        double[] sepChartMinValue;
        double[] sepChartMaxValue;

        bool isCtrlKeyPressed = false;

        Font font;
        Font fontDI;    // Font for Dynamic info
        Font fontDIInd; // Font for Dynamic info Indicators
        Size szDate;
        Size szDateL;
        Size szPrice;
        Size szSL;

        Brush brushBack;
        Brush brushFore;
        Brush brushLabelBkgrd;
        Brush brushLabelFore;
        Brush brushDynamicInfo;
        Brush brushDIIndicator;
        Brush brushEvenRows;
        Brush brushTradeLong;
        Brush brushTradeShort;
        Brush brushTradeClose;
        Brush brushBarWhite;
        Brush brushBarBlack;
        Brush brushSignalRed;

        Pen penCross;
        Pen penGrid;
        Pen penGridSolid;
        Pen penAxes;
        Pen penTradeLong;
        Pen penTradeShort;
        Pen penTradeClose;
        Pen penVolume;
        Pen penBarBorder;
        Pen penBarThick;

        Color colorBarWhite1;
        Color colorBarWhite2;
        Color colorBarBlack1;
        Color colorBarBlack2;

        Color colorLongTrade1;
        Color colorLongTrade2;
        Color colorShortTrade1;
        Color colorShortTrade2;
        Color colorClosedTrade1;
        Color colorClosedTrade2;

// ------------------------------------------------------------
        /// <summary>
        /// The default constructor.
        /// </summary>
        public Chart(Chart_Data chartData)
        {
            this.chartData = chartData;

            BackColor = LayoutColors.ColorFormBack;
            Padding   = new Padding(0);

            pnlCharts = new Panel();
            pnlCharts.Parent = this;
            pnlCharts.Dock   = DockStyle.Fill;

            pnlInfo = new Panel();
            pnlInfo.Parent    = this;
            pnlInfo.BackColor = LayoutColors.ColorControlBack;
            pnlInfo.Dock      = DockStyle.Right;
            pnlInfo.Paint    += new PaintEventHandler(PnlInfo_Paint);

            barPixels            = Configs.ChartZoom;
            isGridShown          = Configs.ChartGrid;
            isCrossShown         = Configs.ChartCross;
            isVolumeShown        = Configs.ChartVolume;
            isPositionLotsShown  = Configs.ChartLots;
            isOrdersShown        = Configs.ChartOrders;
            isPositionPriceShown = Configs.ChartPositionPrice;
            isInfoPanelShown     = Configs.ChartInfoPanel;
            isTrueChartsShown    = Configs.ChartTrueCharts;
            isChartShift         = Configs.ChartShift;
            isChartAutoScroll    = Configs.ChartAutoScroll;

            dynInfoScrollValue = 0;

            font = new Font(Font.FontFamily, 8);

            // Dynamic info fonts
            fontDI    = new Font(Font.FontFamily, 9);
            fontDIInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();
            szDate  = g.MeasureString("99/99 99:99"   , font).ToSize();
            szDateL = g.MeasureString("99/99/99 99:99", font).ToSize();
            //TODO checking exact price with.
            szPrice = g.MeasureString("9.99999", font).ToSize();
            szSL    = g.MeasureString(" SL", font).ToSize();
            g.Dispose();

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            pnlInfo.Visible = isInfoPanelShown;
            pnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 2, 0) : new Padding(0);

            pnlCharts.Resize += new EventHandler(PnlCharts_Resize);
            pnlPrice.Resize  += new EventHandler(PnlPrice_Resize);

            spcTop    = font.Height;
            spcBottom = font.Height * 8 / 5;
            spcLeft   = 0;
            spcRight  = szPrice.Width + szSL.Width + 2;

            brushBack        = new SolidBrush(LayoutColors.ColorChartBack);
            brushFore        = new SolidBrush(LayoutColors.ColorChartFore);
            brushLabelBkgrd  = new SolidBrush(LayoutColors.ColorLabelBack);
            brushLabelFore   = new SolidBrush(LayoutColors.ColorLabelText);
            brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            brushDIIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            brushEvenRows    = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushTradeLong   = new SolidBrush(LayoutColors.ColorTradeLong);
            brushTradeShort  = new SolidBrush(LayoutColors.ColorTradeShort);
            brushTradeClose  = new SolidBrush(LayoutColors.ColorTradeClose);
            brushBarWhite    = new SolidBrush(LayoutColors.ColorBarWhite);
            brushBarBlack    = new SolidBrush(LayoutColors.ColorBarBlack);
            brushSignalRed   = new SolidBrush(LayoutColors.ColorSignalRed);

            penGrid       = new Pen(LayoutColors.ColorChartGrid);
            penGridSolid  = new Pen(LayoutColors.ColorChartGrid);
            penAxes       = new Pen(LayoutColors.ColorChartFore);
            penCross      = new Pen(LayoutColors.ColorChartCross);
            penVolume     = new Pen(LayoutColors.ColorVolume);
            penBarBorder  = new Pen(LayoutColors.ColorBarBorder);
            penBarThick   = new Pen(LayoutColors.ColorBarBorder, 3);
            penTradeLong  = new Pen(LayoutColors.ColorTradeLong);
            penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float[] { 4, 2 };

            colorBarWhite1 = Data.ColorChanage(LayoutColors.ColorBarWhite,  30);
            colorBarWhite2 = Data.ColorChanage(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.ColorChanage(LayoutColors.ColorBarBlack,  30);
            colorBarBlack2 = Data.ColorChanage(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1   = Data.ColorChanage(LayoutColors.ColorTradeLong,   30);
            colorLongTrade2   = Data.ColorChanage(LayoutColors.ColorTradeLong,  -30);
            colorShortTrade1  = Data.ColorChanage(LayoutColors.ColorTradeShort,  30);
            colorShortTrade2  = Data.ColorChanage(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.ColorChanage(LayoutColors.ColorTradeClose,  30);
            colorClosedTrade2 = Data.ColorChanage(LayoutColors.ColorTradeClose, -30);

            return;
        }

        /// <summary>
        /// Performs post initialization settings.
        /// </summary>
        public void InitChart(Chart_Data chartData)
        {
            this.chartData = chartData;

            scroll.Select();
            
            return;
        }

        /// <summary>
        /// Updates the chart after a tick.
        /// </summary>
        public void UpdateChartOnTick(bool repaintChart, Chart_Data chartData)
        {
            this.chartData = chartData;

            if (repaintChart)
                SetupChartTitle();

            bool updateWholeChart = repaintChart;
            double oldMaxPrice = maxPrice;
            double oldMinPrice = minPrice;

            if (isChartAutoScroll || repaintChart)
                SetFirstLastBar();

            SetPriceChartMinMaxValues();

            if (Math.Abs(maxPrice - oldMaxPrice) > chartData.InstrumentProperties.Point ||
                Math.Abs(minPrice - oldMinPrice) > chartData.InstrumentProperties.Point)
                updateWholeChart = true;

            if (updateWholeChart)
            {
                pnlPrice.Invalidate();
            }
            else
            {
                int left  = spcLeft + (chartBars - 2) * barPixels;
                int width = pnlPrice.ClientSize.Width - left;
                Rectangle rect = new Rectangle(left, 0, width, YBottom + 1);
                pnlPrice.Invalidate(rect);
            }

            for (int i = 0; i < pnlInd.Length; i++)
            {
                int slot = (int)pnlInd[i].Tag;
                oldMaxPrice = sepChartMaxValue[slot];
                oldMinPrice = sepChartMinValue[slot];
                SetSepChartsMinMaxValues(i);
                if (Math.Abs(sepChartMaxValue[slot] - oldMaxPrice) > 0.000001 ||
                    Math.Abs(sepChartMinValue[slot] - oldMinPrice) > 0.000001)
                    updateWholeChart = true;

                if (updateWholeChart)
                {
                    pnlInd[i].Invalidate();
                }
                else
                {
                    int left  = spcLeft + (chartBars - 2) * barPixels;
                    int width = pnlInd[i].ClientSize.Width - left;
                    Rectangle rect = new Rectangle(left, 0, width, YBottom + 1);
                    pnlInd[i].Invalidate(rect);
                }
            }

            if (isInfoPanelShown && !isCrossShown)
                GenerateDynamicInfo(lastBar);
            
            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            isCtrlKeyPressed = false;

            ShortcutKeyUp(e);

            return;
        }

        /// <summary>
        /// Create and sets the indicator panels.
        /// <summary>
        void SetupIndicatorPanels()
        {
            pnlPrice = new Panel();
            pnlPrice.Parent      = pnlCharts;
            pnlPrice.Dock        = DockStyle.Fill;
            pnlPrice.BackColor   = LayoutColors.ColorChartBack;
            pnlPrice.MouseLeave += new EventHandler(PnlPrice_MouseLeave);
            pnlPrice.MouseMove  += new MouseEventHandler(PnlPrice_MouseMove);
            pnlPrice.MouseDown  += new MouseEventHandler(Panel_MouseDown);
            pnlPrice.MouseUp    += new MouseEventHandler(Panel_MouseUp);
            pnlPrice.Paint      += new PaintEventHandler(PnlPrice_Paint);

            stripButtons = new ToolStrip();
            stripButtons.Parent = pnlCharts;

            sepChartMinValue = new double[chartData.Strategy.Slots];
            sepChartMaxValue = new double[chartData.Strategy.Slots];

            // Indicator panels
            indPanels = 0;
            string[] asIndicatorTexts = new string[chartData.Strategy.Slots];
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                Indicator indicator = Indicator_Store.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName, chartData.Strategy.Slot[slot].SlotType);
                indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
                asIndicatorTexts[slot] = indicator.ToString();
                indPanels += chartData.Strategy.Slot[slot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            repeatedIndicator = new bool[chartData.Strategy.Slots];
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                repeatedIndicator[slot] = false;
                for (int i = 0; i < slot; i++)
                    repeatedIndicator[slot] = asIndicatorTexts[slot] == asIndicatorTexts[i];
            }
            
            pnlInd = new Panel[indPanels];
            splitterInd = new Splitter [indPanels];
            for (int i = 0; i < indPanels; i++)
            {
                splitterInd[i] = new Splitter();
                splitterInd[i].Parent      = pnlCharts;
                splitterInd[i].BorderStyle = BorderStyle.None;
                splitterInd[i].Dock        = DockStyle.Bottom;
                splitterInd[i].Height      = 2;

                pnlInd[i] = new Panel();
                pnlInd[i].Parent      = pnlCharts;
                pnlInd[i].Dock        = DockStyle.Bottom;
                pnlInd[i].BackColor   = LayoutColors.ColorControlBack;
                pnlInd[i].Paint      += new PaintEventHandler(PnlInd_Paint);
                pnlInd[i].MouseMove  += new MouseEventHandler(PnlInd_MouseMove);
                pnlInd[i].MouseLeave += new EventHandler(PnlInd_MouseLeave);
                pnlInd[i].MouseDown  += new MouseEventHandler(Panel_MouseDown);
                pnlInd[i].MouseUp    += new MouseEventHandler(Panel_MouseUp);
                pnlInd[i].Tag = i; // A temporary tag.
            }

            int index = 0;
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (!chartData.Strategy.Slot[slot].SeparatedChart) continue;
                pnlInd[index].Tag = slot; // The real tag.
                index++;
            }

            scroll = new HScrollBar();
            scroll.Parent        = pnlCharts;
            scroll.Dock          = DockStyle.Bottom;
            scroll.TabStop       = true;
            scroll.SmallChange   = 1;
            scroll.ValueChanged += new EventHandler(Scroll_ValueChanged);
            scroll.MouseWheel   += new MouseEventHandler(Scroll_MouseWheel);
            scroll.KeyUp        += new KeyEventHandler(Scroll_KeyUp);
            scroll.KeyDown      += new KeyEventHandler(Scroll_KeyDown);

            for (int i = 0; i < indPanels; i++)
                pnlInd[i].Resize += new EventHandler(PnlInd_Resize);

            return;
        }

        /// <summary>
        /// Sets up the chart's buttons.
        /// </summary>
        void SetupButtons()
        {
            chartButtons = new ToolStripButton[15];
            for (int i = 0; i < 15; i++)
            {
                chartButtons[i] = new ToolStripButton();
                chartButtons[i].Tag = (ChartButtons)i;
                chartButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Image;
                chartButtons[i].Click += new EventHandler(ButtonChart_Click);
                stripButtons.Items.Add(chartButtons[i]);
                if (i > 11)
                    chartButtons[i].Alignment = ToolStripItemAlignment.Right;
                if (i == 1 || i == 5 || i == 7 || i == 8)
                    stripButtons.Items.Add(new ToolStripSeparator());
            }

            // Grid
            chartButtons[(int)ChartButtons.Grid].Image = Properties.Resources.chart_grid;
            chartButtons[(int)ChartButtons.Grid].ToolTipText = Language.T("Grid") + "   G";
            chartButtons[(int)ChartButtons.Grid].Checked = Configs.ChartGrid;

            // Cross
            chartButtons[(int)ChartButtons.Cross].Image = Properties.Resources.chart_cross;
            chartButtons[(int)ChartButtons.Cross].ToolTipText = Language.T("Cross") + "   C";
            chartButtons[(int)ChartButtons.Cross].Checked = Configs.ChartCross;

            // Volume
            chartButtons[(int)ChartButtons.Volume].Image = Properties.Resources.chart_volume;
            chartButtons[(int)ChartButtons.Volume].ToolTipText = Language.T("Volume") + "   V";
            chartButtons[(int)ChartButtons.Volume].Checked = Configs.ChartVolume;

            // Orders
            chartButtons[(int)ChartButtons.Orders].Image = Properties.Resources.chart_entry_points;
            chartButtons[(int)ChartButtons.Orders].ToolTipText = Language.T("Orders") + "   O";
            chartButtons[(int)ChartButtons.Orders].Checked = Configs.ChartOrders;

            // Position lots
            chartButtons[(int)ChartButtons.PositionLots].Image = Properties.Resources.chart_lots;
            chartButtons[(int)ChartButtons.PositionLots].ToolTipText = Language.T("Position lots") + "   L";
            chartButtons[(int)ChartButtons.PositionLots].Checked = Configs.ChartLots;

            // Position price
            chartButtons[(int)ChartButtons.PositionPrice].Image = Properties.Resources.chart_pos_price;
            chartButtons[(int)ChartButtons.PositionPrice].ToolTipText = Language.T("Position price") + "   P";
            chartButtons[(int)ChartButtons.PositionPrice].Checked = Configs.ChartPositionPrice;

            // Zoom in
            chartButtons[(int)ChartButtons.ZoomIn].Image = Properties.Resources.chart_zoom_in;
            chartButtons[(int)ChartButtons.ZoomIn].ToolTipText = Language.T("Zoom in") + "   +";

            // Zoom out
            chartButtons[(int)ChartButtons.ZoomOut].Image = Properties.Resources.chart_zoom_out;
            chartButtons[(int)ChartButtons.ZoomOut].ToolTipText = Language.T("Zoom out") + "   -";

            // Refresh
            chartButtons[(int)ChartButtons.Refresh].Image = Properties.Resources.chart_refresh;
            chartButtons[(int)ChartButtons.Refresh].ToolTipText = Language.T("Refresh chart") + "   F5";

            // True Charts
            chartButtons[(int)ChartButtons.TrueCharts].Image = Properties.Resources.chart_true_charts;
            chartButtons[(int)ChartButtons.TrueCharts].Checked = Configs.ChartTrueCharts;
            chartButtons[(int)ChartButtons.TrueCharts].ToolTipText = Language.T("True indicator charts") + "   T";

            // Shift
            chartButtons[(int)ChartButtons.Shift].Image = Properties.Resources.chart_shift;
            chartButtons[(int)ChartButtons.Shift].ToolTipText = Language.T("Chart shift") + "   S";
            chartButtons[(int)ChartButtons.Shift].Checked = Configs.ChartShift;

            // Auto Scroll
            chartButtons[(int)ChartButtons.AutoScroll].Image = Properties.Resources.chart_auto_scroll;
            chartButtons[(int)ChartButtons.AutoScroll].ToolTipText = Language.T("Auto scroll") + "   R";
            chartButtons[(int)ChartButtons.AutoScroll].Checked = Configs.ChartAutoScroll;

            // Show dynamic info
            chartButtons[(int)ChartButtons.DynamicInfo].Image = Properties.Resources.chart_dyninfo;
            chartButtons[(int)ChartButtons.DynamicInfo].Checked = Configs.ChartInfoPanel;
            chartButtons[(int)ChartButtons.DynamicInfo].ToolTipText = Language.T("Show / hide info panel") + "   I";

            // Move Dynamic Info Down
            chartButtons[(int)ChartButtons.DInfoDwn].Image = Properties.Resources.chart_dinfo_down;
            chartButtons[(int)ChartButtons.DInfoDwn].ToolTipText = Language.T("Move info down") + "   Z";
            chartButtons[(int)ChartButtons.DInfoDwn].Visible = isInfoPanelShown;

            // Move Dynamic Info Up
            chartButtons[(int)ChartButtons.DInfoUp].Image = Properties.Resources.chart_dinfo_up;
            chartButtons[(int)ChartButtons.DInfoUp].ToolTipText = Language.T("Move info up") + "   A";
            chartButtons[(int)ChartButtons.DInfoUp].Visible = isInfoPanelShown;

            return;
        }

        /// <summary>
        /// Sets the chart's parameters.
        /// </summary>
        void SetFirstLastBar()
        {
            scroll.Minimum = chartData.FirstBar;
            scroll.Maximum = chartData.Bars - 1;

            int shift = isChartShift ? chartRightShift : 0;
            chartBars = (chartWidth - shift - 7) / barPixels;
            chartBars = Math.Min(chartBars, chartData.Bars - chartData.FirstBar);
            firstBar  = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            firstBar  = Math.Min(firstBar, chartData.Bars - 1);
            lastBar   = Math.Max(firstBar + chartBars - 1, firstBar);

            scroll.Value       = firstBar;
            scroll.LargeChange = Math.Max(chartBars, 1);

            return;
        }

        /// <summary>
        /// Sets the min and the max values of price shown on the chart.
        /// </summary>
        void SetPriceChartMinMaxValues()
        {
            // Searching the min and the max price and volume
            maxPrice  = double.MinValue;
            minPrice  = double.MaxValue;
            maxVolume = int.MinValue;
            double spread = chartData.InstrumentProperties.Spread * chartData.InstrumentProperties.Point;
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (chartData.High[bar] + spread > maxPrice) maxPrice = chartData.High[bar] + spread;
                if (chartData.Low[bar] < minPrice) minPrice = chartData.Low[bar];
                if (chartData.Volume[bar] > maxVolume) maxVolume = chartData.Volume[bar];
            }

            double pricePixel = (maxPrice - minPrice) / (YBottom - YTop);
            if (isVolumeShown)
                minPrice -= pricePixel * 30;
            else if (isPositionLotsShown)
                minPrice -= pricePixel * 10;

            maxPrice += pricePixel * verticalScale;
            minPrice -= pricePixel * verticalScale;

            // Grid
            double deltaPoint = (chartData.InstrumentProperties.Digits == 5 || chartData.InstrumentProperties.Digits == 3) ? chartData.InstrumentProperties.Point * 100 : chartData.InstrumentProperties.Point * 10;
            int roundStep = (int)Math.Max(chartData.InstrumentProperties.Digits - 1, 1);
            countLabels = (int)Math.Max((YBottom - YTop) / 35, 1);
            deltaGrid   = Math.Max(Math.Round((maxPrice - minPrice) / countLabels, roundStep), deltaPoint);
            minPrice    = Math.Round(minPrice, roundStep) - deltaPoint;
            countLabels = (int)Math.Ceiling((maxPrice - minPrice) / deltaGrid);
            maxPrice    = minPrice + countLabels * deltaGrid;
            YScale      = (YBottom - YTop) / (countLabels * deltaGrid);
            scaleYVol   = maxVolume > 0 ? 40.0 / maxVolume : 0; // 40 - the highest volume line

            return;
        }

        /// <summary>
        /// Sets parameter of separated charts
        /// </summary>
        void SetSepChartsMinMaxValues(int index)
        {
            Panel panel = pnlInd[index];
            int slot = (int)panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in chartData.Strategy.Slot[slot].SpecValue)
                if (value == 0)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            sepChartMaxValue[slot] = maxValue;
            sepChartMinValue[slot] = minValue;

            return;
        }

        /// <summary>
        /// Sets the indicator chart title
        /// </summary>
        void SetupChartTitle()
        {
            chartTitle = chartData.StrategyName + " " + chartData.Symbol + " " + chartData.PeriodStr + " (" + chartData.Bars + " bars)";

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                    if (component.ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }

                if (isChart)
                {
                    Indicator indicator = Indicator_Store.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName, chartData.Strategy.Slot[slot].SlotType);
                    indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
                    if (!chartTitle.Contains(indicator.ToString()))
                        chartTitle += Environment.NewLine + indicator.ToString();
                }
            }

            return;
        }

        /// <summary>
        /// Sets the sizes of the panels after resizing.
        /// </summary>
        void PnlCharts_Resize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetFirstLastBar();
            SetPriceChartMinMaxValues();
            for (int i = 0; i < pnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            GenerateDynamicInfo(lastBar);
            dynInfoScrollValue = 0;

            return;
        }

        /// <summary>
        /// Calculates the panels' height
        /// </summary>
        void SetAllPanelsHeight()
        {
            int availableHeight = pnlCharts.ClientSize.Height - stripButtons.Height - scroll.Height - indPanels * 2;
            int pnlIndHeight = availableHeight / (2 + indPanels);

            foreach (Panel panel in pnlInd)
                panel.Height = pnlIndHeight;

            return;
        }

        /// <summary>
        /// Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        void PnlPrice_Resize(object sender, EventArgs e)
        {
            XLeft       = spcLeft;
            XRight      = pnlPrice.ClientSize.Width - spcRight;
            chartWidth  = Math.Max(XRight - XLeft, 0);
            YTop        = spcTop;
            YBottom     = pnlPrice.ClientSize.Height - spcBottom;
            YBottomText = pnlPrice.ClientSize.Height - spcBottom * 5 / 8 - 4;

            SetPriceChartMinMaxValues();

            pnlPrice.Invalidate();

            return;
        }

        /// <summary>
        /// Invalidates the panels
        /// </summary>
        void PnlInd_Resize(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            int slot = (int)panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in chartData.Strategy.Slot[slot].SpecValue)
                if (value == 0)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            sepChartMaxValue[slot] = maxValue;
            sepChartMinValue[slot] = minValue;

            panel.Invalidate();

            return;
        }

        /// <summary>
        /// Paints the panel PnlPrice
        /// </summary>
        void PnlPrice_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try { g.Clear(LayoutColors.ColorChartBack); }
            catch { }

            if (chartBars == 0) return;
          
            // Grid
            for (double label = minPrice; label <= maxPrice + chartData.InstrumentProperties.Point; label += deltaGrid)
            {
                int labelY = (int)(YBottom - (label - minPrice) * YScale);
                g.DrawString(label.ToString(Data.FF), font, brushFore, XRight, labelY - Font.Height / 2 - 1);
                if (isGridShown || label == minPrice)
                    g.DrawLine(penGrid, spcLeft, labelY, XRight, labelY);
                else
                    g.DrawLine(penGrid, XRight - 5, labelY, XRight, labelY);
            }
            for (int vertLineBar = lastBar; vertLineBar > firstBar; vertLineBar -= (int)(szDate.Width + 10) / barPixels + 1)
            {
                int XVertLine = (vertLineBar - firstBar) * barPixels + spcLeft + barPixels / 2 - 1;
                if (isGridShown)
                    g.DrawLine(penGrid, XVertLine, YTop, XVertLine, YBottom + 2);
                string date = String.Format("{0} {1}", chartData.Time[vertLineBar].ToString(Data.DFS),
                                                       chartData.Time[vertLineBar].ToString("HH:mm"));
                g.DrawString(date, font, brushFore, XVertLine - szDate.Width / 2, YBottomText);
            }

            // Draws Volume, Lots and Bars
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x       = (bar - firstBar) * barPixels + spcLeft;
                int xCenter = x + (barPixels - 1) / 2 - 1;
                int yOpen   = (int)(YBottom - (chartData.Open[bar] - minPrice) * YScale);
                int yHigh   = (int)(YBottom - (chartData.High[bar] - minPrice) * YScale);
                int yLow    = (int)(YBottom - (chartData.Low[bar] - minPrice) * YScale);
                int yClose  = (int)(YBottom - (chartData.Close[bar] - minPrice) * YScale);
                int yVolume = (int)(YBottom - chartData.Volume[bar] * scaleYVol);

                // Draw the volume
                if (isVolumeShown && yVolume != YBottom)
                    g.DrawLine(penVolume, x + barPixels / 2 - 1, yVolume, x + barPixels / 2 - 1, YBottom);

                // Draw position's lots
                if (isPositionLotsShown && chartData.BarStatistics.ContainsKey(chartData.Time[bar]))
                {
                    PosDirection dir = chartData.BarStatistics[chartData.Time[bar]].PositionDir;
                    if (dir != PosDirection.None)
                    {
                        double lots = chartData.BarStatistics[chartData.Time[bar]].PositionLots;
                        int iPosHight = (int)(Math.Max(lots * 3, 2));
                        int iPosY = YBottom - iPosHight + 1;
                        Rectangle rect = new Rectangle(x - 1, iPosY, barPixels, iPosHight);
                        LinearGradientBrush lgBrush;
                        if (dir == PosDirection.Long)
                            lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        else if (dir == PosDirection.Short)
                            lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        else
                            lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, iPosY, barPixels - 2, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (isCandleChart)
                {
                    if (barPixels < 29)
                        g.DrawLine(penBarBorder, xCenter, yLow, xCenter, yHigh);
                    else
                        g.DrawLine(penBarThick, xCenter, yLow, xCenter, yHigh);

                    if (yClose < yOpen)
                    {   // White bar
                        Rectangle rect = new Rectangle(x + 1, yClose, barPixels - 5, yOpen - yClose);
                        LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarWhite1, colorBarWhite2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(penBarBorder, rect);
                    }
                    else if (yClose > yOpen)
                    {   // Black bar
                        Rectangle rect = new Rectangle(x + 1, yOpen, barPixels - 5, yClose - yOpen);
                        LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(penBarBorder, rect);
                    }
                    else
                    {   // Cross
                        if (barPixels < 29)
                            g.DrawLine(penBarBorder, x + 1, yClose, x + barPixels - 4, yClose);
                        else
                            g.DrawLine(penBarThick, x + 1, yClose, x + barPixels - 4, yClose);
                    }
                }
                else
                {
                    if (barPixels <= 16)
                    {
                        g.DrawLine(penBarBorder, xCenter, yLow, xCenter, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarBorder, x, yOpen, xCenter, yOpen);
                            g.DrawLine(penBarBorder, xCenter, yClose, x + barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarBorder, x, yClose, x + barPixels - 3, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(penBarThick, xCenter, yLow + 2, xCenter, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarThick, x + 1, yOpen, xCenter, yOpen);
                            g.DrawLine(penBarThick, xCenter - 1, yClose, x + barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarThick, x, yClose, x + barPixels - 3, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(spcLeft, YTop, XRight, YBottom - YTop));
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot].SeparatedChart || repeatedIndicator[slot]) continue;

                int cloudUp   = -1; // For Ichimoku and similar
                int cloudDown = -1; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int  indicatorValueShift = 1;
                foreach (ListParam listParam in chartData.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < chartData.Strategy.Slot[slot].Component.Length; comp++)
                {
                    Pen pen = new Pen(chartData.Strategy.Slot[slot].Component[comp].ChartColor);
                    Pen penTC = new Pen(chartData.Strategy.Slot[slot].Component[comp].ChartColor);
                    penTC.DashStyle = DashStyle.Dash;
                    penTC.DashPattern = new float[] { 2, 1 };

                    if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {   // Line
                        if (isTrueChartsShown)
                        {   // True Charts
                            Point[] point = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar) * barPixels + 1 + indicatorValueShift * (barPixels - 5);
                                int y = (int)Math.Round(YBottom - (value - minPrice) * YScale);

                                if (value == 0)
                                    point[bar - firstBar] = point[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    point[bar - firstBar] = new Point(x, y);
                            }

                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {   // All bars except the last one
                                int i = bar - firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                                if (bar == firstBar && isIndicatorValueAtClose)
                                {   // First bar
                                    double value = chartData.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = spcLeft + (bar - firstBar) * barPixels;
                                    int y = (int)Math.Round(YBottom - (value - minPrice) * YScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (barPixels > 3)
                                    {   // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 7, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 6, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                    }
                                    if (deltaY > 4)
                                    {   // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTC, x + barPixels - 4, y + 2, x + barPixels - 4, point[i].Y - 2);
                                        else
                                            g.DrawLine(penTC, x + barPixels - 4, y - 2, x + barPixels - 4, point[i].Y + 2);
                                    }
                                }

                                if (bar < lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);

                                    if (barPixels > 3)
                                    {   // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                        else
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                    }
                                    if (deltaY > 4)
                                    {   // Vertical part
                                        if (point[i + 1].Y > point[i].Y)
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X, point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X, point[i + 1].Y + 2);
                                    }
                                }

                                if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                                {   // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 5, point[i].Y);
                                }
                            }
                        }
                        else
                        {
                            Point[] aPoint = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                                int y = (int)(YBottom - (dValue - minPrice) * YScale);

                                if (dValue == 0)
                                    aPoint[bar - firstBar] = aPoint[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    aPoint[bar - firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);

                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {   // Dots
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                            int y = (int)(YBottom - (dValue - minPrice) * YScale);
                            if (barPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {   // Level
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar) * barPixels + spcLeft;
                            int y = (int)(YBottom - (dValue - minPrice) * YScale);
                            g.DrawLine(pen, x, y, x + barPixels - 1, y);
                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp >= 0 && cloudDown >= 0)
                {
                    PointF[] apntUp   = new PointF[lastBar - firstBar + 1];
                    PointF[] apntDown = new PointF[lastBar - firstBar + 1];
                    for (int bar = firstBar; bar <= lastBar; bar++)
                    {
                        double dValueUp = chartData.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = chartData.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        apntUp[bar - firstBar].X = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                        apntUp[bar - firstBar].Y = (int)(YBottom - (dValueUp - minPrice) * YScale);
                        apntDown[bar - firstBar].X = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                        apntDown[bar - firstBar].Y = (int)(YBottom - (dValueDown - minPrice) * YScale);
                    }

                    GraphicsPath pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(apntUp[0].X, 0), apntUp[0]);
                    pathUp.AddLines(apntUp);
                    pathUp.AddLine(apntUp[lastBar - firstBar], new PointF(apntUp[lastBar - firstBar].X, 0));
                    pathUp.AddLine(new PointF(apntUp[lastBar - firstBar].X, 0), new PointF(apntUp[0].X, 0));
                    
                    GraphicsPath pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(apntDown[0].X, 0), apntDown[0]);
                    pathDown.AddLines(apntDown);
                    pathDown.AddLine(apntDown[lastBar - firstBar], new PointF(apntDown[lastBar - firstBar].X, 0));
                    pathDown.AddLine(new PointF(apntDown[lastBar - firstBar].X, 0), new PointF(apntDown[0].X, 0));

                    Color colorUp   = Color.FromArgb(50, chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    Pen penUp   = new Pen(chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Pen penDown = new Pen(chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    penUp.DashStyle   = DashStyle.Dash;
                    penDown.DashStyle = DashStyle.Dash;

                    Brush brushUp   = new SolidBrush(colorUp);
                    Brush brushDown = new SolidBrush(colorDown);

                    System.Drawing.Region regionUp = new Region(pathUp);
                    regionUp.Exclude(pathDown);
                    g.FillRegion(brushDown, regionUp);

                    System.Drawing.Region regionDown = new Region(pathDown);
                    regionDown.Exclude(pathUp);
                    g.FillRegion(brushUp, regionDown);

                    g.DrawLines(penUp,   apntUp);
                    g.DrawLines(penDown, apntDown);
                }

            }
            g.ResetClip();

            // Draws position price and deals.
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                DateTime bartime = chartData.Time[bar];
                if (!chartData.BarStatistics.ContainsKey(bartime))
                    continue;

                int x = (bar - firstBar) * barPixels + spcLeft;
                int yHigh = (int)(YBottom - (chartData.High[bar] - minPrice) * YScale);

                // Draws the position's price
                if (isPositionPriceShown)
                {
                    double price = chartData.BarStatistics[bartime].PositionPrice;
                    if (price > chartData.InstrumentProperties.Point)
                    {
                        int yPrice = (int)(YBottom - (price - minPrice) * YScale);

                        if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Long)
                        {   // Long
                            g.DrawLine(penTradeLong, x, yPrice, x + barPixels - 2, yPrice);
                        }
                        else if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Short)
                        {   // Short
                            g.DrawLine(penTradeShort, x, yPrice, x + barPixels - 2, yPrice);
                        }
                        else if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Closed)
                        {   // Closed
                            g.DrawLine(penTradeClose, x, yPrice, x + barPixels - 2, yPrice);
                        }
                    }
                }

                // Draw the deals
                if (isOrdersShown)
                {
                    foreach (Operation operation in chartData.BarStatistics[bartime].Operations)
                    {
                        int yOrder = (int)(YBottom - (operation.OperationPrice - minPrice) * YScale);

                        if (operation.OperationType == OperationType.Buy)
                        {   // Buy
                            Pen pen = new Pen(brushTradeLong, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 4, yOrder);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder - 3);
                            }
                            else if (barPixels > 9)
                            {
                                int d  = (barPixels - 1) / 2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x,  yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder - d);
                                g.DrawLine(pen, x2 + 1, yOrder - d + 1, x1 + d / 2 + 1, yOrder - d + 1);
                                g.DrawLine(pen, x2, yOrder - d, x2, yOrder - d / 2);
                            }
                        }
                        else if (operation.OperationType == OperationType.Sell)
                        {   // Sell
                            Pen pen = new Pen(brushTradeShort, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder + 1, x + 4, yOrder + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder + 3);
                            }
                            else if (barPixels > 9)
                            {
                                int d = (barPixels - 1) / 2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x,  yOrder + 1, x1 + 1, yOrder + 1);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder + d);
                                g.DrawLine(pen, x1 + d / 2 + 1, yOrder + d, x2, yOrder + d);
                                g.DrawLine(pen, x2, yOrder + d, x2, yOrder + d / 2 + 1);
                            }
                        }
                        else if (operation.OperationType == OperationType.Close)
                        {   // Close
                            Pen pen = new Pen(brushTradeClose, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 7, yOrder);
                                g.DrawLine(pen, x + 5, yOrder - 2, x + 5, yOrder + 2);
                            }
                            else if (barPixels > 9)
                            {
                                int d = (barPixels - 1) / 2 - 1;
                                int x1 = x + d + 1;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x,  yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder + d / 2, x2, yOrder - d / 2);
                                g.DrawLine(pen, x1, yOrder - d / 2, x2, yOrder + d / 2);
                            }
                        }
                    }
                }
            }

            // Bid price label.
            int yBid = (int)(YBottom - (chartData.Bid - minPrice) * YScale);
            Point    pBid  = new Point(XRight, yBid - szPrice.Height / 2);
            string sBid = (chartData.Bid.ToString(Data.FF));
            int      xBidRight = XRight + szPrice.Width + 1;
            PointF[] apBid = new PointF[] {
                new PointF(XRight - 6, yBid), 
                new PointF(XRight, yBid - szPrice.Height / 2), 
                new PointF(xBidRight, yBid - szPrice.Height / 2 - 1), 
                new PointF(xBidRight, yBid + szPrice.Height / 2 + 1), 
                new PointF(XRight, yBid + szPrice.Height / 2),
            };

            // Position price.
            if (isPositionPriceShown && (chartData.PositionDirection == PosDirection.Long || chartData.PositionDirection == PosDirection.Short))
            {
                int yPos = (int)(YBottom - (chartData.PositionOpenPrice - minPrice) * YScale);
                Point     pPos = new Point(XRight, yPos - szPrice.Height / 2);
                Rectangle rPos = new Rectangle(pPos, szPrice);
                string sPos = (chartData.PositionOpenPrice.ToString(Data.FF));
                SolidBrush brushText = new SolidBrush(LayoutColors.ColorChartBack);

                if (chartData.PositionOpenPrice > minPrice && chartData.PositionOpenPrice < maxPrice)
                {
                    Pen penPos = new Pen(LayoutColors.ColorTradeLong);
                    if (chartData.PositionDirection == PosDirection.Short)
                        penPos = new Pen(LayoutColors.ColorTradeShort);
                    PointF[] apPos = new PointF[] {
                                new PointF(XRight - 6, yPos), 
                                new PointF(XRight, yPos - szPrice.Height / 2), 
                                new PointF(XRight + szPrice.Width, yPos - szPrice.Height / 2), 
                                new PointF(XRight + szPrice.Width, yPos + szPrice.Height / 2), 
                                new PointF(XRight, yPos + szPrice.Height / 2),
                                new PointF(XRight - 6, yPos), 
                            };
                    g.FillPolygon(brushBack, apPos);
                    g.DrawString(sPos, font, brushFore, pPos);
                    g.DrawLines(penPos, apPos);
                }

                // Profit Arrow
                Pen penProfit;
                if (chartData.PositionProfit > 0)
                    penProfit = new Pen(LayoutColors.ColorTradeLong, 7);
                else
                    penProfit = new Pen(LayoutColors.ColorTradeShort, 7);
                penProfit.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(penProfit, xBidRight + 9, yPos, xBidRight + 9, yBid);

                // Close Price
                IndicatorSlot slot = chartData.Strategy.Slot[chartData.Strategy.CloseSlot];
                if (slot.IndParam.ExecutionTime != ExecutionTime.AtBarClosing)
                {
                    double dClosePrice = 0;
                    for (int iComp = 0; iComp < slot.Component.Length; iComp++)
                    {
                        IndComponentType compType = slot.Component[iComp].DataType;
                        if (chartData.PositionDirection == PosDirection.Long && compType == IndComponentType.CloseLongPrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                        else if (chartData.PositionDirection == PosDirection.Short && compType == IndComponentType.CloseShortPrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                        else if (compType == IndComponentType.ClosePrice || compType == IndComponentType.OpenClosePrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                    }
                    if (dClosePrice > minPrice && dClosePrice < maxPrice)
                    {
                        int    yClose = (int)(YBottom - (dClosePrice - minPrice) * YScale);
                        Point  pClose = new Point(XRight, yClose - szPrice.Height / 2);
                        string sClose = (dClosePrice.ToString(Data.FF) + " X");
                        PointF[] apClose = new PointF[] {
                            new PointF(XRight - 6, yClose), 
                            new PointF(XRight, yClose - szPrice.Height / 2), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yClose - szPrice.Height / 2 - 1), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yClose + szPrice.Height / 2 + 1), 
                            new PointF(XRight, yClose + szPrice.Height / 2),
                        };
                        g.FillPolygon(new SolidBrush(LayoutColors.ColorTradeClose), apClose);
                        g.DrawString(sClose, font, brushText, pClose);
                    }

                    // Take Profit
                    if (chartData.PositionTakeProfit > minPrice && chartData.PositionTakeProfit < maxPrice)
                    {
                        int yLimit = (int)(YBottom - (chartData.PositionTakeProfit - minPrice) * YScale);
                        Point     pLimit  = new Point(XRight, yLimit - szPrice.Height / 2);
                        string sLimit = (chartData.PositionTakeProfit.ToString(Data.FF) + " TP");
                        PointF[]  apLimit = new PointF[] {
                            new PointF(XRight - 6, yLimit), 
                            new PointF(XRight, yLimit - szPrice.Height / 2), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yLimit - szPrice.Height / 2 - 1), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yLimit + szPrice.Height / 2 + 1), 
                            new PointF(XRight, yLimit + szPrice.Height / 2),
                        };
                        SolidBrush brushTakeProffit = new SolidBrush(LayoutColors.ColorTradeLong);
                        g.FillPolygon(brushTakeProffit, apLimit);
                        g.DrawString(sLimit, font, brushText, pLimit);
                    }

                    // Stop Loss
                    if (chartData.PositionStopLoss > minPrice && chartData.PositionStopLoss < maxPrice)
                    {
                        int yStop = (int)(YBottom - (chartData.PositionStopLoss - minPrice) * YScale);
                        Point     pStop  = new Point(XRight, yStop - szPrice.Height / 2);
                        string sStop = (chartData.PositionStopLoss.ToString(Data.FF) + " SL");
                        PointF[]  apStop = new PointF[] {
                            new PointF(XRight - 6, yStop), 
                            new PointF(XRight, yStop - szPrice.Height / 2), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yStop - szPrice.Height / 2 - 1), 
                            new PointF(XRight + szPrice.Width + szSL.Width - 2, yStop + szPrice.Height / 2 + 1), 
                            new PointF(XRight, yStop + szPrice.Height / 2),
                        };
                        SolidBrush brushStopLoss = new SolidBrush(LayoutColors.ColorTradeShort);
                        g.FillPolygon(brushStopLoss, apStop);
                        g.DrawString(sStop, font, brushText, pStop);
                    }
                }
            }

            // Draws Bid price label.
            g.FillPolygon(brushLabelBkgrd, apBid);
            g.DrawString(sBid, font, brushLabelFore, pBid);

            // Cross
            if (isCrossShown && mouseX > XLeft - 1 && mouseX < XRight  + 1)
            {
                Point point;
                Rectangle rec;
                int bar;

                bar = (mouseX - spcLeft) / barPixels;
                bar = Math.Max(0, bar);
                bar = Math.Min(chartBars - 1, bar);
                bar += firstBar;
                bar = Math.Min(chartData.Bars - 1, bar);

                // Vertical positions
                point = new Point(mouseX - szDateL.Width / 2, YBottomText);
                rec   = new Rectangle(point, szDateL);

                // Vertical line
                if (isMouseInPriceChart && mouseY > YTop - 1 && mouseY < YBottom + 1)
                {
                    g.DrawLine(penCross, mouseX, YTop, mouseX, mouseY - 10);
                    g.DrawLine(penCross, mouseX, mouseY + 10, mouseX, YBottomText);
                }
                else if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.DrawLine(penCross, mouseX, YTop, mouseX, YBottomText);
                }

                // Date Window
                if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sDate = chartData.Time[bar].ToString(Data.DF) + " " + chartData.Time[bar].ToString("HH:mm");
                    g.DrawString(sDate, font, brushLabelFore, point);
                }

                if (isMouseInPriceChart && mouseY > YTop - 1 && mouseY < YBottom + 1)
                {
                    // Horizontal positions
                    point = new Point(XRight, mouseY - szPrice.Height / 2);
                    rec = new Rectangle(point, szPrice);
                    // Horizontal line
                    g.DrawLine(penCross, XLeft, mouseY, mouseX - 10, mouseY);
                    g.DrawLine(penCross, mouseX + 10, mouseY, XRight, mouseY);
                    // Price Window
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sPrice = ((YBottom - mouseY) / YScale + minPrice).ToString(Data.FF);
                    g.DrawString(sPrice, font, brushLabelFore, point);
                }
            }

            // Chart title
            g.DrawString(chartTitle, font, brushFore, spcLeft, 0);

            return;
        }

        /// <summary>
        /// Paints the panel PnlInd
        /// </summary>
        void PnlInd_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            try { g.Clear(LayoutColors.ColorChartBack); }
            catch { }

            if (chartBars == 0) return;

            int topSpace = font.Height / 2 + 2;
            int bottomSpace = font.Height / 2;

            int slot = (int)pnl.Tag;
            double minValue = sepChartMinValue[slot];
            double maxValue = sepChartMaxValue[slot];

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace) / (Math.Max(maxValue - minValue, 0.0001));

            // Grid
            double label;
            int    labelY;
            String format;
            double deltaLabel;
            int XGridRight = pnl.ClientSize.Width - spcRight + 2;

            // Zero line
            label = 0;
            int labelYZero = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (label >= minValue && label <= maxValue)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10 ? "F4" : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, XRight, labelYZero - font.Height / 2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, XGridRight, labelYZero);
            }

            label = minValue;
            int labelYMin = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMin) >= font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10 ? "F4" : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, XRight, labelYMin - font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMin, XGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMin, XGridRight, labelYMin);
            }
            label = maxValue;
            int labelYMax = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMax) >= font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10 ? "F4" : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, XRight, labelYMax - font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMax, XGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMax, XGridRight, labelYMax);
            }
            if (chartData.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < chartData.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = chartData.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        labelY = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
                        if (Math.Abs(labelY - labelYZero) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMin)  < font.Height) continue;
                        if (Math.Abs(labelY - labelYMax)  < font.Height) continue;
                        deltaLabel = Math.Abs(label);
                        format = deltaLabel < 10 ? "F4" : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(format), font, brushFore, XRight, labelY - font.Height / 2 - 1);
                        if (isGridShown)
                            g.DrawLine(penGrid, spcLeft, labelY, XGridRight, labelY);
                        else
                            g.DrawLine(penGrid, XGridRight - 5, labelY, XGridRight, labelY);
                    }
                }

            if (isGridShown)
            {
                // Vertical lines
                string date = chartData.Time[firstBar].ToString("dd.MM") + " " + chartData.Time[firstBar].ToString("HH:mm");
                int dateWidth = (int)g.MeasureString(date, font).Width;
                for (int vertLineBar = lastBar; vertLineBar > firstBar; vertLineBar -= (dateWidth + 10) / barPixels + 1)
                {
                    int XVertLine = spcLeft + (vertLineBar - firstBar) * barPixels + barPixels / 2 - 1;
                    g.DrawLine(penGrid, XVertLine, topSpace, XVertLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int indicatorValueShift = 1;
            foreach (ListParam listParam in chartData.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
            {
                if (component.ChartType == IndChartType.Histogram)
                {   // Histogram
                    double zero = 0;
                    if (zero < minValue) zero = minValue;
                    if (zero > maxValue) zero = maxValue;
                    int y0 = (int)(pnl.ClientSize.Height - 5 - (zero - minValue) * scale);

                    Rectangle rect;
                    LinearGradientBrush lgBrush;
                    Pen penGreen = new Pen(LayoutColors.ColorTradeLong);
                    Pen penRed = new Pen(LayoutColors.ColorTradeShort);

                    bool isPrevBarGreen = false;

                    if (isTrueChartsShown)
                    {   // True Chart Histogram
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels / 2 - 1;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penGreen, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penRed, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penRed, x, y0 - 2, x, y);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels - 4;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    isPrevBarGreen = true;
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    isPrevBarGreen = false;
                                }
                            }

                        }
                        else
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels / 2 - 1;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penGreen, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penRed, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penRed, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                        }
                    }

                    if (!isTrueChartsShown)
                    {   // Regular Histogram Chart
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - firstBar) * barPixels + spcLeft + 1;
                            int y = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (value - minValue) * scale);

                            if (value > prevValue || value == prevValue && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 4, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = true;
                            }
                            else
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 4, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = false;
                            }
                        }
                    }
                }

                if (component.ChartType == IndChartType.Line)
                {   // Line
                    if (isTrueChartsShown)
                    {   // True Charts
                        Pen pen = new Pen(component.ChartColor);
                        Pen penTC = new Pen(component.ChartColor);
                        penTC.DashStyle = DashStyle.Dash;
                        penTC.DashPattern = new float[] { 2, 1 };

                        int YIndChart = pnl.ClientSize.Height - bottomSpace;

                        Point[] point = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = spcLeft + (bar - firstBar) * barPixels + 1 + indicatorValueShift * (barPixels - 5);
                            int y = (int)Math.Round(YIndChart - (value - minValue) * scale);

                            point[bar - firstBar] = new Point(x, y);
                        }

                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {   // All bars except the last one
                            int i = bar - firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == firstBar && isIndicatorValueAtClose)
                            {   // First bar
                                double value = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels;
                                int y = (int)Math.Round(YIndChart - (value - minValue) * scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (barPixels > 3)
                                {   // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 7, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 6, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                }
                                if (deltaY > 4)
                                {   // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTC, x + barPixels - 4, y + 2, x + barPixels - 4, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTC, x + barPixels - 4, y - 2, x + barPixels - 4, point[i].Y + 2);
                                }
                            }

                            if (bar < lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (barPixels > 3)
                                {   // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                    else
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                }
                                if (deltaY > 4)
                                {   // Vertical part
                                    if (point[i + 1].Y > point[i].Y)
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X, point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X, point[i + 1].Y + 2);
                                }
                            }

                            if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                            {   // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 5, point[i].Y);
                            }
                        }
                    }

                    if (!isTrueChartsShown)
                    {   // Regular Line Chart
                        Point[] points = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = component.Value[bar];
                            int x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                            int y = (int)(pnl.ClientSize.Height - bottomSpace - (dValue - minValue) * scale);
                            points[bar - firstBar] = new Point(x, y);
                        }
                        g.DrawLines(new Pen(component.ChartColor), points);
                        
                    }
                }
            }

            // Vertical cross line
            if (isCrossShown && (isMouseInIndicatorChart || isMouseInPriceChart) && mouseX > XLeft - 1 && mouseX < XRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);

            // Chart title
            Indicator indicator = Indicator_Store.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName, chartData.Strategy.Slot[slot].SlotType);
            indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
            string title = indicator.ToString();
            Size sizeTitle = g.MeasureString(title, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(title, Font, brushFore, spcLeft + 2, 0);

            return;
        }

        /// <summary>
        ///  Invalidates the panels
        /// </summary>
        void InvalidateAllPanels()
        {
            pnlPrice.Invalidate();
            foreach (Panel panel in pnlInd)
                panel.Invalidate();

            return;
        }

        /// <summary>
        /// Sets the width of the info panel
        /// </summary>
        void SetupDynInfoWidth()
        {
            asInfoTitle = new string[200];
            aiInfoType  = new int[200];
            infoRows   = 0;

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Open lots");
            asInfoTitle[infoRows++] = Language.T("Position price");

            for (int iSlot = 0; iSlot < chartData.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = chartData.Strategy.Slot[iSlot].IndicatorName +
                    (chartData.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            Graphics g = CreateGraphics();

            int iMaxLenght = 0;
            foreach (string str in asInfoTitle)
            {
                int iLenght = (int)g.MeasureString(str, fontDI).Width;
                if (iMaxLenght < iLenght) iMaxLenght = iLenght;
            }

            XDynInfoCol2 = iMaxLenght + 10;
            int maxInfoWidth = (int)g.MeasureString("99/99/99     ", fontDI).Width;

            g.Dispose();

            dynInfoWidth = XDynInfoCol2 + maxInfoWidth + (isDEBUG ? 30 : 5);

            pnlInfo.ClientSize = new Size(dynInfoWidth, pnlInfo.ClientSize.Height);

            return;
        }

        /// <summary>
        /// Sets the dynamic info panel
        /// </summary>
        void SetupDynamicInfo()
        {
            asInfoTitle = new string[200];
            aiInfoType  = new int[200];
            infoRows = 0;

            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Open lots");
            asInfoTitle[infoRows++] = Language.T("Position price");

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) 
                        compToShow++;

                if (compToShow == 0) 
                    continue;

                asInfoTitle[infoRows++] = "";
                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = chartData.Strategy.Slot[slot].IndicatorName +
                    (chartData.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");

                foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) 
                        asInfoTitle[infoRows++] = indComp.CompName;
            }

            return;
        }

        /// <summary>
        /// Generates the DynamicInfo.
        /// </summary>
        void GenerateDynamicInfo(int barNumb)
        {
            if (!isInfoPanelShown) return;

            int bar;

            if (barNumb != chartData.Bars - 1)
            {
                barNumb = Math.Max(0, barNumb);
                barNumb = Math.Min(chartBars - 1, barNumb);

                bar = firstBar + barNumb;
                bar = Math.Min(chartData.Bars - 1, bar);
            }
            else
                bar = barNumb;

            if (barOld == bar && bar != lastBar) return;
            barOld = bar;

            int row = 0;
            asInfoValue = new String[200];
            asInfoValue[row++] = chartData.Time[bar].ToString(Data.DF);
            asInfoValue[row++] = chartData.Time[bar].ToString("HH:mm");
            if (isDEBUG)
            {
                asInfoValue[row++] = chartData.Open[bar].ToString();
                asInfoValue[row++] = chartData.High[bar].ToString();
                asInfoValue[row++] = chartData.Low[bar].ToString();
                asInfoValue[row++] = chartData.Close[bar].ToString();
            }
            else
            {

                asInfoValue[row++] = chartData.Open[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.High[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.Low[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.Close[bar].ToString(Data.FF);
            }
            asInfoValue[row++] = chartData.Volume[bar].ToString();

            asInfoValue[row++] = "";
            DateTime baropen = chartData.Time[bar];
            if (chartData.BarStatistics.ContainsKey(baropen))
            {
                asInfoValue[row++] = Language.T(chartData.BarStatistics[baropen].PositionDir.ToString());
                asInfoValue[row++] = chartData.BarStatistics[baropen].PositionLots.ToString();
                asInfoValue[row++] = chartData.BarStatistics[baropen].PositionPrice.ToString(Data.FF);
            }
            else
            {
                asInfoValue[row++] = Language.T("Square");
                asInfoValue[row++] = "   -";
                asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    asInfoValue[row++] = "";
                    asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataTipe = indComp.DataType;
                            if (indDataTipe == IndComponentType.AllowOpenLong  ||
                                indDataTipe == IndComponentType.AllowOpenShort ||
                                indDataTipe == IndComponentType.ForceClose     ||
                                indDataTipe == IndComponentType.ForceCloseLong ||
                                indDataTipe == IndComponentType.ForceCloseShort)
                                asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (isDEBUG)
                                {
                                    asInfoValue[row++] = indComp.Value[bar].ToString();
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFR = dl < 10 ? "F5" : dl < 100 ? "F4" : dl < 1000 ? "F3" : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (indComp.Value[bar] != 0)
                                        asInfoValue[row++] = indComp.Value[bar].ToString(sFR);
                                    else
                                        asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            pnlInfo.Invalidate(new Rectangle(XDynInfoCol2, 0, dynInfoWidth - XDynInfoCol2, pnlInfo.ClientSize.Height));

            return;
        }

        /// <summary>
        /// Paints the panel PnlInfo.
        /// </summary>
        void PnlInfo_Paint(object sender, PaintEventArgs e)
        {
            if (!isInfoPanelShown) return;

            Graphics g = e.Graphics;
            try { g.Clear(LayoutColors.ColorControlBack); }
            catch { }

            int iRowHeight = fontDI.Height + 1;
            Size size = new Size(dynInfoWidth, iRowHeight);

            for (int i = 0; i < infoRows; i++)
            {
                Point point0 = new Point(0, i * iRowHeight + 1);
                Point point1 = new Point(5, i * iRowHeight);
                Point point2 = new Point(XDynInfoCol2, i * iRowHeight);

                if (i % 2f != 0)
                    g.FillRectangle(brushEvenRows, new Rectangle(point0, size));

                if (aiInfoType[i + dynInfoScrollValue] == 1)
                    g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDIInd, brushDIIndicator, point1);
                else
                    g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDI, brushDynamicInfo, point1);
                g.DrawString(asInfoValue[i + dynInfoScrollValue], fontDI, brushDynamicInfo, point2);
            }

            return;
        }

        /// <summary>
        /// Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        void PnlPrice_MouseMove(object sender, MouseEventArgs e)
        {
            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (mouseX > XRight)
                {
                    if (mouseY > mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();

                    return;
                }
                else
                {
                    int newScrollValue = scroll.Value;

                    if (mouseX > mouseXOld)
                        newScrollValue -= (int)(scroll.SmallChange * 0.1 * (100 - barPixels));
                    else if (mouseX < mouseXOld)
                        newScrollValue += (int)(scroll.SmallChange * 0.1 * (100 - barPixels));

                    if (newScrollValue < scroll.Minimum)
                        newScrollValue = scroll.Minimum;
                    else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                        newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                    scroll.Value = newScrollValue;
                }
            }

            // Determines the shown bar.
            int shownBar = lastBar;
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                // Moving inside the chart
                if (mouseX >= XLeft && mouseX <= XRight)
                {
                    isMouseInPriceChart = true;
                    isDrawDinInfo = true;
                    shownBar = (e.X - XLeft) / barPixels;
                    if (isCrossShown)
                        pnlPrice.Cursor = Cursors.Cross;
                }
                // Escaping from the chart
                else
                {
                    isMouseInPriceChart = false;
                    shownBar = lastBar;
                    pnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= XLeft && mouseX <= XRight)
            {   // Entering into the chart
                isMouseInPriceChart = true;
                isDrawDinInfo = true;
                shownBar = (e.X - XLeft) / barPixels;
                if (isCrossShown)
                    pnlPrice.Cursor = Cursors.Cross;
            }

            if (!isCrossShown)
                return;

            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                if (mouseYOld >= YTop && mouseYOld <= YBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, mouseYOld, pnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(XRight - 1, mouseYOld - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
                } 
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
            }

            // Adding the new positions
            if (mouseX >= XLeft && mouseX <= XRight)
            {
                if (mouseY >= YTop && mouseY <= YBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, mouseY, pnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(XRight - 1, mouseY - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
                }  
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseX, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseX - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
            }
            pnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
            {
                GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                    path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));
                if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                    path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlInd[i].ClientSize.Height));
                pnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);

            return;
        }

        /// <summary>
        /// Deletes the cross and Dynamic Info
        /// </summary>
        void PnlPrice_MouseLeave(object sender, EventArgs e)
        {
            pnlPrice.Cursor = Cursors.Default;
            isMouseInPriceChart = false;

            if (!isCrossShown)
                return;
            
            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            // Horizontal Line
            path.AddRectangle(new Rectangle(0, mouseYOld, pnlPrice.ClientSize.Width, 1));
            // PriceBox
            path.AddRectangle(new Rectangle(XRight - 1, mouseYOld - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
            // Vertical Line
            path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
            // DateBox
            path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));

            pnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
                pnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));

            if (isInfoPanelShown)
                GenerateDynamicInfo(lastBar);

            return;
        }

        /// <summary>
        /// Mouse moves inside a chart
        /// </summary>
        void PnlInd_MouseMove(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = scroll.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            // Determines the shown bar.
            int shownBar = lastBar;
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                if (mouseX >= XLeft && mouseX <= XRight)
                {   // Moving inside the chart
                    isMouseInIndicatorChart = true;
                    isDrawDinInfo = true;
                    shownBar = (e.X - XLeft) / barPixels;
                    if (isCrossShown)
                        panel.Cursor = Cursors.Cross;
                }
                else
                {   // Escaping from the bar area of chart
                    isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                    shownBar = lastBar;
                }
            }
            else if (mouseX >= XLeft && mouseX <= XRight)
            {   // Entering into the chart
                isMouseInIndicatorChart = true;
                isDrawDinInfo = true;
                pnlInfo.Invalidate();
                shownBar = (e.X - XLeft) / barPixels;
                if (isCrossShown)
                    panel.Cursor = Cursors.Cross;
            }

            if (!isCrossShown)
                return;
            
            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
            }

            // Adding the new positions
            if (mouseX >= XLeft && mouseX <= XRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseX, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseX - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
            }
            pnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
            {
                GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                    path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));
                if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                    path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlInd[i].ClientSize.Height));
                pnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);

            return;
        }

        /// <summary>
        /// Mouse leaves a chart.
        /// </summary>
        void PnlInd_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.Cursor = Cursors.Default;

            isMouseInIndicatorChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            if (isCrossShown)
            {
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));

                pnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels; i++)
                    pnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));
            }

            return;
        }

        /// <summary>
        /// Mouse Button Up
        /// </summary>
        void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.Cursor = isCrossShown ? Cursors.Cross : Cursors.Default;
            scroll.Focus();
        }

        /// <summary>
        /// Mouse Button Down
        /// </summary>
        void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            if (panel == pnlPrice && mouseX > XRight)
                panel.Cursor = Cursors.SizeNS;
            else  if (!isCrossShown)
                panel.Cursor = Cursors.SizeWE;

            return;
        }

        /// <summary>
        /// Sets the parameters when scrolling.
        /// </summary>
        void Scroll_ValueChanged(object sender, EventArgs e)
        {
            firstBar = scroll.Value;
            lastBar = Math.Min(chartData.Bars - 1, firstBar + chartBars - 1);
            lastBar = Math.Max(lastBar, firstBar);

            SetPriceChartMinMaxValues();
            for (int i = 0; i < pnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);

            InvalidateAllPanels();

            barOld = 0;
            if (isInfoPanelShown && isDrawDinInfo && isCrossShown)
            {
                int selectedBar = (mouseX - spcLeft) / barPixels;
                GenerateDynamicInfo(selectedBar);
            }

            return;
        }

        /// <summary>
        /// Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        void Scroll_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isCtrlKeyPressed)
            {
                if (e.Delta > 0)
                    ZoomIn();
                if (e.Delta < 0)
                    ZoomOut();
            }
            else
            {
                int newScrollValue = scroll.Value + scroll.LargeChange * e.Delta / SystemInformation.MouseWheelScrollLines / 120;

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        void Scroll_KeyUp(object sender, KeyEventArgs e)
        {
            isCtrlKeyPressed = false;

            ShortcutKeyUp(e);

            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        void Scroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                isCtrlKeyPressed = true;

            return;
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        void ButtonChart_Click(object sender, EventArgs e)
        {
            ToolStripButton tsButton = (ToolStripButton)sender;
            ChartButtons buton = (ChartButtons)tsButton.Tag;

            switch (buton)
            {
                case ChartButtons.Grid:
                    ShortcutKeyUp(new KeyEventArgs(Keys.G));
                    break;
                case ChartButtons.Cross:
                    ShortcutKeyUp(new KeyEventArgs(Keys.C));
                    break;
                case ChartButtons.Volume:
                    ShortcutKeyUp(new KeyEventArgs(Keys.V));
                    break;
                case ChartButtons.Orders:
                    ShortcutKeyUp(new KeyEventArgs(Keys.O));
                    break;
                case ChartButtons.PositionLots:
                    ShortcutKeyUp(new KeyEventArgs(Keys.L));
                    break;
                case ChartButtons.PositionPrice:
                    ShortcutKeyUp(new KeyEventArgs(Keys.P));
                    break;
                case ChartButtons.ZoomIn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Add));
                    break;
                case ChartButtons.ZoomOut:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Subtract));
                    break;
                case ChartButtons.Refresh:
                    ShortcutKeyUp(new KeyEventArgs(Keys.F5));
                    break;
                case ChartButtons.TrueCharts:
                    ShortcutKeyUp(new KeyEventArgs(Keys.T));
                    break;
                case ChartButtons.Shift:
                    ShortcutKeyUp(new KeyEventArgs(Keys.S));
                    break;
                case ChartButtons.AutoScroll:
                    ShortcutKeyUp(new KeyEventArgs(Keys.R));
                    break;
                case ChartButtons.DInfoDwn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Z));
                    break;
                case ChartButtons.DInfoUp:
                    ShortcutKeyUp(new KeyEventArgs(Keys.A));
                    break;
                case ChartButtons.DynamicInfo:
                    ShortcutKeyUp(new KeyEventArgs(Keys.I));
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Shortcut keys
        /// </summary>
        void ShortcutKeyUp(KeyEventArgs e)
        {
            // Zoom in
            if (!e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                ZoomIn();
            }
            // Zoom out
            else if (!e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                ZoomOut();
            }
            // Vertical scale increase
            else if (e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                VerticalScaleIncrease();
            }
            // Vertical scale decrease
            else if (e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                VerticalScaleDecrease();
            }
            else if (e.KeyCode == Keys.Space)
            {
                isCandleChart = !isCandleChart;
                pnlPrice.Invalidate();
            }
            // Refresh
            else if (e.KeyCode == Keys.F5)
            {
                SetFirstLastBar();
                SetPriceChartMinMaxValues();
                for (int i = 0; i < pnlInd.Length; i++)
                    SetSepChartsMinMaxValues(i);
                InvalidateAllPanels();
            }
            // Grid
            else if (e.KeyCode == Keys.G)
            {
                isGridShown = !isGridShown;
                Configs.ChartGrid = isGridShown;
                chartButtons[(int)ChartButtons.Grid].Checked = isGridShown;
                InvalidateAllPanels();
            }
            // Cross
            else if (e.KeyCode == Keys.C)
            {
                isCrossShown = !isCrossShown;
                Configs.ChartCross = isCrossShown;
                chartButtons[(int)ChartButtons.Cross].Checked = isCrossShown;
                InvalidateAllPanels();
                if (isCrossShown)
                {
                    GenerateDynamicInfo((mouseX - XLeft) / barPixels);
                    pnlPrice.Cursor = Cursors.Cross;
                    foreach (Panel pnlind in pnlInd)
                        pnlind.Cursor = Cursors.Cross;
                }
                else
                {
                    GenerateDynamicInfo(chartData.Bars - 1);
                    pnlPrice.Cursor = Cursors.Default;
                    foreach (Panel pnlind in pnlInd)
                        pnlind.Cursor = Cursors.Default;
                }
            }
            // Volume
            else if (e.KeyCode == Keys.V)
            {
                isVolumeShown = !isVolumeShown;
                Configs.ChartVolume = isVolumeShown;
                chartButtons[(int)ChartButtons.Volume].Checked = isVolumeShown;
                SetPriceChartMinMaxValues();
                pnlPrice.Invalidate();
            }
            // Lots
            else if (e.KeyCode == Keys.L)
            {
                isPositionLotsShown = !isPositionLotsShown;
                Configs.ChartLots = isPositionLotsShown;
                chartButtons[(int)ChartButtons.PositionLots].Checked = isPositionLotsShown;
                SetPriceChartMinMaxValues();
                pnlPrice.Invalidate();
            }
            // Orders
            else if (e.KeyCode == Keys.O)
            {
                isOrdersShown = !isOrdersShown;
                Configs.ChartOrders = isOrdersShown;
                chartButtons[(int)ChartButtons.Orders].Checked = isOrdersShown;
                pnlPrice.Invalidate();
            }
            // Position price
            else if (e.KeyCode == Keys.P)
            {
                isPositionPriceShown = !isPositionPriceShown;
                Configs.ChartPositionPrice = isPositionPriceShown;
                chartButtons[(int)ChartButtons.PositionPrice].Checked = isPositionPriceShown;
                pnlPrice.Invalidate();
            }
            // True Charts
            else if (e.KeyCode == Keys.T)
            {
                isTrueChartsShown = !isTrueChartsShown;
                Configs.ChartTrueCharts = isTrueChartsShown;
                chartButtons[(int)ChartButtons.TrueCharts].Checked = isTrueChartsShown;
                InvalidateAllPanels();
            }
            // Chart shift
            else if (e.KeyCode == Keys.S)
            {
                isChartShift = !isChartShift;
                Configs.ChartShift = isChartShift;
                chartButtons[(int)ChartButtons.Shift].Checked = isChartShift;
                SetFirstLastBar();
            }
            // Chart Auto Scroll
            else if (e.KeyCode == Keys.R)
            {
                isChartAutoScroll = !isChartAutoScroll;
                Configs.ChartAutoScroll = isChartAutoScroll;
                chartButtons[(int)ChartButtons.AutoScroll].Checked = isChartAutoScroll;
                SetFirstLastBar();
            }
            // Dynamic info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                dynInfoScrollValue += 5;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                pnlInfo.Invalidate();
            }
            // Dynamic info scroll up
            else if (e.KeyCode == Keys.A)
            {
                dynInfoScrollValue -= 5;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                pnlInfo.Invalidate();
            }
            // Show info panel
            else if (e.KeyCode == Keys.I || e.KeyCode == Keys.F2)
            {
                isInfoPanelShown = !isInfoPanelShown;
                Configs.ChartInfoPanel = isInfoPanelShown;
                pnlInfo.Visible = isInfoPanelShown;
                pnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 4, 0) : new Padding(0);
                chartButtons[(int)ChartButtons.DInfoUp].Visible  = isInfoPanelShown;
                chartButtons[(int)ChartButtons.DInfoDwn].Visible = isInfoPanelShown;
            }
            // Debug
            else if (e.KeyCode == Keys.F12)
            {
                isDEBUG = !isDEBUG;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                pnlInfo.Invalidate();
            }

            return;
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        void VerticalScaleDecrease()
        {
            if (verticalScale > 10)
            {
                verticalScale -= 10;
                SetPriceChartMinMaxValues();
                pnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        void VerticalScaleIncrease()
        {
            if (verticalScale < 300)
            {
                verticalScale += 10;
                SetPriceChartMinMaxValues();
                pnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Zooms the chart in.
        /// </summary>
        void ZoomIn()
        {
            barPixels += 4;
            if (barPixels > 45)
                barPixels = 45;
            
            int oldChartBars = chartBars;

            chartBars = chartWidth / barPixels;
            if (chartBars > chartData.Bars - chartData.FirstBar)
                chartBars = chartData.Bars - chartData.FirstBar;

            if (lastBar < chartData.Bars - 1)
            {
                firstBar += (oldChartBars - chartBars) / 2;
                if (firstBar > chartData.Bars - chartBars)
                    firstBar = chartData.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < pnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = barPixels;

            return;
        }

        /// <summary>
        /// Zooms the chart out.
        /// </summary>
        void ZoomOut()
        {
            barPixels -= 4;
            if (barPixels < 9)
                barPixels = 9;

            int oldChartBars = chartBars;

            chartBars = chartWidth / barPixels;
            if (chartBars > chartData.Bars - chartData.FirstBar)
                chartBars = chartData.Bars - chartData.FirstBar;

            if (lastBar < chartData.Bars - 1)
            {
                firstBar -= (chartBars - oldChartBars) / 2;
                if (firstBar < chartData.FirstBar)
                    firstBar = chartData.FirstBar;

                if (firstBar > chartData.Bars - chartBars)
                    firstBar = chartData.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < pnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = barPixels;

            return;
        }
    }
}
