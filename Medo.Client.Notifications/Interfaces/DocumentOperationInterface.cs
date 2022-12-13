using Medo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Notifications.Interfaces
{
    public interface DocumentOperationInterface
    {
        bool RegisterDocumentInSED { get; set; }
        bool RejectRegistrationInSED { get; set; }
        bool DeleteDocumentFromSED { get; set; }
        List<string> RejectStatuses { get; set; }
        string RejectStatus { get; set; }
        Document OperationDocument { get; set; }
    }
}
