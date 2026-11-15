using System;
using System.IO;
using System.Linq;
using DevBetterWeb.Web.Services;
using Shouldly;
using SkiaSharp;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services;

public class AvatarImageResizerTests
{
	private const int Mid = AvatarImageResizer.AvatarSize / 2;
	private const int Quarter = AvatarImageResizer.AvatarSize / 4;
	private const int ThreeQuarters = AvatarImageResizer.AvatarSize * 3 / 4;

	[Theory]
	[InlineData(1200, 800, SKEncodedImageFormat.Png)]
	[InlineData(300, 900, SKEncodedImageFormat.Jpeg)]
	[InlineData(500, 500, SKEncodedImageFormat.Png)]
	[InlineData(50, 50, SKEncodedImageFormat.Jpeg)]
	[InlineData(4000, 3000, SKEncodedImageFormat.Jpeg)]
	public void ResizeToJpeg_Returns_Square_Jpeg_Of_Avatar_Size(int width, int height, SKEncodedImageFormat inputFormat)
	{
		using var input = Encode(Solid(width, height, SKColors.Red), inputFormat);

		using var output = AvatarImageResizer.ResizeToJpeg(input);

		output.Position.ShouldBe(0);
		using var codec = SKCodec.Create(output);
		codec.ShouldNotBeNull();
		codec.EncodedFormat.ShouldBe(SKEncodedImageFormat.Jpeg);
		codec.Info.Width.ShouldBe(AvatarImageResizer.AvatarSize);
		codec.Info.Height.ShouldBe(AvatarImageResizer.AvatarSize);
	}

	[Fact]
	public void ResizeToJpeg_Center_Crops_Wide_Image()
	{
		// 300x100: blue | red | blue. Center crop keeps only red.
		using var bitmap = Solid(300, 100, SKColors.Blue);
		Fill(bitmap, SKRect.Create(100, 0, 100, 100), SKColors.Red);

		using var result = Resize(Encode(bitmap, SKEncodedImageFormat.Png));

		AssertColor(result.GetPixel(10, Mid), SKColors.Red);
		AssertColor(result.GetPixel(Mid, Mid), SKColors.Red);
		AssertColor(result.GetPixel(AvatarImageResizer.AvatarSize - 10, Mid), SKColors.Red);
	}

	[Fact]
	public void ResizeToJpeg_Center_Crops_Tall_Image()
	{
		// 100x300: blue / red / blue. Center crop keeps only red.
		using var bitmap = Solid(100, 300, SKColors.Blue);
		Fill(bitmap, SKRect.Create(0, 100, 100, 100), SKColors.Red);

		using var result = Resize(Encode(bitmap, SKEncodedImageFormat.Png));

		AssertColor(result.GetPixel(Mid, 10), SKColors.Red);
		AssertColor(result.GetPixel(Mid, AvatarImageResizer.AvatarSize - 10), SKColors.Red);
	}

	[Fact]
	public void ResizeToJpeg_Fills_Transparent_Areas_With_White()
	{
		using var bitmap = Solid(200, 200, SKColors.Transparent);

		using var result = Resize(Encode(bitmap, SKEncodedImageFormat.Png));

		AssertColor(result.GetPixel(Mid, Mid), SKColors.White);
	}

	// Stored image quadrants: top-left green, top-right red, bottom half blue.
	// Each EXIF orientation must place green and red in the quadrants a viewer would see.
	[Theory]
	[InlineData(1, "TL", "TR")]
	[InlineData(2, "TR", "TL")]
	[InlineData(3, "BR", "BL")]
	[InlineData(4, "BL", "BR")]
	[InlineData(5, "TL", "BL")]
	[InlineData(6, "TR", "BR")]
	[InlineData(7, "BR", "TR")]
	[InlineData(8, "BL", "TL")]
	public void ResizeToJpeg_Applies_Exif_Orientation(ushort orientation, string greenQuadrant, string redQuadrant)
	{
		using var bitmap = Solid(400, 400, SKColors.Blue);
		Fill(bitmap, SKRect.Create(0, 0, 200, 200), SKColors.Lime);
		Fill(bitmap, SKRect.Create(200, 0, 200, 200), SKColors.Red);
		var jpeg = WithExifOrientation(EncodeBytes(bitmap, SKEncodedImageFormat.Jpeg), orientation);

		using var result = Resize(new MemoryStream(jpeg));

		AssertColor(PixelAt(result, greenQuadrant), SKColors.Lime);
		AssertColor(PixelAt(result, redQuadrant), SKColors.Red);
	}

	[Fact]
	public void ResizeToJpeg_Strips_Exif_Metadata()
	{
		var jpeg = WithExifOrientation(EncodeBytes(Solid(100, 100, SKColors.Red), SKEncodedImageFormat.Jpeg), 6);

		using var output = AvatarImageResizer.ResizeToJpeg(new MemoryStream(jpeg));

		ContainsExifSegment(output.ToArray()).ShouldBeFalse();
	}

	[Fact]
	public void ResizeToJpeg_Rejects_Image_With_Oversized_Dimensions()
	{
		// Header-only PNG claiming 20000x20000 pixels: must be rejected before any pixels are allocated.
		using var input = new MemoryStream(PngHeader(20_000, 20_000));

		Should.Throw<InvalidImageException>(() => AvatarImageResizer.ResizeToJpeg(input));
	}

