using System;
using System.IO;
using DevBetterWeb.Web.Services;
using Shouldly;
using SkiaSharp;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services;

public class AvatarImageResizerTests
{
	[Theory]
	[InlineData(1200, 800, SKEncodedImageFormat.Png)]
	[InlineData(300, 900, SKEncodedImageFormat.Jpeg)]
	[InlineData(500, 500, SKEncodedImageFormat.Png)]
	[InlineData(50, 50, SKEncodedImageFormat.Jpeg)]
	public void ResizeToJpeg_Returns_Square_Jpeg_Of_Avatar_Size(int width, int height, SKEncodedImageFormat inputFormat)
	{
		using var input = CreateImage(width, height, inputFormat, SKColors.Red);

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
		// 300x100: left third blue, middle third red, right third blue. Center crop keeps only red.
		using var bitmap = new SKBitmap(300, 100);
		using (var canvas = new SKCanvas(bitmap))
		{
			canvas.Clear(SKColors.Blue);
			using var paint = new SKPaint { Color = SKColors.Red };
			canvas.DrawRect(SKRect.Create(100, 0, 100, 100), paint);
		}
		using var input = Encode(bitmap, SKEncodedImageFormat.Png);

		using var output = AvatarImageResizer.ResizeToJpeg(input);

		using var result = SKBitmap.Decode(output);
		AssertMostlyRed(result.GetPixel(10, 250));
		AssertMostlyRed(result.GetPixel(250, 250));
		AssertMostlyRed(result.GetPixel(489, 250));
	}

	[Fact]
	public void ResizeToJpeg_Throws_For_Non_Image_Data()
	{
		using var input = new MemoryStream("not an image"u8.ToArray());

		Should.Throw<InvalidOperationException>(() => AvatarImageResizer.ResizeToJpeg(input));
	}

	private static void AssertMostlyRed(SKColor color)
	{
		color.Red.ShouldBeGreaterThan((byte)200);
		color.Blue.ShouldBeLessThan((byte)60);
	}

	private static MemoryStream CreateImage(int width, int height, SKEncodedImageFormat format, SKColor color)
	{
		using var bitmap = new SKBitmap(width, height);
		using (var canvas = new SKCanvas(bitmap))
		{
			canvas.Clear(color);
		}
		return Encode(bitmap, format);
	}

	private static MemoryStream Encode(SKBitmap bitmap, SKEncodedImageFormat format)
	{
		var stream = new MemoryStream();
		bitmap.Encode(stream, format, 90);
		stream.Position = 0;
		return stream;
	}
}
