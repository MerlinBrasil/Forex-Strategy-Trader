// MT4 Errors
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

namespace MT4Bridge
{
    public static class MT4_Errors
    {
        /// <summary>
        /// Return error description
        /// </summary>
        public static string ErrorDescription(int error_code)
        {
           string error_string;

           switch(error_code)
           {
              //---- codes returned from trade server
              case 0:
              case 1:   error_string="No error";                                                  break;
              case 2:   error_string="Common error";                                              break;
              case 3:   error_string="Invalid trade parameters";                                  break;
              case 4:   error_string="Trade server is busy";                                      break;
              case 5:   error_string="Old version of the client terminal";                        break;
              case 6:   error_string="No connection with trade server";                           break;
              case 7:   error_string="Not enough rights";                                         break;
              case 8:   error_string="Too frequent requests";                                     break;
              case 9:   error_string="Malfunctional trade operation (never returned error)";      break;
              case 64:  error_string="Account disabled";                                          break;
              case 65:  error_string="Invalid account";                                           break;
              case 128: error_string="Trade timeout";                                             break;
              case 129: error_string="Invalid price";                                             break;
              case 130: error_string="Invalid stops";                                             break;
              case 131: error_string="Invalid trade volume";                                      break;
              case 132: error_string="Market is closed";                                          break;
              case 133: error_string="Trade is disabled";                                         break;
              case 134: error_string="Not enough money";                                          break;
              case 135: error_string="Price changed";                                             break;
              case 136: error_string="Off quotes";                                                break;
              case 137: error_string="Broker is busy (never returned error)";                     break;
              case 138: error_string="Requote";                                                   break;
              case 139: error_string="Order is locked";                                           break;
              case 140: error_string="Long positions only allowed";                               break;
              case 141: error_string="Too many requests";                                         break;
              case 145: error_string="Modification denied because order too close to market";     break;
              case 146: error_string="Trade context is busy";                                     break;
              case 147: error_string="Expirations are denied by broker";                          break;
              case 148: error_string="Amount of open and pending orders has reached the limit";   break;
              case 149: error_string= "Opening of an opposite position (hedging) is disabled";    break;
              case 150: error_string= "An attempt to close a position contravening the FIFO rule";break; 
              //---- mql4 errors
              case 4000: error_string="No error (never generated code)";                          break;
              case 4001: error_string="Wrong function pointer";                                   break;
              case 4002: error_string="Array index is out of range";                              break;
              case 4003: error_string="No memory for function call stack";                        break;
              case 4004: error_string="Recursive stack overflow";                                 break;
              case 4005: error_string="Not enough stack for parameter";                           break;
              case 4006: error_string="No memory for parameter string";                           break;
              case 4007: error_string="No memory for temp string";                                break;
              case 4008: error_string="Not initialized string";                                   break;
              case 4009: error_string="Not initialized string in array";                          break;
              case 4010: error_string="No memory for array\' string";                             break;
              case 4011: error_string="Too long string";                                          break;
              case 4012: error_string="Remainder from zero divide";                               break;
              case 4013: error_string="Zero divide";                                              break;
              case 4014: error_string="Unknown command";                                          break;
              case 4015: error_string="Wrong jump (never generated error)";                       break;
              case 4016: error_string="Not initialized array";                                    break;
              case 4017: error_string="Dll calls are not allowed";                                break;
              case 4018: error_string="Cannot load library";                                      break;
              case 4019: error_string="Cannot call function";                                     break;
              case 4020: error_string="Expert function calls are not allowed";                    break;
              case 4021: error_string="Not enough memory for temp string returned from function"; break;
              case 4022: error_string="System is busy (never generated error)";                   break;
              case 4050: error_string="Invalid function parameters count";                        break;
              case 4051: error_string="Invalid function parameter value";                         break;
              case 4052: error_string="String function internal error";                           break;
              case 4053: error_string="Some array error";                                         break;
              case 4054: error_string="Incorrect series array using";                             break;
              case 4055: error_string="Custom indicator error";                                   break;
              case 4056: error_string="Arrays are incompatible";                                  break;
              case 4057: error_string="Global variables processing error";                        break;
              case 4058: error_string="Global variable not found";                                break;
              case 4059: error_string="Function is not allowed in testing mode";                  break;
              case 4060: error_string="Function is not confirmed";                                break;
              case 4061: error_string="Send mail error";                                          break;
              case 4062: error_string="String parameter expected";                                break;
              case 4063: error_string="Integer parameter expected";                               break;
              case 4064: error_string="Double parameter expected";                                break;
              case 4065: error_string="Array as parameter expected";                              break;
              case 4066: error_string="Requested history data in update state";                   break;
              case 4099: error_string="End of file";                                              break;
              case 4100: error_string="Some file error";                                          break;
              case 4101: error_string="Wrong file name";                                          break;
              case 4102: error_string="Too many opened files";                                    break;
              case 4103: error_string="Cannot open file";                                         break;
              case 4104: error_string="Incompatible access to a file";                            break;
              case 4105: error_string="No order selected";                                        break;
              case 4106: error_string="Unknown symbol";                                           break;
              case 4107: error_string="Invalid price parameter for trade function";               break;
              case 4108: error_string="Invalid ticket";                                           break;
              case 4109: error_string="Trade is not allowed in the expert properties";            break;
              case 4110: error_string="Longs are not allowed in the expert properties";           break;
              case 4111: error_string="Shorts are not allowed in the expert properties";          break;
              case 4200: error_string="Object is already exist";                                  break;
              case 4201: error_string="Unknown object property";                                  break;
              case 4202: error_string="Object is not exist";                                      break;
              case 4203: error_string="Unknown object type";                                      break;
              case 4204: error_string="No object name";                                           break;
              case 4205: error_string="Object coordinates error";                                 break;
              case 4206: error_string="No specified subwindow";                                   break;
              default:   error_string="Unknown error";                                            break;
          }

            return error_string;
       }
    }
}
