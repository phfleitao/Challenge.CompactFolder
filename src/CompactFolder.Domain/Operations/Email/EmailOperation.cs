using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompactFolder.Domain.Operations.Email
{
    public class EmailOperation : Operation
    {
        public TEmail To { get; private set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public TPath AttachmentFilePath => CompressionPath;

        public EmailOperation(
            Guid id,
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TEmail to,
            ITempPathProvider tempPathProvider = null)
            : base(id, originPath, outputFileName, exclusionRules, tempPathProvider)
        {
            To = to;

            InitializeProperties();
        }

        public EmailOperation(
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TEmail to,
            ITempPathProvider tempPathProvider = null)
            : this(Guid.NewGuid(), originPath, outputFileName, exclusionRules, to, tempPathProvider)
        { }

        private void InitializeProperties()
        {
            Subject = CreateDefaultSubject();
            Body = CreateDefaultBody();
        }

        private string CreateDefaultSubject()
        {
            return $"Compact Folder ({DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}): {Id.ToString()}";
        }

        private string CreateDefaultBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Hello,\n");
            sb.AppendLine($"Attachment sent at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} with:\n");
            sb.AppendLine($"Identification: {Id.ToString()}");
            sb.AppendLine($"Compacted Folder: {OriginPath.Path}");

            return sb.ToString();
        }       
    }
}
