using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCompiler.Shared
{
    public class ExecutionInfo
    {
        public enum ExecutionStatus
        {
            Preparing,
            Compiling,
            CompilationError,
            Running,
            Finished,
            WithWarning
        }

        public ExecutionStatus Status { get; set; }
        public string Output { get; set; }
        public long AddTime { get; set; }
        public long FindTime { get; set; }
        public long DeleteTime { get; set; }
        public int ErrorLineNumber { get; set; } // новое свойство

        public List<string> Hints { get; set; } = new List<string>();

        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        [System.Text.Json.Serialization.JsonConstructor]
        public ExecutionInfo(){ this.Output = String.Empty; }
        public ExecutionInfo(ExecutionStatus resultStatus,  string Output)
        {
            this.Status = resultStatus;
            this.Output = Output;
        }

        public ExecutionInfo(ExecutionStatus resultStatus, long addTime, long findTime, long deleteTime, string Output, List<string> hints)
        {
            this.Status = resultStatus;
            AddTime = addTime;
            FindTime = findTime;
            DeleteTime = deleteTime;
            this.Output = Output;
            this.Hints = hints;
        }
        public ExecutionInfo(ExecutionStatus resultStatus,long addTime, long findTime, long deleteTime, int errorLineNumber, string output, List<string> hints)
        {
            this.Status = resultStatus;
            AddTime = addTime;
            FindTime = findTime;
            DeleteTime = deleteTime;
            this.Output = output;
            this.ErrorLineNumber = errorLineNumber;
            this.Hints = hints;
        }
    }
}
