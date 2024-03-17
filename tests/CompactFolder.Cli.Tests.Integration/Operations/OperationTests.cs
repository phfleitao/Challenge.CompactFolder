using CommandLine;
using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations;
using CompactFolder.Domain.Contracts;
using CompactFolder.Infrastructure.CompressorService;
using FluentAssertions;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Integration.Operations
{
    public class OperationTests : IClassFixture<OperationTestsFixture>
    {
        private readonly OperationTestsFixture _fixture;
        public OperationTests(OperationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Trait("Integration.Operations", "Operation")]
        [Theory]
        [InlineData(@"-i C:\Tests -o file.zip -t localFile")] //only basic args
        [InlineData(@"-i -o file.zip -t localFile")] //No inputpath
        [InlineData(@"-i C:\Tests -o -t localFile")] //No outputfilename
        [InlineData(@"-i C:\Tests -o file.txt -t localFile")] //Invalid outputfilename
        [InlineData(@"-i C:\Tests -o file.zip -t inexistent")] //inexistent output type
        [InlineData(@"-i C:\Tests -o file.zip -t localFile -t email")] //misc output types
        public async Task BasicOperation_GivenInvalidParserArgsCommand_ShouldReturnParserFailureResult(string commandArgs)
        {
            //Arrange
            var args = commandArgs.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args);

            //Assert
            response.IsFailure.Should().BeTrue();
            response.FirstError.Code.Should().Be("Cli.Argument");
        }
    }
}
