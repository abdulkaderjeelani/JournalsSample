using System;
using System.Diagnostics;

namespace Journals.Model
{
    [DebuggerDisplay("Status: {Status}")]
    public class OperationStatus
    {
        //should not be written outside
        public bool Status { get; set; }

        public int RecordsAffected { get; set; }
        public string Message { get; set; }
        public Object OperationID { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionInnerMessage { get; set; }
        public string ExceptionInnerStackTrace { get; set; }

        /// <summary>
        /// Returns the affected Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// fill the result from exception, code in this method should be more cautions, any exceptions in this class will be a
        /// disaster as this is called in catch block, for now this function suppress the internal exceptions,
        /// this shoudl be changed to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void CreateFromException(string message, Exception ex)
        {
            try
            {
                OperationStatus opStatus = this;

                opStatus.Status = false;
                opStatus.Message = message;
                opStatus.OperationID = null;

                if (ex != null)
                {
                    opStatus.ExceptionMessage = ex.Message;
                    opStatus.ExceptionStackTrace = ex.StackTrace;
                    if (ex.InnerException != null)
                    {
                        opStatus.ExceptionInnerMessage = ex.InnerException.Message;
                        opStatus.ExceptionInnerStackTrace = ex.InnerException.StackTrace;
                    }
                }
            }
            catch
            {
                //DIRTY SUPPRESSION TO PREVENT CODE EXECUTION
            }
        }
    }
}