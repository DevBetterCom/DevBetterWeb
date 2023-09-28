using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NimblePros.Vimeo.VideoTusService;

namespace DevBetterWeb.Core.Interfaces;
public interface ICreateVideoService
{
	Task<string> StartAsync(string videoName, long videoSize, string domain, CancellationToken cancellationToken = default);
	Task<UploadChunkStatus> UploadChunkAsync(bool isBaseFolder, string sessionId, string chunk, string? description, long? folderId, CancellationToken cancellationToken = default);
}
