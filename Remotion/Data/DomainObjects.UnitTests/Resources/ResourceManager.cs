using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.DomainObjects.UnitTests.Resources
{
  public static class ResourceManager
  {
    public static byte[] GetImage1 ()
    {
      return ResourceUtility.GetResource(typeof(ResourceManager), "Image1.png");
    }

    public static byte[] GetImage2 ()
    {
      return ResourceUtility.GetResource(typeof(ResourceManager), "Image2.png");
    }

    public static byte[] GetImageLarger1MB ()
    {
      return ResourceUtility.GetResource(typeof(ResourceManager), "ImageLarger1MB.bmp");
    }

    public static byte[] GetMappingExportOutput ()
    {
      return ResourceUtility.GetResource(typeof(ResourceManager), "MappingExportOutput.xml");
    }

    public static byte[] GetRdbmsMappingSchema ()
    {
      return ResourceUtility.GetResource(typeof(ResourceManager), "RdbmsMapping.xsd");
    }

    public static void IsEqualToImage1 (byte[] actual)
    {
      IsEqualToImage1(actual, null);
    }

    public static void IsEqualToImage2 (byte[] actual)
    {
      IsEqualToImage2(actual, null);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual)
    {
      IsEqualToImageLarger1MB(actual, null);
    }

    public static void IsEmptyImage (byte[] actual)
    {
      IsEmptyImage(actual, null);
    }

    public static void IsEqualToImage1 (byte[] actual, string message)
    {
      AreEqual(GetImage1(), actual, message);
    }

    public static void IsEqualToImage2 (byte[] actual, string message)
    {
      AreEqual(GetImage2(), actual, message);
    }

    public static void IsEqualToImageLarger1MB (byte[] actual, string message)
    {
      AreEqual(GetImageLarger1MB(), actual, message);
    }

    public static void IsEmptyImage (byte[] actual, string message)
    {
      AreEqual(new byte[0], actual, message);
    }

    public static void AreEqual (byte[] expected, byte[] actual)
    {
      AreEqual(expected, actual, null);
    }

    public static void AreEqual (byte[] expected, byte[] actual, string message)
    {
      if (expected == actual)
        return;

      if (expected == null)
        Assert.Fail("Expected array is null, but actual array is not null.");

      Assert.That(actual, Is.EqualTo(expected));
    }
  }
}
