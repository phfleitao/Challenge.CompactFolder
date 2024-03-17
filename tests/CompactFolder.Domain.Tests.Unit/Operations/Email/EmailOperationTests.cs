using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.Email;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.Email
{
    public class EmailOperationTests
    {
        private readonly ITempPathProvider _tempPathProvider;

        public Guid Id { get; set; }
        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TEmail EmailTo { get; set; }

        public EmailOperationTests()
        {
            _tempPathProvider = Substitute.For<ITempPathProvider>();

            Id = Guid.NewGuid();
            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            EmailTo = @"pedro@example.com";
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void EmailOperation_WhenCreatedWithValidParameters_ShouldCreateObject()
        {
            // Arrange & Act         
            var object1 = new EmailOperation(OriginPath, OutputFileName, ExclusionRules, EmailTo);
            var object2 = new EmailOperation(Id, OriginPath, OutputFileName, ExclusionRules, EmailTo);
            var object3 = new EmailOperation(OriginPath, OutputFileName, ExclusionRules, EmailTo, _tempPathProvider);
            var object4 = new EmailOperation(Id, OriginPath, OutputFileName, ExclusionRules, EmailTo, _tempPathProvider);

            // Assert
            object1.Should().NotBeNull();
            object1.Id.Should().NotBeEmpty();
            object1.OriginPath.Should().Be(OriginPath);
            object1.OutputFileName.Should().Be(OutputFileName);
            object1.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object1.To.Should().BeEquivalentTo(EmailTo);

            object2.Should().NotBeNull();
            object2.Id.Should().Be(Id);
            object2.OriginPath.Should().Be(OriginPath);
            object2.OutputFileName.Should().Be(OutputFileName);
            object2.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object2.To.Should().BeEquivalentTo(EmailTo);

            object3.Should().NotBeNull();
            object3.Id.Should().NotBeEmpty();
            object3.OriginPath.Should().Be(OriginPath);
            object3.OutputFileName.Should().Be(OutputFileName);
            object3.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object3.To.Should().BeEquivalentTo(EmailTo);

            object4.Should().NotBeNull();
            object4.Id.Should().Be(Id);
            object4.OriginPath.Should().Be(OriginPath);
            object4.OutputFileName.Should().Be(OutputFileName);
            object4.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object4.To.Should().BeEquivalentTo(EmailTo);
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void EmailOperation_WhenCreatedWithInvalidParameters_ShouldThrowException()
        {
            // Arrange & Act         
            Action action1 = () => new EmailOperation(OriginPath, OutputFileName, ExclusionRules, "");
            Action action2 = () => new EmailOperation(Id, OriginPath, OutputFileName, ExclusionRules, "");
            Action action3 = () => new EmailOperation(OriginPath, OutputFileName, ExclusionRules, "", _tempPathProvider);
            Action action4 = () => new EmailOperation(Id, OriginPath, OutputFileName, ExclusionRules, "", _tempPathProvider);

            // Assert
            action1.Should().Throw<ArgumentException>();
            action2.Should().Throw<ArgumentException>();
            action3.Should().Throw<ArgumentException>();
            action4.Should().Throw<ArgumentException>();
        }
    }
}