	[Fact]
	public void ResizeToJpeg_Throws_For_Non_Image_Data()
	{
		using var input = new MemoryStream("not an image"u8.ToArray());

		Should.Throw<InvalidImageException>(() => AvatarImageResizer.ResizeToJpeg(input));
	}

	[Fact]
	public void ResizeToJpeg_Throws_For_Null_Stream()
	{
		Should.Throw<ArgumentNullException>(() => AvatarImageResizer.ResizeToJpeg(null!));
	}

	private static SKBitmap Resize(Stream input)
	{
		using (input)
		using (var output = AvatarImageResizer.ResizeToJpeg(input))
		{
			return SKBitmap.Decode(output);
		}
	}

	private static SKColor PixelAt(SKBitmap bitmap, string quadrant) => quadrant switch
	{
		"TL" => bitmap.GetPixel(Quarter, Quarter),
		"TR" => bitmap.GetPixel(ThreeQuarters, Quarter),
		"BL" => bitmap.GetPixel(Quarter, ThreeQuarters),
		"BR" => bitmap.GetPixel(ThreeQuarters, ThreeQuarters),
		_ => throw new ArgumentOutOfRangeException(nameof(quadrant)),
	};

	private static void AssertColor(SKColor actual, SKColor expected)
	{
		// JPEG is lossy, so allow some tolerance per channel.
		Math.Abs(actual.Red - expected.Red).ShouldBeLessThan(40, $"red channel of {actual}");
		Math.Abs(actual.Green - expected.Green).ShouldBeLessThan(40, $"green channel of {actual}");
		Math.Abs(actual.Blue - expected.Blue).ShouldBeLessThan(40, $"blue channel of {actual}");
	}

	private static SKBitmap Solid(int width, int height, SKColor color)
	{
		var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
		bitmap.Erase(color);
		return bitmap;
	}

	private static void Fill(SKBitmap bitmap, SKRect rect, SKColor color)
	{
		using var canvas = new SKCanvas(bitmap);
		using var paint = new SKPaint { Color = color };
		canvas.DrawRect(rect, paint);
	}

	private static MemoryStream Encode(SKBitmap bitmap, SKEncodedImageFormat format)
	{
		var stream = new MemoryStream(EncodeBytes(bitmap, format));
		bitmap.Dispose();
		return stream;
	}

	private static byte[] EncodeBytes(SKBitmap bitmap, SKEncodedImageFormat format)
	{
		using var stream = new MemoryStream();
		bitmap.Encode(stream, format, 95).ShouldBeTrue();
		return stream.ToArray();
	}

	/// <summary>Inserts a minimal big-endian EXIF APP1 segment with the given orientation right after SOI.</summary>
	private static byte[] WithExifOrientation(byte[] jpeg, ushort orientation)
	{
		byte[] exif =
		[
			(byte)'E', (byte)'x', (byte)'i', (byte)'f', 0, 0,
			(byte)'M', (byte)'M', 0, 42, 0, 0, 0, 8, // TIFF header, IFD0 at offset 8
			0, 1, // one entry
			0x01, 0x12, 0, 3, 0, 0, 0, 1, (byte)(orientation >> 8), (byte)orientation, 0, 0, // Orientation, SHORT, 1
			0, 0, 0, 0, // no next IFD
		];
		int length = exif.Length + 2;
		byte[] segment = [0xFF, 0xE1, (byte)(length >> 8), (byte)length, .. exif];
		return [.. jpeg.Take(2), .. segment, .. jpeg.Skip(2)];
	}

	private static bool ContainsExifSegment(byte[] jpeg)
	{
		for (int i = 0; i + 9 < jpeg.Length; i++)
		{
			if (jpeg[i] == 0xFF && jpeg[i + 1] == 0xE1 && jpeg[i + 4] == 'E' && jpeg[i + 5] == 'x'
				&& jpeg[i + 6] == 'i' && jpeg[i + 7] == 'f')
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>PNG signature plus an IHDR chunk only. Enough for a codec to read dimensions.</summary>
	private static byte[] PngHeader(int width, int height)
	{
		byte[] ihdrData =
		[
			(byte)'I', (byte)'H', (byte)'D', (byte)'R',
			(byte)(width >> 24), (byte)(width >> 16), (byte)(width >> 8), (byte)width,
			(byte)(height >> 24), (byte)(height >> 16), (byte)(height >> 8), (byte)height,
			8, 2, 0, 0, 0, // 8-bit RGB
		];
		uint crc = Crc32(ihdrData);
		return
		[
			0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A,
			0, 0, 0, 13, .. ihdrData,
			(byte)(crc >> 24), (byte)(crc >> 16), (byte)(crc >> 8), (byte)crc,
		];
	}

	private static uint Crc32(byte[] data)
	{
		uint crc = 0xFFFFFFFF;
		foreach (byte b in data)
		{
			crc ^= b;
			for (int k = 0; k < 8; k++)
			{
				crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
			}
		}
		return ~crc;
	}
}
