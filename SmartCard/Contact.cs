using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SmartCardManagement
{
    #region Base Smart Card

    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_IO_REQUEST
    {
        public int dwProtocol;
        public int cbPciLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APDURec
    {
        public byte bCLA;
        public byte bINS;
        public byte bP1;
        public byte bP2;
        public byte bP3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] SW;
        public bool IsSend;
    }

    public class CoreSmartCard
    {
        public static List<KeyValuePair<string, int>> OpenedConnections = new List<KeyValuePair<string, int>>();

        public const string SUCCESS       = "OK";

        public const int SCARD_S_SUCCESS  = 0;
        public const int SCARD_ATR_LENGTH = 33;

        /* ===========================================================
        '  Memory Card type constants
        '===========================================================*/
        public const int CT_MCU        = 0x00;               // MCU
        public const int CT_IIC_Auto   = 0x01;               // IIC (Auto Detect Memory Size)
        public const int CT_IIC_1K     = 0x02;               // IIC (1K)
        public const int CT_IIC_2K     = 0x03;               // IIC (2K)
        public const int CT_IIC_4K     = 0x04;               // IIC (4K)
        public const int CT_IIC_8K     = 0x05;               // IIC (8K)
        public const int CT_IIC_16K    = 0x06;               // IIC (16K)
        public const int CT_IIC_32K    = 0x07;               // IIC (32K)
        public const int CT_IIC_64K    = 0x08;               // IIC (64K)
        public const int CT_IIC_128K   = 0x09;               // IIC (128K)
        public const int CT_IIC_256K   = 0x0A;               // IIC (256K)
        public const int CT_IIC_512K   = 0x0B;               // IIC (512K)
        public const int CT_IIC_1024K  = 0x0C;               // IIC (1024K)
        public const int CT_AT88SC153  = 0x0D;               // AT88SC153
        public const int CT_AT88SC1608 = 0x0E;               // AT88SC1608
        public const int CT_SLE4418    = 0x0F;               // SLE4418
        public const int CT_SLE4428    = 0x10;               // SLE4428
        public const int CT_SLE4432    = 0x11;               // SLE4432
        public const int CT_SLE4442    = 0x12;               // SLE4442
        public const int CT_SLE4406    = 0x13;               // SLE4406
        public const int CT_SLE4436    = 0x14;               // SLE4436
        public const int CT_SLE5536    = 0x15;               // SLE5536
        public const int CT_MCUT0      = 0x16;               // MCU T=0
        public const int CT_MCUT1      = 0x17;               // MCU T=1
        public const int CT_MCU_Auto   = 0x18;               // MCU Autodetect

        /*===============================================================
        ' CONTEXT SCOPE
        ===============================================================	*/
        public const int SCARD_SCOPE_USER         = 0;
        /*===============================================================
        ' The context is a user context, and any database operations 
        '  are performed within the domain of the user.
        '===============================================================*/
        public const int SCARD_SCOPE_TERMINAL     = 1;
        /*===============================================================
        ' The context is that of the current terminal, and any database 
        'operations are performed within the domain of that terminal.  
        '(The calling application must have appropriate access permissions 
        'for any database actions.)
        '===============================================================*/

        public const int SCARD_SCOPE_SYSTEM       = 2;
        /*===============================================================
        ' The context is the system context, and any database operations 
        ' are performed within the domain of the system.  (The calling
        ' application must have appropriate access permissions for any 
        ' database actions.)
        '===============================================================*/
        /*=============================================================== 
        ' Context Scope
        '===============================================================*/
        public const int SCARD_STATE_UNAWARE     = 0x00;
        /*===============================================================
        ' The application is unaware of the current state, and would like 
        ' to know. The use of this value results in an immediate return
        ' from state transition monitoring services. This is represented
        ' by all bits set to zero.
        '===============================================================*/
        public const int SCARD_STATE_IGNORE      = 0x01;
        /*===============================================================
        ' The application requested that this reader be ignored. No other
        ' bits will be set.
        '===============================================================*/

        public const int SCARD_STATE_CHANGED     = 0x02;
        /*===============================================================
        ' This implies that there is a difference between the state 
        ' believed by the application, and the state known by the Service
        ' Manager.When this bit is set, the application may assume a
        ' significant state change has occurred on this reader.
        '===============================================================*/

        public const int SCARD_STATE_UNKNOWN     = 0x04;
        /*===============================================================
        ' This implies that the given reader name is not recognized by
        ' the Service Manager. If this bit is set, then SCARD_STATE_CHANGED
        ' and SCARD_STATE_IGNORE will also be set.
        '===============================================================*/
        public const int SCARD_STATE_UNAVAILABLE = 0x08;
        /*===============================================================
        ' This implies that the actual state of this reader is not
        ' available. If this bit is set, then all the following bits are
        ' clear.
        '===============================================================*/
        public const int SCARD_STATE_EMPTY       = 0x10;
        /*===============================================================
        '  This implies that there is not card in the reader.  If this bit
        '  is set, all the following bits will be clear.
         ===============================================================*/
        public const int SCARD_STATE_PRESENT     = 0x20;
        /*===============================================================
        '  This implies that there is a card in the reader.
         ===============================================================*/
        public const int SCARD_STATE_ATRMATCH    = 0x40;
        /*===============================================================
        '  This implies that there is a card in the reader with an ATR
        '  matching one of the target cards. If this bit is set,
        '  SCARD_STATE_PRESENT will also be set.  This bit is only returned
        '  on the SCardLocateCard() service.
         ===============================================================*/
        public const int SCARD_STATE_EXCLUSIVE   = 0x80;
        /*===============================================================
        '  This implies that the card in the reader is allocated for 
        '  exclusive use by another application. If this bit is set,
        '  SCARD_STATE_PRESENT will also be set.
         ===============================================================*/
        public const int SCARD_STATE_INUSE       = 0x100;
        /*===============================================================
        '  This implies that the card in the reader is in use by one or 
        '  more other applications, but may be connected to in shared mode. 
        '  If this bit is set, SCARD_STATE_PRESENT will also be set.
         ===============================================================*/
        public const int SCARD_STATE_MUTE        = 0x200;
        /*===============================================================
        '  This implies that the card in the reader is unresponsive or not
        '  supported by the reader or software.
        ' ===============================================================*/
        public const int SCARD_STATE_UNPOWERED   = 0x400;
        /*===============================================================
        '  This implies that the card in the reader has not been powered up.
        ' ===============================================================*/

        public const int SCARD_SHARE_EXCLUSIVE   = 1;
        /*===============================================================
        ' This application is not willing to share this card with other 
        'applications.
        '===============================================================*/
        public const int SCARD_SHARE_SHARED      = 2;
        /*===============================================================
        ' This application is willing to share this card with other 
        'applications.
        '===============================================================*/
        public const int SCARD_SHARE_DIRECT      = 3;
        /*===============================================================
        ' This application demands direct control of the reader, so it 
        ' is not available to other applications.
        '===============================================================*/

        /*===========================================================
        '   Disposition
        '===========================================================*/
        public const int SCARD_LEAVE_CARD   = 0;   // Don't do anything special on close
        public const int SCARD_RESET_CARD   = 1;   // Reset the card on close
        public const int SCARD_UNPOWER_CARD = 2;   // Power down the card on close
        public const int SCARD_EJECT_CARD   = 3;   // Eject the card on close

        /* ===========================================================
        ' ACS IOCTL class
        '===========================================================*/
        public const long FILE_DEVICE_SMARTCARD       = 0x310000; // Reader action IOCTLs

        public const long IOCTL_SMARTCARD_DIRECT      = FILE_DEVICE_SMARTCARD + 2050 * 4;
        public const long IOCTL_SMARTCARD_SELECT_SLOT = FILE_DEVICE_SMARTCARD + 2051 * 4;
        public const long IOCTL_SMARTCARD_DRAW_LCDBMP = FILE_DEVICE_SMARTCARD + 2052 * 4;
        public const long IOCTL_SMARTCARD_DISPLAY_LCD = FILE_DEVICE_SMARTCARD + 2053 * 4;
        public const long IOCTL_SMARTCARD_CLR_LCD     = FILE_DEVICE_SMARTCARD + 2054 * 4;
        public const long IOCTL_SMARTCARD_READ_KEYPAD = FILE_DEVICE_SMARTCARD + 2055 * 4;
        public const long IOCTL_SMARTCARD_READ_RTC    = FILE_DEVICE_SMARTCARD + 2057 * 4;
        public const long IOCTL_SMARTCARD_SET_RTC     = FILE_DEVICE_SMARTCARD + 2058 * 4;
        public const long IOCTL_SMARTCARD_SET_OPTION  = FILE_DEVICE_SMARTCARD + 2059 * 4;
        public const long IOCTL_SMARTCARD_SET_LED     = FILE_DEVICE_SMARTCARD + 2060 * 4;
        public const long IOCTL_SMARTCARD_LOAD_KEY    = FILE_DEVICE_SMARTCARD + 2062 * 4;
        public const long IOCTL_SMARTCARD_READ_EEPROM = FILE_DEVICE_SMARTCARD + 2065 * 4;
        public const long IOCTL_SMARTCARD_WRITE_EEPROM = FILE_DEVICE_SMARTCARD + 2066 * 4;
        public const long IOCTL_SMARTCARD_GET_VERSION  = FILE_DEVICE_SMARTCARD + 2067 * 4;
        public const long IOCTL_SMARTCARD_GET_READER_INFO = FILE_DEVICE_SMARTCARD + 2051 * 4;
        public const long IOCTL_SMARTCARD_SET_CARD_TYPE   = FILE_DEVICE_SMARTCARD + 2060 * 4;

        /*===========================================================
        '   Error Codes
        '===========================================================*/
        public const ulong SCARD_F_INTERNAL_ERROR    = 0x80100001;
        public const ulong SCARD_E_CANCELLED         = 0x80100002;
        public const ulong SCARD_E_INVALID_HANDLE    = 0x80100003;
        public const ulong SCARD_E_INVALID_PARAMETER = 0x80100004;
        public const ulong SCARD_E_INVALID_TARGET    = 0x80100005;
        public const ulong SCARD_E_NO_MEMORY         = 0x80100006;
        public const ulong SCARD_F_WAITED_TOO_LONG   = 0x80100007;
        public const ulong SCARD_E_INSUFFICIENT_BUFFER = 0x80100008;
        public const ulong SCARD_E_UNKNOWN_READER    = 0x80100009;

        public const ulong SCARD_E_TIMEOUT           = 0x8010000A;
        public const ulong SCARD_E_SHARING_VIOLATION = 0x8010000B;
        public const ulong SCARD_E_NO_SMARTCARD      = 0x8010000C;
        public const ulong SCARD_E_UNKNOWN_CARD      = 0x8010000D;
        public const ulong SCARD_E_CANT_DISPOSE      = 0x8010000E;
        public const ulong SCARD_E_PROTO_MISMATCH    = 0x8010000F;

        public const ulong SCARD_E_NOT_READY         = 0x80100010;
        public const ulong SCARD_E_INVALID_VALUE     = 0x80100011;
        public const ulong SCARD_E_SYSTEM_CANCELLED  = 0x80100012;
        public const ulong SCARD_F_COMM_ERROR        = 0x80100013;
        public const ulong SCARD_F_UNKNOWN_ERROR     = 0x80100014;
        public const ulong SCARD_E_INVALID_ATR       = 0x80100015;
        public const ulong SCARD_E_NOT_TRANSACTED    = 0x80100016;
        public const ulong SCARD_E_READER_UNAVAILABLE = 0x80100017;
        public const ulong SCARD_P_SHUTDOWN          = 0x80100018;
        public const ulong SCARD_E_PCI_TOO_SMALL     = 0x80100019;

        public const ulong SCARD_E_READER_UNSUPPORTED = 0x8010001A;
        public const ulong SCARD_E_DUPLICATE_READER   = 0x8010001B;
        public const ulong SCARD_E_CARD_UNSUPPORTED   = 0x8010001C;
        public const ulong SCARD_E_NO_SERVICE         = 0x8010001D;
        public const ulong SCARD_E_SERVICE_STOPPED    = 0x8010001E;

        public const ulong SCARD_W_UNSUPPORTED_CARD   = 0x80100065;
        public const ulong SCARD_W_UNRESPONSIVE_CARD  = 0x80100066;
        public const ulong SCARD_W_UNPOWERED_CARD     = 0x80100067;
        public const ulong SCARD_W_RESET_CARD         = 0x80100068;
        public const ulong SCARD_W_REMOVED_CARD       = 0x80100069;

        /*===========================================================
        '   PROTOCOL
        '===========================================================*/
        public const int SCARD_PROTOCOL_UNDEFINED     = 0x00;          // There is no active protocol.
        public const int SCARD_PROTOCOL_T0            = 0x01;          // T=0 is the active protocol.
        public const int SCARD_PROTOCOL_T1            = 0x02;          // T=1 is the active protocol.
        public const int SCARD_PROTOCOL_RAW           = 0x10000;       // Raw is the active protocol.
        public const uint SCARD_PROTOCOL_DEFAULT      = 0x80000000;    // Use implicit PTS.
        /*===========================================================
        '   READER STATE
        '===========================================================*/
        public const int SCARD_UNKNOWN    = 0;
        /*===============================================================
        ' This value implies the driver is unaware of the current 
        ' state of the reader.
        '===============================================================*/
        public const int SCARD_ABSENT     = 1;
        /*===============================================================
        ' This value implies there is no card in the reader.
        '===============================================================*/
        public const int SCARD_PRESENT    = 2;
        /*===============================================================
        ' This value implies there is a card is present in the reader, 
        'but that it has not been moved into position for use.
        '===============================================================*/
        public const int SCARD_SWALLOWED  = 3;
        /*===============================================================
        ' This value implies there is a card in the reader in position 
        ' for use.  The card is not powered.
        '===============================================================*/
        public const int SCARD_POWERED    = 4;
        /*===============================================================
        ' This value implies there is power is being provided to the card, 
        ' but the Reader Driver is unaware of the mode of the card.
        '===============================================================*/
        public const int SCARD_NEGOTIABLE = 5;
        /*===============================================================
        ' This value implies the card has been reset and is awaiting 
        ' PTS negotiation.
        '===============================================================*/
        public const int SCARD_SPECIFIC   = 6;
        /*===============================================================
        ' This value implies the card has been reset and specific 
        ' communication protocols have been established.
        '===============================================================*/

        /*==========================================================================
        ' Prototypes
        '==========================================================================*/

        [DllImport("mcscm.dll")]
        public static extern int MCardGetAttrib(int cardHandle, int attribID, ref int attribute, ref int length);

        [DllImport("mcscm.dll")]
        public static extern int MCardWriteMemory(int cardHandle, byte zone, int offset, ref byte data, ref int length);

        [DllImport("mcscm.dll")]
        public static extern int MCardReadMemory(int cardHandle, byte zone, int offset, ref byte data, ref int length);

        [DllImport("winscard.dll")]
        public static extern int SCardEstablishContext(int dwScope, int pvReserved1, int pvReserved2, ref int phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardReleaseContext(int phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardConnect(int hContext, string szReaderName, int dwShareMode, int dwPrefProtocol, ref int phCard, ref int ActiveProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardBeginTransaction(int hCard);

        [DllImport("winscard.dll")]
        public static extern int SCardDisconnect(int hCard, int Disposition);

        [DllImport("winscard.dll")]
        public static extern int SCardListReaderGroups(int hContext, ref string mzGroups, ref int pcchGroups);

        [DllImport("winscard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(int hContext, byte[] Groups, byte[] Readers, ref int pcchReaders);

        [DllImport("winscard.dll")]
        public static extern int SCardStatus(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, ref byte ATR, ref int ATRLen);

        [DllImport("winscard.dll")]
        public static extern int SCardEndTransaction(int hCard, int Disposition);

        [DllImport("winscard.dll")]
        public static extern int SCardState(int hCard, ref uint State, ref uint Protocol, ref byte ATR, ref uint ATRLen);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);

        private static List<KeyValuePair<string, string>> swMessages = new List<KeyValuePair<string, string>>();
        static CoreSmartCard()
        {
            swMessages.Add(new KeyValuePair<string, string>("6200", "No information given"));
            swMessages.Add(new KeyValuePair<string, string>("6281", "Part of returned data may be corrupted"));
            swMessages.Add(new KeyValuePair<string, string>("6282", "End of file/record reached before reading LE bytes "));
            swMessages.Add(new KeyValuePair<string, string>("6283", "Selected file invalidated (Deactivated) "));
            swMessages.Add(new KeyValuePair<string, string>("6284", "FCI not formatted "));
            swMessages.Add(new KeyValuePair<string, string>("6300", "No information given"));
            swMessages.Add(new KeyValuePair<string, string>("6381", "File filled by last write"));
            swMessages.Add(new KeyValuePair<string, string>("6400", "Execution error"));
            swMessages.Add(new KeyValuePair<string, string>("6401", "Immediate response required by card"));
            swMessages.Add(new KeyValuePair<string, string>("6500", "Selected file deactivated"));
            swMessages.Add(new KeyValuePair<string, string>("6581", "Memory failure"));
            swMessages.Add(new KeyValuePair<string, string>("6600", "ENV cannot set or modified"));
            swMessages.Add(new KeyValuePair<string, string>("6800", "No information given"));
            swMessages.Add(new KeyValuePair<string, string>("6881", "Logical channel not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6882", "SM not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6883", "Last command of the chain expected"));
            swMessages.Add(new KeyValuePair<string, string>("6884", "CMD chaining not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6900", "No information given"));
            swMessages.Add(new KeyValuePair<string, string>("6981", "CMD incompatible with file structure"));
            swMessages.Add(new KeyValuePair<string, string>("6982", "Security status not satisfied"));
            swMessages.Add(new KeyValuePair<string, string>("6983", "AUTHENTICATION blocked"));
            swMessages.Add(new KeyValuePair<string, string>("6984", "Referenced data invalidated"));
            swMessages.Add(new KeyValuePair<string, string>("6985", "Conditions of use not satisfied"));
            swMessages.Add(new KeyValuePair<string, string>("6986", "No current EF"));
            swMessages.Add(new KeyValuePair<string, string>("6987", "Expected SM_DO missing"));
            swMessages.Add(new KeyValuePair<string, string>("6988", "SM_DO incorrect"));
            swMessages.Add(new KeyValuePair<string, string>("6A00", "No information given"));
            swMessages.Add(new KeyValuePair<string, string>("6A80", "Incorrect parameters in the command data field"));
            swMessages.Add(new KeyValuePair<string, string>("6A81", "Function not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6A82", "File not found"));
            swMessages.Add(new KeyValuePair<string, string>("6A83", "Record not found"));
            swMessages.Add(new KeyValuePair<string, string>("6A84", "Not enough memory in file"));
            swMessages.Add(new KeyValuePair<string, string>("6A85", "LC inconsistent with TLV"));
            swMessages.Add(new KeyValuePair<string, string>("6A86", "In correct params P1P2"));
            swMessages.Add(new KeyValuePair<string, string>("6A87", "LC inconsistent with P1P2"));
            swMessages.Add(new KeyValuePair<string, string>("6A88", "Referenced data not found"));
            swMessages.Add(new KeyValuePair<string, string>("6A89", "File already exists"));
            swMessages.Add(new KeyValuePair<string, string>("6A8A", "DF name already exists"));
            swMessages.Add(new KeyValuePair<string, string>("6700", "Wrong length"));
            swMessages.Add(new KeyValuePair<string, string>("6B00", "Offset outside EF"));
            swMessages.Add(new KeyValuePair<string, string>("6D00", "INS not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6E00", "CLA not supported"));
            swMessages.Add(new KeyValuePair<string, string>("6F00", "No precise diagnosis"));
            swMessages.Add(new KeyValuePair<string, string>("9000", "OK"));
            swMessages.Add(new KeyValuePair<string, string>("6285", "Selected file invalidated (Terminated)"));
        }

        public static string ParseErrorCode(ulong errorCode)
        {
            switch (errorCode)
            {
                case CoreSmartCard.SCARD_E_CANCELLED:
                    return "The action was canceled by an SCardCancel request.";
                case CoreSmartCard.SCARD_E_CANT_DISPOSE:
                    return "The system could not dispose of the media in the requested manner.";
                case CoreSmartCard.SCARD_E_CARD_UNSUPPORTED:
                    return "The smart card does not meet minimal requirements for support.";
                case CoreSmartCard.SCARD_E_DUPLICATE_READER:
                    return "The reader driver didn't produce a unique reader name.";
                case CoreSmartCard.SCARD_E_INSUFFICIENT_BUFFER:
                    return "The data buffer for returned data is too small for the returned data.";
                case CoreSmartCard.SCARD_E_INVALID_ATR:
                    return "An ATR string obtained from the registry is not a valid ATR string.";
                case CoreSmartCard.SCARD_E_INVALID_HANDLE:
                    return "The supplied handle was invalid.";
                case CoreSmartCard.SCARD_E_INVALID_PARAMETER:
                    return "One or more of the supplied parameters could not be properly interpreted.";
                case CoreSmartCard.SCARD_E_INVALID_TARGET:
                    return "Registry startup information is missing or invalid.";
                case CoreSmartCard.SCARD_E_INVALID_VALUE:
                    return "One or more of the supplied parameter values could not be properly interpreted.";
                case CoreSmartCard.SCARD_E_NOT_READY:
                    return "The reader or card is not ready to accept commands.";
                case CoreSmartCard.SCARD_E_NOT_TRANSACTED:
                    return "An attempt was made to end a non-existent transaction.";
                case CoreSmartCard.SCARD_E_NO_MEMORY:
                    return "Not enough memory available to complete this command.";
                case CoreSmartCard.SCARD_E_NO_SERVICE:
                    return "The smart card resource manager is not running.";
                case CoreSmartCard.SCARD_E_NO_SMARTCARD:
                    return "The operation requires a smart card, but no smart card is currently in the device.";
                case CoreSmartCard.SCARD_E_PCI_TOO_SMALL:
                    return "The PCI receive buffer was too small.";
                case CoreSmartCard.SCARD_E_PROTO_MISMATCH:
                    return "The requested protocols are incompatible with the protocol currently in use with the card.";
                case CoreSmartCard.SCARD_E_READER_UNAVAILABLE:
                    return "The specified reader is not currently available for use.";
                case CoreSmartCard.SCARD_E_READER_UNSUPPORTED:
                    return "The reader driver does not meet minimal requirements for support.";
                case CoreSmartCard.SCARD_E_SERVICE_STOPPED:
                    return "The smart card resource manager has shut down.";
                case CoreSmartCard.SCARD_E_SHARING_VIOLATION:
                    return "The smart card cannot be accessed because of other outstanding connections.";
                case CoreSmartCard.SCARD_E_SYSTEM_CANCELLED:
                    return "The action was canceled by the system, presumably to log off or shut down.";
                case CoreSmartCard.SCARD_E_TIMEOUT:
                    return "The user-specified timeout value has expired.";
                case CoreSmartCard.SCARD_E_UNKNOWN_CARD:
                    return "The specified smart card name is not recognized.";
                case CoreSmartCard.SCARD_E_UNKNOWN_READER:
                    return "The specified reader name is not recognized.";
                case CoreSmartCard.SCARD_F_COMM_ERROR:
                    return "An internal communications error has been detected.";
                case CoreSmartCard.SCARD_F_INTERNAL_ERROR:
                    return "An internal consistency check failed.";
                case CoreSmartCard.SCARD_F_UNKNOWN_ERROR:
                    return "An internal error has been detected, but the source is unknown.";
                case CoreSmartCard.SCARD_F_WAITED_TOO_LONG:
                    return "An internal consistency timer has expired.";
                case CoreSmartCard.SCARD_S_SUCCESS:
                    return "No error was encountered.";
                case CoreSmartCard.SCARD_W_REMOVED_CARD:
                    return "The smart card has been removed, so that further communication is not possible.";
                case CoreSmartCard.SCARD_W_RESET_CARD:
                    return "The smart card has been reset, so any shared state information is invalid.";
                case CoreSmartCard.SCARD_W_UNPOWERED_CARD:
                    return "Power has been removed from the smart card, so that further communication is not possible.";
                case CoreSmartCard.SCARD_W_UNRESPONSIVE_CARD:
                    return "The smart card is not responding to a reset.";
                case CoreSmartCard.SCARD_W_UNSUPPORTED_CARD:
                    return "The reader cannot communicate with the card, due to ATR string configuration conflicts.";
                default:
                    return "Unexpected error occurred while accessing the smart card, check the smart card device and card for the connection";
            }
        }

        public static string ParseSWMessage(string swValue)
        {
            KeyValuePair<string,string> message =  swMessages.Find(delegate(KeyValuePair<string, string> sw)
                                                                    {
                                                                        return sw.Key.Equals(swValue);
                                                                    });
            if (!string.IsNullOrEmpty(message.Key) && !string.IsNullOrEmpty(message.Value))
                return message.Value;
            else
                return "Invalid Response";
        }

        internal byte[] ExecuteTransmit(AccessType type, int cardHandle, int protocol, byte cla, byte ins, byte p1, byte p2, byte length, byte[] dataBuffer, ref byte[] receivedBuffer, ref int receivedLength)
        {
            /*APDURec apdu = new APDURec();
            apdu.bCLA    = cla;
            apdu.bINS    = ins;
            apdu.bP1     = p1;
            apdu.bP2     = p2;
            apdu.bP3     = length;

            if (type == AccessType.WRITE)
            {
                apdu.Data   = dataBuffer;
                apdu.IsSend = true;
            }
            else
                apdu.IsSend = false;*/
             
            SCARD_IO_REQUEST ioRequest = new SCARD_IO_REQUEST();
            ioRequest.dwProtocol  = protocol;
            ioRequest.cbPciLength = Marshal.SizeOf(ioRequest);

            byte[] sendBuffer    = new byte[262];
            int sendBufferLength = 0;

            sendBuffer[0] = cla;
            sendBuffer[1] = ins;
            sendBuffer[2] = p1;
            sendBuffer[3] = p2;
            sendBuffer[4] = length;

            if (type == AccessType.WRITE)
            {
                for (int idx = 0; idx < length; idx++)
                    sendBuffer[5 + idx] = dataBuffer[idx];

                sendBufferLength = 5 + length;
                receivedLength   = 2;
            }
            else
            {
                sendBufferLength = 5;
                receivedLength   = 2 + length;
            }

            int code = SCardTransmit(cardHandle, ref ioRequest, ref sendBuffer[0], sendBufferLength, ref ioRequest, ref receivedBuffer[0], ref receivedLength);
            if (code != SCARD_S_SUCCESS)
                ParseErrorCode((ulong)code);

            if (type == AccessType.WRITE)
                return receivedBuffer;
            else
            {
                byte[] data = new byte[2];
                data[0]     = receivedBuffer[receivedLength - 2];
                data[1]     = receivedBuffer[receivedLength - 1];
                return data;
            }
        }
    }

    #endregion

    #region Public Enums

    public enum AccessType   { READ, WRITE }

    public enum SCDeviceType { DIRECT_DEVICE, EVOLIS_SC_DEVICE, SMART_SC_DEVICE }

    public enum SecurityVerify
    {
        MF_SPECIFIC_PASSWORD = 0x0,
        DF_SPECIFIC_PASSWORD = 0x80
    }

    public enum INSCode
    {
        CREATE_FILE          = 0xE0,
        DELETE_FILE          = 0xE4,
        TERMINATE_DF         = 0xE6,
        TERMINATE_EF         = 0xE8,
        ACTIVATE_FILE        = 0x44,
        DEACTIVATE_FILE      = 0x04,
        WRITE_BINARY         = 0xD0,
        WRITE_BINARY_SECURE  = 0xD1,
        WRITE_RECORD         = 0xD2,
        UPDATE_BINARY        = 0xD6,
        UPDATE_RECORD        = 0xDC,
        ERASE_BINARY         = 0x0E,
        ERASE_BINARY_SECURE  = 0x0F,
        ERASE_RECORD         = 0x0C,
        APPEND_RECORD        = 0xE2,
        PUT_DATA             = 0xDA,
        PUT_DATA_SECURE      = 0xDB,
        READ_BINARY          = 0xB0,
        READ_BINARY_SECURE   = 0xB1,
        READ_RECORD          = 0xB2,
        READ_RECORD_SECURE   = 0xB3,
        SEARCH_BINARY        = 0xA0,
        SEARCH_RECORD        = 0xA2,
        GET_DATA             = 0xCA,
        GET_DATA_SECURE      = 0xCB,
        SELECT_FILE          = 0xA4,
        UPDATE_BINARY_SECURE = 0xD7,
        UPDATE_RECORD_SECURE = 0xDD,
        SEARCH_BINARY_SECURE = 0xA1,
        VERIFY               = 0x20,
        MANAGE_SECURITY_ENV  = 0x22,
        GET_CHALLENGE        = 0x84,
        INTERNAL_AUTH        = 0x88,
        GET_RESPONSE         = 0xC0,
        EXTERNAL_MUTUAL_AUTH = 0x82
    }

    public enum FileDescriptor
    {
        NOT_SHAREABLE          = 0x0,
        SHAREABLE              = 0x40,
        DF                     = 0x38,
        WORKING_EF             = 0x0,
        INTERNAL_EF            = 0x8,
        EF_TRANSPARENT         = 0x1,
        EF_LINEAR_FIXED        = 0x2,
        EF_LINEAR_FIXED_TLV    = 0x3,
        EF_LINEAR_VARIABLE     = 0x4,
        EF_LINEAR_VARIABLE_TLV = 0x5,
        EF_CYCLIC_FIXED        = 0x6,
        EF_CYCLIC_FIXED_TLV    = 0x7,
        NO_INFORMATION         = 0x0
    }

    public enum FileLifeCycle
    {
        NO_INFORMATION              = 0x0,
        CREATION_STATE              = 0x1,
        INITIALIZATION_STATE        = 0x3,
        OPERATION_STATE_ACTIVATED   = 0x5,
        OPERATION_STATE_DEACTIVATED = 0x4,
        TERMINATION_STATE           = 0xC
    }

    public enum CompactAccessModeDF
    {
        DELETE_FILE_SELF  = 0x40,
        TERMINATE_MF_DF   = 0x20,
        ACTIVATE_FILE     = 0x10,
        DEACTIVATE_FILE   = 0x8,
        CREATE_FILE_DF    = 0x4,
        CREATE_FILE_EF    = 0x2,
        DELETE_CHILD_FILE = 0x1
    }

    public enum CompactAccessModeEF
    {
        DELETE_FILE          = 0x40,
        TERMINATE_EF         = 0x20,
        ACTIVATE_FILE        = 0x10,
        DEACTIVATE_FILE      = 0x8,
        WRITE_BINARY_RECORD  = 0x4,
        APPEND_RECORD        = 0x4,
        UPDATE_BINARY_RECORD = 0x2,
        ERASE_BINARY_RECORD  = 0x2,
        READ_BINARY_RECORD   = 0x1,
        SEARCH_BINARY_RECORD = 0x1
    }

    public enum CompactAccessModeDataObject
    {
        MANAGE_SECURITY_ENVIRON = 0x4,
        PUT_DATA                = 0x2,
        GET_DATA                = 0x1
    }

    public enum CompactAccessModeTableAndView
    {
        CREATE_DELETE_USER           = 0x40,
        GRANT_REVOKE                 = 0x20,
        CREATE_TABLE_VIEW_DICTIONARY = 0x10,
        DROP_TABLE_VIEW              = 0x8,
        INSERT                       = 0x4,
        UPDATE_DELETE                = 0x2,
        FETCH                        = 0x1
    }

    public enum ExpandAccessModeDataObject
    {
        COMPACT_FORMAT = 0x80,
        VALUE_OF_CLA   = 0x88,
        VALUE_OF_INS   = 0x84,
        VALUE_OF_P1    = 0x82,
        VALUE_OF_P2    = 0x81
    }

    public enum DataCodingByte
    {
        ONE_TIME_WRITE              = 0x0,
        PROPRIETARY                 = 0x20,
        WRITE_OR                    = 0x40,
        WRITE_AND                   = 0x60,
        DATA_UNIT_SIZE_ONE_BYTE     = 0x1,
        DATA_UNIT_SIZE_FOUR_BYTE    = 0x2,
        DATA_UNIT_SIZE_EIGHT_BYTE   = 0x3,
        DATA_UNIT_SIZE_SIXTEEN_BYTE = 0x4
    }

    public enum ExpandSecurityCondition
    {
        ALWAYS = 0x90,
        NEVER  = 0x97,
        COMPACT_SECURITY_CONDITION = 0x9E
    }

    public enum CompactSecurityCondition
    {
        NO_CONDITION    = 0x0,
        NEVER           = 0xFF,
        SECURITY_ENV_1  = 0x21,
        SECURITY_ENV_2  = 0x22,
        SECURITY_ENV_3  = 0x23,
        SECURITY_ENV_4  = 0x24,
        SECURITY_ENV_5  = 0x25,
        SECURITY_ENV_6  = 0x26,
        SECURITY_ENV_7  = 0x27,
        SECURITY_ENV_8  = 0x28,
        SECURITY_ENV_9  = 0x29,
        SECURITY_ENV_10 = 0x2A,
        SECURITY_ENV_11 = 0x2B,
        SECURITY_ENV_12 = 0x2C,
        SECURITY_ENV_13 = 0x2D,
        SECURITY_ENV_14 = 0x2E
        /*ATLEAST_ONE_CONDITION   = 0x0,
        ALL_CONDITIONS          = 0x80,
        SECURE_MESSAGE          = 0x40,
        EXTERNAL_AUTHENTICATION = 0x20,
        USER_AUTHENTICATION     = 0x10*/
    }

    public enum RecordIdentifer
    {
        FIRST_RECORD    = 0x0,
        LAST_RECORD     = 0x1,
        NEXT_RECORD     = 0x2,
        PREVIOUS_RECORD = 0x3
    }

    public enum EraseRecordType
    {
        CURRENT_RECORD_ONLY = 0x4,
        ALL_FROM_CURRENT    = 0x5
    }

    public enum SecurityMessage
    {
        COMMAND_DATA  = 0x10,
        RESPONSE_DATA = 0x20,
        COMPUTATION_DECIPHER_INTERNALAUTH_KEYAGGR = 0x40,
        VERIFY_ENCIPHER_EXTAUTH_KEYAGGR           = 0x80
    }

    public enum SecurityEnvironment
    {
        SET     = 0x1,
        STORE   = 0xF2,
        RESTORE = 0xF3,
        ERASE   = 0xF4
    }

    public enum SecurityCtrlRefTemplate
    {
        AUTHENTICATION         = 0xA4,
        KEY_AGREEMENT          = 0xA6,
        HASH_CODE              = 0xAA,
        CRYPTOGRAPHIC_CHECKSUM = 0xB4,
        DIGITAL_SIGNATURE      = 0xB6,
        CONFIDENTIAL           = 0xB8
    }

    #endregion

    #region Smart Card

    public sealed class Context : CoreSmartCard, IDisposable
    {
        private int? _contextHandle = null;

        public int ContextHandle
        {
            get
            {
                if (!_contextHandle.HasValue)
                    throw new InvalidSmartCardContextException("No valid context established until now");
                return _contextHandle.Value;
            }
        }

        public Context()
        {
            CoreSmartCard.OpenedConnections.Clear();
            int context = 0;
            int code    = SCardEstablishContext(SCARD_SCOPE_USER, 0, 0, ref context);

            if (code != SCARD_S_SUCCESS)
                throw new InvalidSmartCardContextException(ParseErrorCode((ulong)code));
            _contextHandle = context;
        }

        public void Dispose()
        {
            if (_contextHandle.HasValue)
            {
                int code = SCardReleaseContext(_contextHandle.Value);
                if (code != SCARD_S_SUCCESS)
                {
                }
            }
        }

        //need to be reviewed since the property doesn't return more than 1 card name
        public List<string> SmartCardsList
        {
            get
            {
                List<string> cards  = new List<string>();
                int readersCount    = -1;
                byte[] returnData   = null;

                int code = SCardListReaders(_contextHandle.Value, null, null, ref readersCount);
                if (code != SCARD_S_SUCCESS)
                    throw new InvalidSmartCardContextException(ParseErrorCode((ulong)code));

                returnData = new byte[readersCount];
                code = SCardListReaders(_contextHandle.Value, null, returnData, ref readersCount);
                if (code != SCARD_S_SUCCESS)
                    throw new InvalidSmartCardContextException(ParseErrorCode((ulong)code));

                foreach (string card in Convertor.ByteArrayToString(returnData).Split('\0'))
                    if (!string.IsNullOrEmpty(card))
                        cards.Add(card);

                return cards;
            }
        }

        public SmartCard ConnectTo(string readerName)
        {
            SmartCard card = new SmartCard(_contextHandle.Value, readerName);
            return card;
        }

        public SmartCard ConnectTo(string readerName, bool closeExistingConnection)
        {
            SmartCard card = new SmartCard(_contextHandle.Value, readerName, closeExistingConnection);
            return card;
        }
    }

    public sealed class SmartCard : CoreSmartCard, IDisposable
    {
        private int? _cardHandle         = null;
        private int? _activeProtocol     = null;
        private int _contextHandle       = 0;
        private string _readerName       = string.Empty;
        private SCDeviceType _deviceType = SCDeviceType.DIRECT_DEVICE; 

        internal SmartCard(int contextHandle, string readerName)
        {
            _contextHandle = contextHandle;
            _readerName    = readerName;
            OpenConnection(contextHandle, readerName);
        }

        internal SmartCard(int contextHandle, string readerName, SCDeviceType deviceType)
        {
            throw new NotImplementedException("This constructor is not yet implemented");
            /*_contextHandle = contextHandle;
            _readerName    = readerName;
            _deviceType    = deviceType;*/
        }

        internal SmartCard(int contextHandle, string readerName, bool closeExistingConnection)
        {
            if (closeExistingConnection && SmartCard.GetConnection(readerName).HasValue)
            {
                int code = SCardDisconnect(SmartCard.GetConnection(readerName).Value, SCARD_UNPOWER_CARD);
                if (code != SCARD_S_SUCCESS)
                {
                }
                else
                    CoreSmartCard.OpenedConnections.RemoveAll(delegate(KeyValuePair<string, int> c)
                                                                {
                                                                    return c.Key.Equals(readerName);
                                                                });
            }
            _contextHandle = contextHandle;
            _readerName    = readerName;
            OpenConnection(contextHandle, readerName);
        }

        internal SmartCard(int contextHandle, string readerName, bool closeExistingConnection, SCDeviceType deviceType)
        {
            throw new NotImplementedException("This constructor is not yet implemented");
            /*_contextHandle = contextHandle;
            _readerName    = readerName;
            _deviceType    = deviceType;*/
        }

        public int CardHandle    { get { return _cardHandle.Value; } }
        public string ReaderName { get { return _readerName;       } }
        public int ContextHandle { get { return _contextHandle;    } }
        public int Protocol      { get { return _activeProtocol.Value; } }
        public SCDeviceType DeviceType { get { return _deviceType;     } }

        public void Dispose()
        {
            if (_cardHandle.HasValue)
            {
                int code = SCardDisconnect(_cardHandle.Value, SCARD_UNPOWER_CARD);
                if (code != SCARD_S_SUCCESS)
                {
                }
                else
                    CoreSmartCard.OpenedConnections.RemoveAll(delegate(KeyValuePair<string, int> c)
                                                                {
                                                                    return c.Value.Equals(_cardHandle.Value);
                                                                });
            }
        }

        private static int? GetConnection(string readerName)
        {
            KeyValuePair<string, int> card =  CoreSmartCard.OpenedConnections.Find(delegate(KeyValuePair<string, int> c)
                                                                                    {
                                                                                        return c.Key.Equals(readerName);
                                                                                    });

            if (!string.IsNullOrEmpty(card.Key))
                return card.Value;
            return null;
        }

        private void OpenConnection(int contextHandle, string readerName)
        {
            int card = 0, protocol = 0;
            int code = SCardConnect(contextHandle, readerName, SCARD_SHARE_EXCLUSIVE, SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1, ref card, ref protocol);
            if (code != SCARD_S_SUCCESS)
                throw new InvalidSmartCardException(string.Format("Error Code: {0}, {1}", code, ParseErrorCode((ulong)code)));

            CoreSmartCard.OpenedConnections.Add(new KeyValuePair<string, int>(readerName, card));
            _cardHandle     = card;
            _activeProtocol = protocol;
        }
    }

    public sealed class File : CoreSmartCard
    {
        #region File Properties

        private string _fileName = string.Empty;
        private int _cardHandle  = 0;
        private int _protocol    = 0;
        private FileControlParams _parameters = null;

        public FileControlParams Parameters
        {
            get { return _parameters;  }
            set { _parameters = value; }
        }

        public string FileName
        {
            get { return _fileName;  }
            set { _fileName = value; }
        }

        public File(string fileName, int cardHandle, int protocol)
        {
            _fileName   = fileName;
            _cardHandle = cardHandle;
            _protocol   = protocol;
        }

        private byte GetByteFromInteger(int number, byte part)
        {
            string hex = string.Format("{0:X4}", number);
            if (part == 1)
                return Convert.ToByte(hex.Substring(0, 2), 16);
            else if (part == 2)
                return Convert.ToByte(hex.Substring(2, 2), 16);

            return 0;
        }

        #endregion

        #region Put/Get Data

        public string PutData(byte? recordIdentifier, byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (recordIdentifier.HasValue)
                {
                    p1 = 0x2;
                    p2 = recordIdentifier.Value;
                }
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.PUT_DATA), p1, p2, (dataLength.HasValue ? dataLength.Value : Convert.ToByte(data.Length)), data, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        public string GetData(byte? recordIdentifier, byte dataLength, ref byte[] outputData)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] temp = new byte[2];
            int length  = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (recordIdentifier.HasValue)
                {
                    p1 = 0x2;
                    p2 = recordIdentifier.Value;
                }
                if (outputData.Length <= dataLength)
                    outputData = new byte[dataLength + 2];
                temp = ExecuteTransmit(AccessType.READ, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.GET_DATA), p1, p2, dataLength, temp, ref outputData, ref length);
                string sw = SUCCESS;
                if (temp[0] != 0x61 && temp[1] != 0x90)
                    sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", temp[0], temp[1]));

                if (sw.Equals(SUCCESS))
                {
                    byte[] output = new byte[dataLength];
                    for (int idx = 0; idx < dataLength; idx++)
                        output[idx] = outputData[idx];
                    outputData = output;
                }
                else
                    return sw;
            }
            else
                return message;
            return SUCCESS;
        }

        #endregion

        #region Activate/Deactivate File

        public string ActivateFile()
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.ACTIVATE_FILE), 0x0, 0x0, 0, output, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        public string DeactivateFile()
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.DEACTIVATE_FILE), 0x0, 0x0, 0, output, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        #endregion

        #region Record Functions

        public string WriteRecord(byte? recordNumber, byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 1, p2  = 0;
                byte[] inputData = data;
                if (recordNumber.HasValue)
                {
                    p1 = recordNumber.Value;
                    p2 = 0x4;
                }
                if (dataLength.HasValue)
                {
                    inputData    = new byte[Convert.ToByte(dataLength + 2)];
                    inputData[0] = recordNumber.Value;
                    inputData[1] = dataLength.Value;
                    for (byte index = 2, index1 = 0; index < dataLength + 2; index++, index1++)
                        inputData[index] = data[index1];
                }
                else
                {
                    inputData    = new byte[Convert.ToByte(data.Length + 2)];
                    inputData[0] = recordNumber.Value;
                    inputData[1] = Convert.ToByte(data.Length);
                    for (byte index = 2, index1 = 0; index < data.Length + 2; index++, index1++)
                        inputData[index] = data[index1];
                }
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.WRITE_RECORD), p1, p2, (dataLength.HasValue ? Convert.ToByte(dataLength.Value + 2) : Convert.ToByte(data.Length + 2)), inputData, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        public string UpdateRecord(byte? recordNumber, byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 1, p2  = 0;
                byte[] inputData = data;
                if (recordNumber.HasValue)
                {
                    p1 = recordNumber.Value;
                    p2 = 0x4;
                }
                if (dataLength.HasValue)
                {
                    inputData    = new byte[Convert.ToByte(dataLength + 2)];
                    inputData[0] = recordNumber.Value;
                    inputData[1] = dataLength.Value;
                    for (byte index = 2, index1 = 0; index < dataLength + 2; index++, index1++)
                        inputData[index] = data[index1];
                }
                else
                {
                    inputData    = new byte[Convert.ToByte(data.Length + 2)];
                    inputData[0] = recordNumber.Value;
                    inputData[1] = Convert.ToByte(data.Length);
                    for (byte index = 2, index1 = 0; index < data.Length + 2; index++, index1++)
                        inputData[index] = data[index1];
                }
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.UPDATE_RECORD), p1, p2, (dataLength.HasValue ? dataLength.Value : Convert.ToByte(data.Length)), data, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        public string ReadRecord(byte? recordNumber, byte dataLength, ref byte[] outputData)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] temp = new byte[2];
            int length  = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (recordNumber.HasValue)
                {
                    p1 = recordNumber.Value;
                    p2 = 0x4;
                }
                if (outputData.Length <= (dataLength + 2))
                    outputData = new byte[dataLength + 4];
                temp = ExecuteTransmit(AccessType.READ, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.READ_RECORD), p1, p2, Convert.ToByte(dataLength + 2), temp, ref outputData, ref length);
                string sw = SUCCESS;
                if (temp[0] != 0x61 && temp[1] != 0x90)
                    sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", temp[0], temp[1]));

                if (sw.Equals(SUCCESS))
                {
                    byte[] output = new byte[dataLength];
                    for (int idx = 2, idx1 = 0; idx < dataLength + 2; idx++, idx1++)
                        output[idx1] = outputData[idx];
                    outputData = output;
                }
                else
                    return sw;
            }
            else
                return message;
            return SUCCESS;
        }

        public string EraseRecord(byte? recordNumber, EraseRecordType? erase)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] temp = new byte[2];
            int length  = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 1, p2 = 0;
                if (recordNumber.HasValue)
                {
                    p1 = recordNumber.Value;
                    if (erase.HasValue)
                        p2 = (byte)erase;
                    else
                        p2 = (byte)EraseRecordType.CURRENT_RECORD_ONLY;
                }
                temp = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.ERASE_RECORD), p1, p2, 0x0, temp, ref temp, ref length);
                if (temp[0] != 0x61 && temp[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", temp[0], temp[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        public string AppendRecord(byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message   = SelectFile();
            byte[] inputData = data;
            if (message.Equals(SUCCESS))
            {
                if (dataLength.HasValue)
                {
                    inputData    = new byte[Convert.ToByte(dataLength + 2)];
                    inputData[0] = 0;
                    inputData[1] = dataLength.Value;
                    for (byte index = 2, index1 = 0; index < dataLength + 2; index++, index1++)
                        inputData[index] = data[index1];
                }
                else
                {
                    inputData = new byte[Convert.ToByte(data.Length + 2)];
                    inputData[0] = 0;
                    inputData[1] = Convert.ToByte(data.Length);
                    for (byte index = 2, index1 = 0; index < data.Length + 2; index++, index1++)
                        inputData[index] = data[index1];
                }

                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.APPEND_RECORD), 0x0, 0x0, (dataLength.HasValue ? Convert.ToByte(dataLength.Value + 2) : Convert.ToByte(data.Length + 2)), inputData, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        #endregion

        #region Binary Functions

        public string WriteBinary(int? offset, byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (offset.HasValue)
                {
                    p1 = GetByteFromInteger(offset.Value, 1);
                    p2 = GetByteFromInteger(offset.Value, 2);
                }
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.WRITE_BINARY), p1, p2, (dataLength.HasValue ? dataLength.Value : Convert.ToByte(data.Length)), data, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;

            return SUCCESS;
        }

        public string UpdateBinary(int? offset, byte[] data, byte? dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] output = new byte[2];
            int length    = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (offset.HasValue)
                {
                    p1 = GetByteFromInteger(offset.Value, 1);
                    p2 = GetByteFromInteger(offset.Value, 2);
                }
                output = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.UPDATE_BINARY), p1, p2, (dataLength.HasValue ? dataLength.Value : Convert.ToByte(data.Length)), data, ref output, ref length);
                if (output[0] != 0x61 && output[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));
            }
            else
                return message;

            return SUCCESS;
        }

        public string ReadBinary(int? offset, byte dataLength, ref byte[] outputData)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] temp = new byte[2];
            int length  = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (offset.HasValue)
                {
                    p1 = GetByteFromInteger(offset.Value, 1);
                    p2 = GetByteFromInteger(offset.Value, 2);
                }
                if (outputData.Length <= dataLength)
                    outputData = new byte[dataLength + 2];
                temp = ExecuteTransmit(AccessType.READ, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.READ_BINARY), p1, p2, dataLength, temp, ref outputData, ref length);
                string sw = SUCCESS;
                if (temp[0] != 0x61 && temp[1] != 0x90)
                    sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", temp[0], temp[1]));

                if (sw.Equals(SUCCESS))
                {
                    byte[] output = new byte[dataLength];
                    for (int idx = 0; idx < dataLength; idx++)
                        output[idx] = outputData[idx];
                    outputData = output;
                }
                else
                    return sw;
            }
            else
                return message;
            return SUCCESS;
        }

        public string EraseBinary(int? offset, byte dataLength)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            byte[] temp = new byte[2];
            int length  = 0;

            string message = SelectFile();
            if (message.Equals(SUCCESS))
            {
                byte p1 = 0, p2 = 0;
                if (offset.HasValue)
                {
                    p1 = GetByteFromInteger(offset.Value, 1);
                    p2 = GetByteFromInteger(offset.Value, 2);
                }
                temp = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.ERASE_BINARY), p1, p2, 0x0, temp, ref temp, ref length);
                if (temp[0] != 0x61 && temp[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", temp[0], temp[1]));
            }
            else
                return message;
            return SUCCESS;
        }

        #endregion

        #region File related functions

        public string SelectFile()
        {
            CLA cla = new CLA();
            INS ins = new INS();

            byte[] data = new byte[2];
            int length  = 0;
            data[0]     = Convert.ToByte(_fileName.Substring(0, 2), 16);
            data[1]     = Convert.ToByte(_fileName.Substring(2, 2), 16);

            data = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.SELECT_FILE), 0x0, 0x0, 0x2, data, ref data, ref length);

            if (data[0] != 0x61 && data[1] != 0x90)
                return ParseSWMessage(string.Format("{0:X2}{1:X2}", data[0], data[1]));
            return SUCCESS;
        }

        public bool FileExists()
        {
            if (SelectFile().Equals(SUCCESS))
                return true;
            return false;
        }
        
        public string DeleteFile()
        {
            //throw new NotImplementedException("This function is not yet implemented");
            CLA cla     = new CLA();
            INS ins     = new INS();
            byte[] data = new byte[2];
            int length  = 0;
            data[0] = Convert.ToByte(_fileName.Substring(0, 2), 16);
            data[1] = Convert.ToByte(_fileName.Substring(2, 2), 16);
            data = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.DELETE_FILE), 0x0, 0x0, 0x2, data, ref data, ref length);

            if (data[0] != 0x61 && data[1] != 0x90)
                return ParseSWMessage(string.Format("{0:X2}{1:X2}", data[0], data[1]));
            return SUCCESS;
        }

        public string CreateFile(bool dontExecute, ref string compiledString)
        {
            CLA cla      = new CLA();
            INS ins      = new INS();
            byte[] data  = new byte[255];
            byte pointer = 0;

            byte length  = 0;
            byte temp    = 0;

            if (_parameters == null)
                throw new InvalidSmartCardFileException("File Descriptor value can't be null");

            data[pointer++] = 0x62; //file control parameter template
            data[pointer++] = 0;
            if (_parameters.SizeExcludeStruct.HasValue)
            {
                data[pointer++] = 0x80;
                if (_parameters.SizeExcludeStruct.Value >= 0 && _parameters.SizeExcludeStruct.Value <= 255)
                {
                    if (_parameters.IsSizeExcludeStructLengthExt.HasValue && _parameters.IsSizeExcludeStructLengthExt.Value)
                    {
                        data[pointer++] = 0x2;
                        data[pointer++] = 0x0;
                        data[pointer++] = Convert.ToByte(_parameters.SizeExcludeStruct.Value);
                    }
                    else
                    {
                        data[pointer++] = 0x1;
                        data[pointer++] = Convert.ToByte(_parameters.SizeExcludeStruct.Value);
                    }
                }
                else if (_parameters.SizeExcludeStruct.Value >= 256 && _parameters.SizeExcludeStruct.Value <= 65535)
                {
                    data[pointer++] = 0x2;
                    data[pointer++] = GetByteFromInteger(_parameters.SizeExcludeStruct.Value, 1);
                    data[pointer++] = GetByteFromInteger(_parameters.SizeExcludeStruct.Value, 2);
                }
            }
            if (_parameters.SizeIncludeStruct.HasValue)
            {
                data[pointer++] = 0x81;
                if (_parameters.SizeIncludeStruct.Value >= 0 && _parameters.SizeIncludeStruct.Value <= 255)
                {
                    if (_parameters.IsSizeIncludeStructLengthExt.HasValue && _parameters.IsSizeIncludeStructLengthExt.Value)
                    {
                        data[pointer++] = 0x2;
                        data[pointer++] = 0x0;
                        data[pointer++] = Convert.ToByte(_parameters.SizeIncludeStruct.Value);
                    }
                    else
                    {
                        data[pointer++] = 0x1;
                        data[pointer++] = Convert.ToByte(_parameters.SizeIncludeStruct.Value);
                    }
                }
                else if (_parameters.SizeIncludeStruct.Value >= 256 && _parameters.SizeIncludeStruct.Value <= 65535)
                {
                    data[pointer++] = 0x2;
                    data[pointer++] = GetByteFromInteger(_parameters.SizeIncludeStruct.Value, 1);
                    data[pointer++] = GetByteFromInteger(_parameters.SizeIncludeStruct.Value, 2);
                }
            }
            if (_parameters.Descriptors.Count > 0)
            {
                data[pointer++] = 0x82;

                length = 1;
                if (_parameters.CodingByte.HasValue)
                    length++;
                if (_parameters.MaxRecordSize.HasValue)
                {
                    if (_parameters.MaxRecordSize.Value >= 0 && _parameters.MaxRecordSize.Value <= 255)
                    {
                        if (_parameters.NoOfRecords.HasValue)
                            length += 2;
                        else
                            length++;
                    }
                    else if (_parameters.MaxRecordSize.Value >= 256 && _parameters.MaxRecordSize.Value <= 65535)
                        length += 2;

                    if (_parameters.NoOfRecords.HasValue)
                    {
                        if (_parameters.NoOfRecords.Value >= 0 && _parameters.NoOfRecords.Value <= 255)
                            length++;
                        else if (_parameters.NoOfRecords.Value >= 256 && _parameters.NoOfRecords.Value <= 65535)
                            length += 2;
                    }
                }
                data[pointer++] = length;

                byte descriptorValue = 0;
                foreach (FileDescriptor d in _parameters.Descriptors)
                    descriptorValue += (byte)d;
                data[pointer++] = descriptorValue;

                if (_parameters.CodingByte.HasValue)
                    data[pointer++] = (byte)_parameters.CodingByte.Value;

                if (_parameters.MaxRecordSize.HasValue)
                {
                    if (_parameters.MaxRecordSize.Value >= 0 && _parameters.MaxRecordSize.Value <= 255)
                    {
                        if (_parameters.NoOfRecords.HasValue)
                            data[pointer++] = 0;
                        data[pointer++] = (byte)_parameters.MaxRecordSize.Value;
                    }
                    else if (_parameters.MaxRecordSize.Value >= 256 && _parameters.MaxRecordSize.Value <= 65535)
                    {
                        data[pointer++] = GetByteFromInteger(_parameters.MaxRecordSize.Value, 1);
                        data[pointer++] = GetByteFromInteger(_parameters.MaxRecordSize.Value, 2);
                    }
                    if (_parameters.NoOfRecords.HasValue)
                    {
                        if (_parameters.NoOfRecords.Value >= 0 && _parameters.NoOfRecords.Value <= 255)
                            data[pointer++] = (byte)_parameters.NoOfRecords.Value;
                        else if (_parameters.NoOfRecords.Value >= 256 && _parameters.NoOfRecords.Value <= 65535)
                        {
                            data[pointer++] = GetByteFromInteger(_parameters.NoOfRecords.Value, 1);
                            data[pointer++] = GetByteFromInteger(_parameters.NoOfRecords.Value, 2);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(_parameters.FileIdentifier))
            {
                data[pointer++] = 0x83;
                data[pointer++] = 0x2;
                data[pointer++] = Convert.ToByte(_parameters.FileIdentifier.Substring(0, 2), 16);
                data[pointer++] = Convert.ToByte(_parameters.FileIdentifier.Substring(2, 2), 16);
            }

            if (!string.IsNullOrEmpty(_parameters.DFName))
            {
                data[pointer++] = 0x84;
                data[pointer++] = 0;
                temp   = Convert.ToByte(pointer - 1);
                length = 0;
                for (int idx = 0; idx <= _parameters.DFName.Length; idx+=2)
                {
                    if (idx == _parameters.DFName.Length)
                        data[pointer++] = Convert.ToByte(_parameters.DFName.Substring(idx), 16);
                    else
                        data[pointer++] = Convert.ToByte(_parameters.DFName.Substring(idx, 2), 16);
                    length++;
                }
                data[temp] = length;
            }

            if (!string.IsNullOrEmpty(_parameters.ShortEFIdentifer))
            {
                data[pointer++] = 0x88;
                data[pointer++] = 0x1;
                data[pointer++] = Convert.ToByte(_parameters.ShortEFIdentifer.Substring(0), 16);
            }

            if (_parameters.LifeCycle.HasValue)
            {
                data[pointer++] = 0x8A;
                data[pointer++] = 0x1;
                data[pointer++] = (byte)_parameters.LifeCycle.Value;
            }

            if (!string.IsNullOrEmpty(_parameters.ExtendedEFOfFileInfo))
            {
                data[pointer++] = 0x87;
                data[pointer++] = 0x2;
                data[pointer++] = Convert.ToByte(_parameters.ExtendedEFOfFileInfo.Substring(0, 2), 16);
                data[pointer++] = Convert.ToByte(_parameters.ExtendedEFOfFileInfo.Substring(2, 2), 16);
            }

            if (!string.IsNullOrEmpty(_parameters.EFForSecurityEnvTemplate))
            {
                data[pointer++] = 0x8D;
                data[pointer++] = 0x2;
                data[pointer++] = Convert.ToByte(_parameters.EFForSecurityEnvTemplate.Substring(0, 2), 16);
                data[pointer++] = Convert.ToByte(_parameters.EFForSecurityEnvTemplate.Substring(2, 2), 16);
            }

            if (_parameters.CompactAccessModeForDF.Count > 0)
            {
                data[pointer++] = 0x8C;
                data[pointer++] = 0;
                length = 1;
                temp   = Convert.ToByte(pointer - 1);

                byte accessMode = 0;
                foreach (KeyValuePair<CompactAccessModeDF, CompactSecurityCondition> df in _parameters.CompactAccessModeForDF)
                    accessMode += (byte)df.Key;
                data[pointer++] = accessMode;

                foreach (KeyValuePair<CompactAccessModeDF, CompactSecurityCondition> df in _parameters.CompactAccessModeForDF)
                {
                    length++;
                    data[pointer++] = (byte)df.Value;
                }
                data[temp] = length;
            }
            if (_parameters.CompactAccessModeForEF.Count > 0)
            {
                data[pointer++] = 0x8C;
                data[pointer++] = 0;
                length = 1;
                temp   = Convert.ToByte(pointer - 1);

                byte accessMode = 0;
                foreach (KeyValuePair<CompactAccessModeEF, CompactSecurityCondition> df in _parameters.CompactAccessModeForEF)
                    accessMode += (byte)df.Key;
                data[pointer++] = accessMode;

                foreach (KeyValuePair<CompactAccessModeEF, CompactSecurityCondition> df in _parameters.CompactAccessModeForEF)
                {
                    length++;
                    data[pointer++] = (byte)df.Value;
                }
                data[temp] = length;
            }
            if (_parameters.CompactAccessModeForDataObject.Count > 0)
            {
                data[pointer++] = 0x8C;
                data[pointer++] = 0;
                length = 1;
                temp = Convert.ToByte(pointer - 1);

                byte accessMode = 0;
                foreach (KeyValuePair<CompactAccessModeDataObject, CompactSecurityCondition> df in _parameters.CompactAccessModeForDataObject)
                    accessMode += (byte)df.Key;
                data[pointer++] = accessMode;

                foreach (KeyValuePair<CompactAccessModeDataObject, CompactSecurityCondition> df in _parameters.CompactAccessModeForDataObject)
                {
                    length++;
                    data[pointer++] = (byte)df.Value;
                }
                data[temp] = length;
            }

            if (_parameters.ExpandedSecurityCondition != null)
            {
                data[pointer++] = 0xAB;
                data[pointer++] = 0;
                length = 0;
                temp   = Convert.ToByte(pointer - 1);

                if (_parameters.ExpandedSecurityCondition.ExpandedAccessModeDataObject.HasValue)
                {
                    data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.ExpandedAccessModeDataObject.Value;
                    data[pointer++] = 0x1;
                    if (_parameters.ExpandedSecurityCondition.ExpandedAccessModeDataObject.Value == ExpandAccessModeDataObject.COMPACT_FORMAT)
                    {
                        if (_parameters.ExpandedSecurityCondition.CompactAccessModeForDF.HasValue)
                            data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.CompactAccessModeForDF.Value;
                        else if (_parameters.ExpandedSecurityCondition.CompactAccessModeForEF.HasValue)
                            data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.CompactAccessModeForEF.Value;
                        else if (_parameters.ExpandedSecurityCondition.CompactAccessModeForDataObject.HasValue)
                            data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.CompactAccessModeForDataObject.Value;
                    }
                    else
                    {
                        if (_parameters.ExpandedSecurityCondition.INSCode != null)
                            data[pointer++] = _parameters.ExpandedSecurityCondition.INSCode.GetINS();
                    }
                    length += 3;
                }

                if (_parameters.ExpandedSecurityCondition.ExpandedSecurityCondition.HasValue)
                {
                    data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.ExpandedSecurityCondition.Value;
                    if (_parameters.ExpandedSecurityCondition.ExpandedSecurityCondition.Value == ExpandSecurityCondition.COMPACT_SECURITY_CONDITION)
                    {
                        data[pointer++] = 0x1;
                        if (_parameters.ExpandedSecurityCondition.CompactSecurityInExpandSecurity.HasValue)
                        {
                            data[pointer++] = (byte)_parameters.ExpandedSecurityCondition.CompactSecurityInExpandSecurity.Value;
                            length += 3;
                        }
                        else
                            length += 2;
                    }
                    else
                    {
                        data[pointer++] = 0x0;
                        length += 2;
                    }
                }
                data[temp] = length;
            }
            data[1] = Convert.ToByte(pointer - 2);

            compiledString = cla.GetCLA().ToString("X2") + ins.GetINS(INSCode.CREATE_FILE).ToString("X2") + "0000" + pointer.ToString("X2");
            for (byte idx = 0; idx < pointer; idx++)
                compiledString = string.Format("{0}{1}", compiledString, string.Format("{0:X2}", data[idx]));
            if (!dontExecute)
            {
                byte[] sw = new byte[2];
                int len   = 0;
                sw = ExecuteTransmit(AccessType.WRITE, _cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.CREATE_FILE), 0x0, 0x0, pointer, data, ref sw, ref len);

                if (sw[0] != 0x61 && sw[1] != 0x90)
                    return ParseSWMessage(string.Format("{0:X2}{1:X2}", sw[0], sw[1]));
            }

            return SUCCESS;
        }

        #endregion
    }

    #region Security

    public sealed class Security : CoreSmartCard
    {
        private int _protocol    = 0;

        public Security(int protocol)
        {
            _protocol    = protocol;
        }

        public string Verify(int cardHandle, SecurityVerify verifyType, byte[] securityKey, byte securityLength, byte secretNumber)
        {
            CLA cla = new CLA();
            INS ins = new INS();

            byte p2    = Convert.ToByte((byte)verifyType + secretNumber);
            int length = 0;
            byte[] output = new byte[2];
            output = ExecuteTransmit(AccessType.WRITE, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.VERIFY), 0x0, p2, securityLength, securityKey, ref output, ref length);

            string sw = SUCCESS;
            if (output[0] != 0x61 && output[1] != 0x90)
                sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            if (!sw.Equals(SUCCESS))
            {
                string count = string.Format("{0:X2}", output[1]);
                return string.Format("{0}, you are further allowed to retry for {1} times", sw, count.Substring(count.Length - 1));
            }
            return SUCCESS;
        }

        public string ManageSecurityEnvironment(int cardHandle, SecurityEnvironment environ, byte? seID, byte[] securityKey, byte securityLength, SecurityCtrlRefTemplate? crt, List<SecurityMessage> messages)
        {
            if (environ != SecurityEnvironment.SET)
            {
                if (!seID.HasValue)
                    return "SEID is required in the range of 1 and 14 for STORE/RESTORE/ERASE operation";
                else
                    if (seID.Value < 1 || seID.Value > 14)
                        return "SEID should be in the range of 1 and 14";
            }
            else
            {
                if (!crt.HasValue)
                    return "Control Reference Template is mandatory for SET operation";
                if (messages.Count == 0)
                    return "Security Message is mandatory for SET operation";
            }

            byte p1 = 0, p2 = 0;

            p1 = (byte)environ;
            if (environ == SecurityEnvironment.SET)
            {
                foreach (SecurityMessage m in messages)
                    p1 += (byte)m;

                p2 = (byte)crt;
            }
            else
                p2 = seID.Value;

            int length    = 0;
            byte[] output = new byte[2];
            CLA cla = new CLA();
            INS ins = new INS();

            if (environ != SecurityEnvironment.SET)
                output = ExecuteTransmit(AccessType.WRITE, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.MANAGE_SECURITY_ENV), p1, p2, 0x0, output, ref output, ref length);
            else
            {
                byte[] temp = new byte[securityLength + 2];
                temp[0] = 0x94;
                temp[1] = securityLength;
                for (byte idx = 0; idx < securityLength; idx++)
                    temp[idx + 2] = securityKey[idx];

                output = ExecuteTransmit(AccessType.WRITE, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.MANAGE_SECURITY_ENV), p1, p2, Convert.ToByte(securityLength + 2), temp, ref output, ref length);
            }

            if (output[0] != 0x61 && output[1] != 0x90)
                return ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            return SUCCESS;
        }

        public string GetChallenge(int cardHandle, byte challengeLength, ref byte[] challengeData)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            int length    = 0;
            byte[] output = new byte[2];

            if (challengeData.Length <= challengeLength)
                challengeData = new byte[challengeLength + 2];
            output = ExecuteTransmit(AccessType.READ, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.GET_CHALLENGE), 0x0, 0x0, challengeLength, output, ref challengeData, ref length);
            string sw = SUCCESS;
            if (output[0] != 0x61 && output[1] != 0x90)
                sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            if (sw.Equals(SUCCESS))
            {
                byte[] temp = new byte[challengeLength];
                for (int idx = 0; idx < challengeLength; idx++)
                    temp[idx] = challengeData[idx];
                challengeData = temp;
            }
            else
                return sw;

            return SUCCESS;
        }

        public string InternalAuthenticate(int cardHandle, byte challengeLength, byte[] challengeData, SecurityVerify verifyType, byte secretNumber)
        {
            CLA cla = new CLA();
            INS ins = new INS();

            byte p2 = Convert.ToByte((byte)verifyType + secretNumber);
            int length = 0;
            byte[] output = new byte[2];
            output = ExecuteTransmit(AccessType.WRITE, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.INTERNAL_AUTH), 0x0, p2, challengeLength, challengeData, ref output, ref length);

            string sw = SUCCESS;
            if (output[0] != 0x61 && output[1] != 0x90)
                sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            if (!sw.Equals(SUCCESS))
            {
                string count = string.Format("{0:X2}", output[1]);
                return string.Format("{0}, you are further allowed to retry for {1} times", sw, count.Substring(count.Length - 1));
            }
            return SUCCESS;
        }

        public string GetResponse(int cardHandle, byte responseLength, ref byte[] responseData)
        {
            CLA cla = new CLA();
            INS ins = new INS();
            int length = 0;
            byte[] output = new byte[2];

            if (responseData.Length <= responseLength)
                responseData = new byte[responseLength + 2];
            output = ExecuteTransmit(AccessType.READ, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.GET_RESPONSE), 0x0, 0x0, responseLength, output, ref responseData, ref length);
            string sw = SUCCESS;
            if (output[0] != 0x61 && output[1] != 0x90)
                sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            if (sw.Equals(SUCCESS))
            {
                byte[] temp = new byte[responseLength];
                for (int idx = 0; idx < responseLength; idx++)
                    temp[idx] = responseData[idx];
                responseData = temp;
            }
            else
                return sw;

            return SUCCESS;
        }

        public string ExternalAuthenticate(int cardHandle, byte responseLength, byte[] responseData, SecurityVerify verifyType, byte secretNumber)
        {
            CLA cla = new CLA();
            INS ins = new INS();

            byte p2    = Convert.ToByte((byte)verifyType + secretNumber);
            int length = 0;
            byte[] output = new byte[2];
            output = ExecuteTransmit(AccessType.WRITE, cardHandle, _protocol, cla.GetCLA(), ins.GetINS(INSCode.EXTERNAL_MUTUAL_AUTH), 0x0, p2, responseLength, responseData, ref output, ref length);

            string sw = SUCCESS;
            if (output[0] != 0x61 && output[1] != 0x90)
                sw = ParseSWMessage(string.Format("{0:X2}{1:X2}", output[0], output[1]));

            if (!sw.Equals(SUCCESS))
            {
                string count = string.Format("{0:X2}", output[1]);
                return string.Format("{0}, you are further allowed to retry for {1} times", sw, count.Substring(count.Length - 1));
            }
            return SUCCESS;
        }

        public string Authenticate(int beneficiaryCard, int mkcCard, string password, string seURN1, byte seID, byte seID1, byte challengeLength)
        {
            string seURN = (seURN1.Length > 16 ? seURN1.Substring(0, 16) : seURN1);
            if (seID < 1 || seID > 14)
                return "SEID should be in the range of 1 and 14";

            byte[] pwd     = Convertor.StringToByteArray(password, null, null);
            string message = string.Empty;
            List<SecurityMessage> messages = new List<SecurityMessage>();

            File file = new File("3F00", mkcCard, _protocol);
            message   = file.SelectFile();
            if (!message.Equals(SUCCESS))
                return string.Format("Enter a valid Card, {0}", message);

            file.FileName = "B100";
            message = file.SelectFile();
            if (!message.Equals(SUCCESS))
                return string.Format("Enter a valid Card, {0}", message);

            message = Verify(mkcCard, SecurityVerify.DF_SPECIFIC_PASSWORD, pwd, Convert.ToByte(pwd.Length), Convert.ToByte(seID - 1));
            if (!message.Equals(SUCCESS))
                return message;

            File file1 = new File("3F00", beneficiaryCard, _protocol);
            message = file1.SelectFile();
            if (!message.Equals(SUCCESS))
                return string.Format("Enter a valid Card, {0}", message);

            file1.FileName = "E000";
            message = file1.SelectFile();
            if (!message.Equals(SUCCESS))
                return string.Format("Enter a valid Card, {0}", message);

            pwd = Convertor.StringToByteArray(seURN, null, null);
            message = ManageSecurityEnvironment(mkcCard, SecurityEnvironment.RESTORE, seID, pwd, Convert.ToByte(pwd.Length), null, messages);
            if (!message.Equals(SUCCESS))
                return message;

            messages.Add(SecurityMessage.VERIFY_ENCIPHER_EXTAUTH_KEYAGGR);
            message = ManageSecurityEnvironment(mkcCard, SecurityEnvironment.SET, null, pwd, Convert.ToByte(pwd.Length), SecurityCtrlRefTemplate.AUTHENTICATION, messages);
            if (!message.Equals(SUCCESS))
                return message;

            message = GetChallenge(mkcCard, challengeLength, ref pwd);
            if (!message.Equals(SUCCESS))
                return message;

            message = InternalAuthenticate(beneficiaryCard, challengeLength, pwd, SecurityVerify.DF_SPECIFIC_PASSWORD, Convert.ToByte(seID - 1));
            if (!message.Equals(SUCCESS))
                return message;

            message = GetResponse(beneficiaryCard, challengeLength, ref pwd);
            if (!message.Equals(SUCCESS))
                return message;

            message = ExternalAuthenticate(mkcCard, challengeLength, pwd, SecurityVerify.DF_SPECIFIC_PASSWORD, Convert.ToByte(seID - 1));
            if (!message.Equals(SUCCESS))
                return message;

            messages.Clear();
            pwd = Convertor.StringToByteArray(seURN, null, null);
            message = ManageSecurityEnvironment(mkcCard, SecurityEnvironment.RESTORE, seID1, pwd, Convert.ToByte(pwd.Length), null, messages);
            if (!message.Equals(SUCCESS))
                return message;

            messages.Add(SecurityMessage.COMPUTATION_DECIPHER_INTERNALAUTH_KEYAGGR);
            message = ManageSecurityEnvironment(mkcCard, SecurityEnvironment.SET, null, pwd, Convert.ToByte(pwd.Length), SecurityCtrlRefTemplate.AUTHENTICATION, messages);
            if (!message.Equals(SUCCESS))
                return message;

            message = GetChallenge(beneficiaryCard, challengeLength, ref pwd);
            if (!message.Equals(SUCCESS))
                return message;

            message = InternalAuthenticate(mkcCard, challengeLength, pwd, SecurityVerify.DF_SPECIFIC_PASSWORD, Convert.ToByte(seID1 - 1));
            if (!message.Equals(SUCCESS))
                return message;

            message = GetResponse(mkcCard, challengeLength, ref pwd);
            if (!message.Equals(SUCCESS))
                return message;

            message = ExternalAuthenticate(beneficiaryCard, challengeLength, pwd, SecurityVerify.DF_SPECIFIC_PASSWORD, 0x3);
            if (!message.Equals(SUCCESS))
                return message;

            return SUCCESS;
        }
    }

    #endregion

    #region File Control Parameters

    public sealed class FileControlParams
    {
        public class ExpandedSecurityFormat
        {
            private ExpandAccessModeDataObject? _expandAccessMode = null;
            private ExpandSecurityCondition? _expandSecurity      = null;
            private CompactSecurityCondition? _compactInExpand    = null;
            private CompactAccessModeDF? _compactAccessModeDF                 = null;
            private CompactAccessModeEF? _compactAccessModeEF                 = null;
            private CompactAccessModeDataObject? _compactAccessModeDataObject = null;
            private INS _ins = null;

            public INS INSCode
            {
                get { return _ins;  }
                set { _ins = value; }
            }

            public CompactAccessModeDF? CompactAccessModeForDF
            {
                get { return _compactAccessModeDF;  }
                set { _compactAccessModeDF = value; }
            }

            public CompactAccessModeEF? CompactAccessModeForEF
            {
                get { return _compactAccessModeEF;  }
                set { _compactAccessModeEF = value; }
            }

            public CompactAccessModeDataObject? CompactAccessModeForDataObject
            {
                get { return _compactAccessModeDataObject;  }
                set { _compactAccessModeDataObject = value; }
            }

            public ExpandAccessModeDataObject? ExpandedAccessModeDataObject
            {
                get { return _expandAccessMode;  }
                set { _expandAccessMode = value; }
            }

            public ExpandSecurityCondition? ExpandedSecurityCondition
            {
                get { return _expandSecurity;  }
                set { _expandSecurity = value; }
            }

            public CompactSecurityCondition? CompactSecurityInExpandSecurity
            {
                get { return _compactInExpand;  }
                set { _compactInExpand = value; }
            }
        }

        private int? _sizeExcludeStruct          = null;
        private int? _sizeIncludeStruct          = null;
        private bool? _sizeExcludeStructExt      = null;
        private bool? _sizeIncludeStructExt      = null;
        private List<FileDescriptor> _descriptor = new List<FileDescriptor>();
        private DataCodingByte? _dataCoding      = null;
        private int? _maxRecordSize              = null;
        private int? _noOfRecords                = null;
        private string _fileIdentifier           = string.Empty;
        private string _dfName                   = string.Empty;
        private string _shortEFIdentifier        = string.Empty;
        private FileLifeCycle? _lifeCycle        = null;
        private string _extEFOfFileInfo          = string.Empty;
        private string _efForSecurityEnvTemplate = string.Empty;
        private List<KeyValuePair<CompactAccessModeDF, CompactSecurityCondition>> _compactAccessModeDF = new List<KeyValuePair<CompactAccessModeDF, CompactSecurityCondition>>();
        private List<KeyValuePair<CompactAccessModeEF, CompactSecurityCondition>> _compactAccessModeEF = new List<KeyValuePair<CompactAccessModeEF, CompactSecurityCondition>>();
        private List<KeyValuePair<CompactAccessModeDataObject, CompactSecurityCondition>> _compactAccessModeDataObject = new List<KeyValuePair<CompactAccessModeDataObject, CompactSecurityCondition>>();
        private ExpandedSecurityFormat _expandSecurityFormat = null;

        public ExpandedSecurityFormat ExpandedSecurityCondition
        {
            get { return _expandSecurityFormat;  }
            set { _expandSecurityFormat = value; }
        }

        public int? SizeExcludeStruct
        {
            get { return _sizeExcludeStruct;  }
            set 
            {
                if (value.HasValue && value > 65535)
                    throw new ArgumentOutOfRangeException("The size can't be more than 65535");
                _sizeExcludeStruct = value; 
            }
        }

        public bool? IsSizeExcludeStructLengthExt
        {
            get { return _sizeExcludeStructExt;  }
            set { _sizeExcludeStructExt = value; }
        }

        public int? SizeIncludeStruct
        {
            get { return _sizeIncludeStruct;  }
            set 
            {
                if (value.HasValue && value > 65535)
                    throw new ArgumentOutOfRangeException("The size can't be more than 65535");
                _sizeIncludeStruct = value; 
            }
        }

        public bool? IsSizeIncludeStructLengthExt
        {
            get { return _sizeIncludeStructExt;  }
            set { _sizeIncludeStructExt = value; }
        }

        public List<FileDescriptor> Descriptors
        {
            get { return _descriptor;  }
            set { _descriptor = value; }
        }

        public DataCodingByte? CodingByte
        {
            get { return _dataCoding;  }
            set { _dataCoding = value; }
        }

        public int? MaxRecordSize
        {
            get { return _maxRecordSize;  }
            set 
            {
                if (value.HasValue && value > 65535)
                    throw new ArgumentOutOfRangeException("The size can't be more than 65535");
                _maxRecordSize= value; 
            }
        }

        public int? NoOfRecords
        {
            get { return _noOfRecords;  }
            set 
            {
                if (value.HasValue && value > 65535)
                    throw new ArgumentOutOfRangeException("The size can't be more than 65535");
                _noOfRecords= value; 
            }
        }

        public string FileIdentifier
        {
            get { return _fileIdentifier;  }
            set 
            {
                if (value.Length != 4)
                    throw new InvalidSmartCardFileException("Invalid File Name specified");
                _fileIdentifier = value; 
            }
        }

        public string EFForSecurityEnvTemplate
        {
            get { return _efForSecurityEnvTemplate;  }
            set 
            {
                if (value.Length != 4)
                    throw new InvalidSmartCardFileException("Invalid EF Identifier for holding Security Environment Templates");
                _efForSecurityEnvTemplate = value; 
            }
        }

        public string DFName
        {
            get { return _dfName;  }
            set 
            {
                if (value.Length > 32)
                    throw new InvalidSmartCardFileException("Invalid DF Name specified");
                _dfName = value; 
            }
        }

        public string ShortEFIdentifer
        {
            get { return _shortEFIdentifier;  }
            set 
            {
                if (value.Length > 2)
                    throw new InvalidSmartCardFileException("Invalid Short EF Identifier");
                _shortEFIdentifier = value; 
            }
        }

        public FileLifeCycle? LifeCycle
        {
            get { return _lifeCycle;  }
            set { _lifeCycle = value; }
        }

        public string ExtendedEFOfFileInfo
        {
            get { return _extEFOfFileInfo;  }
            set 
            {
                if (value.Length != 4)
                    throw new InvalidSmartCardFileException("Invalid Extended EF (for holding file control information) specified");
                _extEFOfFileInfo = value; 
            }
        }

        public List<KeyValuePair<CompactAccessModeDF, CompactSecurityCondition>> CompactAccessModeForDF
        {
            get { return _compactAccessModeDF;  }
            set { _compactAccessModeDF = value; }
        }

        public List<KeyValuePair<CompactAccessModeEF, CompactSecurityCondition>> CompactAccessModeForEF
        {
            get { return _compactAccessModeEF;  }
            set { _compactAccessModeEF = value; }
        }

        public List<KeyValuePair<CompactAccessModeDataObject, CompactSecurityCondition>> CompactAccessModeForDataObject
        {
            get { return _compactAccessModeDataObject;  }
            set { _compactAccessModeDataObject = value; }
        }
    }

    public sealed class CLA
    {
        public byte GetCLA()
        {
            return 0x00;
        }
    }

    public sealed class INS
    {
        private INSCode _ins = INSCode.SELECT_FILE;

        public INSCode SelectedINS
        {
            get { return _ins;  }
            set { _ins = value; }
        }

        public byte GetINS()
        {
            return (byte)_ins;
        }

        public byte GetINS(INSCode ins)
        {
            return (byte)ins;
        }
    }

    #endregion

    #region Exception Classes

    public sealed class InvalidSmartCardContextException : Exception
    {
        private string _errorMessage = string.Empty;

        public InvalidSmartCardContextException(string error)
        {
            _errorMessage = error;
        }

        public override string ToString()
        {
            return _errorMessage;
        }

        public override string Message
        {
            get { return _errorMessage; }
        }
    }

    public sealed class InvalidSmartCardException : Exception
    {
        private string _errorMessage = string.Empty;

        public InvalidSmartCardException(string error)
        {
            _errorMessage = error;
        }

        public override string ToString()
        {
            return _errorMessage;
        }

        public override string Message
        {
            get { return _errorMessage; }
        }
    }

    public sealed class InvalidSmartCardFileException : Exception
    {
        private string _errorMessage = string.Empty;

        public InvalidSmartCardFileException(string error)
        {
            _errorMessage = error;
        }

        public override string ToString()
        {
            return _errorMessage;
        }

        public override string Message
        {
            get { return _errorMessage; }
        }
    }

    #endregion

    #endregion
}