using System;
using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderAttachmentExtensions
	{
		public static int SetupAttachment(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentAttachments attachment)
		{
			SetupAttachmentIds(reader, stream);
			SetupAttachmentValues(reader, stream, attachment);
			return SetupAttachmentSize(reader, stream, attachment);
		}

		private static int SetupAttachmentSize(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentAttachments attachment)
		{
			var attachmentSize = attachment.GetSize();
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter++ switch
					                         {
						                         0 => attachmentSize,
						                         5 => 1,
						                         _ => 5
					                         };
					       s.Position = returnValue == 1
						       ? s.Position
						       : s.Position + 1;
					       return returnValue;
				       });

			return attachmentSize;
		}

		private static void SetupAttachmentValues(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentAttachments attachment)
		{
			var attachedFile = attachment.AttachedFiles.Single();
			reader.SetupSequence(s => s.ReadBytes(stream, 5))
			      .Returns(Encoding.UTF8.GetBytes(attachedFile.Description))
			      .Returns(Encoding.UTF8.GetBytes(attachedFile.Name))
			      .Returns(Encoding.UTF8.GetBytes(attachedFile.MimeType))
			      .Returns(BitConverter.GetBytes(attachedFile.Uid).Reverse().ToArray());
		}

		private static void SetupAttachmentIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns("1941A469".ToBytes().ToArray())
			      .Returns("61A7".ToBytes().ToArray())
			      .Returns("467E".ToBytes().ToArray())
			      .Returns("466E".ToBytes().ToArray())
			      .Returns("4660".ToBytes().ToArray())
			      .Returns("465C".ToBytes().ToArray())
			      .Returns("46AE".ToBytes().ToArray());
		}
	}
}