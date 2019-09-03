// Copyright 2009-2019 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace CsvHelper.Tests.Async
{
    [TestClass]
    public class DisposeAsyncTests
    {
        [TestMethod]
        public async Task WriterFlushOnDisposeTest()
        {
            using var writer = new StringWriter();
            await using (var csv = new CsvWriter(writer))
            {
                csv.WriteField("A");
            }

            Assert.AreEqual("A", writer.ToString());
        }

        [TestMethod]
        public async Task WriterFlushOnDisposeWithFlushTest()
        {
            using var writer = new StringWriter();
            await using (var csv = new CsvWriter(writer))
            {
                csv.WriteField("A");
                await csv.FlushAsync();
            }

            Assert.AreEqual("A", writer.ToString());
        }

        [TestMethod]
        public async Task DisposeShouldBeCallableMultipleTimes()
        {
            using var writer = new StringWriter();
            var csv = new CsvWriter(writer);

            csv.WriteField("A");

            await csv.DisposeAsync();
            await csv.DisposeAsync();
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "With LeaveOpen true, WritingContext is not disposed when CsvWriter is.")]
        [DataRow(false, DisplayName = "With LeaveOpen false, WritingContext is disposed when CsvWriter is.")]
        public async Task Dispose(bool leaveOpen)
        {
            using var writer = new StringWriter();
            var csv = new CsvWriter(writer, leaveOpen);

            WritingContext context = csv.Context;

            csv.WriteField("A");
            await csv.DisposeAsync();

            if (leaveOpen)
            {
                Assert.IsNotNull(context.Writer);
            }
            else
            {
                Assert.IsNull(context.Writer);
            }
        }
    }
}
