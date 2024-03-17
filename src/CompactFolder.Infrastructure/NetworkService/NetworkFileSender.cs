using CompactFolder.Application.Services.NetworkService;
using CompactFolder.Application.Services.NetworkService.Contracts;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CompactFolder.Infrastructure.FileService
{
    public class NetworkFileSender : INetworkFileSender
    {
        private readonly ILogger<NetworkFileSender> _logger;
        public NetworkFileSender(ILogger<NetworkFileSender> logger)
        {
            _logger = logger;
        }
        public BaseResult Send(string originFilePath, string sharedDestinationFilePath)
        {
            try
            {
                File.Copy(originFilePath, sharedDestinationFilePath, true);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure(NetworkFileSenderErrors.GenericError);
            }
        }
    }
}
