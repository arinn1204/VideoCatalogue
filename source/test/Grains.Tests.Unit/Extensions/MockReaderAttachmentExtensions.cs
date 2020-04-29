using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			SegmentAttachment attachment)
		{
			SetupAttachmentIds(reader, stream);
			SetupAttachmentValues(reader, stream, attachment);
			return SetupAttachmentSize(reader, stream, attachment);
		}

		private static int SetupAttachmentSize(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentAttachment attachment)
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
						                         5 => 1L,
						                         _ => 5L
					                         };
					       s.Position = returnValue == 1
						       ? s.Position
						       : s.Position + 1;
					       return Task.FromResult(returnValue);
				       });

			return attachmentSize;
		}

		private static void SetupAttachmentValues(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentAttachment attachment)
		{
			var attachedFile = attachment.AttachedFiles.Single();
			reader.SetupSequence(s => s.ReadBytes(stream, 5))
			      .ReturnsAsync(Encoding.UTF8.GetBytes(attachedFile.Description))
			      .ReturnsAsync(Encoding.UTF8.GetBytes(attachedFile.Name))
			      .ReturnsAsync(Encoding.UTF8.GetBytes(attachedFile.MimeType))
			      .ReturnsAsync(BitConverter.GetBytes(attachedFile.Uid).Reverse().ToArray());
		}

		private static void SetupAttachmentIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .ReturnsAsync("1941A469".ToBytes().ToArray())
			      .ReturnsAsync("61A7".ToBytes().ToArray())
			      .ReturnsAsync("467E".ToBytes().ToArray())
			      .ReturnsAsync("466E".ToBytes().ToArray())
			      .ReturnsAsync("4660".ToBytes().ToArray())
			      .ReturnsAsync("465C".ToBytes().ToArray())
			      .ReturnsAsync("46AE".ToBytes().ToArray());
		}
	}
}