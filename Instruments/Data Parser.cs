// Data_Parser Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Forex_Strategy_Trader
{
    public class Data_Parser
    {
        int    iBars;
        Bar[]  aBar;
        string sInput;
        string sParsingErrorMessage;
        string sGeneralDataRowMatchPattern;
        string sNumberDecimalSeparator;
        string sColumnDelimiter;
        string sTimeMatchPattern;
        string sDateSeparator;
        string sDateMatchPattern;
        string sPriceMatchPattern;
        string sDataRowMatchPattern;
        bool   bIsSeconds;
        bool   bIsVolumeColumn;
        bool   bIsFileMatchPattern;

        /// <summary>
        /// Gets the count of the data bars
        /// </summary>
        public int Bars
        {
            get { return iBars; }
        }

        /// <summary>
        /// Gets the the data array
        /// </summary>
        public Bar[] Bar
        {
            get { return aBar; }
        }

        /// <summary>
        /// Gets the parsing error message
        /// </summary>
        public string ParsingErrorMessage
        {
            get { return sParsingErrorMessage; }
        }

        /// <summary>
        /// Gets or sets the general data row match pattern
        /// </summary>
        string GeneralDataRowMatchPattern
        {
            get { return sGeneralDataRowMatchPattern; }
            set { sGeneralDataRowMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the number decimal separator
        /// </summary>
        string NumberDecimalSeparator
        {
            get { return sNumberDecimalSeparator; }
            set { sNumberDecimalSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the column delimiter
        /// </summary>
        string ColumnDelimiter
        {
            get { return sColumnDelimiter; }
            set { sColumnDelimiter = value; }
        }

        /// <summary>
        /// Gets or sets the time match pattern
        /// </summary>
        string TimeMatchPattern
        {
            get { return sTimeMatchPattern; }
            set { sTimeMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the date separator
        /// </summary>
        string DateSeparator
        {
            get { return sDateSeparator; }
            set { sDateSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the date match pattern
        /// </summary>
        string DateMatchPattern
        {
            get { return sDateMatchPattern; }
            set { sDateMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the price match pattern
        /// </summary>
        string PriceMatchPattern
        {
            get { return sPriceMatchPattern; }
            set { sPriceMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the data row match pattern
        /// </summary>
        string DataRowMatchPattern
        {
            get { return sDataRowMatchPattern; }
            set { sDataRowMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets whether a seconds info present
        /// </summary>
        bool IsSeconds
        {
            get { return bIsSeconds; }
            set { bIsSeconds = value; }
        }

        /// <summary>
        /// Gets or sets whether a volume column present
        /// </summary>
        bool IsVolumeColumn
        {
            get { return bIsVolumeColumn; }
            set { bIsVolumeColumn = value; }
        }

        /// <summary>
        /// Gets or sets whether the file matches the pattern
        /// </summary>
        bool IsFileMatchPattern
        {
            get { return bIsFileMatchPattern; }
            set { bIsFileMatchPattern = value; }
        }

        /// <summary>
        /// Analyzes and parses an input string
        /// </summary>
        /// <param name="sInput"></param>
        public Data_Parser(string sInput)
        {
            this.sInput = sInput;
        }

        /// <summary>
        /// Parses the input string
        /// </summary>
        public int Parse()
        {
            int iReturn = 0;

            try
            {
                iReturn = AnaliseInput();
            }
            catch (Exception e)
            {
                iReturn = -1;
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage + Environment.NewLine + e.Message,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

            if (iReturn != 0)
            {
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                return iReturn;
            }

            try
            {
                iReturn = ParseInput();
            }
            catch (Exception e)
            {
                iReturn = -1;
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage + "\r\n" + e.Message,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return iReturn;
            }

            if (iReturn != 0)
            {
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                return iReturn;
            }

            return iReturn;
        }

        /// <summary>
        /// Analyzes the input file
        /// </summary>
        int AnaliseInput()
        {
            GeneralDataRowMatchPattern = @"^[\t ;,]*\d{1,4}[\./-]\d{1,4}[\./-]\d{1,4}[\t ;,]+\d{2}(:\d{2}){1,2}([\t ;,]+\d+[\.,]\d+){4}([\t ;,]+\d{1,10})?";
            Regex regexGeneralDataRowMatchPattern = new Regex(GeneralDataRowMatchPattern, RegexOptions.Compiled);

            // Takes a data line
            string sDataLine = null;
            StringReader sr = new StringReader(sInput);
            while ((sDataLine = sr.ReadLine()) != null)
            {
                if (regexGeneralDataRowMatchPattern.IsMatch(sDataLine))
                    break;
            }
            sr.Close();

            if (sDataLine == null)
            {
                sParsingErrorMessage = Language.T("Could not recognize the data file format!");
                return -1;
            }

            // Number decimal separator
            if (Regex.IsMatch(sDataLine, @"([\t, ;]+\d+\.\d+){4}"))
                NumberDecimalSeparator = @"\.";
            else if (Regex.IsMatch(sDataLine, @"([\t ;]+\d+,\d+){4}"))
                NumberDecimalSeparator = @",";
            else
            {
                sParsingErrorMessage = Language.T("Could not determine the number decimal separator!");
                return -1;
            }

            // Column delimiter
            if (NumberDecimalSeparator == @"\.")
                ColumnDelimiter = @"[\t, ;]";
            else if (sNumberDecimalSeparator == @",")
                ColumnDelimiter = @"[\t ;]";

            // Time format
            if (Regex.IsMatch(sDataLine, ColumnDelimiter + @"\d{2}:\d{2}" + ColumnDelimiter))
            {
                TimeMatchPattern = @"(?<hour>\d{2}):(?<min>\d{2})";
                IsSeconds = false;
            }
            else if (Regex.IsMatch(sDataLine, ColumnDelimiter + @"\d{2}:\d{2}:\d{2}" + ColumnDelimiter))
            {
                TimeMatchPattern = @"(?<hour>\d{2}):(?<min>\d{2}):(?<sec>\d{2})";
                IsSeconds = true;
            }
            else
            {
                sParsingErrorMessage = Language.T("Could not determine the time format!");
                return -1;
            }

            // Date separator
            if (Regex.IsMatch(sDataLine, @"\d{1,4}\.\d{1,4}\.\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"\.";
            else if (Regex.IsMatch(sDataLine, @"\d{1,4}/\d{1,4}/\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"/";
            else if (Regex.IsMatch(sDataLine, @"\d{1,4}-\d{1,4}-\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"-";
            else
            {
                sParsingErrorMessage = Language.T("Could not determine the date separator!");
                return -1;
            }

            // Date format
            string sLine;
            int iYearPos  = 0;
            int iMonthPos = 0;
            int iDayPos   = 0;
            Regex regexGeneralDataPattern = new Regex(@"(?<1>\d{1,4})" + DateSeparator + @"(?<2>\d{1,4})" + DateSeparator + @"(?<3>\d{1,4})", RegexOptions.Compiled);
            sr = new StringReader(sInput);
            while ((sLine = sr.ReadLine()) != null)
            {
                Match mDate = regexGeneralDataPattern.Match(sLine);

                if (!mDate.Success)
                    continue;

                int iDate1 = int.Parse(mDate.Result("$1"));
                int iDate2 = int.Parse(mDate.Result("$2"));
                int iDate3 = int.Parse(mDate.Result("$3"));

                // Determines the year index
                if (iYearPos == 0)
                {
                    if (iDate1 > 31) iYearPos = 1;
                    else if (iDate2 > 31) iYearPos = 2;
                    else if (iDate3 > 31) iYearPos = 3;
                }

                // Determines the day index
                if (iDayPos == 0 && iYearPos > 0)
                {
                    if (iYearPos == 1)
                    {
                        if (iDate2 > 12) iDayPos = 2;
                        else if (iDate3 > 12) iDayPos = 3;
                    }
                    else if (iYearPos == 2)
                    {
                        if (iDate1 > 12) iDayPos = 1;
                        else if (iDate3 > 12) iDayPos = 3;
                    }
                    else if (iYearPos == 3)
                    {
                        if (iDate1 > 12) iDayPos = 1;
                        else if (iDate2 > 12) iDayPos = 2;
                    }
                }

                // Determines the month index
                if (iDayPos > 0 && iYearPos > 0)
                {
                    if (iYearPos != 1 && iDayPos != 1)
                        iMonthPos = 1;
                    else if (iYearPos != 2 && iDayPos != 2)
                        iMonthPos = 2;
                    else if (iYearPos != 3 && iDayPos != 3)
                        iMonthPos = 3;
                }

                if (iYearPos > 0 && iMonthPos > 0 && iDayPos > 0)
                    break;
            }
            sr.Close();

            // If the date format is not recognized we try to find the number of changes.
            if (iYearPos == 0 || iMonthPos == 0 || iDayPos == 0)
            {
                int iDateOld1 = 0;
                int iDateOld2 = 0;
                int iDateOld3 = 0;

                int iDateChanges1 = -1;
                int iDateChanges2 = -1;
                int iDateChanges3 = -1;

                sr = new StringReader(sInput);
                while ((sLine = sr.ReadLine()) != null)
                {
                    Match mDate = regexGeneralDataPattern.Match(sLine);

                    if (!mDate.Success)
                        continue;

                    int iDate1 = int.Parse(mDate.Result("$1"));
                    int iDate2 = int.Parse(mDate.Result("$2"));
                    int iDate3 = int.Parse(mDate.Result("$3"));

                    if (iDate1 != iDateOld1)
                    {   // iDate1 has changed
                        iDateOld1 = iDate1;
                        iDateChanges1++;
                    }

                    if (iDate2 != iDateOld2)
                    {   // iDate2 has changed
                        iDateOld2 = iDate2;
                        iDateChanges2++;
                    }

                    if (iDate3 != iDateOld3)
                    {   // iDate2 has changed
                        iDateOld3 = iDate3;
                        iDateChanges3++;
                    }
                }
                sr.Close();

                if (iYearPos > 0)
                {   // The year position is known
                    if (iYearPos == 1)
                    {
                        if (iDateChanges3 > iDateChanges2)
                        {
                            iMonthPos = 2;
                            iDayPos   = 3;
                        }
                        else if (iDateChanges2 > iDateChanges3)
                        {
                            iMonthPos = 3;
                            iDayPos   = 2;
                        }
                    }
                    else if (iYearPos == 2)
                    {
                        if (iDateChanges3 > iDateChanges1)
                        {
                            iMonthPos = 1;
                            iDayPos   = 3;
                        }
                        else if (iDateChanges1 > iDateChanges3)
                        {
                            iMonthPos = 3;
                            iDayPos   = 1;
                        }
                    }
                    else if (iYearPos == 3)
                    {
                        if (iDateChanges2 > iDateChanges1)
                        {
                            iMonthPos = 1;
                            iDayPos   = 2;
                        }
                        else if (iDateChanges1 > iDateChanges2)
                        {
                            iMonthPos = 2;
                            iDayPos   = 1;
                        }
                    }
                }
                else
                {   // The year position is unknown
                    if (iDateChanges1 >= 0 && iDateChanges2 > iDateChanges1 && iDateChanges3 > iDateChanges2)
                    {
                        iYearPos  = 1;
                        iMonthPos = 2;
                        iDayPos   = 3;
                    }
                    else if (iDateChanges1 >= 0 && iDateChanges3 > iDateChanges1 && iDateChanges2 > iDateChanges3)
                    {
                        iYearPos  = 1;
                        iMonthPos = 3;
                        iDayPos   = 2;
                    }
                    else if (iDateChanges2 >= 0 && iDateChanges1 > iDateChanges2 && iDateChanges3 > iDateChanges1)
                    {
                        iYearPos  = 2;
                        iMonthPos = 1;
                        iDayPos   = 3;
                    }
                    else if (iDateChanges2 >= 0 && iDateChanges3 > iDateChanges2 && iDateChanges1 > iDateChanges3)
                    {
                        iYearPos  = 2;
                        iMonthPos = 3;
                        iDayPos   = 1;
                    }
                    else if (iDateChanges3 >= 0 && iDateChanges1 > iDateChanges3 && iDateChanges2 > iDateChanges1)
                    {
                        iYearPos  = 3;
                        iMonthPos = 1;
                        iDayPos   = 2;
                    }
                    else if (iDateChanges3 >= 0 && iDateChanges2 > iDateChanges3 && iDateChanges1 > iDateChanges2)
                    {
                        iYearPos  = 3;
                        iMonthPos = 2;
                        iDayPos   = 1;
                    }
                }
            }

            if (iYearPos * iMonthPos * iDayPos > 0)
            {
                if (iYearPos == 1 && iMonthPos == 2 && iDayPos == 3)
                    DateMatchPattern = @"(?<year>\d{1,4})"  + DateSeparator + @"(?<month>\d{1,4})" + DateSeparator + @"(?<day>\d{1,4})";
                else if (iYearPos == 1 && iMonthPos == 3 && iDayPos == 2)
                    DateMatchPattern = @"(?<year>\d{1,4})"  + DateSeparator + @"(?<day>\d{1,4})"   + DateSeparator + @"(?<month>\d{1,4})";
                else if (iYearPos == 2 && iMonthPos == 1 && iDayPos == 3)
                    DateMatchPattern = @"(?<month>\d{1,4})" + DateSeparator + @"(?<year>\d{1,4})"  + DateSeparator + @"(?<day>\d{1,4})";
                else if (iYearPos == 2 && iMonthPos == 3 && iDayPos == 1)
                    DateMatchPattern = @"(?<day>\d{1,4})"   + DateSeparator + @"(?<year>\d{1,4})"  + DateSeparator + @"(?<month>\d{1,4})";
                else if (iYearPos == 3 && iMonthPos == 1 && iDayPos == 2)
                    DateMatchPattern = @"(?<month>\d{1,4})" + DateSeparator + @"(?<day>\d{1,4})"   + DateSeparator + @"(?<year>\d{1,4})";
                else if (iYearPos == 3 && iMonthPos == 2 && iDayPos == 1)
                    DateMatchPattern = @"(?<day>\d{1,4})"   + DateSeparator + @"(?<month>\d{1,4})" + DateSeparator + @"(?<year>\d{1,4})";
            }
            else
            {
                sParsingErrorMessage = Language.T("Could not determine the date format!");
                return -1;
            }

            // Price match pattern
            PriceMatchPattern = "";
            string sCurrentNumberDecimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            char cCurrentNumberDecimalSeparator = sCurrentNumberDecimalSeparator.ToCharArray()[0];
            char cNumberDecimalSeparator = sNumberDecimalSeparator.ToCharArray()[sNumberDecimalSeparator.ToCharArray().Length - 1];
            sr = new StringReader(sInput);
            while ((sLine = sr.ReadLine()) != null)
            {
                if (!regexGeneralDataRowMatchPattern.IsMatch(sLine))
                    continue;

                Match mPrice = Regex.Match(sLine,
                    ColumnDelimiter + @"+(?<1>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<2>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<3>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<4>\d+" + NumberDecimalSeparator + @"\d+)");

                string sPrice1 = mPrice.Result("$1").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice2 = mPrice.Result("$2").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice3 = mPrice.Result("$3").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice4 = mPrice.Result("$4").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);

                double dPrice1 = double.Parse(sPrice1);
                double dPrice2 = double.Parse(sPrice2);
                double dPrice3 = double.Parse(sPrice3);
                double dPrice4 = double.Parse(sPrice4);

                if (dPrice2 > dPrice1 + 0.00001 && dPrice2 > dPrice3 + 0.00001 && dPrice2 > dPrice4 + 0.00001 &&
                    dPrice3 < dPrice1 - 0.00001 && dPrice3 < dPrice2 - 0.00001 && dPrice3 < dPrice4 - 0.00001)
                {
                    PriceMatchPattern = @"(?<open>\d+" + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<high>\d+"  + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<low>\d+"   + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<close>\d+" + NumberDecimalSeparator + @"\d+)";
                    break;
                }
                if (dPrice3 > dPrice1 + 0.00001 && dPrice3 > dPrice2 + 0.00001 && dPrice3 > dPrice4 + 0.00001 &&
                    dPrice2 < dPrice1 - 0.00001 && dPrice2 < dPrice3 - 0.00001 && dPrice2 < dPrice4 - 0.00001)
                {
                    PriceMatchPattern = @"(?<open>\d+" + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<low>\d+"   + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<high>\d+"  + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<close>\d+" + NumberDecimalSeparator + @"\d+)";
                    break;
                }
            }
            sr.Close();

            if (PriceMatchPattern == "")
            {
                sParsingErrorMessage = Language.T("Could not determine the price columns order!");
                return -1;
            }

            // Check for a volume column
            IsVolumeColumn = Regex.IsMatch(sDataLine, PriceMatchPattern + ColumnDelimiter + @"+\d+" + ColumnDelimiter + "*$");

            DataRowMatchPattern = "^" + ColumnDelimiter + "*" + DateMatchPattern + ColumnDelimiter + "*" +
                TimeMatchPattern + ColumnDelimiter + "*" + PriceMatchPattern + ColumnDelimiter + "*" +
                (IsVolumeColumn ? @"(?<volume>\d+)" : "") + ColumnDelimiter + "*$";

            return 0;
        }

        /// <summary>
        /// Parses the input file
        /// </summary>
        int ParseInput()
        {
            iBars = 0;
            StringReader sr;
            string sLine;
            Match  mLine;
            string sCurrentNumberDecimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            char   cCurrentNumberDecimalSeparator = sCurrentNumberDecimalSeparator.ToCharArray()[0];
            char   cNumberDecimalSeparator   = sNumberDecimalSeparator.ToCharArray()[sNumberDecimalSeparator.ToCharArray().Length - 1];
            bool   bToChangeDecimalSeparator = cNumberDecimalSeparator != cCurrentNumberDecimalSeparator;
            Regex  rx = new Regex(DataRowMatchPattern, RegexOptions.Compiled);

            // Counts the data bars
            iBars = 0;
            sr = new StringReader(sInput);
            while ((sLine = sr.ReadLine()) != null)
                if (rx.IsMatch(sLine))
                    iBars++;
            sr.Close();

            if (iBars == 0)
            {
                sParsingErrorMessage = Language.T("Could not count the data bars!");
                return -1;
            }

            int iBar = 0;
            aBar = new Bar[iBars];
            sr   = new StringReader(sInput);
            while ((sLine = sr.ReadLine()) != null)
            {
                mLine = rx.Match(sLine);
                if (mLine.Success)
                {
                    int iYear  = int.Parse(mLine.Groups["year"].Value);
                    int iMonth = int.Parse(mLine.Groups["month"].Value);
                    int iDay   = int.Parse(mLine.Groups["day"].Value);
                    int iHour  = int.Parse(mLine.Groups["hour"].Value);
                    int iMin   = int.Parse(mLine.Groups["min"].Value);
                    int iSec   = (IsSeconds ? int.Parse(mLine.Groups["sec"].Value) : 0);

                    if (bToChangeDecimalSeparator)
                    {
                        aBar[iBar].Time   = new DateTime(iYear, iMonth, iDay, iHour, iMin, iSec);
                        aBar[iBar].Open   = double.Parse(mLine.Groups["open"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[iBar].High   = double.Parse(mLine.Groups["high"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[iBar].Low    = double.Parse(mLine.Groups["low"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[iBar].Close  = double.Parse(mLine.Groups["close"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[iBar].Volume = (IsVolumeColumn ? int.Parse(mLine.Groups["volume"].Value) : 0);
                    }
                    else
                    {
                        aBar[iBar].Time   = new DateTime(iYear, iMonth, iDay, iHour, iMin, iSec);
                        aBar[iBar].Open   = double.Parse(mLine.Groups["open"].Value);
                        aBar[iBar].High   = double.Parse(mLine.Groups["high"].Value);
                        aBar[iBar].Low    = double.Parse(mLine.Groups["low"].Value);
                        aBar[iBar].Close  = double.Parse(mLine.Groups["close"].Value);
                        aBar[iBar].Volume = (IsVolumeColumn ? int.Parse(mLine.Groups["volume"].Value) : 0);
                    }

                    iBar++;
                }
            }

            sr.Close();

            return 0;
        }
    }
}
