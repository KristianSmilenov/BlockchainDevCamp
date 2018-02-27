using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class SubmitBlockResponse
    {
        public BlockResponseStatus Status;
        public string Message;
    }

    public enum BlockResponseStatus
    {
        Success,
        Error
    }
}
