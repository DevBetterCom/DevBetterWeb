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

	/// <summary>Largest accepted source width or height, in pixels.</summary>
	public const int MaxSourceDimension = 10_000;

	/// <summary>Largest accepted source pixel count (40 megapixels).</summary>
	public const long MaxSourcePixels = 40_000_000;

	/// <summary>
	/// Applies EXIF orientation, center-crops the image to a square, and scales it to
	/// <see cref="AvatarSize"/> x <see cref="AvatarSize"/> JPEG. Output carries no metadata.
	/// </summary>
	/// <exception cref="InvalidImageException">The stream is not a decodable image or is too large.</exception>
	public static MemoryStream ResizeToJpeg(Stream input)
	{
		ArgumentNullException.ThrowIfNull(input);

		using var codec = SKCodec.Create(input)
			?? throw new InvalidImageException("Uploaded file is not a supported image.");

		var info = codec.Info;
		if (info.Width <= 0 || info.Height <= 0
			|| info.Width > MaxSourceDimension || info.Height > MaxSourceDimension
			|| (long)info.Width * info.Height > MaxSourcePixels)
		{
			throw new InvalidImageException("Uploaded image is too large.");
		}

		using var source = Decode(codec);
		using var oriented = ApplyOrientation(source, codec.EncodedOrigin);
		oriented.SetImmutable();

		int side = Math.Min(oriented.Width, oriented.Height);
		var sourceRect = SKRect.Create((oriented.Width - side) / 2f, (oriented.Height - side) / 2f, side, side);

		using var surface = SKSurface.Create(new SKImageInfo(AvatarSize, AvatarSize))
			?? throw new InvalidOperationException("Unable to allocate avatar surface.");
		surface.Canvas.Clear(SKColors.White);
		using var image = SKImage.FromBitmap(oriented);
		surface.Canvas.DrawImage(image, sourceRect, SKRect.Create(AvatarSize, AvatarSize),
			new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

		using var resized = surface.Snapshot();
		using var data = resized.Encode(SKEncodedImageFormat.Jpeg, JpegQuality)
			?? throw new InvalidOperationException("Unable to encode avatar.");

		var output = new MemoryStream();
		data.SaveTo(output);
		output.Position = 0;
		return output;
	}

	private static SKBitmap Decode(SKCodec codec)
	{
		// Let the codec downsample during decode (cheap for JPEG) while keeping enough detail for the avatar.
		var info = codec.Info;
		float scale = Math.Min(1f, (float)AvatarSize * 2 / Math.Min(info.Width, info.Height));
		var size = codec.GetScaledDimensions(scale);
		var decodeInfo = new SKImageInfo(size.Width, size.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

		var bitmap = new SKBitmap(decodeInfo);
		var result = codec.GetPixels(decodeInfo, bitmap.GetPixels());
		if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
		{
			// The codec cannot scale to that size; decode at full size instead.
			bitmap.Dispose();
			decodeInfo = info.WithColorType(SKColorType.Rgba8888).WithAlphaType(SKAlphaType.Premul);
			bitmap = new SKBitmap(decodeInfo);
			result = codec.GetPixels(decodeInfo, bitmap.GetPixels());
		}

		if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
		{
			bitmap.Dispose();
			throw new InvalidImageException("Uploaded file is not a supported image.");
		}

		return bitmap;
	}

	private static SKBitmap ApplyOrientation(SKBitmap source, SKEncodedOrigin origin)
	{
		if (origin == SKEncodedOrigin.TopLeft)
		{
			return source.Copy();
		}

		bool swapsAxes = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
			or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
		int width = swapsAxes ? source.Height : source.Width;
		int height = swapsAxes ? source.Width : source.Height;

		var rotated = new SKBitmap(new SKImageInfo(width, height, source.ColorType, source.AlphaType));
		using var canvas = new SKCanvas(rotated);
		switch (origin)
		{
			case SKEncodedOrigin.TopRight:
				canvas.Scale(-1, 1, width / 2f, 0);
				break;
			case SKEncodedOrigin.BottomRight:
				canvas.RotateDegrees(180, width / 2f, height / 2f);
				break;
			case SKEncodedOrigin.BottomLeft:
				canvas.Scale(1, -1, 0, height / 2f);
				break;
			case SKEncodedOrigin.LeftTop: // transpose
				canvas.Translate(width, 0);
				canvas.RotateDegrees(90);
				canvas.Translate(0, source.Height);
				canvas.Scale(1, -1);
				break;
			case SKEncodedOrigin.RightTop: // rotate 90 clockwise
				canvas.Translate(width, 0);
				canvas.RotateDegrees(90);
				break;
			case SKEncodedOrigin.RightBottom: // transverse
				canvas.Translate(width, 0);
				canvas.RotateDegrees(90);
				canvas.Translate(source.Width, 0);
				canvas.Scale(-1, 1);
				break;
			case SKEncodedOrigin.LeftBottom: // rotate 270 clockwise
				canvas.Translate(0, height);
				canvas.RotateDegrees(270);
				break;
		}
		canvas.DrawBitmap(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
		return rotated;
	}
}

/// <summary>Thrown when an uploaded image cannot be used as an avatar.</summary>
public class InvalidImageException(string message) : Exception(message);
