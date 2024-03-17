using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.Email;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.Operations.LocalFile;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.LocalFile
{
    public class LocalFileOperationTests
    {
        private readonly ITempPathProvider _tempPathProvider;

        public Guid Id { get; set; }
        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TPath DestinationPath { get; set; }

        public LocalFileOperationTests()
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
            DestinationPath = @"C:\example";
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void LocalFileOperation_WhenCreatedWithValidParameters_ShouldCreateObject()
        {
            // Arrange & Act         
            var object1 = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, DestinationPath);
            var object2 = new LocalFileOperation(Id, OriginPath, OutputFileName, ExclusionRules, DestinationPath);
            var object3 = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, DestinationPath, _tempPathProvider);
            var object4 = new LocalFileOperation(Id, OriginPath, OutputFileName, ExclusionRules, DestinationPath, _tempPathProvider);

            // Assert
            object1.Should().NotBeNull();
            object1.Id.Should().NotBeEmpty();
            object1.OriginPath.Should().Be(OriginPath);
            object1.OutputFileName.Should().Be(OutputFileName);
            object1.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object1.DestinationPath.Should().BeEquivalentTo(DestinationPath);
            object1.DestinationFullPath.Path.Should().BeEquivalentTo(Path.Combine(DestinationPath.Path, OutputFileName.Path));

            object2.Should().NotBeNull();
            object2.Id.Should().Be(Id);
            object2.OriginPath.Should().Be(OriginPath);
            object2.OutputFileName.Should().Be(OutputFileName);
            object2.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object2.DestinationPath.Should().BeEquivalentTo(DestinationPath);
            object2.DestinationFullPath.Path.Should().BeEquivalentTo(Path.Combine(DestinationPath.Path, OutputFileName.Path));

            object3.Should().NotBeNull();
            object3.Id.Should().NotBeEmpty();
            object3.OriginPath.Should().Be(OriginPath);
            object3.OutputFileName.Should().Be(OutputFileName);
            object3.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object3.DestinationPath.Should().BeEquivalentTo(DestinationPath);
            object3.DestinationFullPath.Path.Should().BeEquivalentTo(Path.Combine(DestinationPath.Path, OutputFileName.Path));

            object4.Should().NotBeNull();
            object4.Id.Should().Be(Id);
            object4.OriginPath.Should().Be(OriginPath);
            object4.OutputFileName.Should().Be(OutputFileName);
            object4.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
            object4.DestinationPath.Should().BeEquivalentTo(DestinationPath);
            object4.DestinationFullPath.Path.Should().BeEquivalentTo(Path.Combine(DestinationPath.Path, OutputFileName.Path));
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void LocalFileOperation_WhenCreatedWithInvalidParameters_ShouldThrowException()
        {
            // Arrange & Act         
            Action action1 = () => new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, "");
            Action action2 = () => new LocalFileOperation(Id, OriginPath, OutputFileName, ExclusionRules, "");
            Action action3 = () => new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, "", _tempPathProvider);
            Action action4 = () => new LocalFileOperation(Id, OriginPath, OutputFileName, ExclusionRules, "", _tempPathProvider);

            // Assert
            action1.Should().Throw<ArgumentException>();
            action2.Should().Throw<ArgumentException>();
            action3.Should().Throw<ArgumentException>();
            action4.Should().Throw<ArgumentException>();
        }
    }
}
