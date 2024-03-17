using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CompactFolder.Infrastructure.CompressorService
{
    public class CompressorCreator : ICompressorCreator
    {
        private readonly ILogger<CompressorCreator> _logger;

        private string _originPath;
        private string _destinationPath;
        private IEnumerable<IExclusionRule> _exclusionRules;

        public CompressorCreator(ILogger<CompressorCreator> logger)
        {
            _logger = logger;
        }

        public BaseResult Create(string originPath, string destinationPath, IEnumerable<IExclusionRule> exclusionRules)
        {
            _originPath = originPath;
            _destinationPath = destinationPath;
            _exclusionRules = exclusionRules;

            try
            {
                var result = Validate();
                if (result.IsFailure)
                {
                    return result;    
                }

                ZipFolder();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure(CompressorCreatorErrors.GenericError);
            }            
        }

        private BaseResult Validate()
        {
            if (Path.HasExtension(_originPath) && !File.Exists(_originPath))
                return Result.Failure(CompressorCreatorErrors.OriginNotFound);

            if (!Path.HasExtension(_originPath) && !Directory.Exists(_originPath))
                return Result.Failure(CompressorCreatorErrors.OriginNotFound);

            if (!Directory.Exists(Path.GetDirectoryName(_destinationPath)))
                return Result.Failure(CompressorCreatorErrors.DestinationNotFound);

            return Result.Success();
        }

        private void ZipFolder()
        {
            using (FileStream zipToCreate = new FileStream(_destinationPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    AddFolderToZip(_originPath, archive, string.Empty);
                }
            }
        }

        private void AddFolderToZip(string sourceFolder, ZipArchive archive, string relativePath)
        {
            AddFilesToZip(sourceFolder, archive, relativePath);
            AddFoldersToZip(sourceFolder, archive, relativePath);
        }

        private void AddFilesToZip(string sourceFolder, ZipArchive archive, string relativePath)
        {
            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                if (ShouldExcludeFile(filePath))
                {
                    continue;
                }

                var entryPoint = Path.Combine(relativePath, Path.GetFileName(filePath));
                archive.CreateEntryFromFile(filePath, entryPoint);
            }
        }

        private void AddFoldersToZip(string sourceFolder, ZipArchive archive, string relativePath)
        {
            foreach (string subfolder in Directory.GetDirectories(sourceFolder))
            {
                if (ShouldExcludeFolder(subfolder))
                {
                    continue;
                }

                var entryPoint = Path.Combine(relativePath, Path.GetFileName(subfolder));
                archive.CreateEntry(entryPoint+"/");

                AddFolderToZip(subfolder, archive, entryPoint);
            }
        }

        private bool ShouldExcludeFile(string file)
        {
            return _exclusionRules.Any(er => (er is FileNameExclusionRule && er.IsExcluded(file)) ||
                                            (er is FileExtensionExclusionRule && er.IsExcluded(file)));
        }

        private bool ShouldExcludeFolder(string folder)
        {
            return _exclusionRules.Any(er => (er is DirectoryNameExclusionRule && er.IsExcluded(folder)));
        }
    }
}
