using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Sample : Form
    {
        public Sample()
        {
            SmartCardManagement.Context context = new SmartCardManagement.Context();
            List<string> cards = context.SmartCardsList;

            SmartCardManagement.SmartCard card = context.ConnectTo(cards[0]);
            SmartCardManagement.File file = new SmartCardManagement.File("3F00", card.CardHandle, card.Protocol);
            string s = file.SelectFile();
            /*file.FileName = "E000";
            s = file.SelectFile();
            file.FileName = "E001";
            s = file.SelectFile();
            byte[] b = new byte[100];
            s = file.ReadBinary(0, 15, ref b);*/

            InitializeComponent();
        }

        private void CreateDfFile(SmartCardManagement.SmartCard card, string fileName)
        {
            var file = new SmartCardManagement.File(fileName, card.CardHandle, card.Protocol);
            var s = file.SelectFile();

            var param = new SmartCardManagement.FileControlParams();
            var security = new List<KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>>();
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.DELETE_FILE_SELF, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.TERMINATE_MF_DF, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.DEACTIVATE_FILE, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.CREATE_FILE_DF, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.CREATE_FILE_EF, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeDF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeDF.DELETE_CHILD_FILE, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            param.CompactAccessModeForDF = security;

            var desc = new List<SmartCardManagement.FileDescriptor>();
            desc.Add(SmartCardManagement.FileDescriptor.DF);
            param.Descriptors = desc;
            param.FileIdentifier = fileName;
            param.LifeCycle = SmartCardManagement.FileLifeCycle.OPERATION_STATE_ACTIVATED;

            /*param.ExpandedSecurityCondition = new SmartCardManagement.FileControlParams.ExpandedSecurityFormat();
            param.ExpandedSecurityCondition.ExpandedAccessModeDataObject = SmartCardManagement.ExpandAccessModeDataObject.VALUE_OF_INS; 
            param.ExpandedSecurityCondition.INSCode = new SmartCardManagement.INS { SelectedINS = SmartCardManagement.INSCode.PUT_DATA };
            param.ExpandedSecurityCondition.ExpandedSecurityCondition = SmartCardManagement.ExpandSecurityCondition.NEVER;*/

            file.Parameters = param;

            string msg, msg1 = string.Empty;
            msg = file.CreateFile(false, ref msg1);
        }

        private void CreateEfFile(SmartCardManagement.SmartCard card, string fileName, string shortFileName)
        {
            var file = new SmartCardManagement.File(fileName, card.CardHandle, card.Protocol);
            var s = file.SelectFile();

            var param = new SmartCardManagement.FileControlParams();
            var security = new List<KeyValuePair<SmartCardManagement.CompactAccessModeEF, SmartCardManagement.CompactSecurityCondition>>();
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeEF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeEF.DELETE_FILE, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeEF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeEF.TERMINATE_EF, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeEF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeEF.DEACTIVATE_FILE, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            security.Add(new KeyValuePair<SmartCardManagement.CompactAccessModeEF, SmartCardManagement.CompactSecurityCondition>(SmartCardManagement.CompactAccessModeEF.UPDATE_BINARY_RECORD, SmartCardManagement.CompactSecurityCondition.NO_CONDITION));
            
            param.CompactAccessModeForEF = security;
            List<SmartCardManagement.FileDescriptor> desc = new List<SmartCardManagement.FileDescriptor>();
            desc.Add(SmartCardManagement.FileDescriptor.EF_TRANSPARENT);
            param.Descriptors = desc;
            param.CodingByte = SmartCardManagement.DataCodingByte.DATA_UNIT_SIZE_ONE_BYTE;
            param.FileIdentifier = fileName;
            param.ShortEFIdentifer = shortFileName;
            param.SizeExcludeStruct = 520;

            file.Parameters = param;

            string msg, msg1 = string.Empty;
            msg = file.CreateFile(false, ref msg1);

            byte[] b = SmartCardManagement.Convertor.StringToByteArray("Thank God", null, null);
            s = file.WriteBinary(0, b, null);
            byte[] b1 = new byte[100];
            s = file.ReadBinary(0, 15, ref b1);
            s = file.UpdateBinary(0, SmartCardManagement.Convertor.StringToByteArray("Thank you god", null, null), null);
            s = SmartCardManagement.Convertor.ByteArrayToString(b1);
        }
    }
}
