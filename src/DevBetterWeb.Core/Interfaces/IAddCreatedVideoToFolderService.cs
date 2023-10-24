using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;
public interface IAddCreatedVideoToFolderService
{
	Task<bool> ExecuteAsync(bool isBaseFolder, long? folderId, ArchiveVideo archiveVideo, CancellationToken cancellationToken = default);
}
