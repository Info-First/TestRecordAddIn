using System;
using System.Diagnostics;
using System.IO;
using TRIM.SDK;

namespace RecordAddIn
{
    public class RecordAddIn : ITrimAddIn
    {
        /*
		 * Member variables
		 */

        private string logFile;

        /*
		 *  Private methods
		 */

        private string InternalError { get; set; }

        /*
		 * Public Methods
		 */

        //public override string ErrorMessage => InternalError;
        public override string ErrorMessage { get { return InternalError; } }

       public override void Initialise(Database db)
        {
            /* TODO: Here you can initialize any local members of your class. */
            logFile = db.GetTRIMFolder(TrimPathType.ClientLogs, false) + "\\RecordAddin";

            string logPath = Path.Combine(logFile + "_" + db.Id + "_" + DateTime.Today.ToString("yyyy_M_dd") + ".log");

            Trace.Listeners.Clear();

            if (!File.Exists(logPath))
                using (File.Create(logPath)) { }

            try
            {
                Trace.Listeners.Add(new TextWriterTraceListener(logPath, "TextWriterOutputListener"));
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now + " " + ex.Message);
            }

            Trace.AutoFlush = true;

            Trace.TraceInformation(DateTime.Now + " Logging started");
            Trace.TraceInformation(DateTime.Now + " Dataset ID set to: " + db.Id);
            Trace.TraceInformation(DateTime.Now + " Record AddIn initialized on database: {0}.", db.Name);
        }

        public override void Setup(TrimMainObject newObject) { }

        public override TrimMenuLink[] GetMenuLinks()
        {
            return new TrimMenuLink[0];
        }

        public override bool IsMenuItemEnabled(int cmdId, TrimMainObject forObject) => false;

        public override void ExecuteLink(int cmdId, TrimMainObject forObject, ref bool itemWasChanged) { }

        public override void ExecuteLink(int cmdId, TrimMainObjectSearch forTaggedObjects) { }

        public override bool PreDelete(TrimMainObject modifiedObject) => true;

        public override void PostDelete(TrimMainObject deletedObject) { }

        public override bool PreSave(TrimMainObject modifiedObject)
        {
            Record record = modifiedObject as Record;

            if (record.TypedTitle == "TEST TITLE")
            {
                InternalError = "Please enter a more descriptive title";
                return false;
            }
            return true;
        }

        public override void PostSave(TrimMainObject savedObject, bool justCreated) { }

        public override bool SupportsField(FieldDefinition field) => true;

        public override bool SelectFieldValue(FieldDefinition field, TrimMainObject trimObject, string previousValue) => true;

        public override bool VerifyFieldValue(FieldDefinition field, TrimMainObject trimObject, string newValue)
        {
            return true;
        }

        private string GetFieldValueAsString(Record record, string fieldName)
        {
            Database db = record.Database;
            var fieldDefinition = db.FindTrimObjectByName(BaseObjectTypes.FieldDefinition, fieldName) as FieldDefinition;
            return record.GetFieldValue(fieldDefinition)?.ToString() ?? string.Empty;
        }

        private DateTime GetFieldValueAsDate(Record record, string fieldName)
        {
            Database db = record.Database;
            var fieldDefinition = db.FindTrimObjectByName(BaseObjectTypes.FieldDefinition, fieldName) as FieldDefinition;
            return record.GetFieldValue(fieldDefinition).AsDate();
        }
    }
}