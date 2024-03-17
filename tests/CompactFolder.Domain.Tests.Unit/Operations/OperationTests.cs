using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations
{
    public partial class OperationTests
    {
        private readonly ITempPathProvider _tempPathProvider;

        public Guid Id { get; set; }
        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }

        public OperationTests()
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
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void Operation_WhenCreatedWithValidParameters_ShouldCreateObject()
        {
            // Arrange & Act         
            var object1 = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);
            var object2 = new ConcreteObjectOperation(Id, OriginPath, OutputFileName, ExclusionRules);
            var object3 = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules, _tempPathProvider);
            var object4 = new ConcreteObjectOperation(Id, OriginPath, OutputFileName, ExclusionRules, _tempPathProvider);

            // Assert
            object1.Should().NotBeNull();
            object1.Id.Should().NotBeEmpty();
            object1.OriginPath.Should().Be(OriginPath);
            object1.OutputFileName.Should().Be(OutputFileName);
            object1.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);

            object2.Should().NotBeNull();
            object2.Id.Should().Be(Id);
            object2.OriginPath.Should().Be(OriginPath);
            object2.OutputFileName.Should().Be(OutputFileName);
            object2.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);

            object3.Should().NotBeNull();
            object3.Id.Should().NotBeEmpty();
            object3.OriginPath.Should().Be(OriginPath);
            object3.OutputFileName.Should().Be(OutputFileName);
            object3.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);

            object4.Should().NotBeNull();
            object4.Id.Should().Be(Id);
            object4.OriginPath.Should().Be(OriginPath);
            object4.OutputFileName.Should().Be(OutputFileName);
            object4.ExclusionRules.Should().BeEquivalentTo(ExclusionRules);
        }

        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData("", "")]
        [InlineData(@"C:\folder", "")]
        [InlineData("", "file.zip")]
        public void Operation_WhenCreatedWithInvalidParameters_ShouldThrowException(string originPath, string outputFileName)
        {
            // Arrange & Act         
            Action action1 = () => new ConcreteObjectOperation(originPath, outputFileName, null);
            Action action2 = () => new ConcreteObjectOperation(Guid.Empty, originPath, outputFileName, null);
            Action action3 = () => new ConcreteObjectOperation(originPath, outputFileName, null, _tempPathProvider);
            Action action4 = () => new ConcreteObjectOperation(Guid.Empty, originPath, outputFileName, null, _tempPathProvider);

            // Assert
            action1.Should().Throw<ArgumentException>();
            action2.Should().Throw<ArgumentException>();
            action3.Should().Throw<ArgumentException>();
            action4.Should().Throw<ArgumentException>();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void GetTempPath_WhenTempPathProviderIsNull_ShouldReturnDefaultTempPath()
        {
            // Arrange
            _tempPathProvider.GetTempPath().Returns((string)null);
            var operationWithInjection = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules, _tempPathProvider);
            var operationWithNoInjection = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);

            // Act
            var resultWithInjection = operationWithInjection.GetTempPath();
            var resultWithNoInjection = operationWithNoInjection.GetTempPath();

            // Assert
            resultWithInjection.Should().Be(Path.GetTempPath());
            resultWithNoInjection.Should().Be(Path.GetTempPath());
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void GetTempPath_WhenTempPathProviderIsNotNull_ShouldReturnProviderTempPath()
        {
            // Arrange
            var expectedPath = "tempPath";
            _tempPathProvider.GetTempPath().Returns(expectedPath);
            var operation = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules, _tempPathProvider);

            // Act
            var result = operation.GetTempPath();

            // Assert
            result.Should().Be(expectedPath);
        }
    }
}
