using CompactFolder.Domain.Operations.Contracts;
using System;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.Operations.Email.Models
{
    public class EmailOperationResponse
    {
        public Guid Id { get; set; }
        public string OriginPath { get; set; }
        public string OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentFilePath { get; set; }
    }
}
