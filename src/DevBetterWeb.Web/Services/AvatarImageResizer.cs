using System;
using System.IO;
using SkiaSharp;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Converts an uploaded image into a square JPEG avatar.
/// </summary>
public static class AvatarImageResizer
{
	public const int AvatarSize = 500;
	public const int JpegQuality = 80;

	/// <summary>
	/// Center-crops the image to a square and scales it to <see cref="AvatarSize"/> x <see cref="AvatarSize"/> JPEG.
	/// </summary>
	/// <exception cref="InvalidOperationException">The stream is not a decodable image.</exception>
	public static MemoryStream ResizeToJpeg(Stream input)
	{
		ArgumentNullException.ThrowIfNull(input);

		using var source = SKBitmap.Decode(input)
			?? throw new InvalidOperationException("Uploaded file is not a supported image.");

		int side = Math.Min(source.Width, source.Height);
		var sourceRect = SKRect.Create((source.Width - side) / 2f, (source.Height - side) / 2f, side, side);

		using var surface = SKSurface.Create(new SKImageInfo(AvatarSize, AvatarSize));
		surface.Canvas.Clear(SKColors.White);
		using var sourceImage = SKImage.FromBitmap(source);
		surface.Canvas.DrawImage(sourceImage, sourceRect, SKRect.Create(AvatarSize, AvatarSize),
			new SKSamplingOptions(SKCubicResampler.Mitchell));

		using var resized = surface.Snapshot();
		using var data = resized.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);

		var output = new MemoryStream();
		data.SaveTo(output);
		output.Position = 0;
		return output;
	}
}
